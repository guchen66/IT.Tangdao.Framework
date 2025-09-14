using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.Tangdao.Framework.Parameters.EventArg
{
    public class XmlFileChangedEventArgs : EventArgs
    {
        public string FilePath { get; }

        public WatcherChangeTypes ChangeType { get; }

        public string OldContent { get; }

        public string NewContent { get; }

        public string ChangeDetails { get; }

        public XmlFileChangedEventArgs(string filePath, WatcherChangeTypes changeType, string oldContent = null, string newContent = null, string changeDetails = null)
        {
            FilePath = filePath;
            ChangeType = changeType;
            OldContent = oldContent;
            NewContent = newContent;
            ChangeDetails = changeDetails;
        }
    }
}