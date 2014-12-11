using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using DataTable = System.Data.DataTable;

namespace FAT
{
    
    public delegate void GuiDelegateJobFinished();
    public delegate void GuiDelegateAllJobsFinished();

    public delegate void GuiAddErrorDelegate(string errorType, string dataType, string filename, string message);
    public delegate void GuiAppendErrorMessageDelegate(string message);
    //public delegate void allJobsFinishedDelegate();

    public partial class FATMainForm : Form
    {
        private const string PROGRAM_TITLE = "FAT - File Analyzer Tool";
        private const string VERSION = "1.0.9";
        private const string CONTACT = "sebastian.colutto@uibk.ac.at";

        private const bool DO_FILE_LOG = true;
        private const string MASTERLIST_FN = "Masterlist.xlsx";
        private const int MAX_N_LANGUAGES = 4;

        //Stopwatch sw = new Stopwatch();
        private Timer _timer = new Timer();
        FATChecker _fatChecker = null;
        private DataTable _masterListDataTable = null;

        public FATMainForm()
        {
            InitializeComponent();

            //string fn = @"X:\tmp_sebastian\4FAT\28_ica1_1.tif";
            //ImageMetadata metadata = new ImageMetadata();
            //TifUtils.ReadTifHeader(fn, ref metadata);
            //Trace.WriteLine("metadata = "+metadata.ToString());

            //string file = @"C:\newsPaperTestData\Bnf\Bnf_00001\2000\20000101\file_000.jp2";
            //var stream = new FileStream(file, FileMode.Open);
            //string str1 = Checksummer.CheckSumString(stream);
            //stream.Close();
            //stream = new FileStream(file, FileMode.Open);
            //stream.ReadByte();
            //stream.Position = 0;

            //string str2 = Checksummer.CheckSumString(stream);
            //stream.Close();

            //Trace.WriteLine("cs1 = "+str1);
            //Trace.WriteLine("cs2 = " + str2);
        }

        private void InitLogging(bool doFileLog)
        {
            if (doFileLog)
            {
                Trace.Listeners.Add(new TextWriterTraceListener(new FileStream("logfile.txt", FileMode.Create)));
            }
            //Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            Trace.AutoFlush = true;
        }

        private void Init()
        {
            //Text = PROGRAM_TITLE + " - Version " + VERSION + ", Buildtime: " + File.GetCreationTime(Assembly.GetExecutingAssembly().Location);
            Text = PROGRAM_TITLE + " - Version " + VERSION;

            Trace.WriteLine("hello to the fat, time: "+DateTime.Now);
            //this.Text += ", v. "+VERSION;

            this.Closing += this.FATMainForm_Close;

            progressBar.Step = 1;
            this.EnableFileCheckButtons(true);
            this.startButton.Enabled = false;
            messageInfoLabel.Text = "";
            timeLabel.Text = "";

            loadedDataLabel.Text = "";
            fileStatusesLabel.Text = "";
            fileCheckStatusLabel.Text = "";
            metadataStatusLabel.Text = "";
            viewingFilesStatusLabel.Text = "";

            UpdateDataInfo();

            //directoryTreeView.NodeMouseClick += this.OnNodeSelected;
            directoryTreeView.AfterSelect += this.OnNodeSelected;

            for (int i = 1; i <= 100; i++)
                nThreadsComboBox.Items.Add(i);
            nThreadsComboBox.SelectedIndex = 1;

            _timer.Interval = 500;
            _timer.Tick += TimerTick;

            // test:
            //this.inputFolderTextBox.Text = Directory.GetCurrentDirectory();
            //this.inputFolderTextBox.Text = @"C:\test_in_structure";

            //this.inputFolderTextBox.Text = @"X:\tmp_sebastian\newspaperDummyData";
            //this.inputFolderTextBox.Text = @"C:\newsPaperTestData";
            //inputFolderTextBox.Text = @"E:\newsPaperTestData";


            //UpdateTreeView();

            InitLogging(DO_FILE_LOG);

            reloadMasterListButton_Click(this, null);
        }

        //private void readMasterList()
        //{
        //    if (!File.Exists(MASTERLIST_FN))
        //    {
        //        MessageBox.Show(this, MASTERLIST_FN + " not found!", "Error reading file", MessageBoxButtons.OK,
        //                        MessageBoxIcon.Error);
        //        Environment.Exit(1);
        //    }

        //    var fi = new FileInfo(MASTERLIST_FN);
        //    masterListInfoLabel.Text = fi.LastWriteTime.ToString("yyyy-MM-dd HH:mm");

        //    try
        //    {
        //        //throw new Exception("testing COM...");
        //        _masterListDataTable = readMasterListOLEDB();
        //    }
        //    //catch (InvalidOperationException e)
        //    catch (Exception e)
        //    {
        //        Trace.WriteLine("Cannot read masterlist with OLEB driver... using COM!");
        //        _masterListDataTable = readMasterListCOM();
        //    }
        //}

        //private DataTable readMasterListCOM()
        //{
        //    Trace.WriteLine("reading masterlist using COM...");
        //    // benötigte Objekte vorbereiten
        //    Microsoft.Office.Interop.Excel.Application excel = null;
        //    Workbook wb = null;
        //    //try
        //    //{
        //        // Excel starten
        //        excel = new Microsoft.Office.Interop.Excel.Application();
        //        excel.Visible = false;

        //        // Datei öffnen
        //        wb = excel.Workbooks.Open(
        //            new FileInfo(MASTERLIST_FN).FullName,
        //            ExcelKonstanten.UpdateLinks.DontUpdate,
        //            ExcelKonstanten.ReadOnly,
        //            ExcelKonstanten.Format.Nothing,
        //            "", // Passwort
        //            "", // WriteResPasswort
        //            ExcelKonstanten.IgnoreReadOnlyRecommended,
        //            XlPlatform.xlWindows,
        //            "", // Trennzeichen
        //            ExcelKonstanten.Editable,
        //            ExcelKonstanten.DontNotifiy,
        //            ExcelKonstanten.Converter.Default,
        //            ExcelKonstanten.DontAddToMru,
        //            ExcelKonstanten.Local,
        //            ExcelKonstanten.CorruptLoad.NormalLoad);

        //        // Arbeitsblätter lesen
        //        Sheets sheets = wb.Worksheets;
                

        //        // ein Arbeitsblatt auswählen…
        //        //Worksheet ws = (Worksheet)sheets.get_Item("owssvr");
        //        Worksheet ws = (Worksheet)sheets[1];
        //        ws.Select();

        //        DataTable dt = new DataTable();
        //        var xlRange = ws.UsedRange;
        //        if (xlRange != null)
        //        {
        //            int nRows = xlRange.Rows.Count;
        //            int nCols = xlRange.Columns.Count;
        //            //Console.Write("nRows = " + nRows);
        //            //Console.Write("nRows = " + nCols);

        //            // parse 1st row:
        //            var cols = new object[nCols];
                    
        //            for (int j = 1; j <= nCols; ++j )
        //            {
        //                var r = (Microsoft.Office.Interop.Excel.Range)ws.Cells[1, j];
        //                dt.Columns.Add(r.Value2);
        //            }

        //            var rowData = new object[nCols];
        //            for (int i = 2; i <= nRows; ++i )
        //            {
        //                for (int j=1; j<=nCols; ++j)
        //                {
        //                    var r = (Microsoft.Office.Interop.Excel.Range)ws.Cells[i, j];
        //                    rowData[j - 1] = r.Value2;
        //                }
        //                dt.Rows.Add(rowData);
        //            }
        //            Trace.WriteLine("readMasterListCOM: converted masterlist to datatable!");

        //            //DataRow[] result = dt.Select("UID = 'KB_00031'");
        //            //Console.WriteLine("result size = " + result.Count());
        //            //foreach (DataRow r in result)
        //            //{
        //            //    //Console.WriteLine("result: ");
        //            //    foreach (var i in r.ItemArray)
        //            //    {
        //            //        Console.Write(i + " ");
        //            //    }
        //            //}
        //        }
        //    //}
        //    //catch (Exception e)
        //    //{
        //    //    Console.WriteLine(e.Message);
        //    //}
        //    //finally
        //    //{
        //    //    wb.Close(false, null, null);
        //    //    excel.Quit();
        //    //}

        //    wb.Close(false, null, null);
        //    excel.Quit();

        //    return dt;
        //}

        //private DataTable readMasterListOLEDB()
        //{
        //    Trace.WriteLine("reading masterlist using OLEDB...");
        //    //try
        //    //{
        //        //var connectionString =
        //            //string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0}; Extended Properties=Excel 12.0;HDR=Yes;IMEX=1;",
        //                          //"c:\\Masterlist.xlsx");
        //        //var connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 12.0 Xml;HDR=YES;IMEX=1;'", "c:\\Masterlist.xlsx");
        //        var connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 12.0 Xml;HDR=YES;IMEX=1;'", MASTERLIST_FN);
        //        //Trace.WriteLine("readMasterListOLEDB: connectionString: " + connectionString);

        //        var con = new OleDbConnection(connectionString);
        //        var command = new OleDbCommand();
        //        var dt = new DataTable();
        //        con.Open();
        //        var dtSchema = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
        //        var sheet1Name = dtSchema.Rows[0].Field<string>("TABLE_NAME");
        //        //Trace.WriteLine("sheet1Name = "+sheet1Name);
        //        var myCommand = new OleDbDataAdapter("select * from ["+sheet1Name+"]", con);
        //        //var myCommand = new OleDbDataAdapter("select * from [owssvr$]", con);
        //        myCommand.Fill(dt);
        //        //DataRow[] result = dt.Select("UID = 'KB_00031'");
        //        //Console.WriteLine("result size = "+result.Count());
        //        //foreach (DataRow r in result)
        //        //{
        //        //    Console.WriteLine("result: ");    
        //        //    foreach (var i in r.ItemArray)
        //        //    {
        //        //        Console.Write(i + " ");
        //        //    }
        //        //}
        //        //Console.WriteLine(); 
        //    //}
        //    //catch (Exception e)
        //    //{
        //    //    Console.WriteLine("Message: "+e.Message);
        //    //    Console.WriteLine(e.StackTrace);
        //    //}

        //    return dt;
        //}

        private bool IsValidLanguageString(string inputStr)
        {
            var languages = inputStr.Split(',');

            return languages.All(l => languageClb.Items.Contains(l));
        }

        private bool IsValidTexttype(string inputStr)
        {
            return texttypeComboBox.Items.Contains(inputStr);
        }

        private void CheckLanguagesFromString(string inputStr)
        {
            for (int i = 0; i < languageClb.Items.Count; ++i)
            {
                var l = (string)languageClb.Items[i];
                languageClb.SetItemChecked(i, inputStr != null && inputStr.Contains(l));
            }
        }

        private int GetNrOfSelectedLanguages()
        {
            int count = 0;
            for (int i = 0; i < languageClb.Items.Count; ++i)
            {
                if (languageClb.GetItemChecked(i))
                {
                    count++;
                }
            }
            return count;
        }

        private string GetSelectedLanguagesString() {
            string languageStr = "";
            for (int i = 0; i < languageClb.Items.Count; ++i)
            {
                if (languageClb.GetItemChecked(i))
                {
                    if (languageStr.Count()!=0)
                        languageStr += ",";
                    languageStr += languageClb.Items[i];
                }
            }
            if (languageStr.Equals("")) return null;
            return languageStr;
        }

        private void OnNodeSelected(object sender, TreeViewEventArgs e)
        {
            if (_fatChecker == null) return;

            var selNode = directoryTreeView.SelectedNode;
            if (selNode == null) return;

            nodeInfoTextBox.Clear();
            if (selNode.Level >=0 && selNode.Level <=_fatChecker.getLastFolderLevel())
            {
                var nd = (DirNodeData)selNode.Tag;
                editorGroupBox.Visible = true;

                CheckLanguagesFromString(nd.Language);
                texttypeComboBox.SelectedItem = nd.Texttype;

                nodeInfoTextBox.AppendText("Path: " + nd.DirInfo.FullName + Environment.NewLine);

                if (selNode.Level == _fatChecker.getIssueLevel())
                    nodeInfoTextBox.AppendText("Nr. of files in this issue: " + ((MyTreeNode)selNode).HiddenNodes.Count);
                else
                    nodeInfoTextBox.AppendText("Nr. of subdirectories: " + ((MyTreeNode)selNode).Nodes.Count);
            }
            else if (selNode.Level > _fatChecker.getLastFolderLevel())
            {
                var nd = (FileNodeData)selNode.Tag;
                editorGroupBox.Visible = false;

                //nodeInfoTextBox.AppendText();
                nodeInfoTextBox.AppendText("Path: " + nd.FileInfo.FullName + Environment.NewLine);
                nodeInfoTextBox.AppendText("Status: " + nd.Status + Environment.NewLine);
                nodeInfoTextBox.AppendText("Message: " + nd.Message + Environment.NewLine);
                
                if (nd.Metadata!=null)
                {
                    nodeInfoTextBox.AppendText("Metadata: " + nd.Metadata.ToString() + Environment.NewLine);
                }
            }
        }


        private void FATMainForm_Load(object sender, EventArgs e)
        {
            Init();
        }

        private void FATMainForm_Close(object sender, EventArgs e)
        {
            Trace.WriteLine("Closing!!");
            this.StopButtonClick(this, null);
        }

        private void inputFolderButton_Click(object sender, EventArgs e)
        {
            if (_fatChecker != null)
            {
                if (MessageBox.Show("Unsaved data will be lost - resume?", "Do you really want to load new data", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                {
                    return;
                }
            }

            if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
            {
                this.inputFolderTextBox.Text = folderBrowserDialog1.SelectedPath;
                UpdateTreeView();
            }
        }

        private void reloadButton_Click(object sender, EventArgs e)
        {
            if (_fatChecker != null)
            {
                if (MessageBox.Show("Unsaved data will be lost - resume?", "Do you really want to load new data", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                {
                    return;
                }
            }

            UpdateTreeView();
        }

        private List<string> GetExts()
        {
            var list = new List<string>();
            if (tifCb.Checked)
            {
                list.Add("tif");
                list.Add("tiff");
            }
            if (jpgCb.Checked)
            {
                list.Add("jpg");
                list.Add("jpeg");
                //list.Add("jpe");
                //list.Add("jfif");
            }
            if (jpg2000Cb.Checked)
            {
                list.Add("jp2");
                list.Add("j2k");
                //list.Add("jpf");
                //list.Add("jpg2");
                //list.Add("jpx");
                //list.Add("jpm");
                //list.Add("m2j");
                //list.Add("mjp2");
            }

            return list;
        }

        public static List<string> ParseInputFiles(string dirin, List<string> exts)
        {
            var allFiles = new List<string>();

            foreach (string ext in exts)
            {
                try
                {
                    string[] files = Directory.GetFiles(dirin, "*." + ext, SearchOption.AllDirectories);
                    allFiles.AddRange(files);
                }
                catch (DirectoryNotFoundException e)
                {
                    MessageBox.Show(@"Cannot find input directory: " + e.Message);
                    return new List<string>();
                }
            }

            return allFiles;
        }

#if true // use standard update tree view
        private void UpdateTreeView()
        {
            reloadMasterListButton_Click(this, null);

            Enabled = false;
            EnableFileCheckButtons(false);
            try
            {
                errorLogTextBox.Clear();
                _fatChecker = new FATChecker(this, inputFolderTextBox.Text, GetExts(), ref directoryTreeView, hasYearCb.Checked, _masterListDataTable);

                UpdateErrors();
                //directoryTreeView.ExpandAll();


                EnableDirViewButtons(true);
                messageInfoLabel.Text = "Successfully parsed folder structure - nr. of folders = " + _fatChecker.getNFolders()
                    + ", nr. of files = " + _fatChecker.getNFiles();
                _fatChecker.UpdateTreeViewColors();
                directoryTreeView.SelectedNode = directoryTreeView.Nodes[0];
            }
            catch (DirectoryNotFoundException e)
            {
                _fatChecker = null;
                MessageBox.Show(e.Message, "No valid directory", MessageBoxButtons.OK,
                                   MessageBoxIcon.Error);
                messageInfoLabel.Text = e.Message;

                directoryTreeView.Nodes.Clear();
                EnableDirViewButtons(false);
            }
            catch (InvalidDataException e)
            {
                _fatChecker = null;
                Trace.Write(e.Message);

                MessageBox.Show(e.Message, "Error while parsing folder structure", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                messageInfoLabel.Text = e.Message;

                directoryTreeView.Nodes.Clear();
                EnableDirViewButtons(false);

            }
            finally
            {
                EnableFileCheckButtons(true);
                startButton.Enabled = (_fatChecker != null);
                UpdateDataInfo();
                this.Enabled = true;
                messageInfoLabel.Text = "";
                UpdateFolderIcons();
                progressBar.Value = 0;


            }
        } // end UpdateTreeView
#else // use background worker
        private void UpdateTreeView()
        {
            Enabled = false;
            EnableFileCheckButtons(false);
            errorLogTextBox.Clear();
            var pars =
            new object[]
                {this, inputFolderTextBox.Text,
                    GetExts(), directoryTreeView, hasYearCb.Checked, _masterListDataTable
                };

            backgroundWorkerLoadDirectory.RunWorkerAsync(pars);
        } // end UpdateTreeView

        //[DebuggerNonUserCodeAttribute]
        private void backgroundWorkerLoadDirectory_DoWork(object sender, DoWorkEventArgs e)
        {
            var pars = (object[]) e.Argument;
            var tv = (TreeView) pars[3];
            tv.Enabled = false;

            FATChecker fatChecker = null;
            try
            {
                fatChecker = new FATChecker((FATMainForm) pars[0], (string) pars[1], (List<string>) pars[2], ref tv,
                                                (bool) pars[4], (DataTable) pars[5]);
                e.Result = fatChecker;
            }
            catch (Exception ex)
            {
                throw ex;
            }   
        }

        private void backgroundWorkerLoadDirectory_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                    throw e.Error;

                _fatChecker = e.Result as FATChecker;

                EnableDirViewButtons(true);
                messageInfoLabel.Text = "Successfully parsed folder structure - nr. of folders = " + _fatChecker.getNFolders()
                    + ", nr. of files = " + _fatChecker.getNFiles();
                _fatChecker.UpdateTreeViewColors();
                directoryTreeView.SelectedNode = directoryTreeView.Nodes[0];
            }
            catch (DirectoryNotFoundException ex)
            {
                _fatChecker = null;
                MessageBox.Show(ex.Message, "No valid directory", MessageBoxButtons.OK,
                                   MessageBoxIcon.Error);
                messageInfoLabel.Text = ex.Message;

                directoryTreeView.Nodes.Clear();
                EnableDirViewButtons(false);
            }
            catch (InvalidDataException ex)
            {
                _fatChecker = null;
                Trace.Write(ex.Message);

                MessageBox.Show(ex.Message, "Error while parsing folder structure", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                messageInfoLabel.Text = ex.Message;

                directoryTreeView.Nodes.Clear();
                EnableDirViewButtons(false);

            }
            finally
            {
                EnableFileCheckButtons(true);
                startButton.Enabled = (_fatChecker != null);
                UpdateDataInfo();
                this.Enabled = true;
                messageInfoLabel.Text = "";
                UpdateFolderIcons();
                progressBar.Value = 0;
            }            
        }
#endif

        public void UpdateFolderIcons()
        {
            if (_fatChecker == null) return;

            // root node:
            directoryTreeView.Nodes[0].SelectedImageIndex = 3;
            directoryTreeView.Nodes[0].ImageIndex = 3;
            // other:
            foreach (MyTreeNode n in _fatChecker.getDirectoryNodes())
            {
                int index = 0;
                
                if (n.Level == 0) // root
                {
                    index = 1;
                }
                else if (n.Level == 1) // newspaper
                {
                    index = 2;
                }
                //else if(n.Level == 2) // newspaper
                //{
                //    index = 2;
                //}
                //else if (n.Level >=3) // year, issue
                //{
                //    index = 0;
                //}

                n.SelectedImageIndex = index;
                n.ImageIndex = index;
            }


        }

        private void StartButtonClick(object sender, EventArgs e)
        {
            if (_fatChecker == null) return;

            messageInfoLabel.Text = "";
            errorLogTextBox.Clear();

            progressBar.Minimum = 0;
            progressBar.Maximum = _fatChecker.getNFiles();
            progressBar.Value = 0;


            //int nThreads = Environment.ProcessorCount;
            int nThreads = Convert.ToInt32(nThreadsComboBox.Text);
            _timer.Start();
            tabControl1.SelectedIndex = 0;
            if (_fatChecker.getNFiles() == 0) return;

            _fatChecker.CheckFiles(nThreads, stopOnFirstError.Checked, recheckCb.Checked, doPlausibilityCheckCb.Checked, calculateChecksumCb.Checked);
            EnableFileCheckButtons(false);
        }

        private void StopButtonClick(object sender, EventArgs e)
        {
            if (_fatChecker == null) return;

            _fatChecker.StopThreads();
            //StopFileCheck();
        }

        //public void StopFileCheck()
        //{
        //    _fatChecker.StopThreads();
        //    AllJobsFinished();            
        //}

        public void AddFileError(string errorType, string dataType, string name, string message)
        {
            errorLogTextBox.AppendText(errorType.ToUpper()+": "+dataType+": "+name+", Message: "+message+Environment.NewLine);

            

        }
        public void AppendErrorMessage(string message)
        {
            errorLogTextBox.AppendText(message+Environment.NewLine);
        }

        private void PrintFileErrorSummary()
        {
            if (_fatChecker == null || _fatChecker.getNFileErrors()==0) return;

            errorLogTextBox.AppendText("------------------- File error summary: -------------------"+Environment.NewLine);
            var errorCount = _fatChecker.getErrorCount();
            foreach (ErrorType t in Enum.GetValues(typeof(ErrorType)))
            {
                if (t == ErrorType.None) continue;
                string message = "Nr. of " + Enum.GetName(typeof (ErrorType), t) + " errors: " + errorCount[t];
                errorLogTextBox.AppendText(message+Environment.NewLine+Environment.NewLine);
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            UpdateTime(false);
        }

        public void JobFinished()
        {
            progressBar.PerformStep();
            messageInfoLabel.Text = "Finished file " + progressBar.Value + " of " + progressBar.Maximum;

            //UpdateTime(false);
        }

        public void AllJobsFinished()
        {
            _timer.Stop();

            string output = "Finished " + _fatChecker.getNCompletedFileCheck() + "/" + _fatChecker.getNFiles()+" file checks";
            Trace.WriteLine(output);
            messageInfoLabel.Text = output;

            UpdateTime(true);
            _fatChecker.UpdateTreeViewColors();
            PrintFileErrorSummary();
            
            EnableFileCheckButtons(true);
            UpdateDataInfo();
        }

        public void UpdateTime(bool finished)
        {
            TimeSpan tsElapsed = _fatChecker.getElapsedTime();
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", tsElapsed.Hours, tsElapsed.Minutes, tsElapsed.Seconds,
                                                                                                tsElapsed.Milliseconds / 10);

            timeLabel.Text = "Elapsed time: " + elapsedTime;
            timeLabel.Text += ", Avg. time per check: " + decimal.Round((decimal)_fatChecker.getAverageTimePerCheck(), 2) + " secs";

            if (finished) return;
            double rt = _fatChecker.getExpectedRestTime();
            int h = (int)(rt / 3600);
            int m = (int)(rt / 60 - h * 60);
            int s = (int)rt - h * 3600 - m * 60;
            string restTime = String.Format("{0:00}:{1:00}:{2:00}", h, m, s);
            timeLabel.Text += ", estimated rest time: " + restTime;
        }

        private void EnableFileCheckButtons(bool value)
        {
            startButton.Enabled = value;
            stopButton.Enabled = !value;
            //this.refreshButton.Enabled = value;
            inputFolderButton.Enabled = value;
            loadButton.Enabled = value;
            this.saveButton.Enabled = value;
            this.directoryTreeView.Enabled = value;
            //this.tabControl1.Enabled = value;
            this.metadataTabPage.Enabled = value;
            this.reloadButton.Enabled = value;
            this.expandIssue.Enabled = value;

            this.progressBar.Enabled = !value;
        }

        private void EnableDirViewButtons(bool value)
        {
            this.startButton.Enabled = value;
            //this.loadButton.Enabled = value;
            this.saveButton.Enabled = value;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (_fatChecker == null) return;

#if true // use fixed xml out name
            var fn = _fatChecker.getDefaultXMLFileName();
            try
            {
                this.Enabled = false;
                _fatChecker.WriteXMLOutput(fn);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.StackTrace);
                MessageBox.Show(this, ex.Message, "Error saving to XML", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Enabled = true;
            }
#else
            saveFileDialog1.AddExtension = true;
            saveFileDialog1.DefaultExt = ".xml";
            saveFileDialog1.Title = "Specify XML file where to store results";
            saveFileDialog1.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
            if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                this.Enabled = false;
                string fn = saveFileDialog1.FileName;
                Trace.WriteLine("Saving XML file: "+fn);
                try
                {
                    _fatChecker.WriteXMLOutput(fn);    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Error saving to XML", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    this.Enabled = true;    
                }
            }
#endif

        }

#if false
        private void loadButton_Click(object sender, EventArgs e)
        {
            if (_fatChecker != null)
            {
                if (MessageBox.Show("Unsaved data will be lost - resume?", "Do you really want to load new data", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                {
                    return;
                }
            }

            openFileDialog1.Title = "Specify XML file where results are stored";
            openFileDialog1.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                string fn = openFileDialog1.FileName;
                Trace.WriteLine("Loading XML file: "+fn);

                Enabled = false;
                EnableFileCheckButtons(false);
                errorLogTextBox.Clear();
                try
                {
                    _fatChecker = new FATChecker(this, fn, ref directoryTreeView, GetExts());
                    directoryTreeView = _fatChecker.GetTreeView();
                    hasYearCb.Checked = _fatChecker.hasYearLevel();
                    var rootNodeData = (DirNodeData)directoryTreeView.Nodes[0].Tag;
                    inputFolderTextBox.Text = rootNodeData.DirInfo.FullName;
                    
                    UpdateErrors();

                    messageInfoLabel.Text = "Successfully parsed folder structure from XML - nr. of folders = " + _fatChecker.getNFolders()
                    + ", nr. of files = " + _fatChecker.getNFiles();
                    EnableDirViewButtons(true);
                    expandIssue_Click(null, null);
                    _fatChecker.UpdateTreeViewColors();
                    directoryTreeView.SelectedNode = directoryTreeView.Nodes[0];
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error while loading XML file", MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                    _fatChecker = null;
                    EnableDirViewButtons(false);
                    messageInfoLabel.Text = ex.Message;
                    directoryTreeView.Nodes.Clear();
                    EnableFileCheckButtons(true);
                    EnableDirViewButtons(false);
                }
                finally
                {
                    this.Enabled = true;
                    EnableFileCheckButtons(true);
                    UpdateDataInfo();
                    UpdateFolderIcons();
                }
            }
        }
#endif

        private void UpdateDataInfo()
        {
            //Trace.WriteLine("UpdateDataInfo");
            //metadataTabPage.Enabled = false;
            metadataTabPage.Enabled = true;
            if (_fatChecker == null)
            {
                loadedDataLabel.Text = "";
                fileStatusesLabel.Text = "";
                fileCheckStatusLabel.Text = "";
                metadataStatusLabel.Text = "";
                viewingFilesStatusLabel.Text = "";
                //Trace.WriteLine("clearing ");
                return;
            }



            loadedDataLabel.Text = "Nr. of newspaper folders: " + _fatChecker.getNNewspaperFolders();
            loadedDataLabel.Text += ", Nr. of year folders: " + _fatChecker.getNYearFolders();
            loadedDataLabel.Text += ", Nr. of issue folders: " + _fatChecker.getNIssueFolders();
            loadedDataLabel.Text += ", Nr. of files: " + _fatChecker.getNFiles();
            int nExistingViewingFiles = _fatChecker.GetNExistingViewingFiles();
            int nNeededViewingFiles = _fatChecker.GetNNeededViewingFiles();

            int checkedFiles = _fatChecker.getNCheckedFiles();
            int fileErrors = _fatChecker.getNFileErrors();
            int fileWarnings = _fatChecker.getNFileWarnings();

            fileStatusesLabel.Text = "Nr. of issue folders with metadata: " + _fatChecker.getNIssueFoldersWithMetadata() + "/" + _fatChecker.getNIssueFolders();
            fileStatusesLabel.Text += ", Nr. of checked and valid files: " + (checkedFiles + fileWarnings) + "/" + _fatChecker.getNFiles();
            fileStatusesLabel.Text += ", Nr. of viewing files: " + nExistingViewingFiles + "/" + nNeededViewingFiles;
            fileStatusesLabel.Text += ", Nr. of file errors: " + fileErrors;
            fileStatusesLabel.Text += ", Nr. of file warnings: " + fileWarnings;

            if (nExistingViewingFiles != nNeededViewingFiles)
            {
                viewingFilesStatusLabel.ForeColor = Color.Red;
                viewingFilesStatusLabel.Text = "There are viewing files missing!";
            }
            else
            {
                viewingFilesStatusLabel.ForeColor = Color.Green;
                viewingFilesStatusLabel.Text = "All viewing files are there!";
            }

            if (checkedFiles+fileWarnings != _fatChecker.getNFiles() || fileErrors > 0)
            {
                fileCheckStatusLabel.ForeColor = Color.Red;
                fileCheckStatusLabel.Text = "There are missing filechecks or errors in checked files!";
            }
            else
            {
                metadataTabPage.Enabled = true;
                fileCheckStatusLabel.ForeColor = Color.Green;
                fileCheckStatusLabel.Text = "All files checked and valid!";
            }

            if (_fatChecker.getNIssueFoldersWithoutMetadata()!=0)
            {
                metadataStatusLabel.ForeColor = Color.Red;
                metadataStatusLabel.Text = "There are issues with missing metadata!";
            }
            else
            {
                metadataStatusLabel.ForeColor = Color.Green;
                metadataStatusLabel.Text = "All issue folders contain metadata!";
            }
        }

        private void showMissingViewingFiles_Click(object sender, EventArgs e)
        {
            if (_fatChecker == null) return;
            var statuses = _fatChecker.GetViewingFileStatuses();

            errorLogTextBox.Clear();
            this.tabControl1.SelectedIndex = 0;
            foreach (var stat in statuses)
            {
                if (stat.Value == 0)
                    AppendErrorMessage("Viewing file missing for: "+stat.Key);
            }

            errorLogTabPage.Select();
            tabControl1.SelectedIndex = 0;
        }

        private void UpdateErrors()
        {
            if (_fatChecker == null) return;

            var errors = _fatChecker.getErrorsAndWarnings();
            //this.errorLogTextBox.Clear();
            foreach (FileNodeData nd in errors)
            {
                AddFileError(nd.Status, "File", nd.FileInfo.FullName, nd.Message);
            }
            PrintFileErrorSummary();
        }

        private void expandAll_Click(object sender, EventArgs e)
        {
            if (directoryTreeView.Nodes.Count == 0) return;

            directoryTreeView.ExpandAll();
        }

        private void expandIssue_Click(object sender, EventArgs e)
        {
            if (_fatChecker == null) return;
            if (directoryTreeView.Nodes.Count == 0) return;

            var nodes = new Stack<TreeNode>();
            nodes.Push(directoryTreeView.Nodes[0]);

            while (nodes.Count != 0)
            {
                TreeNode currentNode = nodes.Pop();
                if (currentNode.Level < _fatChecker.getLastFolderLevel()) currentNode.Expand();
                else currentNode.Collapse();

                foreach (TreeNode n in currentNode.Nodes)
                {
                    nodes.Push(n);
                }
            }
        }

        private void okSetMetadataButton_Click(object sender, EventArgs e)
        {
            if (directoryTreeView.SelectedNode == null) return;
            if (directoryTreeView.SelectedNode.Level > _fatChecker.getLastFolderLevel())
            {
                Trace.WriteLine("Errror - selected node is not a directory node - should not happen!");
                return;
            }
            if (GetNrOfSelectedLanguages() > MAX_N_LANGUAGES)
            {
                MessageBox.Show(this, "Cannot set more than 4 languages - deselect some entries", "Error setting metadata", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return;
            }

            UpdateDirectoryNodeData(directoryTreeView.SelectedNode, GetSelectedLanguagesString(), (string)texttypeComboBox.SelectedItem, false);
            UpdateDataInfo();
        }

        private void setMetadataAndPopulateButton_Click(object sender, EventArgs e)
        {
            if (directoryTreeView.SelectedNode == null) return;
            if (directoryTreeView.SelectedNode.Level > _fatChecker.getLastFolderLevel())
            {
                Trace.WriteLine("Errror - selected node is not a directory node - should not happen!");
                return;
            }
            if (GetNrOfSelectedLanguages() > MAX_N_LANGUAGES)
            {
                MessageBox.Show(this, "Cannot set more than 4 languages - deselect some entries", "Error setting metadata", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return;
            }

            UpdateDirectoryNodeData(directoryTreeView.SelectedNode, GetSelectedLanguagesString(), (string)texttypeComboBox.SelectedItem, true);
            UpdateDataInfo();
        }

        private void UpdateDirectoryNodeData(TreeNode rootNode, string language, string texttype, bool populateDown)
        {
            //Trace.WriteLine("rootNode, Level = "+rootNode.Level + " l = "+language+" sizie = "+size+" texttype = "+texttype);
            if (_fatChecker == null) return;


            if (rootNode.Level == _fatChecker.getLastFolderLevel())
            {
                ((DirNodeData) rootNode.Tag).Language = language;
                //((DirNodeData) rootNode.Tag).Size = size;
                ((DirNodeData) rootNode.Tag).Texttype = texttype;

                if (language == null && texttype == null)
                {
                    rootNode.ForeColor = Color.Red;
                }
                else if (language == null || texttype == null)
                {
                    rootNode.ForeColor = Color.DarkKhaki;
                }
                else
                {
                    rootNode.ForeColor = Color.Green;
                }
            }

            foreach (TreeNode n in rootNode.Nodes)
            {
                if (n.Level >= 0 && n.Level <= _fatChecker.getLastFolderLevel())
                {
                    UpdateDirectoryNodeData(n, language, texttype, populateDown);
                }
            }


            if (rootNode.NextNode != null && populateDown)
                UpdateDirectoryNodeData(rootNode.NextNode, language, texttype, populateDown);
        }

        public DataTable getMasterListDataTable()
        {
            return _masterListDataTable; 
        }

        private void reloadMasterListButton_Click(object sender, EventArgs e)
        {
            try
            {
                //throw new Exception("bla");

                _masterListDataTable = MasterlistReader.ReadMasterList(MASTERLIST_FN);
                var fi = new FileInfo(MASTERLIST_FN);
                masterListInfoLabel.Text = fi.LastWriteTime.ToString("yyyy-MM-dd HH:mm");

                //readMasterList();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.StackTrace);
                MessageBox.Show(this, "Error message: " + ex.Message + Environment.NewLine + Environment.NewLine + "Please try to install and download the Office 2007 system drivers from here:" + Environment.NewLine + "http://www.microsoft.com/en-us/download/confirmation.aspx?id=23734", "Unable to read "+MASTERLIST_FN,
                                MessageBoxButtons.OK, MessageBoxIcon.Stop);

                Environment.Exit(1);
            }
        }

        private void applyMetadataFromMasterlist_Click(object sender, EventArgs e)
        {
            if (_fatChecker == null) return;

            Trace.WriteLine("Applying metadata from masterlist...");
            //Stopwatch sw=new Stopwatch();

            foreach (var n in _fatChecker.getDirectoryNodes())
            {
                if (n.Level != 1) continue;

                var dirname = ((DirNodeData) n.Tag).DirInfo.Name;
                //Trace.WriteLine("dirname = " + dirname);

                //sw.Restart();
                var row = _masterListDataTable.Select("UID = '" + dirname + "'");
                //sw.Stop();
                //Trace.WriteLine("time for select: "+sw.Elapsed);
                if (row.Count() != 1) // should never happen!
                {
                    throw new Exception("Fatal exception while parsing masterlist: ambigious UID!");
                }

                var language = (string)row[0][_masterListDataTable.Columns.IndexOf("Language")];
                var texttype = (string)row[0][_masterListDataTable.Columns.IndexOf("FontType")];
                //Trace.WriteLine("dirname = "+dirname+" language = "+language+" texttype = "+texttype);

                //var determinedTextType = DetermineTexttypeFromMasterlistString(texttype);
                texttype = texttype.Replace(" ", string.Empty);
                texttype = texttype.Replace(";#", ",");
                language = language.Replace(" ", string.Empty);
                language = language.Replace(";#", ",");
                //Trace.WriteLine("isvalidlanguage: "+language+": "+IsValidLanguageString(language));

                UpdateDirectoryNodeData(n, IsValidLanguageString(language) ? language : null, IsValidTexttype(texttype) ? texttype : "Normal", false);
            }

            UpdateDataInfo();
            OnNodeSelected(this, null);
            Trace.WriteLine("Applying metadata from masterlist done");
        }

        //private string DetermineTexttypeFromMasterlistString(string str)
        //{
        //    string[] strs = str.Split(',');
        //    if (strs.Count() == 1)
        //        return strs[0];

        //    if (strs.Any(s => s.Trim().Equals("Gothic")))
        //    {
        //        return "Combined";
        //    }
        //    return "Antiqua";
        //}



    }
}
