using BMSCore;
using BMSPlayer;
using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using UnityEngine;

namespace DatabaseManager
{
    public class SQLiteTable
    {
        public IDbConnection dbconn;
        public IDbCommand dbcommand;
        public IDataReader dbreader;

        private static SQLiteTable instance;

        public static SQLiteTable Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SQLiteTable();
                }
                else
                {
                    if (!instance.checkDBConn())
                    {
                        instance = new SQLiteTable();
                    }
                }
                return instance;
            }
        }

        private SQLiteTable()
        {
            connectDB(Const.TableDBPath);
        }

        public void connectDB(string dbfile)
        {
            if(!File.Exists(dbfile))
            {
                Directory.CreateDirectory(Directory.GetParent(dbfile).FullName);
                SqliteConnection.CreateFile(dbfile);
            }

            dbconn = new SqliteConnection("Data Source="+dbfile);
            dbcommand = new SqliteCommand();
            dbconn.Open();
        }

        public bool checkDBConn()
        {
            if (dbconn == null) return false;
            else return true;
        }

        public void closeDB()
        {
            if(dbcommand != null)
            {
                dbcommand.Dispose();
                dbconn.Close();
                dbcommand = null;
                dbconn = null;
            }
        }

        public void InitDiffSl()
        {
            dbcommand = dbconn.CreateCommand();
            string table =
                @"create table diffSl (
                id integer primary key autoincrement,
                md5hash text unique not null,
                title text not null,
                artist text not null,
                url text not null,
                level integer not null)";

            dbcommand.CommandText = table;
            dbcommand.ExecuteNonQuery();
        }

        public void InitDiffSt()
        {
            dbcommand = dbconn.CreateCommand();
            string table =
                @"create table diffSt (
                id integer primary key autoincrement,
                md5hash text unique not null,
                title text not null,
                artist text not null,
                url text not null,
                level integer not null)";

            dbcommand.CommandText = table;
            dbcommand.ExecuteNonQuery();
        }

        public void InitDiffGeNM()
        {
            dbcommand = dbconn.CreateCommand();
            string table =
                @"create table diffGeNM (
                id integer primary key autoincrement,
                md5hash text unique not null,
                title text not null,
                artist text not null,
                url text not null,
                level integer not null)";

            dbcommand.CommandText = table;
            dbcommand.ExecuteNonQuery();
        }

        public void InitDiffGeINS()
        {
            dbcommand = dbconn.CreateCommand();
            string table =
                @"create table diffGeINS (
                id integer primary key autoincrement,
                md5hash text unique not null,
                title text not null,
                artist text not null,
                url text not null,
                level integer not null)";

            dbcommand.CommandText = table;
            dbcommand.ExecuteNonQuery();
        }

        public void DropTableSl()
        {
            dbcommand = dbconn.CreateCommand();
            string query = @"drop table if exists diffSl";
            dbcommand.CommandText = query;
            dbcommand.ExecuteNonQuery();
        }

        public void DropTableSt()
        {
            dbcommand = dbconn.CreateCommand();
            string query = @"drop table if exists diffSt";
            dbcommand.CommandText = query;
            dbcommand.ExecuteNonQuery();
        }

        public void DropTableGeNM()
        {
            dbcommand = dbconn.CreateCommand();
            string query = @"drop table if exists diffGeNM";
            dbcommand.CommandText = query;
            dbcommand.ExecuteNonQuery();
        }

        public void DropTableGeINS()
        {
            dbcommand = dbconn.CreateCommand();
            string query = @"drop table if exists diffGeINS";
            dbcommand.CommandText = query;
            dbcommand.ExecuteNonQuery();
        }

        public void InsertDiffTableStSl(DiffTableMode type, List<DiffTableData> paramList)
        {
            dbcommand = dbconn.CreateCommand();

            for (int i = 0; i < paramList.Count; i++)
            {
                DiffTableData param = paramList[i];
                param.Title = param.Title.Replace("'", "''");
                param.Artist = param.Artist.Replace("'", "''");
            }

            string table = "";
            switch(type)
            {
                case DiffTableMode.SATELLITE: table = "diffSl"; break;
                case DiffTableMode.STELLA: table = "diffSt"; break;
            }

            string query =
                "insert into " + table +
                " (title, artist, level, url, md5hash) values ";

            foreach (DiffTableData param in paramList)
            {
                query += "('" +
                        param.Title + "','" +
                        param.Artist + "'," +
                        param.Level + ",'" +
                        param.URL + "','" +
                        param.MD5 + "')";

                if (paramList.IndexOf(param) != paramList.Count - 1)
                {
                    query += ",";
                }
            }

            dbcommand.CommandText = query;
            dbcommand.ExecuteNonQuery();
        }

        public void InsertDiffTableGe(DiffTableMode type, List<DiffTableData> paramList)
        {
            dbcommand = dbconn.CreateCommand();

            for (int i = 0; i < paramList.Count; i++)
            {
                DiffTableData param = paramList[i];
                param.Title = param.Title.Replace("'", "''");
                param.Artist = param.Artist.Replace("'", "''");
            }

            string table = "";
            switch (type)
            {
                case DiffTableMode.GENONM: table = "diffGeNM"; break;
                case DiffTableMode.GENOINS: table = "diffGeINS"; break;
            }

            string query = "insert into " + table +
                "(title, artist, level, url, md5hash) values ";

            foreach (DiffTableData param in paramList)
            {
                query += "('" +
                        param.Title + "','" +
                        param.Artist + "'," +
                        param.Level + ",'" +
                        param.URL + "','" +
                        param.MD5 + "')";

                if (paramList.IndexOf(param) != paramList.Count - 1)
                {
                    query += ",";
                }
            }

            dbcommand.CommandText = query;
            dbcommand.ExecuteNonQuery();
        }

        public bool CheckDiffTableExist(DiffTableMode type)
        {
            dbcommand = dbconn.CreateCommand();
            string query = "SELECT name FROM sqlite_master WHERE type='table'";

            dbcommand.CommandText = query;
            dbreader = dbcommand.ExecuteReader();

            string table = "";
            switch(type)
            {
                case DiffTableMode.SATELLITE: table = "diffSl"; break;
                case DiffTableMode.STELLA: table = "diffSt"; break;
                case DiffTableMode.GENONM: table = "diffGeNM"; break;
                case DiffTableMode.GENOINS: table = "diffGeINS"; break;
            }

            bool existrec = false;
            while (dbreader.Read())
            {
                string name = dbreader.GetString(0);
                if (name == table) existrec = true;
            }
            return existrec;
        }

        public List<DiffTableData> GetDiffTableStSl(int lv, DiffTableMode mode)
        {
            List<DiffTableData> musiclist = new List<DiffTableData>();

            dbcommand = dbconn.CreateCommand();

            string table = "";
            if (mode == DiffTableMode.SATELLITE) table = "diffSl";
            else if (mode == DiffTableMode.STELLA) table = "diffSt";

            string query = "select * from " + table + " where level = " + lv;

            dbcommand.CommandText = query;
            dbreader = dbcommand.ExecuteReader();

            while (dbreader.Read())
            {
                string ltitle = dbreader.GetString(2);
                string lartist = dbreader.GetString(3);
                int llv = dbreader.GetInt32(5);
                string lurl = dbreader.GetString(4);
                string lmd5 = dbreader.GetString(1);

                DiffTableData data = new DiffTableData(
                    ltitle, lartist, llv, lurl, lmd5);

                musiclist.Add(data);
            }

            return musiclist;
        }

        public List<DiffTableData> GetDiffTableGe(int lv, DiffTableMode mode)
        {
            List<DiffTableData> musiclist = new List<DiffTableData>();

            dbcommand = dbconn.CreateCommand();

            string table = "";
            if (mode == DiffTableMode.GENONM) table = "diffGeNM";
            else if (mode == DiffTableMode.GENOINS) table = "diffGeINS";

            string query = "select * from " + table + " where level = " + lv;

            dbcommand.CommandText = query;
            dbreader = dbcommand.ExecuteReader();

            while (dbreader.Read())
            {
                string ltitle = dbreader.GetString(2);
                string lartist = dbreader.GetString(3);
                int llv = dbreader.GetInt32(5);
                string lurl = dbreader.GetString(4);
                string lmd5 = dbreader.GetString(1);

                DiffTableData data = new DiffTableData(
                    ltitle, lartist, llv, lurl, lmd5);

                musiclist.Add(data);
            }

            return musiclist;
        }

        public List<int> GetTableLevelList(DiffTableMode type)
        {
            List<int> levelList = new List<int>();

            dbcommand = dbconn.CreateCommand();

            string table = "";
            switch(type)
            {
                case DiffTableMode.SATELLITE: table = "diffSl"; break;
                case DiffTableMode.STELLA: table = "diffSt"; break;
                case DiffTableMode.GENONM: table = "diffGeNM"; break;
                case DiffTableMode.GENOINS: table = "diffGeINS"; break;
            }
            string query = "select distinct level from " + table;

            dbcommand.CommandText = query;
            dbreader = dbcommand.ExecuteReader();

            while (dbreader.Read())
            {
                int lv = dbreader.GetInt32(0);

                levelList.Add(lv);
            }

            return levelList;
        }
    }
}
