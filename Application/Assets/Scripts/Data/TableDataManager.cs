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
            if (!SQLiteTable.Instance.CheckDiffTableExist(DiffTableMode.SATELLITE))
                SQLiteTable.Instance.InitDiffSl();

            if (!SQLiteTable.Instance.CheckDiffTableExist(DiffTableMode.STELLA))
                SQLiteTable.Instance.InitDiffSt();

            if (!SQLiteTable.Instance.CheckDiffTableExist(DiffTableMode.GENONM))
                SQLiteTable.Instance.InitDiffGeNM();

            if (!SQLiteTable.Instance.CheckDiffTableExist(DiffTableMode.GENOINS))
                SQLiteTable.Instance.InitDiffGeINS();
        }

        private bool isExecutorNull()
        {
            return (SQLiteTable.Instance == null);
        }

        public void Close()
        {
            SQLiteTable.Instance.closeDB();
            md5.Clear();
        }

        public List<int> GetTableLevelList(DiffTableMode type)
        {
            return SQLiteTable.Instance.GetTableLevelList(type);
        }

        public MusicListData GetMusicOfMD5Hash(string hash)
        {
            return SQLiteMusicList.Instance.GetMusicOfMD5Hash(hash);
        }

        public MusicListData GetMusicOfTitle(string title)
        {
            return SQLiteMusicList.Instance.GetMusicOfTitle(title);
        }

        public List<DiffTableData> LoadTableStSlByLevel(int lv, DiffTableMode mode)
        {
            return SQLiteTable.Instance.GetDiffTableStSl(lv, mode);
        }

        public List<DiffTableData> LoadTableGeByLevel(int lv, DiffTableMode mode)
        {
            return SQLiteTable.Instance.GetDiffTableGe(lv, mode);
        }

        public void AddDataToDB(
            List<DiffTableData> list,
            DiffTableMode? mode,
            ref string strLoading)
        {
            if (list.Count != 0)
            {
                // drop
                switch (mode)
                {
                    case DiffTableMode.SATELLITE:
                        SQLiteTable.Instance.DropTableSl();
                        break;
                    case DiffTableMode.STELLA:
                        SQLiteTable.Instance.DropTableSt();
                        break;
                    case DiffTableMode.GENONM:
                        SQLiteTable.Instance.DropTableGeNM();
                        break;
                    case DiffTableMode.GENOINS:
                        SQLiteTable.Instance.DropTableGeINS();
                        break;
                }

                List<DiffTableData> dataList = new List<DiffTableData>();
                // 리스트의 각 파일을 DB에 등록(이 때 MD5 Hash값도 계산)
                for (int i = 0; i < list.Count; i++)
                {
                    strLoading = "Gathering data to register to database (" + (i + 1) + "/" + list.Count + ")";
                    DiffTableData d = list[i];
                }
                strLoading = "Registering into Database";

                switch (mode)
                {
                    case DiffTableMode.GENOINS:
                    case DiffTableMode.GENONM:
                        SQLiteTable.Instance.InsertDiffTableGe((DiffTableMode)mode, list);
                        break;
                    case DiffTableMode.SATELLITE:
                    case DiffTableMode.STELLA:
                        SQLiteTable.Instance.InsertDiffTableStSl((DiffTableMode)mode, list);
                        break;
                }
            }
        }
    }
}