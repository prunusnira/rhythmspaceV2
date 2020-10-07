using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace BMSCore
{
    public class LoadBMSList
    {
        public List<string> GetDirectories(string path)
        {
            List<string> dirs = new List<string>();
            dirs.Add(path);
            foreach(string s in
                Directory.GetDirectories(path, "*", SearchOption.AllDirectories))
            {
                dirs.Add(s);
            }

            return dirs;
        }

        public List<string> GetFiles(List<string> dirs)
        {
            List<string> files = new List<string>();
            foreach (string subdir in dirs)
            {
                string[] bmsfiles = Directory.GetFiles(subdir, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(s => s.ToLower().EndsWith(".bms") || s.ToLower().EndsWith(".bme")).ToArray();

                foreach(string bms in bmsfiles)
                {
                    files.Add(bms);
                }
            }

            return files;
        }

        public MusicListData LoadBms(string path, int count)
        {
            BMS bms = new BMS(path);
            BMSAnalyzer analyzer = new BMSAnalyzer();
            analyzer.HeaderAnalyzer(bms);
            if (bms.Player != 1) return null;

            MusicListData data = new MusicListData(
                    count, bms.Title, bms.SubTitle,
                    bms.Artist, bms.SubArtist, bms.Gerne,
                    bms.BPMStart, bms.BPMMin, bms.BPMMax,
                    bms.FolderPath, bms.Level, bms.Difficulty,
                    bms.FileName, bms.StageFile
                );

            return data;
        }
    }
}
