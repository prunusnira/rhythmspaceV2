﻿namespace BMSCore
{
    public class MusicListData {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Artist { get; set; }
        public string SubArtist { get; set; }
        public double BPMstart { get; set; }
        public double BPMmin { get; set; }
        public double BPMmax { get; set; }
        public string Path { get; set; }
        public string MD5Hash { get; set; }
        public int Level { get; set; }
        public int Difficulty { get; set; }
        public string FileName { get; set; }
        public string Jacket { get; set; }
        public string Gerne { get; set; }
        public int TotalNotes { get; set; }
        public int PlayTime { get; set; }
        public int Rank { get; set; }
        public bool LNExist { get; set; }
        public string Preview { get; set; }

        public MusicListData() { }

        public MusicListData(string title, string subtitle,
            string artist, string subartist, string gerne,
            double bpmstart, double bpmmin, double bpmmax,
            string path, string md5hash, int level, int diff,
            string fileName, string jacket, int totalNotes, int playtime,
            int rank, bool ln, string preview)
        {
            Title = title;
            SubTitle = subtitle;
            Artist = artist;
            SubArtist = subartist;
            BPMstart = bpmstart;
            BPMmin = bpmmin;
            BPMmax = bpmmax;
            Path = path;
            MD5Hash = md5hash;
            Level = level;
            Difficulty = diff;
            FileName = fileName;
            Jacket = jacket;
            Gerne = gerne;
            TotalNotes = totalNotes;
            PlayTime = playtime;
            Rank = rank;
            LNExist = ln;
            Preview = preview;
        }
    }
}