namespace BMSCore
{
    public class MusicListData {
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Artist
        {
            get { return artist; }
            set { artist = value; }
        }

        public double BPMstart
        {
            get { return bpmstart; }
            set { bpmstart = value; }
        }

        public double BPMmin
        {
            get { return bpmmin; }
            set { bpmmin = value; }
        }

        public double BPMmax
        {
            get { return bpmmax; }
            set { bpmmax = value; }
        }

        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        public string MD5Hash
        {
            get { return md5hash; }
            set { md5hash = value; }
        }

        public int Level
        {
            get { return level; }
            set { level = value; }
        }

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        public string Jacket
        {
            get { return jacket; }
            set { jacket = value; }
        }

        public string Gerne
        {
            get { return gerne; }
            set { gerne = value; }
        }

        private int id;
        private string name;
        private string artist;
        private double bpmstart;
        private double bpmmin;
        private double bpmmax;
        private string path;
        private string md5hash;
        private int level;
        private string fileName;
        private string jacket;
        private string gerne;

        public MusicListData() { }

        public MusicListData(int id, string name, string artist, string gerne,
            double bpmstart, double bpmmin, double bpmmax,
            string path, string md5hash, int level, string fileName, string jacket)
        {
            Id = id;
            Name = name;
            Artist = artist;
            BPMstart = bpmstart;
            BPMmin = bpmmin;
            BPMmax = bpmmax;
            Path = path;
            MD5Hash = md5hash;
            Level = level;
            FileName = fileName;
            Jacket = jacket;
            Gerne = gerne;
        }

        public MusicListData(int id, string name, string artist, string gerne,
            double bpmstart, double bpmmin, double bpmmax,
            string path, int level, string fileName, string jacket)
        {
            Id = id;
            Name = name;
            Artist = artist;
            BPMstart = bpmstart;
            BPMmin = bpmmin;
            BPMmax = bpmmax;
            Path = path;
            Level = level;
            FileName = fileName;
            Jacket = jacket;
            Gerne = gerne;
        }
    }
}