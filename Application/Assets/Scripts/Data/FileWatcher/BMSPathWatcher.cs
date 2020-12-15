using System.Collections.Generic;
using System.IO;

namespace BMSPlayer
{
    class BMSPathWatcher
    {
        private BMSPathWatcher instance;
        public BMSPathWatcher Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new BMSPathWatcher();
                }
                return instance;
            }
        }

        private HashSet<string> addlist;
        private HashSet<string> rmlist;

        private FileSystemWatcher watcher;

        public void Init(string path)
        {
            watcher = new FileSystemWatcher();
            watcher.Path = path + "\\";

            watcher.NotifyFilter = NotifyFilters.FileName;
            watcher.IncludeSubdirectories = true;

            watcher.Created += new FileSystemEventHandler(OnFileAdded);
            watcher.Deleted += new FileSystemEventHandler(OnFileDeleted);

            addlist = new HashSet<string>();
            rmlist = new HashSet<string>();
        }

        private void OnFileAdded(object sender, FileSystemEventArgs e)
        {
            if(e.FullPath.ToLower().EndsWith(".bms") ||
                e.FullPath.ToLower().EndsWith(".bme") ||
                e.FullPath.ToLower().EndsWith(".bml"))
            {
                addlist.Add(e.FullPath);
            }
        }

        private void OnFileDeleted(object sender, FileSystemEventArgs e)
        {
            if (e.FullPath.ToLower().EndsWith(".bms") ||
                e.FullPath.ToLower().EndsWith(".bme") ||
                e.FullPath.ToLower().EndsWith(".bml"))
            {
                rmlist.Add(e.FullPath);
            }
        }
    }
}
