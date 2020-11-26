using BMSCore;
using DatabaseManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace BMSPlayer
{
    public class TableDataManager
    {
        MD5 md5;

        private static TableDataManager instance;
        public static TableDataManager Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new TableDataManager();
                }
                if(instance.isExecutorNull())
                {
                    instance.Close();
                    instance = new TableDataManager();
                }
                return instance;
            }
        }

        private TableDataManager()
        {
            md5 = MD5.Create();
            if (!SQLiteExecutor.Instance.CheckDiffTableSlExist())
                SQLiteExecutor.Instance.InitDiffSl();

            if (!SQLiteExecutor.Instance.CheckDiffTableStExist())
                SQLiteExecutor.Instance.InitDiffSt();

            if (!SQLiteExecutor.Instance.CheckDiffTableGeExist())
                SQLiteExecutor.Instance.InitDiffGe();
        }

        private bool isExecutorNull()
        {
            return (SQLiteExecutor.Instance == null);
        }

        public void Close()
        {
            SQLiteExecutor.Instance.closeDB();
            md5.Clear();
        }

        public List<int> GetTableLevelList(int type)
        {
            return SQLiteExecutor.Instance.GetTableLevelList(type);
        }

        public MusicListData GetMusicOfMD5Hash(string hash)
        {
            return SQLiteExecutor.Instance.GetMusicOfMD5Hash(hash);
        }

        public MusicListData GetMusicOfTitle(string title)
        {
            return SQLiteExecutor.Instance.GetMusicOfTitle(title);
        }

        public List<DiffTableData> LoadTableStSlByLevel(int lv, DiffTableMode mode)
        {
            return SQLiteExecutor.Instance.GetDiffTableStSl(lv, mode);
        }

        public List<DiffTableData> LoadTableGeByLevel(int lv)
        {
            return SQLiteExecutor.Instance.GetDiffTableGe(lv);
        }

        public void AddDataToDB(
            List<DiffTableData> list,
            DiffTableMode? mode,
            ref string strLoading)
        {
            if (list.Count != 0)
            {
                // drop
                if (mode == null)
                {
                    SQLiteExecutor.Instance.DropTableGe();
                }
                else if(mode == DiffTableMode.STELLA)
                {
                    SQLiteExecutor.Instance.DropTableSt();
                }
                else if (mode == DiffTableMode.SATELLITE)
                {
                    SQLiteExecutor.Instance.DropTableSl();
                }

                List<DiffTableData> dataList = new List<DiffTableData>();
                // 리스트의 각 파일을 DB에 등록(이 때 MD5 Hash값도 계산)
                for (int i = 0; i < list.Count; i++)
                {
                    strLoading = "Gathering data to register to database (" + (i + 1) + "/" + list.Count + ")";
                    DiffTableData d = list[i];
                }
                strLoading = "Registering into Database";
                if(mode == null)
                {
                    SQLiteExecutor.Instance.InsertDiffTableGe(list);
                }
                else
                {
                    SQLiteExecutor.Instance.InsertDiffTableStSl((DiffTableMode)mode, list);
                }
            }
        }
    }
}