namespace BMSPlayer
{
    public class RecordData
    {
        public string MD5Hash { get; set; }
        public string Rank { get; set; }
        public int Score { get; set; }
        public GaugeType GaugeType { get; set; }
        public ClearType Clear { get; set; }
        public int Perfect { get; set; }
        public int Great { get; set; }
        public int Good { get; set; }
        public int OK { get; set; }
        public int Poor { get; set; }
        public int MaxCombo { get; set; }
        public int CBreak { get; set; }

        public RecordData(
            string md5hash, string rank,
            int score, int judge,
            int clear, int pf, int gr, int gd, int ok,
            int pr, int combo, int cb)
        {
            MD5Hash = md5hash;
            Rank = rank;
            Score = score;
            GaugeType = (GaugeType)judge;
            Clear = (ClearType)clear;
            Perfect = pf;
            Great = gr;
            Good = gd;
            OK = ok;
            Poor = pr;
            MaxCombo = combo;
            CBreak = cb;
        }
    }
}
