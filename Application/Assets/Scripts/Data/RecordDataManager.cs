using DatabaseManager;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace BMSPlayer
{
    public class RecordDataManager
    {
        MD5 md5;

        public RecordDataManager()
        {
            md5 = MD5.Create();
        }

        public void Close()
        {
            SQLiteExecutor.Instance.closeDB();
            md5.Clear();
        }

        public void CreateNewTable()
        {
            SQLiteExecutor.Instance.InitRecordTable();
        }

        public void DropTable()
        {
            SQLiteExecutor.Instance.DropRecord();
        }

        public void RegisterRecord(RecordData data)
        {
            SQLiteExecutor.Instance.InsertRecord(
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
            return SQLiteExecutor.Instance.SelectRecordClear(hash);
        }

        public int GetPlayCnt(string hash)
        {
            return SQLiteExecutor.Instance.SelectRecordPlayCnt(hash);
        }

        public RecordData GetFullClearStat(string hash)
        {
            return SQLiteExecutor.Instance.SelectRecord(hash);
        }
    }
}
