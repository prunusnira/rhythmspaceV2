using DatabaseManager;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace BMSPlayer
{
    public class RecordDataManager
    {
        private static RecordDataManager instance;
        public static RecordDataManager Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new RecordDataManager();
                }
                return instance;
            }
        }

        public void Close()
        {
            SQLiteRecord.Instance.closeDB();
        }

        public void CreateNewTable()
        {
            SQLiteRecord.Instance.InitRecordTable();
        }

        public void DropTable()
        {
            SQLiteRecord.Instance.DropRecord();
        }

        public void RegisterRecord(RecordData data)
        {
            SQLiteRecord.Instance.InsertRecord(
                data.MD5Hash,
                data.Rank,
                data.Score,
                (int)data.GaugeType,
                (int)data.Clear,
                data.Perfect,
                data.Great,
                data.Good,
                data.OK,
                data.Poor,
                data.MaxCombo,
                data.CBreak,
                data.PlayCnt
            );
        }

        public int GetClearStat(string hash)
        {
            return SQLiteRecord.Instance.SelectRecordClear(hash);
        }

        public int GetPlayCnt(string hash)
        {
            return SQLiteRecord.Instance.SelectRecordPlayCnt(hash);
        }

        public RecordData GetFullClearStat(string hash)
        {
            return SQLiteRecord.Instance.SelectRecord(hash);
        }

        public List<string> GetRankHash(string rank)
        {
            return SQLiteRecord.Instance.SelectRankHash(rank);
        }
    }
}
