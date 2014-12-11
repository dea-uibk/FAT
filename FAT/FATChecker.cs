using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using DataTable = System.Data.DataTable;
using ImageMetadataUtils;

namespace FAT
{
    public enum ErrorType
    {
        InvalidData,
        MissingData,
        Resolution,
        None
    }

    class RangePair
    {
        public int Start;
        public int End;

        public RangePair(int start, int end)
        {
            Start = start; End = end;
        }
    }

    public enum FolderNodeType
    {
        RootDir,
        RootId,
        NewsId,
        Year,
        Issue,
        File,
        Undefined
    }

    public class MyTreeNode : TreeNode
    {
        public List<MyTreeNode> HiddenNodes = new List<MyTreeNode>();
        //public FolderNodeType NodeType = FolderNodeType.Undefined;

        public MyTreeNode(string text)
            : base(text)
        {
        }

    }

    public class FileNodeData
    {
        public FileInfo FileInfo=null;
        public ImageMetadata Metadata = null;
        public String Status = "unchecked";
        public String Message = "";
        public ErrorType Type = ErrorType.None;
        public FileNodeData(FileInfo fileInfo)
        {
            FileInfo = fileInfo;
        }

    }

    public class DirNodeData
    {
        public DirectoryInfo DirInfo=null;
        public String Language = null;
        //public String Size = null;
        public String Texttype = null;

        public DirNodeData(DirectoryInfo dirInfo)
        {
            DirInfo = dirInfo;
        }
    }

    //class 

    class FATChecker
    {
        public short MinDpi = 300;
        public short OverwriteDpi = 600;
        public short MaxDpi = 600;
        public short MaxDpiBw = 610;

        private TreeView _treeView = null;

        private List<MyTreeNode> _fileNodes = null;
        private List<MyTreeNode> _directoryNodes = null;

        private Thread[] _threads = null;
        private volatile int _completedFileChecks = 0;
        private volatile int _completedThreads = 0;
        private double _averageTimePerCheck = 0.0f;

        private volatile Boolean _stopThreads = false;
        private FATMainForm _gui;
        private bool _stopOnFirstError = false;
        private bool _recheck = false;

        private Dictionary<ErrorType, int> _errors;
        private Stopwatch _stopW = new Stopwatch();
        private bool _hasYearLevel = true;
        private bool _doPlausibilityCheck = true;
        private bool _calculateChecksum = true;
        private Dictionary<string, int> _viewing_file_statuses;

        private readonly DataTable _masterListDataTable;
        private List<string> _partnerNames;

        /**
         * Default constructor
         */
        public FATChecker(FATMainForm gui, String dirname, List<string> exts, ref TreeView treeView, bool hasYearLevel, DataTable masterListDataTable)
        {
            Trace.WriteLine("Loading directory "+dirname);
            var sw = Stopwatch.StartNew();
            
            _gui = gui;
            _hasYearLevel = hasYearLevel;
            _masterListDataTable = masterListDataTable;
            parseMasterlist();

            if (!System.IO.Directory.Exists(dirname))
            {
                throw new DirectoryNotFoundException("Directory does not exist: "+dirname);
            }
            _treeView = treeView;
            //_treeView = new TreeView();

            ResetErrors();

            if (UpdateFolderStructureTreeView(new DirectoryInfo(dirname), exts, masterListDataTable)>0)
            {
                throw new InvalidDataException("Errors occured while parsing folder structure");
            }
            ComputeViewingFileStatuses();

            var ts = sw.Elapsed;
            try
            {
                SyncWithXML();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error message: "+e.Message, "Error while parsing  XML File!", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                Trace.WriteLine(e.StackTrace);
            }
            sw.Stop();
            Trace.WriteLine("Time for syncing with XML: " + (sw.Elapsed-ts));
            Trace.WriteLine("Loading directory " + dirname + " finished, time: "+sw.Elapsed);
            setDpiValues();
        }

        private void setDpiValues() {
            MinDpi = (short)_gui.minDpiUpDown.Value;
            MaxDpi = (short)_gui.maxDpiUpDown.Value;
            MaxDpiBw = (short)_gui.maxDpiBwUpDown.Value;
            OverwriteDpi = (short)_gui.overwriteDpiUpDown.Value;
        }

        private void parseMasterlist()
        {
            // extract partner names:
            _partnerNames = new List<string>();
            foreach (DataRow row in _masterListDataTable.Rows) {
                if (_partnerNames.Contains(row[2].ToString())) continue;

                Trace.WriteLine("partnername: " + row[2].ToString());
                _partnerNames.Add(row[2].ToString());
            }


        }

        /**
         * Constructor with given xml filename; NOT USED CURRENTLY
         */
#if false
        public FATChecker(FATMainForm gui, String xmlFileName, ref TreeView treeView, List<string> exts)
        {
            _gui = gui;
            _treeView = treeView;
            if (!System.IO.File.Exists(xmlFileName))
            {
                throw new FileNotFoundException("File does not exist: " + xmlFileName);
            }
            ResetErrors();

            var sw = new Stopwatch();
            sw.Start();
            ReadXMLOutput(xmlFileName, ref _treeView, ref _directoryNodes, ref _fileNodes, exts);

            sw.Stop();
            TimeSpan tsElapsed = sw.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", tsElapsed.Hours, tsElapsed.Minutes, tsElapsed.Seconds,
                                                                                                tsElapsed.Milliseconds / 10);

            Trace.WriteLine("Elapsed time for crosscheck: " + elapsedTime);
            ComputeViewingFileStatuses();
        }
#endif

        private void ResetErrors()
        {
            _errors = new Dictionary<ErrorType, int>();
            foreach (ErrorType error in Enum.GetValues(typeof(ErrorType)))
                _errors.Add(error, 0);
        }

        public TreeView GetTreeView()
        {
            return _treeView;
        }

        public MyTreeNode FindNodeWithPath(string path, string containingPath, MyTreeNode rootNode = null)
        {
            //containingPath = containingPath.TrimEnd('\\').ToLower();
            path = path.TrimEnd('\\').ToLower();

            //Trace.WriteLine("path = "+ path + " containingPath= "+containingPath);

            //if (!path.StartsWith(containingPath)) return null;
            //Trace.WriteLine("here!");

            if (rootNode == null)
            {
                rootNode = (MyTreeNode) _treeView.Nodes[0];
            }
            string rootPath = ((DirNodeData)rootNode.Tag).DirInfo.FullName.TrimEnd('\\').ToLower();
            if (path.Equals(rootPath)) return rootNode;

            var nodes = new Stack<MyTreeNode>();
            nodes.Push(rootNode);

            while (nodes.Count != 0)
            {
                var currentNode = nodes.Pop();
                var nodePath = "";
                foreach (MyTreeNode c in currentNode.Nodes)
                {
                    var dat = (DirNodeData)c.Tag;
                    nodePath = dat.DirInfo.FullName.TrimEnd('\\').ToLower();

                    if (path.Equals(nodePath))
                    {
                        return c;
                    }
                    if (path.StartsWith(nodePath))
                    {
                        nodes.Push((MyTreeNode) c);
                        //break;
                    }
                }
                foreach (var c in currentNode.HiddenNodes)
                {
                    var dat = (FileNodeData)c.Tag;
                    nodePath = dat.FileInfo.FullName.TrimEnd('\\').ToLower();

                    if (path.Equals(nodePath))
                    {
                        return c;
                    }
                    //if (path.StartsWith(nodePath))
                    //{
                    //    nodes.Push((MyTreeNode)c);
                    //    break;
                    //}              
                }
            }
            return null;
        }

        private static bool parseDate(string dirname, ref DateTime dt)
        {
            //var dateRegEx = new Regex(@"^([a-zA-Z0-9]+_){0,1}[1-9][0-9]{7}(_[a-zA-Z0-9]+){0,1}$");
            var dateRegEx = new Regex(@"^[1-9][0-9]{7}$");

            string[] splits = dirname.Split('_');
            foreach (var s in splits)
            {
                if (dateRegEx.IsMatch(s))
                {
                    return DateTime.TryParseExact(s, "yyyyMMdd", new CultureInfo("en-US"), DateTimeStyles.None, out dt);
                }
            }
            return false;
        }

        private static bool IsValidDate(DateTime dt)
        {
            var leftInterval = new DateTime(1500, 1, 1);
            return (dt >= leftInterval && dt <= DateTime.Now);
        }

        private List<FileInfo> GetAllFilesFromExts(DirectoryInfo directory, IEnumerable<string> exts)
        {
            var files = new List<FileInfo>();
            foreach (var ext in exts)
            {
                files.AddRange(directory.GetFiles("*." + ext, SearchOption.TopDirectoryOnly).OrderBy(f=>f.Name));
            }
            return files;
        }

        private bool CheckFileForExts(FileInfo file, IEnumerable<string> exts)
        {
            return exts.Any(ext => file.FullName.ToLower().EndsWith(ext.ToLower()));
        }

        

        private int UpdateFolderStructureTreeView(DirectoryInfo rootDir, List<string> exts, DataTable masterListDataTable)
        {
            Trace.WriteLine("Updating and checking folder structure...");

            _treeView.Invoke(new Action(_treeView.Nodes.Clear));
            //_treeView.Nodes.Clear();

            _fileNodes = new List<MyTreeNode>();
            _directoryNodes = new List<MyTreeNode>();
            int nErrors = 0;
          
            var stack = new Stack<MyTreeNode>();
            var node = new MyTreeNode(rootDir.FullName) { Tag = new DirNodeData(rootDir) };
            
            stack.Push(node);
            //var rootFolderRegEx = new Regex("^[_a-zA-Z0-9]+_[0-9]+$");
            var alphaNumericRegEx = new Regex("^[a-zA-Z0-9_-]+$");
            const string alphaNumericErrorMsg =
                "Foldername is not valid! (only alphanumeric characters, underscores (_) and hyphens (-) are allowed!)";
            //Trace.WriteLine("Checking root folder: " + dirname);

            // check if root foldername generally valid:
            if (!alphaNumericRegEx.IsMatch(rootDir.Name)) {
                //throw new InvalidDataException("Root foldername is not valid: " + directory.FullName);
                nErrors++;
                //var pars = new object[4] { "error", "Directory", rootDir.FullName, "Root foldername is not valid! (only alphanumeric values are allowed: [a-zA-Z0-9])" };
                var pars = new object[4] { "error", "Directory", rootDir.FullName, alphaNumericErrorMsg };
                _gui.Invoke(new GuiAddErrorDelegate(_gui.AddFileError), pars);
                return nErrors;
            }
            // check if root foldername starts with a partner name:
            bool startsWithPartnername = _partnerNames.Any(f => rootDir.Name.StartsWith(f, true, CultureInfo.CurrentCulture));
            if (!startsWithPartnername)
            {
                nErrors++;
                var pars = new object[4] { "error", "Directory", rootDir.FullName, "Root foldername does not start with a valid partner name - check your foldername and compare it with the partner names in the masterlist!" };
                _gui.Invoke(new GuiAddErrorDelegate(_gui.AddFileError), pars);
                return nErrors;
            }
            //DataRow[] result = masterListDataTable.Select("UID = '" + rootDir.Name + "'");

            _directoryNodes.Add(node);

            bool parsedNewsLevel = false;
            bool parsedYearLevel = false;
            bool parsedIssueLevel = false;
            
            while (stack.Count > 0)
            {
                var currentNode = stack.Pop();
                currentNode.Expand();

                var directoryInfo = ((DirNodeData)currentNode.Tag).DirInfo;
                var dirs = directoryInfo.GetDirectories().OrderBy(f => f.Name).ToArray();
                foreach (var directory in dirs)
                {
                    var childDirectoryNode = new MyTreeNode(directory.Name) { Tag = new DirNodeData(directory) };
                    string dirname = directory.Name;

                    //childDirectoryNode.ForeColor = System.Drawing.Color.Red;

                    _directoryNodes.Add(childDirectoryNode);
                    currentNode.Nodes.Add(childDirectoryNode);
                    if (childDirectoryNode.Level >= 1 && childDirectoryNode.Level < getLastFolderLevel()) // check if top level folder contains files
                    {
                        var files = ParseFiles(directory, new List<String> {"*"}, false);
                        //Trace.WriteLine("Cont = "+files.Count);
                        if (files.Count > 0)
                        {
                            //throw new InvalidDataException("A top level folder contains files: "+directory.FullName);
                            nErrors++;
                            var pars = new object[4] { "error", "Directory", directory.FullName, "Top level folder contains files" };
                            _gui.Invoke(new GuiAddErrorDelegate(_gui.AddFileError), pars);
                        }
                    }
                    // check newspaper, year and issue level folders:
                    if (childDirectoryNode.Level == 1) // newsID level
                    {
                        if (dirname.Equals("viewing_files") || dirname.Equals("binarized_files"))
                        {
                            _directoryNodes.Remove(childDirectoryNode);
                            currentNode.Nodes.Remove(childDirectoryNode);
                            continue;
                        }

                        DataRow[] result = masterListDataTable.Select("UID = '"+dirname+"'");
                        if (result.Count()!=1)
                        {
                            //throw new InvalidDataException("Newspaper foldername is not valid: " + directory.FullName);
                            nErrors++;
                            var pars = new object[4] { "error", "Directory", directory.FullName, "Newspaper foldername is not valid! (only values from UID column of masterlist are allowed!)" };
                            _gui.Invoke(new GuiAddErrorDelegate(_gui.AddFileError), pars);
                            continue;                            
                        }


                        //var newsFolderRegEx = new Regex("[^a-zA-Z0-9_]");
                        ////Trace.WriteLine("Checking news folder: " + dirname);
                        //if (newsFolderRegEx.IsMatch(dirname))
                        //{
                        //    //throw new InvalidDataException("Newspaper foldername is not valid: " + directory.FullName);
                        //    nErrors++;
                        //    var pars = new object[4] { "error", "Directory", directory.FullName, "Newspaper foldername is not valid! (only alphanumeric characters and underscores are allowed)" };
                        //    _gui.Invoke(new GuiAddErrorDelegate(_gui.AddFileError), pars);
                        //    continue;
                        //}

                        parsedNewsLevel = true;
                    }
                    else if (childDirectoryNode.Level == 2 && _hasYearLevel) // year level
                    {
                        var yearFolderRegEx = new Regex("^[1-9][0-9]{3}$");
                        //Trace.WriteLine("Checking year folder: " + dirname);
                        if (!yearFolderRegEx.IsMatch(dirname))
                        {
                            //throw new InvalidDataException("Year foldername is not valid: " + directory.FullName);
                            nErrors++;
                            var pars = new object[4] { "error", "Directory", directory.FullName, "Year foldername is not valid!" };
                            _gui.Invoke(new GuiAddErrorDelegate(_gui.AddFileError), pars);
                            continue;
                        }
                        int year = Convert.ToInt32(dirname);
                        if (year < 1500 || year > DateTime.Now.Year)
                        {
                            //throw new InvalidDataException("Year foldername contains no valid year (i.e. year < 1500 or > current year!): " + directory.FullName);
                            nErrors++;
                            var pars = new object[4] { "error", "Directory", directory.FullName, "Year foldername contains no valid year (i.e. year < 1500 or > current year!)!" };
                            _gui.Invoke(new GuiAddErrorDelegate(_gui.AddFileError), pars);
                            continue;
                        }

                        parsedYearLevel = true;
                    }
                    else if (childDirectoryNode.Level == 3 || (!_hasYearLevel && childDirectoryNode.Level == 2) ) // issue level
                    {
                        //var issueFolderRegEx = new Regex(@"^[1-9][0-9]{7}(_[0-9]+){0,2}$");
                        //var issueFolderRegEx = new Regex(@"^([a-zA-Z0-9]+_){0,1}[1-9][0-9]{7}(_[a-zA-Z0-9]+){0,1}$");
                        //var issueFolderRegEx = new Regex(@"^{0,1}[1-9][0-9]{7}$");
                        //var issueFolderRegEx = new Regex("[^a-zA-Z0-9_]");
                        //var issueFolderRegEx = new Regex(@"^.+$"); // just for testing purposes -> accepts every string
                        //Trace.WriteLine("Checking issue folder: " + dirname);
                        var dt = new DateTime();
                        if (true)
                        {
                            if (!alphaNumericRegEx.IsMatch(dirname)) {
                                nErrors++;
                                var pars = new object[4] { "error", "Directory", directory.FullName, alphaNumericErrorMsg };
                                _gui.Invoke(new GuiAddErrorDelegate(_gui.AddFileError), pars);
                                continue;
                            }
                            if (!parseDate(dirname, ref dt)) {
                                //throw new InvalidDataException("Issue foldername is not valid: " + directory.FullName);
                                nErrors++;
                                var pars = new object[4]
                                               {
                                                   "error", "Directory", directory.FullName,
                                                   "Issue foldername does not contain a valid date - expected format is: *yyyymmdd* where * is a pre/suffix containing alphanumeric characters deliminated by an underscore; examples: issue_19900112, 19990230_issue, 19900112"
                                               };
                                _gui.Invoke(new GuiAddErrorDelegate(_gui.AddFileError), pars);
                                continue;
                            }
                            if (!IsValidDate(dt)) {
                                //throw new InvalidDataException("Issue foldername is not valid: " + directory.FullName);
                                nErrors++;
                                var pars = new object[4]
                                               {
                                                   "error", "Directory", directory.FullName,
                                                   "Issue foldername does not contain a valid date - the date must lie between 1500-01-01 and today!"
                                               };
                                _gui.Invoke(new GuiAddErrorDelegate(_gui.AddFileError), pars);
                                continue;                                
                            }
                        }

                        // throw error if there are subfolders at this level:
                        if (directory.GetDirectories().Count() != 0)
                        {
                            //throw new InvalidDataException("No subfolder allowed at issue level: "+directory.FullName);
                            nErrors++;
                            var pars = new object[4] { "error", "Directory", directory.FullName, "No subfolder allowed at issue level!" };
                            _gui.Invoke(new GuiAddErrorDelegate(_gui.AddFileError), pars);
                            continue;
                        }

                        // add files at issue level:
                        FileInfo fileBefore = null;
                        //var files = GetAllFilesFromExts(directory, exts);
                        var files = new List<FileInfo>();
                        files.AddRange(directory.GetFiles("*.*", SearchOption.TopDirectoryOnly).OrderBy(f=>f.Name.Length).ThenBy(f=>f.Name));
                        //files.AddRange(directory.GetFiles("*.*", SearchOption.TopDirectoryOnly).OrderBy(f => f.Name)); // old sorting -> only by name
                        if (files.Count==0)
                        {
                            var pars = new object[4] { "warning", "Directory", directory.FullName, "Issue folder does not contain any files!" };
                            _gui.Invoke(new GuiAddErrorDelegate(_gui.AddFileError), pars);
                        }

                        foreach (var file in files)
                        {
                            if (!CheckFileForExts(file, exts)) // if this is not a valid file
                            {
                                if (file.Name.ToLower().Equals("thumbs.db"))
                                    continue;

                                var pars = new object[4] { "warning", "File", file.FullName, "Input file format not supported!" };
                                _gui.Invoke(new GuiAddErrorDelegate(_gui.AddFileError), pars);
                                
                                continue;
                            }
                            //Trace.WriteLine("file: " + file.Name);
                            var fileNode = new MyTreeNode(file.Name) { Tag = new FileNodeData(file) };
                            //childDirectoryNode.Nodes.Add(fileNode);
                            childDirectoryNode.HiddenNodes.Add(fileNode);
                                
                            _fileNodes.Add(fileNode);
                                
                            //Trace.WriteLine("Parsing file: "+file.Name);

                            var filesRegEx = new Regex("[^a-zA-Z0-9_-]");
                            if (filesRegEx.IsMatch(Path.GetFileNameWithoutExtension(file.Name)))
                            {
                                //throw new InvalidDataException("Filename is not valid: " + file.FullName);
                                nErrors++;
                                var pars = new object[4] { "error", "File", file.FullName, "Filename is not valid - only alphanumeric characters, underscores (_) and hyphens (-) are allowed!" };
                                _gui.Invoke(new GuiAddErrorDelegate(_gui.AddFileError), pars);
                                continue;
                            }
                            if (fileBefore != null)
                            {
                                if (fileBefore.Name.Length != file.Name.Length && _gui.checkForLengthCb.Checked) // check for equal file length
                                {
                                    //throw new InvalidDataException("Filenames do not have the same length in folder: " + directory.FullName);
                                    nErrors++;
                                    var pars = new object[4] { "error", "Directory", directory.FullName, "Filenames do not have the same length in folder! (invalid file: "+file.FullName+")" };
                                    _gui.Invoke(new GuiAddErrorDelegate(_gui.AddFileError), pars);
                                    break;
                                }
                            }

                            fileBefore = file;
                        }
                        childDirectoryNode.Collapse();

                        parsedIssueLevel = true;
                    }

                    //Trace.WriteLine("added node, level = " + childDirectoryNode.Level);
                    stack.Push(childDirectoryNode);
                } // end foreach subdir
            } // end while

            if (nErrors == 0)
            {
                if (!parsedNewsLevel)
                {
                    //throw new InvalidDataException("No newspaper folder specified!");
                    nErrors++;
                    var pars = new object[4] {"error", "Directory", rootDir.FullName, "No newspaper folder specified!"};
                    _gui.Invoke(new GuiAddErrorDelegate(_gui.AddFileError), pars);
                }
                if (!parsedYearLevel && _hasYearLevel)
                {
                    //throw new InvalidDataException("No year folder specified!");
                    nErrors++;
                    var pars = new object[4] {"error", "Directory", rootDir.FullName, "No year folder specified!"};
                    _gui.Invoke(new GuiAddErrorDelegate(_gui.AddFileError), pars);
                }
                if (!parsedIssueLevel)
                {
                    //throw new InvalidDataException("No issue folder specified!");
                    nErrors++;
                    var pars = new object[4] {"error", "Directory", rootDir.FullName, "No issue folder specified!"};
                    _gui.Invoke(new GuiAddErrorDelegate(_gui.AddFileError), pars);
                }
            }

            _treeView.Invoke(new Func<TreeNode, int>(_treeView.Nodes.Add), new object[] { node });
            
            //_treeView.Nodes.Add(node);
            
            Trace.WriteLine("Updating and checking folder structure done, nodesCount = "+_treeView.Nodes.Count);

            return nErrors;
        } // end UpdateFolderStructureTreeView

        public string getDefaultXMLFileName()
        {
            var rootDir = ((DirNodeData)_treeView.Nodes[0].Tag).DirInfo;
            var fn = rootDir.FullName + "\\" + rootDir.Name + ".xml";
            return fn;
        }

        private bool SyncXMLCheckIfDirExists(MyTreeNode node, string dirname, ref int deletedDirsCount)
        {
            if (node == null)
            {
                //throw new FileNotFoundException("File does not exist anymore: " + imageFile.FullName);
                var pars = new object[4] { "warning", "Directory", dirname, "Directory does not exist anymore" };
                _gui.Invoke(new GuiAddErrorDelegate(_gui.AddFileError), pars);
                deletedDirsCount++;
                return false;
            }
            return true;
        }

        //private void SyncWithXML(out List<MyTreeNode> directoryNodes, out List<MyTreeNode> fileNodes, List<string> exts)
        private void SyncWithXML()
        {
            Trace.WriteLine("Syncing with xml file...");
            if (!File.Exists(getDefaultXMLFileName()))
            {
                Trace.WriteLine("No XML file found... no sync possible!");
                return;
            }

            //int globalFileCount = 0;
            //int newFilesCount = 0;
            int deletedFilesCount = 0;
            int deletedDirsCount = 0;

            //fileNodes = new List<MyTreeNode>();
            //directoryNodes = new List<MyTreeNode>();
            ResetErrors();

            var reader = XmlReader.Create(getDefaultXMLFileName());
            
            MyTreeNode currentRootIdNode = null, currentNewsIDNode = null, currentYearNode = null, currentIssueNode = null;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    var elementName = reader.Name;
                    switch (elementName)
                    {
                        case "RootID":
                            reader.MoveToAttribute("path");
                            currentRootIdNode = FindNodeWithPath(reader.Value, Directory.GetParent(reader.Value).FullName, null);
                            SyncXMLCheckIfDirExists(currentRootIdNode, reader.Value, ref deletedDirsCount);

                            break;
                        case "NewsID":
                            if (currentRootIdNode == null) continue;

                            reader.MoveToAttribute("path");
                            currentNewsIDNode = FindNodeWithPath(reader.Value, Directory.GetParent(reader.Value).FullName, currentRootIdNode);
                            SyncXMLCheckIfDirExists(currentNewsIDNode, reader.Value, ref deletedDirsCount);
                            
                            break;
                        case "Year":
                            if (currentNewsIDNode == null) continue;

                            reader.MoveToAttribute("path");
                            currentYearNode = FindNodeWithPath(reader.Value, Directory.GetParent(reader.Value).FullName, currentNewsIDNode);
                            SyncXMLCheckIfDirExists(currentYearNode, reader.Value, ref deletedDirsCount);

                            break;
                        case "Issue":
                            var parentNode = hasYearLevel() ? currentYearNode : currentNewsIDNode;

                            if (parentNode == null) continue;

                            reader.MoveToAttribute("path");
                            currentIssueNode = FindNodeWithPath(reader.Value, Directory.GetParent(reader.Value).FullName, parentNode);
                            bool check = SyncXMLCheckIfDirExists(currentIssueNode, reader.Value, ref deletedDirsCount);
                            if (!check) continue;

                            //Trace.WriteLine("SYNC: ISSUE LEVEL!!!!!!!!!");
                            //var directory = new DirectoryInfo(reader.Value);
                            var directory = new DirectoryInfo(reader.Value);
                            var dirNodeData = new DirNodeData(directory);
                            if (reader.MoveToAttribute("language"))
                            {
                                dirNodeData.Language = reader.Value;
                            }
                            if (reader.MoveToAttribute("texttype"))
                            {
                                dirNodeData.Texttype = reader.Value;
                            }
                            //Trace.WriteLine("SYNC: SETTING DIRNODE DATA");
                            currentIssueNode.Tag = dirNodeData;
                                
                            break;
                        case "Image":
                            if (currentIssueNode == null)
                            {
                                //Trace.WriteLine("2 parentNode == null - skipping!");
                                continue;
                            }

                            //reader.MoveToAttribute("path");
                            reader.MoveToAttribute("path");
                            string path = reader.Value;
                            var sw = Stopwatch.StartNew();
                            //Trace.WriteLine("currentDirNode: "+currentDirNode.FullPath);
                            var currentFileNode = FindNodeWithPath(reader.Value, Directory.GetParent(path).FullName, currentIssueNode);
                            //Trace.WriteLine("time for one findnodewithpath (image) = "+sw.Elapsed);
                            if (currentFileNode == null)
                            {
                                deletedFilesCount++;
                                //throw new FileNotFoundException("File does not exist anymore: " + imageFile.FullName);
                                var pars = new object[4] { "warning", "File", path, "File does not exist anymore" };
                                _gui.Invoke(new GuiAddErrorDelegate(_gui.AddFileError), pars);
                                continue;
                            }
                            var fileNode = ReadXMLImageNode(ref reader);
                            currentFileNode.Tag = fileNode.Tag;

                            //Trace.WriteLine("time for parsing image: "+swGlob.Elapsed);
                            break;
                    } // end switch
                    //if (directory != null)
                    //{
                    //    if (!directory.Exists)
                    //    {
                    //        throw new FileNotFoundException("Directory does not exist anymore: " + directory.FullName);
                    //    }
                    //}
                } // end if node == ...
                if (reader.NodeType == XmlNodeType.EndElement)
                {
                    var elementName = reader.Name;
                    switch (elementName)
                    {
                        case "RootID":
                            currentRootIdNode = null;
                            break;
                        case "NewsID":
                            currentNewsIDNode = null;
                            break;
                        case "Year":
                            currentYearNode = null;
                            break;
                        case "Issue":
                            currentIssueNode = null;
                            break;
                    }
                }


            } // end while read
            //Trace.WriteLine("enddepth = "+currentDepth);

            //_treeView.ExpandAll();

            reader.Close();
            Trace.WriteLine(" deletedFilesCount = " + deletedFilesCount + " deletedDirsCount =" + deletedDirsCount);

            // print summary if new or deleted files:
            if (deletedFilesCount > 0 || deletedDirsCount > 0)
            {
                var appendErrorMessageDelegate = new GuiAppendErrorMessageDelegate(_gui.AppendErrorMessage);

                _gui.Invoke(appendErrorMessageDelegate, new object[1] { "------------------- XML-Sync summary: -------------------" });
                _gui.Invoke(appendErrorMessageDelegate, new object[1] { "Nr. of deleted files: " + deletedFilesCount + ", Nr. of deleted directories: " + deletedDirsCount });
                _gui.Invoke(appendErrorMessageDelegate, new object[1] { "---------------------------------------------------------" });
            }
            Trace.WriteLine("Syncing with xml file done");

            //throw new NotImplementedException();
        } // end ReadXMLOutput

        public static List<FileInfo> ParseFiles(DirectoryInfo dir, List<string> exts, bool recursive)
        {
            var allFiles = new List<FileInfo>();
            foreach (string ext in exts)
            {
                FileInfo[] files = dir.GetFiles("*." + ext, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).OrderBy(f => f.Name).ToArray();
                allFiles.AddRange(files);
            }
            return allFiles;
        }

        public void StopThreads()
        {
            Trace.WriteLine("stopThreads called!");
            _stopThreads = true;
        }

        private void FileCheckCompleted(TimeSpan elapsed)
        {
            lock (typeof(FATChecker))
            {
                _completedFileChecks++;
                _averageTimePerCheck += (elapsed.TotalSeconds - _averageTimePerCheck)/(_completedFileChecks);
            }

            //Trace.WriteLine("File checks completed = " + _completedFileChecks + "/" + getNFiles());

            //var pars = new object[1] { elapsed };
            _gui.Invoke(new GuiDelegateJobFinished(_gui.JobFinished));
        }

        private void FileCheckThreadCompleted()
        {
            _completedThreads++;

            if (_completedThreads == _threads.Length)
            {
                _stopW.Stop();
                _gui.Invoke(new GuiDelegateAllJobsFinished(_gui.AllJobsFinished));
            }
        }

        private int CreateThreads(int nThreads, int nJobs)
        {
            int nActThreads = nThreads;
            if (nThreads > nJobs) nActThreads = nJobs;

            //nActThreads = 10;

            _threads = new Thread[nActThreads];
            return nActThreads;
        }

        public void UpdateTreeViewColors()
        {
            //foreach (MyTreeNode n in _fileNodes)
            //{
            //    var nd = (FileNodeData)n.Tag;
            //    if (nd.Status.Equals("error"))
            //    {
            //        n.ForeColor = System.Drawing.Color.Red;
            //    }
            //    else if (nd.Status.Equals("unchecked"))
            //    {
            //        n.ForeColor = System.Drawing.Color.Black;
            //    }
            //    else if (nd.Status.Equals("checked"))
            //    {
            //        n.ForeColor = System.Drawing.Color.Green;
            //    }
            //    else if (nd.Status.Equals("warning"))
            //    {
            //        n.ForeColor = System.Drawing.Color.DarkKhaki;
            //    }
            //}
            foreach (MyTreeNode n in _directoryNodes)
            {
                if (n.Level == getLastFolderLevel()) // only edit color on issue level
                {
                    var nd = (DirNodeData)n.Tag;
                    if (nd.Language == null && nd.Texttype == null) // all info missing
                    {
                        n.ForeColor = System.Drawing.Color.Red;
                    }
                    else if (nd.Language == null || nd.Texttype == null) // some info present
                    {
                        n.ForeColor = System.Drawing.Color.DarkKhaki;
                    }
                    else // all info present
                    {
                        n.ForeColor = System.Drawing.Color.Green;
                    }
                }
            }
        }

        public void CheckFiles(int nDesiredThreads, bool stopOnFirstError, bool recheck, bool doPlausibilityCheck, bool calculateChecksum)
        {
            _doPlausibilityCheck = doPlausibilityCheck;
            _calculateChecksum = calculateChecksum;
            _completedFileChecks = 0;
            _completedThreads = 0;
            _averageTimePerCheck = 0.0f;
            _stopThreads = false;
            _stopOnFirstError = stopOnFirstError;
            _recheck = recheck;
            setDpiValues();
            ResetErrors();

            int nFiles = getNFiles();

            if (nFiles == 0) {
                return;
            }
            int nThreads = CreateThreads(nDesiredThreads, nFiles);
            Trace.WriteLine("Created "+nThreads+" threads!");

            int portion = (int)Math.Round((double)nFiles / (double)nThreads);

            for (int i = 0; i < nThreads; ++i) {
                _threads[i] = new Thread(DoFileCheck);
            }

            _stopW.Reset();
            _stopW.Start();
            for (int i = 0; i < nThreads; ++i)
            {
                //_threads[i] = new Thread(new ParameterizedThreadStart(DoFileCheck));
                int startIndex = i*(portion);
                int endIndex = (i + 1)*(portion);
                if (i == nThreads - 1) endIndex = nFiles;
                Trace.WriteLine("CheckDirectories: Starting thread " + i + " si = " + startIndex + " ei = " + endIndex +
                                " t = " + nFiles);
                try
                {
                    _threads[i].Name = "CheckerThread_" + i + "_Range=" + startIndex + "-" + endIndex;
                    _threads[i].Start(new RangePair(startIndex, endIndex));
                    
                }
                catch (Exception e)
                {
                    Trace.WriteLine("FATAL Exception while creating thread "+i);
                }
            }
            Trace.WriteLine("Started all threads!");
        }

        private void DoFileCheck(object obj)
        {
            var pair = (RangePair)obj;

            //Trace.WriteLine("Hi, I am thread "+Thread.CurrentThread.Name+" start = "+pair.Start+" end = "+pair.End);
            var sw = new Stopwatch();

            try
            {
                for (int i = pair.Start; i < pair.End; ++i)
                {
                    MyTreeNode node = _fileNodes[i];
                    var nodeData = ((FileNodeData)node.Tag);

                    if (!nodeData.Status.Equals("checked") || _recheck)
                    {
                        FileInfo fi = nodeData.FileInfo;
                        var oldMd = nodeData.Metadata ?? new ImageMetadata();
                        string ext = fi.Extension.ToLower();

                        sw.Reset();
                        sw.Start();
                        try
                        {
                            lock (typeof(FATChecker))
                            {
                                nodeData.Metadata = new ImageMetadata { IsResOverwritten = oldMd.IsResOverwritten };
                                ImageMetadataUtils.ImageMetadataUtils.ReadImageHeader(fi.FullName, ref nodeData.Metadata, _doPlausibilityCheck, _calculateChecksum, MinDpi, MinDpi, OverwriteDpi, OverwriteDpi);
                            }
                            ErrorType error = CheckFileNode(ref nodeData);
                            _errors[error]++;
                        }
                        catch (InvalidDataException e)
                        {
                            var error = ErrorType.InvalidData;
                            _errors[error]++;
                            nodeData.Status = "error";
                            nodeData.Message = e.Message;
                            nodeData.Type = ErrorType.InvalidData;
                        }
                        catch (Exception e)
                        {
                            var error = ErrorType.InvalidData;
                            _errors[error]++;
                            nodeData.Status = "error";
                            nodeData.Message = e.Message;
                            nodeData.Type = ErrorType.InvalidData;
                            Trace.WriteLine(e.StackTrace);
                        }
                        finally
                        {
                            sw.Stop();
                        }

                        //Trace.WriteLine("Checked file, metadata: " + nodeData.Metadata.ToString());
                        //Trace.WriteLine("Status: " + nodeData.Status);
                        //Trace.WriteLine("Message: " + nodeData.Message);

                        //Trace.WriteLine("_stopThreads = " + _stopThreads);


                        if (nodeData.Status.Equals("error") || nodeData.Status.Equals("warning"))
                        {
                            var pars = new object[4] { nodeData.Status, "File", nodeData.FileInfo.FullName, nodeData.Message };
                            _gui.Invoke(new GuiAddErrorDelegate(_gui.AddFileError), pars);
                            //node.ForeColor = System.Drawing.Color.Red;

                            Trace.WriteLine("An error occured in file "+nodeData.FileInfo.FullName + ", StopOnFirstError = " + _stopOnFirstError);
                            _stopThreads = _stopOnFirstError;
                        }
                        else
                        {
                            //node.ForeColor = System.Drawing.Color.Green;
                        }
                    }
                    
                    FileCheckCompleted(sw.Elapsed);
                    if (_stopThreads)
                    {
                        Trace.WriteLine("Stopping!");
                        break;
                    }
                } // end for
            }
            catch (Exception e)
            {
                Trace.WriteLine("FATAL Exception in thread running from " + pair.Start + " to "+pair.End);
            }
            FileCheckThreadCompleted();
        }

        private ErrorType CheckFileNode(ref FileNodeData nodeData)
        {
            ImageMetadata md = nodeData.Metadata;
            if (md == null) // should not happen
            {
                throw new Exception("No metadata given at filenode: " + nodeData.FileInfo.FullName);
            }

            // check basic infos:
            if (md.Width == 0 || md.Height == 0)
            {
                nodeData.Status = "error";
                nodeData.Message = "Could not parse width or height";
                nodeData.Type = ErrorType.MissingData;
                return ErrorType.MissingData;
            }
            if (md.Bitdepth == null)
            {
                nodeData.Status = "error";
                nodeData.Message = "Could not parse bitdepth";
                nodeData.Type = ErrorType.MissingData;
                return ErrorType.MissingData;
            }
            if (md.Colortype == ImageColorType.Undefined)
            {
                nodeData.Status = "error";
                nodeData.Message = "Could not determine colortype (Bitonal/Grayscale/RGB)";
                nodeData.Type = ErrorType.MissingData;
                return ErrorType.MissingData;
            }
            if (md.Checksum.Equals("undefined") && _calculateChecksum)
            {
                nodeData.Status = "error";
                nodeData.Message = "Could not calculate checksum";
                nodeData.Type = ErrorType.MissingData;
                return ErrorType.MissingData;
            }
            if (md.Size == 0)
            {
                nodeData.Status = "error";
                nodeData.Message = "Could not determine byte size";
                nodeData.Type = ErrorType.MissingData;
                return ErrorType.MissingData;
            }
            if (md.Mimetype == "undefined")
            {
                nodeData.Status = "error";
                nodeData.Message = "Could not determine mimetype";
                nodeData.Type = ErrorType.MissingData;
                return ErrorType.MissingData;
            }

            // check resolution: -------------------------------
            if (md.XRes < 0 || md.YRes < 0)
            {
                nodeData.Status = "error";
                nodeData.Message = "No resolution given!";
                nodeData.Type = ErrorType.Resolution;
                return ErrorType.Resolution;
            }
            if (md.XRes < MinDpi || md.YRes < MinDpi)
            {
                nodeData.Status = "error";

                if (_doPlausibilityCheck)
                    nodeData.Message = "Resolution is lower than " + MinDpi + " dpi and plausibility check for " + OverwriteDpi + " dpi failed!";
                else
                    nodeData.Message = "Resolution is lower than " + MinDpi + " dpi";

                nodeData.Type = ErrorType.Resolution;
                return ErrorType.Resolution;
            }
            int maxDpi = MaxDpi;
            string imageType = "";
            if (md.Colortype == ImageColorType.Binary)
            {
                maxDpi = MaxDpiBw;
                imageType = "binary";
            }
            else if (md.Colortype == ImageColorType.RGB)
            {
                maxDpi = MaxDpi;
                imageType = "RGB";
            }
            else if (md.Colortype == ImageColorType.Grayscale)
            {
                maxDpi = MaxDpi;
                imageType = "Grayscale";
            }
            else if (md.Colortype == ImageColorType.IndexedColor)
            {
                maxDpi = MaxDpi;
                imageType = "IndexedColor";
            }
            else // should never happen (was checked already before!!)
            {
                throw new Exception("No image color type given at filenode: " + nodeData.FileInfo.FullName);
                nodeData.Type = ErrorType.Resolution;
                return ErrorType.Resolution;
            }

            if (md.XRes > maxDpi || md.YRes > maxDpi)
            {
                nodeData.Status = "error";
                nodeData.Message = "Resolution is higher than " + maxDpi + " dpi for this binary image!";
                nodeData.Type = ErrorType.Resolution;
                return ErrorType.Resolution;
            }
            // ---------------------------------------------------------------------------------------
            nodeData.Status = "checked";
            nodeData.Message = "";

            return ErrorType.None;
        }

        public void WriteXMLOutput(String outFn)
        {
            if (_treeView.Nodes.Count == 0) return;

            if (File.Exists(outFn))
                File.Delete(outFn);

            var settings = new XmlWriterSettings {Indent = true, IndentChars = "\t"};
            var writer = XmlWriter.Create(outFn, settings);

            var rootNode = (MyTreeNode) _treeView.Nodes[0];
            var rootNodeData = (DirNodeData) rootNode.Tag;

            //Trace.WriteLine("root node = "+rootNodeData.DirInfo.FullName);

            // get harddisk id:
            char drive = rootNodeData.DirInfo.FullName[0];
            var dsk = new ManagementObject(@"win32_logicaldisk.deviceid=""" + drive + @":""");
            dsk.Get();
            string volumeSerial = dsk["VolumeSerialNumber"].ToString();
            // get n of missing viewing files:
            int nMissingViewingFiles = GetNMissingViewingFiles();

            // Start writing xml doc:
            writer.WriteStartDocument();
            writer.WriteStartElement("RootID");
            writer.WriteAttributeString("value", rootNodeData.DirInfo.Name);
            writer.WriteAttributeString("path", rootNodeData.DirInfo.FullName);
            writer.WriteAttributeString("diskID", volumeSerial);
            writer.WriteAttributeString("nFiles", Convert.ToString(getNFiles()));
            writer.WriteAttributeString("nFileErrors", Convert.ToString(getNFileErrors()));
            writer.WriteAttributeString("nFileWarnings", Convert.ToString(getNFileWarnings()));
            writer.WriteAttributeString("nNewspapers", Convert.ToString(getNNewspaperFolders()));
            writer.WriteAttributeString("nIssues", Convert.ToString(getNIssueFolders()));
            writer.WriteAttributeString("nIssuesMissingMetadata", Convert.ToString(getNIssueFoldersWithoutMetadata()));
            writer.WriteAttributeString("nMissingViewingFiles", Convert.ToString(nMissingViewingFiles));
            writer.WriteAttributeString("hasYearLevel", Convert.ToString(_hasYearLevel));
            writer.WriteAttributeString("date", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
            
            foreach (var node in rootNode.Nodes)
                WriteMyTreeNodeXML((MyTreeNode) node, writer, false, DateTime.Now);

            writer.WriteEndElement();
            writer.WriteEndDocument();

            writer.Close();
        }

        private void WriteMyTreeNodeXML(MyTreeNode node, XmlWriter writer, bool isFileNode, DateTime date)
        {   
            if (!isFileNode)
            {
                //Trace.WriteLine("node level = "+node.Level +" dir = "+((DirNodeData)node.Tag).DirInfo.Name);
                if (node.Level == 1) // newspaperID level
                {
                    var nodeData = ((DirNodeData)node.Tag);
                    writer.WriteStartElement("NewsID");
                    writer.WriteAttributeString("value", nodeData.DirInfo.Name);
                    writer.WriteAttributeString("path", nodeData.DirInfo.FullName);
                    //writer.WriteAttributeString("date", date.ToString("yyyy-MM-dd HH:mm"));

                    var dt = _gui.getMasterListDataTable();
                    DataRow[] result = dt.Select("UID = '" + nodeData.DirInfo.Name + "'");
                    if (result.Count() != 1) throw new InvalidDataException("Cannot find newspaper id from masterlist while writing XML - should not happen!");
                    var row = result[0];
                    for (int i = 1; i < row.ItemArray.Count(); ++i)
                    {
                        var column = dt.Columns[i].ToString();
                        //Trace.WriteLine("column = " + dt.Columns[i].ToString());
                        if (column.Equals(@"Item Type") || column.Equals(@"Path")) continue;
                        //if (column.Contains(@" ") || column.Equals(@"Path")) continue;

                        writer.WriteAttributeString(column, row[i].ToString().Replace(";#",","));
                    }


                }
                else if (node.Level == 2 && _hasYearLevel) // year level
                {
                    var nodeData = ((DirNodeData)node.Tag);
                    writer.WriteStartElement("Year");
                    writer.WriteAttributeString("value", nodeData.DirInfo.Name);
                    writer.WriteAttributeString("path", nodeData.DirInfo.FullName);
                }
                else if (node.Level == 3 || (node.Level == 2 && !_hasYearLevel)) // issue level
                {
                    var nodeData = ((DirNodeData)node.Tag);
                    writer.WriteStartElement("Issue");
                    writer.WriteAttributeString("value", nodeData.DirInfo.Name);
                    writer.WriteAttributeString("path", nodeData.DirInfo.FullName);

                    var dt = new DateTime();
                    bool valid = parseDate(nodeData.DirInfo.Name, ref dt);
                    //if (!valid) throw new InvalidDataException("No valid year can be parsed while writing XML - should not happen - please contact support");
                    writer.WriteAttributeString("year", dt.Year.ToString());
                    writer.WriteAttributeString("month", dt.Month.ToString());
                    writer.WriteAttributeString("day", dt.Day.ToString());

                    if (nodeData.Language!=null) writer.WriteAttributeString("language", nodeData.Language);
                    //if (nodeData.Size != null) writer.WriteAttributeString("size", nodeData.Size);
                    if (nodeData.Texttype != null) writer.WriteAttributeString("texttype", nodeData.Texttype);
                }
                else
                {
                    throw new InvalidDataException("Invalid nodes detected while writing XML files, node level = " + node.Level);
                }
            }
            else // file node
            {
                var nodeData = ((FileNodeData)node.Tag);
                writer.WriteStartElement("Image");

                var md = nodeData.Metadata;

                writer.WriteAttributeString("name", nodeData.FileInfo.Name);
                writer.WriteAttributeString("hasViewingFile", Convert.ToString(HasViewingFile(nodeData.FileInfo.FullName)));

                writer.WriteAttributeString("status", nodeData.Status);

                if (nodeData.Type != ErrorType.None)
                    writer.WriteAttributeString("errorType", Enum.GetName(typeof(ErrorType), nodeData.Type));
                if (nodeData.Message.Count()!=0)
                    writer.WriteAttributeString("message", nodeData.Message);

                writer.WriteAttributeString("path", nodeData.FileInfo.FullName);

                if (md != null)
                {
                    writer.WriteAttributeString("width", "" + md.Width);
                    writer.WriteAttributeString("height", "" + md.Height);
                    writer.WriteAttributeString("xres", "" + md.XRes);
                    writer.WriteAttributeString("yres", "" + md.YRes);
                    writer.WriteAttributeString("IsResOverwritten", ""+md.IsResOverwritten);
                    if (md.Bitdepth != null)
                    {
                        string bdStr = "(" + md.Bitdepth[0];
                        for (int i = 1; i < md.Bitdepth.Count(); ++i) bdStr += "," + md.Bitdepth[i];
                        bdStr += ")";
                        writer.WriteAttributeString("bitdepth", bdStr);
                    }
                    writer.WriteAttributeString("color_type", Enum.GetName(typeof (ImageColorType), md.Colortype));
                    writer.WriteAttributeString("checksum", md.Checksum);
                    writer.WriteAttributeString("size", "" + md.Size);
                    writer.WriteAttributeString("mimetype", "" + md.Mimetype);
                    //writer.WriteAttributeString("compression", ""+(int)md.Compression);// write compression as int
                    writer.WriteAttributeString("compression", Enum.GetName(typeof(TiffCompressionType), md.Compression));
                    writer.WriteAttributeString("endian", Enum.GetName(typeof(EndianType), md.Endian));
                }
            }
            //else
            //{
            //    throw new InvalidDataException("Invalid nodes detected while writing XML files, node level = "+node.Level);
            //}
            foreach (MyTreeNode child in node.Nodes)
            {
                WriteMyTreeNodeXML(child, writer, false, date);
            }
            foreach (MyTreeNode child in node.HiddenNodes)
            {
                WriteMyTreeNodeXML(child, writer, true, date);
            }
            writer.WriteEndElement();
        }


//#define USE_WO_SYNC
#if USE_WO_SYNC
        private static void ReadXMLOutput(string fn, ref TreeView treeView, ref List<MyTreeNode> directoryNodes, ref List<MyTreeNode>  fileNodes)
        {
            treeView.Nodes.Clear();
            treeView.ExpandAll();
            fileNodes = new List<MyTreeNode>();
            directoryNodes = new List<MyTreeNode>();

            var reader = XmlReader.Create(fn);

            MyTreeNode baseNode = null;
            MyTreeNode rootIDNode = null;
            MyTreeNode currentNewsIDNode = null;
            MyTreeNode currentYearNode = null;
            MyTreeNode currentIssueNode = null;
            MyTreeNode currentFileNode = null;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    DirectoryInfo directory = null;
                    FileInfo file = null;

                    switch (reader.Name)
                    {
                        case "inputDirectory":
                            if (baseNode != null)
                            {
                                throw new Exception("Invalid XML file!");
                            }
                            reader.MoveToAttribute("path");
                            directory = new DirectoryInfo(reader.Value);
                            baseNode = new MyTreeNode(directory.FullName) { Tag = new DirNodeData(directory) };
                            baseNode.Expand();

                            
                            treeView.Nodes.Add(baseNode);
                            treeView.Nodes[treeView.Nodes.Count-1].Expand();

                            break;
                        case "RootID":
                            if (baseNode == null)
                            {
                                throw new Exception("Invalid XML file!");
                            }
                            reader.MoveToAttribute("path");
                            
                            directory = new DirectoryInfo(reader.Value);
                            
                            rootIDNode = new MyTreeNode(directory.Name) { Tag = new DirNodeData(directory) };
                            rootIDNode.Expand();
                            directoryNodes.Add(rootIDNode);
                            baseNode.Nodes.Add(rootIDNode);

                            break;
                        case "NewsID":
                            if (rootIDNode == null)
                            {
                                throw new Exception("Invalid XML file!");
                            }
                            reader.MoveToAttribute("path");
                            
                            directory = new DirectoryInfo(reader.Value);
                            currentNewsIDNode = new MyTreeNode(directory.Name) { Tag = new DirNodeData(directory) };
                            currentNewsIDNode.Expand();
                            directoryNodes.Add(currentNewsIDNode);
                            rootIDNode.Nodes.Add(currentNewsIDNode);

                            break;
                        case "Year":
                            if (currentNewsIDNode == null)
                            {
                                throw new Exception("Invalid XML file!");
                            }
                            reader.MoveToAttribute("path");
                            
                            directory = new DirectoryInfo(reader.Value);
                            currentYearNode = new MyTreeNode(directory.Name) { Tag = new DirNodeData(directory) };
                            currentYearNode.Expand();
                            directoryNodes.Add(currentYearNode);
                            currentNewsIDNode.Nodes.Add(currentYearNode);

                            break;
                        case "Issue":
                            if (currentYearNode == null)
                            {
                                throw new Exception("Invalid XML file!");
                            }
                            reader.MoveToAttribute("path");
                            
                            directory = new DirectoryInfo(reader.Value);
                            var dirNodeData = new DirNodeData(directory);
                            currentIssueNode = new MyTreeNode(directory.Name) { Tag = dirNodeData };

                            if (reader.MoveToAttribute("language"))
                            {
                                dirNodeData.Language = reader.Value;
                            }
                            if (reader.MoveToAttribute("size"))
                            {
                                dirNodeData.Size = reader.Value;
                            }
                            if (reader.MoveToAttribute("texttype"))
                            {
                                dirNodeData.Texttype = reader.Value;
                            }

                            currentIssueNode.Collapse();
                            directoryNodes.Add(currentIssueNode);
                            currentYearNode.Nodes.Add(currentIssueNode);

                            break;
                        case "Image":
                            if (currentIssueNode == null)
                            {
                                throw new Exception("Invalid XML file!");
                            }

                            reader.MoveToAttribute("path");
                            file = new FileInfo(reader.Value);
                            currentFileNode = new MyTreeNode(file.Name) { Tag = new FileNodeData(file) };
                            fileNodes.Add(currentFileNode);
                            //currentFileNode.
                            //currentIssueNode.Nodes.Add(currentFileNode);
                            currentIssueNode.HiddenNodes.Add(currentFileNode);

                            reader.MoveToAttribute("status");
                            var nd = ((FileNodeData)currentFileNode.Tag);
                            nd.Status = reader.Value;

                            if (reader.MoveToAttribute("message"))
                            {
                                nd.Message = reader.Value;
                            }

                            string stat = reader.Value;
                            if (!stat.Equals("unchecked")) // metadata must be present
                            {
                                nd.Metadata = new ImageMetadata();
                                reader.MoveToAttribute("width");
                                nd.Metadata.Width = Convert.ToUInt32(reader.Value);
                                reader.MoveToAttribute("height");
                                nd.Metadata.Height = Convert.ToUInt32(reader.Value);
                                reader.MoveToAttribute("xres");
                                nd.Metadata.XRes = Convert.ToDouble(reader.Value);
                                reader.MoveToAttribute("yres");
                                nd.Metadata.YRes = Convert.ToDouble(reader.Value);
                                if (reader.MoveToAttribute("bitdepth"))
                                {
                                    //Trace.WriteLine("biddepth str = "+reader.Value);
                                    string[] values = reader.Value.Split(new char[] { '(', ')', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                    //Trace.WriteLine("here: "+values.Count());
                                    nd.Metadata.Bitdepth = new uint[values.Length];
                                    for (int i = 0; i < values.Length; ++i)
                                    {
                                        //Trace.WriteLine("value " + i + "= " + values[i]);
                                        nd.Metadata.Bitdepth[i] = Convert.ToUInt32(values[i]);
                                    }
                                }
                                
                                reader.MoveToAttribute("color_type");
                                nd.Metadata.Colortype = (ImageColorType) Enum.Parse(typeof(ImageColorType), reader.Value);
                                reader.MoveToAttribute("checksum");
                                nd.Metadata.Checksum = reader.Value;
                                reader.MoveToAttribute("size");
                                nd.Metadata.Size = Convert.ToInt64(reader.Value);
                                reader.MoveToAttribute("mimetype");
                                nd.Metadata.Mimetype = reader.Value;
                            }

                            break;
                    } // end switch
                    if (directory!=null)
                    {
                        if (!directory.Exists)
                        {
                            throw new FileNotFoundException("Directory does not exist anymore: " + directory.FullName);
                        }
                    }
                    if (file != null)
                    {
                        if (!file.Exists)
                        {
                            throw new FileNotFoundException("File does not exist anymore: " + file.FullName);
                        }               
                    }
                } // end if node == ...


            } // end while read

            //_treeView.ExpandAll();

            reader.Close();

            //throw new NotImplementedException();
        } // end ReadXMLOutput
#else
        private void ReadXMLOutput(string fn, ref TreeView treeView, ref List<MyTreeNode> directoryNodes, ref List<MyTreeNode>  fileNodes, List<string> exts)
        {
            var reader = XmlReader.Create(fn);
            try
            {
                int globalFileCount = 0;
                int newFilesCount = 0;
                int deletedCount = 0;

                treeView.Nodes.Clear();
                treeView.ExpandAll();
                fileNodes = new List<MyTreeNode>();
                directoryNodes = new List<MyTreeNode>();
                ResetErrors();

                MyTreeNode rootIDNode = null;
                MyTreeNode currentNewsIDNode = null;
                MyTreeNode currentYearNode = null;
                MyTreeNode currentIssueNode = null;
                MyTreeNode currentFileNode = null;

                while (reader.Read())
                {
                    if (reader.NodeType != XmlNodeType.Element) continue;
                    DirectoryInfo directory = null;
                    FileInfo file = null;

                    switch (reader.Name)
                    {
                        case "RootID":
                            if (rootIDNode != null)
                            {
                                throw new Exception("Invalid XML file!");
                            }
                            reader.MoveToAttribute("path");
                            directory = new DirectoryInfo(reader.Value);
                            reader.MoveToAttribute("hasYearLevel");
                            _hasYearLevel = Convert.ToBoolean(reader.Value);

                            rootIDNode = new MyTreeNode(directory.Name) {Tag = new DirNodeData(directory)};
                            rootIDNode.Expand();

                            treeView.Nodes.Add(rootIDNode);
                            directoryNodes.Add(rootIDNode);
                            break;
                        case "NewsID":
                            if (rootIDNode == null)
                            {
                                throw new Exception("Invalid XML file!");
                            }
                            reader.MoveToAttribute("path");

                            directory = new DirectoryInfo(reader.Value);
                            currentNewsIDNode = new MyTreeNode(directory.Name) {Tag = new DirNodeData(directory)};
                            currentNewsIDNode.Expand();
                            directoryNodes.Add(currentNewsIDNode);
                            rootIDNode.Nodes.Add(currentNewsIDNode);

                            break;
                        case "Year":
                            if (currentNewsIDNode == null)
                            {
                                throw new Exception("Invalid XML file!");
                            }
                            reader.MoveToAttribute("path");

                            directory = new DirectoryInfo(reader.Value);
                            currentYearNode = new MyTreeNode(directory.Name) {Tag = new DirNodeData(directory)};
                            currentYearNode.Expand();
                            directoryNodes.Add(currentYearNode);
                            currentNewsIDNode.Nodes.Add(currentYearNode);

                            break;
                        case "Issue":
                            var parentNode = _hasYearLevel ? currentYearNode : currentNewsIDNode;
                            if (parentNode == null)
                            {
                                throw new Exception("Invalid XML file!");
                            }


                            reader.MoveToAttribute("path");
                            string path = reader.Value;
                            //Trace.WriteLine("Issue: " + reader.Value);

                            directory = new DirectoryInfo(reader.Value);
                            var dirNodeData = new DirNodeData(directory);
                            currentIssueNode = new MyTreeNode(directory.Name) {Tag = dirNodeData};

                            if (reader.MoveToAttribute("language"))
                            {
                                dirNodeData.Language = reader.Value;
                            }
                            //if (reader.MoveToAttribute("size"))
                            //{
                            //    dirNodeData.Size = reader.Value;
                            //}
                            if (reader.MoveToAttribute("texttype"))
                            {
                                dirNodeData.Texttype = reader.Value;
                            }

                            currentIssueNode.Collapse();
                            directoryNodes.Add(currentIssueNode);
                            parentNode.Nodes.Add(currentIssueNode);

                            // --- read files from issue folder:

                            // 1st: read files from xml:
                            reader.MoveToElement();
                            reader.ReadToDescendant("Image");
                            var currentXmlFiles = new List<FileInfo>();
                            do
                            {
                                var imageNode = ReadXMLImageNode(ref reader);
                                var imageFile = ((FileNodeData) imageNode.Tag).FileInfo;
                                if (!imageFile.Exists)
                                {
                                    //throw new FileNotFoundException("File does not exist anymore: " + imageFile.FullName);
                                    var pars = new object[4]
                                                   {
                                                       "warning", "File", imageFile.FullName,
                                                       "File does not exist anymore"
                                                   };
                                    _gui.Invoke(new GuiAddErrorDelegate(_gui.AddFileError), pars);
                                    deletedCount++;
                                    continue;
                                }

                                currentXmlFiles.Add(((FileNodeData) imageNode.Tag).FileInfo);
                                globalFileCount++;

                                fileNodes.Add(imageNode);
                                currentIssueNode.HiddenNodes.Add(imageNode);
                                reader.MoveToElement();
                            } while (reader.ReadToNextSibling("Image"));
                            reader.MoveToElement();
                            // now check for new files in this directory:
                            var files = ParseFiles(new DirectoryInfo(path), exts, false);
                            foreach (FileInfo fi in files)
                            {
                                if (currentXmlFiles.Exists(fi2 => fi2.FullName.Equals(fi.FullName)))
                                    continue; // continue if file already there

                                var pars = new object[4] {"warning", "File", fi.FullName, "New file found"};
                                _gui.Invoke(new GuiAddErrorDelegate(_gui.AddFileError), pars);

                                Trace.WriteLine("xml-read: a new file was found: " + fi.FullName);
                                var imageNode = new MyTreeNode(fi.Name) {Tag = new FileNodeData(fi)};
                                fileNodes.Add(imageNode);
                                currentIssueNode.HiddenNodes.Add(imageNode);
                                globalFileCount++;
                                newFilesCount++;
                            }
                            // --- end read files from issue folder

                            break;
                            //case "Image":
                            //    if (currentIssueNode == null)
                            //    {
                            //        throw new Exception("Invalid XML file!");
                            //    }

                            //    currentFileNode = ReadXMLImageNode(ref reader);
                            //    file = ((FileNodeData)currentFileNode.Tag).FileInfo;

                            //    fileNodes.Add(currentFileNode);
                            //    currentIssueNode.HiddenNodes.Add(currentFileNode);
                            //    break;
                    } // end switch
                    if (directory != null)
                    {
                        if (!directory.Exists)
                        {
                            throw new FileNotFoundException("Directory does not exist anymore: " +
                                                            directory.FullName);
                        }
                    }
                } // end while read

                //_treeView.ExpandAll();

                reader.Close();

                Trace.WriteLine("globalFileCount = " + globalFileCount + " deletedCount = " + deletedCount +
                                " newFilesCount =" + newFilesCount);

                // print summary if new or deleted files:
                if (deletedCount > 0 || newFilesCount > 0)
                {
                    var appendErrorMessageDelegate = new GuiAppendErrorMessageDelegate(_gui.AppendErrorMessage);

                    _gui.Invoke(appendErrorMessageDelegate,
                                new object[1] {"------------------- XML-Read summary: -------------------"});
                    _gui.Invoke(appendErrorMessageDelegate,
                                new object[1]
                                    {"Nr. of new files: " + newFilesCount + ", nr. of deleted files: " + deletedCount});
                    _gui.Invoke(appendErrorMessageDelegate,
                                new object[1] {"---------------------------------------------------------"});
                }

                //throw new NotImplementedException();
            }
            catch (Exception e)
            {
                reader.Close();
                throw e;
            }
        } // end ReadXMLOutput
#endif

        private MyTreeNode ReadXMLImageNode(ref XmlReader reader)
        {
            reader.MoveToAttribute("path");
            var file = new FileInfo(reader.Value);
            var currentFileNode = new MyTreeNode(file.Name) { Tag = new FileNodeData(file) };

            reader.MoveToAttribute("status");
            var nd = ((FileNodeData)currentFileNode.Tag);
            nd.Status = reader.Value;

            if (reader.MoveToAttribute("message"))
            {
                nd.Message = reader.Value;
            }

            if (reader.MoveToAttribute("errorType"))
            {
                nd.Type = (ErrorType) Enum.Parse(typeof (ErrorType), reader.Value);
                _errors[nd.Type]++;
            }

            string stat = reader.Value;
            if (!stat.Equals("unchecked")) // metadata must be present
            {
                nd.Metadata = new ImageMetadata();
                reader.MoveToAttribute("width");
                nd.Metadata.Width = Convert.ToUInt32(reader.Value);
                reader.MoveToAttribute("height");
                nd.Metadata.Height = Convert.ToUInt32(reader.Value);
                reader.MoveToAttribute("xres");
                nd.Metadata.XRes = Convert.ToDouble(reader.Value);
                reader.MoveToAttribute("yres");
                nd.Metadata.YRes = Convert.ToDouble(reader.Value);
                if (reader.MoveToAttribute("bitdepth"))
                {
                    //Trace.WriteLine("biddepth str = "+reader.Value);
                    string[] values = reader.Value.Split(new char[] { '(', ')', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    //Trace.WriteLine("here: "+values.Count());
                    nd.Metadata.Bitdepth = new uint[values.Length];
                    for (int i = 0; i < values.Length; ++i)
                    {
                        //Trace.WriteLine("value " + i + "= " + values[i]);
                        nd.Metadata.Bitdepth[i] = Convert.ToUInt32(values[i]);
                    }
                }

                reader.MoveToAttribute("color_type");
                nd.Metadata.Colortype = (ImageColorType)Enum.Parse(typeof(ImageColorType), reader.Value);
                reader.MoveToAttribute("checksum");
                nd.Metadata.Checksum = reader.Value;
                reader.MoveToAttribute("size");
                nd.Metadata.Size = Convert.ToInt64(reader.Value);
                reader.MoveToAttribute("mimetype");
                nd.Metadata.Mimetype = reader.Value;
                if (reader.MoveToAttribute("compression"))
                {
                    //nd.Metadata.Compression = (TiffCompressionType) Convert.ToInt32(reader.Value);
                    nd.Metadata.Compression = (TiffCompressionType) Enum.Parse(typeof(TiffCompressionType), reader.Value);
                }
                if (reader.MoveToAttribute("endian"))
                {
                    nd.Metadata.Endian = (EndianType)Enum.Parse(typeof (EndianType), reader.Value);
                }
            }

            return currentFileNode;
        } // end ReadXMLImageNode

        public int GetNExistingViewingFiles()
        {
            return _viewing_file_statuses.Count(p => p.Value == 1);
        }

        public int GetNNeededViewingFiles()
        {
            return _viewing_file_statuses.Count();
        }

        public int GetNMissingViewingFiles()
        {
            return GetNNeededViewingFiles() - GetNExistingViewingFiles();
        }

        public Dictionary<string, int> GetViewingFileStatuses()
        {
            return _viewing_file_statuses;
        }

        public bool HasViewingFile(string file)
        {
            if (!_viewing_file_statuses.ContainsKey(file)) return false;

            return _viewing_file_statuses[file] == 1;
        }

        private void ComputeViewingFileStatuses()
        {
            Trace.WriteLine("computing viewing files statuses...");
            if (_treeView.Nodes.Count == 0)
            {
                Trace.WriteLine("computing viewing files statuses: no nodes found!");
                return;
            }       

            var rootDir = ((DirNodeData) _treeView.Nodes[0].Tag).DirInfo;
            var rootDirUri = new Uri(rootDir.FullName+"\\");
            //Trace.WriteLine("rootDirUri = "+rootDirUri.ToString());

            _viewing_file_statuses = new Dictionary<string, int>();
            foreach (var node in _fileNodes)
            {
                var dat = (FileNodeData) node.Tag;
                var fi = dat.FileInfo;
                //Trace.WriteLine("fi uri = " + new Uri(fi.FullName).ToString());
                string uid = getNewsIdNameFromFile(fi);
                //Trace.WriteLine("ComputeViewingFileStatuses: UID = " + uid + " for file: " + fi.FullName);
                var dt = _gui.getMasterListDataTable();
                DataRow[] result = dt.Select("UID = '" + uid + "'");
                if (result.Count() != 1) // should never happen!
                {
                    throw new Exception("Fatal exception while parsing masterlist: ambigious UID!");
                }
                const string colName = @"TELPresentation";
                int colIndex = dt.Columns.IndexOf(colName);
                //Trace.WriteLine("col index = "+colIndex);
                var value = result[0][colIndex] ?? "";
                var colStr = value.ToString().Trim();

                if (!colStr.Equals("1") && ! colStr.Equals("2"))
                {
                    if (!colStr.Any())
                        _gui.Invoke(new GuiAppendErrorMessageDelegate(_gui.AppendErrorMessage), "WARNING: TELPresentation column not set for newspaper with UID " + uid + " file: " + fi.FullName);

                    //Trace.WriteLine("no viewing files for this file!");
                    continue;
                }

                // CHECK FOR VIEWING FILES:
                string relativePath = rootDirUri.MakeRelativeUri(new Uri(fi.FullName)).ToString();
                //Trace.WriteLine("relativePath = " + relativePath);
                string viewingFn = rootDir.FullName + "\\viewing_files\\" + rootDir.Name + '\\' + relativePath;
                viewingFn = viewingFn.Replace('/', Path.DirectorySeparatorChar);
                //Trace.WriteLine("viewing file: " + viewingFn);

                var exts = new string[] {"jpg", "png", "gif"};
                int stat = 0;
                if (exts.Any(e => File.Exists(Path.ChangeExtension(viewingFn, e))))
                {
                    stat = 1;
                }
                _viewing_file_statuses.Add(fi.FullName, stat);
            }
            Trace.WriteLine("computing viewing files statuses done");
        }

        public string getNewsIdNameFromFile(FileInfo fi)
        {
            int nsteps = 2;
            if (!_hasYearLevel) nsteps = 1;

            var dir = fi.Directory;
            if (dir == null) return "";
            for (int i = 0; i < nsteps; ++i )
                dir = Directory.GetParent(dir.FullName);

            return dir.Name;
        }

        public int getLastFolderLevel()
        {
            return _hasYearLevel ? 3 : 2;
        }

        public bool hasYearLevel()
        {
            return _hasYearLevel;
        }

        public int getNFiles()
        {
            return _fileNodes.Count;
        }
        public int getNFolders()
        {
            return _directoryNodes.Count;
        }
        //public int getNodeCountAtLevel(int level)
        //{
        //    return _directoryNodes.Count(n => n.Level == level);
        //}

        public int getYearLevel()
        {
            return _hasYearLevel ? 2 : -1;
        }
        public int getIssueLevel()
        {
            return _hasYearLevel ? 3 : 2;
        }

        public int getNNewspaperFolders()
        {
            return _directoryNodes.Count(n => n.Level == 1);
        }

        public int getNYearFolders()
        {
            if (!_hasYearLevel) return 0;
            else return _directoryNodes.Count(n => n.Level == 2);
        }

        public int getNIssueFolders()
        {
            int issueLevel = _hasYearLevel ? 3 : 2;
            return _directoryNodes.Count(n => n.Level == issueLevel);
        }

        public int getNCompletedFileCheck()
        {
            return _completedFileChecks;
        }

        public int getNIssueFoldersWithMetadata()
        {
            return (from TreeNode n in _directoryNodes where n.Level == getIssueLevel() select (DirNodeData) n.Tag).Count(nd => nd.Language != null && nd.Texttype != null);
        }

        public int getNIssueFoldersWithoutMetadata()
        {
            return getNIssueFolders() - getNIssueFoldersWithMetadata();
        }

        public double getAverageTimePerCheck()
        {
            return _averageTimePerCheck;
        }
        public double getExpectedRestTime()
        {
            return _averageTimePerCheck*(getNFiles() - getNCompletedFileCheck());
        }

        public TimeSpan getElapsedTime()
        {
            return _stopW.Elapsed;
        }

        public List<FileNodeData> getErrorsAndWarnings()
        {
            return _fileNodes.Select(n => (FileNodeData)n.Tag).Where(nd => nd.Status.Equals("error") || nd.Status.Equals("warning")).ToList();
        }

        public int getNCheckedFiles()
        {
            return _fileNodes.Select(n => ((FileNodeData)n.Tag).Status).Count(stat => stat.Equals("checked"));
        }

        public int getNFileErrors()
        {
            return _fileNodes.Select(n => ((FileNodeData)n.Tag).Status).Count(stat => stat.Equals("error"));
        }

        public int getNFileWarnings()
        {
            return _fileNodes.Select(n => ((FileNodeData)n.Tag).Status).Count(stat => stat.Equals("warning"));
        }

        //public int getNErrors()
        //{
        //    return (from ErrorType t in Enum.GetValues(typeof(ErrorType)) where t != ErrorType.None select _errors[t]).Sum();
        //}

        public Dictionary<ErrorType, int> getErrorCount()
        {
            return _errors;
        }



        public List<MyTreeNode>  getDirectoryNodes()
        {
            return _directoryNodes;
        }

        //public Dictionary<string, int> getViewingFileStatuses()
        //{
        //    return _viewing_file_statuses;
        //}
    }
}
