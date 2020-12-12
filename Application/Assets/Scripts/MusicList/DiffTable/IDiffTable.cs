namespace BMSPlayer
{
    public enum DiffTableMode
    {
        STELLA = 0,
        SATELLITE = 1,
        GENONM = 2,
        GENOINS = 3
    }

    public interface IDiffTable
    {
        void CrawlTable();
        bool IsWorkDone();
    }
}
