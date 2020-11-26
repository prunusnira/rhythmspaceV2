namespace BMSCore
{
    public class DiffTableData {
        public string Title { get; set; }
        public string Artist { get; set; }
        public int Level { get; set; }
        public string URL { get; set; }
        public string MD5 { get; set; }

        public DiffTableData(string title, string artist, int level,
            string url, string md5)
        {
            Title = title;
            Artist = artist;
            Level = level;
            URL = url;
            MD5 = md5;
        }
    }
}