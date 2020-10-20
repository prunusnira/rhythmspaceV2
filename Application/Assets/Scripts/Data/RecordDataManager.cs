using DatabaseManager;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace BMSPlayer
{
    public class RecordDataManager
    {
        SQLiteExecutor executor;
        MD5 md5;

        public RecordDataManager()
        {
            executor = SQLiteExecutor.getInstance();
            md5 = MD5.Create();
        }

        public void Close()
        {
            executor.closeDB();
            md5.Clear();
        }

        public void CreateNewTable()
        {
            executor.InitRecordTable();
        }

        public void DropTable()
        {
            executor.DropRecord();
        }

        public void RegisterRecord(RecordData data)
        {
            executor.InsertRecord(
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
                data.CBreak
            );
        }

        public int GetClearStat(string hash)
        {
            return executor.SelectRecordClear(hash);
        }

        public RecordData GetFullClearStat(string hash)
        {
            return executor.SelectRecord(hash);
        }
    }
}
