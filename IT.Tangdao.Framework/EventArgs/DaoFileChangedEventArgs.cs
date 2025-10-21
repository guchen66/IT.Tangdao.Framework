using IT.Tangdao.Framework.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.EventArg
{
    public class DaoFileChangedEventArgs : EventArgs
    {
        public string FilePath { get; }
        public DaoFileType FileType { get; }
        public WatcherChangeTypes ChangeType { get; }
        public string OldContent { get; }
        public string NewContent { get; }
        public string ChangeDetails { get; }
        public DateTime ChangeTime { get; }

        public DaoFileChangedEventArgs(string filePath, DaoFileType fileType, WatcherChangeTypes changeType,
                                  string oldContent, string newContent, string changeDetails)
        {
            FilePath = filePath;
            FileType = fileType;
            ChangeType = changeType;
            OldContent = oldContent;
            NewContent = newContent;
            ChangeDetails = changeDetails;
            ChangeTime = DateTime.Now;
        }
    }
}