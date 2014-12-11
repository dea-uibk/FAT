using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FAT
{
    class Report
    {
        private List<string> _directoryErrors = new List<string>();
        private List<string> _filenameErrors = new List<string>();
        private Dictionary<string, string> _checksums = new Dictionary<string, string>();

        public Report()
        {
        }

        public void AddDirectoryError(DirectoryInfo dir)
        {
            _directoryErrors.Add(dir.FullName);
        }

        public void AddFilenameError(FileInfo file)
        {
            _filenameErrors.Add(file.FullName);
        }

        public void AddChecksum(FileInfo file, string cs)
        {
            _checksums.Add(file.FullName, cs);
        }

        public List<String> DirectoryErrors
        {
            get { return _directoryErrors; }
        }

        public List<String> FilenameErrors
        {
            get { return _filenameErrors; }
        }

        public Dictionary<string, string> Checksums
        {
            get { return _checksums; }
        }

        public void saveReport()
        {
            
        }
    }
}
