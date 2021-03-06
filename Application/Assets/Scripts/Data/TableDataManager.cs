﻿using BMSCore;
using DatabaseManager;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace BMSPlayer
{
    public class TableDataManager
    {
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
            DiffTableMode? mode)
        {
            if (list.Count != 0)
            {
                // drop
                switch (mode)
                {
                    case DiffTableMode.SATELLITE:
                        SQLiteTable.Instance.DropTableSl();
                        SQLiteTable.Instance.InitDiffSl();
                        break;
                    case DiffTableMode.STELLA:
                        SQLiteTable.Instance.DropTableSt();
                        SQLiteTable.Instance.InitDiffSt();
                        break;
                    case DiffTableMode.GENONM:
                        SQLiteTable.Instance.DropTableGeNM();
                        SQLiteTable.Instance.InitDiffGeNM();
                        break;
                    case DiffTableMode.GENOINS:
                        SQLiteTable.Instance.DropTableGeINS();
                        SQLiteTable.Instance.InitDiffGeINS();
                        break;
                }

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