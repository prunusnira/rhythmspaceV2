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
    public class SQLiteExecutor
    {
        public IDbConnection dbconn;
        public IDbCommand dbcommand;
        public IDataReader dbreader;

        private static SQLiteExecutor instance;

        public static SQLiteExecutor Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SQLiteExecutor();
                    if (!instance.CheckListTableExist()) instance.InitListTable();
                    if (!instance.CheckRecordTableExist()) instance.InitRecordTable();
                }
                else
                {
                    if (!instance.checkDBConn())
                    {
                        instance = new SQLiteExecutor();
                    }
                }
                return instance;
            }
        }

        private SQLiteExecutor()
        {
            connectDB(Const.DBPath);
        }

        public void connectDB(string dbfile)
        {
            if(!File.Exists(dbfile))
            {
                Directory.CreateDirectory(Directory.GetParent(dbfile).FullName);
                SqliteConnection.CreateFile(dbfile);
            }

            dbconn = (IDbConnection)new SqliteConnection("Data Source="+dbfile);
            dbcommand = (IDbCommand)new SqliteCommand();
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

        public void InitListTable()
        {
            dbcommand = dbconn.CreateCommand();
            string list =
                @"create table list (
                id integer primary key autoincrement,
                title text not null,
                subtitle text not null,
                artist text not null,
                subartist text not null,
                gerne text,
                bpmstart float not null,
                bpmmin float not null,
                bpmmax float not null,
                path text not null,
                md5hash text not null,
                level integer not null,
                diff integer default 2,
                fname text not null,
                jacket text,
                totalnotes integer not null,
                time integer not null,
                rank integer default 2,
                lnexist integer not null,
                preview text not null default '')";

            dbcommand.CommandText = list;
            dbcommand.ExecuteNonQuery();
        }

        public void InitRecordTable()
        {
            dbcommand = dbconn.CreateCommand();
            string record =
                @"create table record (
                id integer primary key autoincrement,
                md5hash text unique not null,
                rank text not null,
                score integer not null,
                judge integer not null,
                clear integer not null,
                pf integer not null,
                gr integer not null,
                gd integer not null,
                ok integer not null,
                pr integer not null,
                combo integer not null,
                cb integer not null,
                playcnt integer not null)";

            dbcommand.CommandText = record;
            dbcommand.ExecuteNonQuery();
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

        public void InitDiffGe()
        {
            dbcommand = dbconn.CreateCommand();
            string table =
                @"create table diffGe (
                id integer primary key autoincrement,
                title text not null,
                artist text not null,
                url text not null,
                level integer not null)";

            dbcommand.CommandText = table;
            dbcommand.ExecuteNonQuery();
        }

        public void DropList()
        {
            dbcommand = dbconn.CreateCommand();
            string query = "delete from list";
            dbcommand.CommandText = query;
            dbcommand.ExecuteNonQuery();
            // AUTO INC RESET
            query = "update sqlite_sequence set seq=0 where name='list'";
            dbcommand.CommandText = query;
            dbcommand.ExecuteNonQuery();
        }

        public void DropRecord()
        {
            dbcommand = dbconn.CreateCommand();
            string query = @"drop table record";
            dbcommand.CommandText = query;
            dbcommand.ExecuteNonQuery();
        }

        public void DropTableSl()
        {
            dbcommand = dbconn.CreateCommand();
            string query = @"delete from diffSl";
            dbcommand.CommandText = query;
            dbcommand.ExecuteNonQuery();
        }

        public void DropTableSt()
        {
            dbcommand = dbconn.CreateCommand();
            string query = @"delete from diffSt";
            dbcommand.CommandText = query;
            dbcommand.ExecuteNonQuery();
        }

        public void DropTableGe()
        {
            dbcommand = dbconn.CreateCommand();
            string query = @"delete from diffGe";
            dbcommand.CommandText = query;
            dbcommand.ExecuteNonQuery();
        }

        public void InsertBMS(List<MusicListData> paramList)
        {
            dbcommand = dbconn.CreateCommand();

            for(int i = 0; i < paramList.Count; i++)
            {
                MusicListData param = paramList[i];
                param.Title = param.Title.Replace("'", "''");
                param.SubTitle = param.SubTitle.Replace("'", "''");
                param.Artist = param.Artist.Replace("'", "''");
                param.SubArtist = param.SubArtist.Replace("'", "''");
                param.Gerne = param.Gerne.Replace("'", "''");
                param.FileName = param.FileName.Replace("'", "''");
                param.Path = param.Path.Replace("'", "''");
                param.Jacket = param.Jacket.Replace("'", "''");
            }

            string query =
                @"insert into list
                    (title, subtitle, artist, subartist, gerne,
                    bpmstart, bpmmin, bpmmax,
                    path, md5hash, level, diff,
                    fname, jacket, totalnotes, time, rank,
                    lnexist, preview)
                    values";

            foreach(MusicListData param in paramList)
            {
                int ln = param.LNExist ? 1 : 0;

                query += "('" +
                        param.Title + "','" +
                        param.SubTitle + "','" +
                        param.Artist + "','" +
                        param.SubArtist + "','" +
                        param.Gerne + "'," +
                        param.BPMstart + "," +
                        param.BPMmin + "," +
                        param.BPMmax + ",'" +
                        param.Path + "', '" +
                        param.MD5Hash + "'," +
                        param.Level + "," +
                        param.Difficulty + ",'" +
                        param.FileName + "','" +
                        param.Jacket + "', " +
                        param.TotalNotes + ", " +
                        param.PlayTime + ", "+
                        param.Rank + ", " +
                        ln + ", '" +
                        param.Preview + "')";

                if(paramList.IndexOf(param) != paramList.Count - 1)
                {
                    query += ",";
                }
            }
            
            dbcommand.CommandText = query;
            dbcommand.ExecuteNonQuery();
        }

        public void InsertRecord(
            string hash, string rank,
            int score, int judge,
            int clear, int pf, int gr, int gd,
            int ok, int pr, int combo, int cb, int playcnt
            )
        {
            dbcommand = dbconn.CreateCommand();

            string query =
                @"insert into record
                    (md5hash, rank, score, judge,
                        clear, pf, gr, gd, ok, pr, combo, cb, playcnt) values
                    ('" + hash + "','" +
                        rank + "'," +
                        score + "," +
                        judge + "," +
                        clear + "," +
                        pf + "," +
                        gr + "," +
                        gd + "," +
                        ok + "," +
                        pr + "," +
                        combo + "," +
                        cb + "," +
                        playcnt + ") " +
                    " on conflict(md5hash) do update set"+
                        " rank='" + rank + "'," +
                        " score=" + score + "," +
                        " judge=" + judge + "," +
                        " clear=" + clear + "," +
                        " pf=" + pf + "," +
                        " gr=" + gr + "," +
                        " gd=" + gd + "," +
                        " ok=" + ok + "," +
                        " pr=" + pr + "," +
                        " combo=" + combo + "," +
                        " cb=" + cb + "," +
                        " playcnt=" + playcnt;

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

        public void InsertDiffTableGe(List<DiffTableData> paramList)
        {
            dbcommand = dbconn.CreateCommand();

            for (int i = 0; i < paramList.Count; i++)
            {
                DiffTableData param = paramList[i];
                param.Title = param.Title.Replace("'", "''");
                param.Artist = param.Artist.Replace("'", "''");
            }

            string query =
                @"insert into diffGe
                 (title, artist, level, url) values ";

            foreach (DiffTableData param in paramList)
            {
                query += "('" +
                        param.Title + "','" +
                        param.Artist + "'," +
                        param.Level + ",'" +
                        param.URL + "')";

                if (paramList.IndexOf(param) != paramList.Count - 1)
                {
                    query += ",";
                }
            }

            dbcommand.CommandText = query;
            dbcommand.ExecuteNonQuery();
        }

        public bool CheckListTableExist()
        {
            dbcommand = dbconn.CreateCommand();
            string query = "SELECT name FROM sqlite_master WHERE type='table'";

            dbcommand.CommandText = query;
            dbreader = dbcommand.ExecuteReader();

            bool existlist = false;
            while(dbreader.Read())
            {
                string name = dbreader.GetString(0);
                if (name == "list") existlist = true;
            }
            return existlist;
        }

        public bool CheckRecordTableExist()
        {
            dbcommand = dbconn.CreateCommand();
            string query = "SELECT name FROM sqlite_master WHERE type='table'";

            dbcommand.CommandText = query;
            dbreader = dbcommand.ExecuteReader();

            bool existrec = false;
            while (dbreader.Read())
            {
                string name = dbreader.GetString(0);
                if (name == "record") existrec = true;
            }
            return existrec;
        }

        public bool CheckDiffTableStExist()
        {
            dbcommand = dbconn.CreateCommand();
            string query = "SELECT name FROM sqlite_master WHERE type='table'";

            dbcommand.CommandText = query;
            dbreader = dbcommand.ExecuteReader();

            bool existrec = false;
            while (dbreader.Read())
            {
                string name = dbreader.GetString(0);
                if (name == "diffSt") existrec = true;
            }
            return existrec;
        }

        public bool CheckDiffTableSlExist()
        {
            dbcommand = dbconn.CreateCommand();
            string query = "SELECT name FROM sqlite_master WHERE type='table'";

            dbcommand.CommandText = query;
            dbreader = dbcommand.ExecuteReader();

            bool existrec = false;
            while (dbreader.Read())
            {
                string name = dbreader.GetString(0);
                if (name == "diffSl") existrec = true;
            }
            return existrec;
        }

        public bool CheckDiffTableGeExist()
        {
            dbcommand = dbconn.CreateCommand();
            string query = "SELECT name FROM sqlite_master WHERE type='table'";

            dbcommand.CommandText = query;
            dbreader = dbcommand.ExecuteReader();

            bool existrec = false;
            while (dbreader.Read())
            {
                string name = dbreader.GetString(0);
                if (name == "diffGe") existrec = true;
            }
            return existrec;
        }

        public List<MusicListData> SelectMusicList(string param = null)
        {
            List<MusicListData> musiclist = new List<MusicListData>();

            dbcommand = dbconn.CreateCommand();

            string query = "select * from list";

            if(param != null)
            {
                param = param.Replace("'", "''");
                query += " where path='"+param+"/'";
            }

            dbcommand.CommandText = query;
            dbreader = dbcommand.ExecuteReader();

            while(dbreader.Read())
            {
                int lid = dbreader.GetInt32(0);
                string ltitle = dbreader.GetString(1);
                string lsubtitle = dbreader.GetString(2);
                string lartist = dbreader.GetString(3);
                string lsubartist = dbreader.GetString(4);
                string lgerne = dbreader.GetString(5);
                float lbpmstart = dbreader.GetFloat(6);
                float lbpmmin = dbreader.GetFloat(7);
                float lbpmmax = dbreader.GetFloat(8);
                string lpath = dbreader.GetString(9);
                string lmd5 = dbreader.GetString(10);
                int llv = dbreader.GetInt32(11);
                int ldiff = dbreader.GetInt32(12);
                string lfname = dbreader.GetString(13);
                string ljacket = dbreader.GetString(14);
                int ltotalnotes = dbreader.GetInt32(15);
                int ltime = dbreader.GetInt32(16);
                int lrank = dbreader.GetInt32(17);
                bool lln = dbreader.GetInt32(18) == 1 ? true : false;
                string lprev = dbreader.GetString(19);

                MusicListData data = new MusicListData(
                    lid, ltitle, lsubtitle, lartist, lsubartist, lgerne,
                    lbpmstart, lbpmmin, lbpmmax,
                    lpath, lmd5, llv, ldiff, lfname,
                    ljacket, ltotalnotes, ltime, lrank,
                    lln, lprev);

                musiclist.Add(data);
            }

            return musiclist;
        }

        public List<MusicListData> FindBMSWithTitle(string queryText)
        {
            List<MusicListData> musiclist = new List<MusicListData>();

            dbcommand = dbconn.CreateCommand();

            queryText = queryText.Replace("'", "''");
            string query = "select * from list where replace(title, ' ', '') like '%"
                +queryText+"%' collate nocase";

            dbcommand.CommandText = query;
            dbreader = dbcommand.ExecuteReader();

            while (dbreader.Read())
            {
                int lid = dbreader.GetInt32(0);
                string ltitle = dbreader.GetString(1);
                string lsubtitle = dbreader.GetString(2);
                string lartist = dbreader.GetString(3);
                string lsubartist = dbreader.GetString(4);
                string lgerne = dbreader.GetString(5);
                float lbpmstart = dbreader.GetFloat(6);
                float lbpmmin = dbreader.GetFloat(7);
                float lbpmmax = dbreader.GetFloat(8);
                string lpath = dbreader.GetString(9);
                string lmd5 = dbreader.GetString(10);
                int llv = dbreader.GetInt32(11);
                int ldiff = dbreader.GetInt32(12);
                string lfname = dbreader.GetString(13);
                string ljacket = dbreader.GetString(14);
                int ltotalnotes = dbreader.GetInt32(15);
                int ltime = dbreader.GetInt32(16);
                int lrank = dbreader.GetInt32(17);
                bool lln = dbreader.GetInt32(18) == 1 ? true : false;
                string lprev = dbreader.GetString(19);

                MusicListData data = new MusicListData(
                    lid, ltitle, lsubtitle, lartist, lsubartist, lgerne,
                    lbpmstart, lbpmmin, lbpmmax,
                    lpath, lmd5, llv, ldiff, lfname,
                    ljacket, ltotalnotes, ltime, lrank,
                    lln, lprev);

                musiclist.Add(data);
            }

            return musiclist;
        }

        public MusicListData FindBMSWithPath(string path)
        {
            dbcommand = dbconn.CreateCommand();

            path = path.Replace("'", "''");
            string query = "select * from list where path='"
                + path + "' collate nocase";

            dbcommand.CommandText = query;
            dbreader = dbcommand.ExecuteReader();

            if(dbreader.Read())
            {
                int lid = dbreader.GetInt32(0);
                string ltitle = dbreader.GetString(1);
                string lsubtitle = dbreader.GetString(2);
                string lartist = dbreader.GetString(3);
                string lsubartist = dbreader.GetString(4);
                string lgerne = dbreader.GetString(5);
                float lbpmstart = dbreader.GetFloat(6);
                float lbpmmin = dbreader.GetFloat(7);
                float lbpmmax = dbreader.GetFloat(8);
                string lpath = dbreader.GetString(9);
                string lmd5 = dbreader.GetString(10);
                int llv = dbreader.GetInt32(11);
                int ldiff = dbreader.GetInt32(12);
                string lfname = dbreader.GetString(13);
                string ljacket = dbreader.GetString(14);
                int ltotalnotes = dbreader.GetInt32(15);
                int ltime = dbreader.GetInt32(16);
                int lrank = dbreader.GetInt32(17);
                bool lln = dbreader.GetInt32(18) == 1 ? true : false;
                string lprev = dbreader.GetString(19);

                return new MusicListData(
                    lid, ltitle, lsubtitle, lartist, lsubartist, lgerne,
                    lbpmstart, lbpmmin, lbpmmax,
                    lpath, lmd5, llv, ldiff, lfname,
                    ljacket, ltotalnotes, ltime, lrank,
                    lln, lprev);
            }

            return null;
        }

        public RecordData SelectRecord(string param = null)
        {
            dbcommand = dbconn.CreateCommand();

            string query = "select * from record";

            if (param != null)
            {
                query += " where md5hash='" + param + "'";
            }

            dbcommand.CommandText = query;
            dbreader = dbcommand.ExecuteReader();
            bool exist = dbreader.Read();
            if(exist)
            {
                int recid = dbreader.GetInt32(0);
                string recmd5 = dbreader.GetString(1);
                string recrank = dbreader.GetString(2);
                int recscore = dbreader.GetInt32(3);
                int recjudge = dbreader.GetInt32(4);
                int recclear = dbreader.GetInt32(5);
                int recpf = dbreader.GetInt32(6);
                int recgr = dbreader.GetInt32(7);
                int recgd = dbreader.GetInt32(8);
                int recok = dbreader.GetInt32(9);
                int recpr = dbreader.GetInt32(10);
                int reccombo = dbreader.GetInt32(11);
                int reccb = dbreader.GetInt32(12);
                int recplaycnt = dbreader.GetInt32(13);

                return new RecordData(recmd5, recrank,
                    recscore, recjudge,
                    recclear, recpf, recgr, recgd, recok, recpr,
                    reccombo, reccb, recplaycnt);
            }
            else
            {
                return null;
            }
        }

        public int SelectRecordClear(string param)
        {
            dbcommand = dbconn.CreateCommand();

            string query = "select clear from record";
            query += " where md5hash='" + param + "'";

            dbcommand.CommandText = query;
            dbreader = dbcommand.ExecuteReader();

            bool resultExist = dbreader.Read();
            if(resultExist)
            {
                return dbreader.GetInt32(0);
            }
            else
            {
                return -1;
            }
        }

        public int SelectRecordPlayCnt(string param)
        {
            dbcommand = dbconn.CreateCommand();

            string query = "select playcnt from record";
            query += " where md5hash='" + param + "'";

            dbcommand.CommandText = query;
            dbreader = dbcommand.ExecuteReader();

            bool resultExist = dbreader.Read();
            if (resultExist)
            {
                return dbreader.GetInt32(0);
            }
            else
            {
                return 0;
            }
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

        public List<DiffTableData> GetDiffTableGe(int lv)
        {
            List<DiffTableData> musiclist = new List<DiffTableData>();

            dbcommand = dbconn.CreateCommand();

            string query = "select * from diffGe where level = " + lv;

            dbcommand.CommandText = query;
            dbreader = dbcommand.ExecuteReader();

            while (dbreader.Read())
            {
                string ltitle = dbreader.GetString(1);
                string lartist = dbreader.GetString(2);
                int llv = dbreader.GetInt32(4);
                string lurl = dbreader.GetString(3);

                DiffTableData data = new DiffTableData(
                    ltitle, lartist, llv, lurl, "");

                musiclist.Add(data);
            }

            return musiclist;
        }

        public List<int> GetTableLevelList(int type)
        {
            List<int> levelList = new List<int>();

            dbcommand = dbconn.CreateCommand();

            string table = "";
            switch(type)
            {
                case 0: table = "diffSl"; break;
                case 1: table = "diffSt"; break;
                case 2: table = "diffGe"; break;
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

        public MusicListData GetMusicOfMD5Hash(string hash)
        {
            dbcommand = dbconn.CreateCommand();

            string query = "select * from list where md5hash='" + hash + "'";

            dbcommand.CommandText = query;
            dbreader = dbcommand.ExecuteReader();

            if(dbreader.Read())
            {
                int lid = dbreader.GetInt32(0);
                string ltitle = dbreader.GetString(1);
                string lsubtitle = dbreader.GetString(2);
                string lartist = dbreader.GetString(3);
                string lsubartist = dbreader.GetString(4);
                string lgerne = dbreader.GetString(5);
                float lbpmstart = dbreader.GetFloat(6);
                float lbpmmin = dbreader.GetFloat(7);
                float lbpmmax = dbreader.GetFloat(8);
                string lpath = dbreader.GetString(9);
                string lmd5 = dbreader.GetString(10);
                int llv = dbreader.GetInt32(11);
                int ldiff = dbreader.GetInt32(12);
                string lfname = dbreader.GetString(13);
                string ljacket = dbreader.GetString(14);
                int ltotalnotes = dbreader.GetInt32(15);
                int ltime = dbreader.GetInt32(16);
                int lrank = dbreader.GetInt32(17);
                bool lln = dbreader.GetInt32(18) == 1 ? true : false;
                string lprev = dbreader.GetString(19);

                return new MusicListData(
                    lid, ltitle, lsubtitle, lartist, lsubartist, lgerne,
                    lbpmstart, lbpmmin, lbpmmax,
                    lpath, lmd5, llv, ldiff, lfname,
                    ljacket, ltotalnotes, ltime, lrank,
                    lln, lprev);
            }
            else
            {
                return null;
            }
        }

        public MusicListData GetMusicOfTitle(string title)
        {
            dbcommand = dbconn.CreateCommand();

            title = title.Replace("'", "''");
            string query = "select * from list where title='" + title + "'";

            dbcommand.CommandText = query;
            dbreader = dbcommand.ExecuteReader();

            if (dbreader.Read())
            {
                int lid = dbreader.GetInt32(0);
                string ltitle = dbreader.GetString(1);
                string lsubtitle = dbreader.GetString(2);
                string lartist = dbreader.GetString(3);
                string lsubartist = dbreader.GetString(4);
                string lgerne = dbreader.GetString(5);
                float lbpmstart = dbreader.GetFloat(6);
                float lbpmmin = dbreader.GetFloat(7);
                float lbpmmax = dbreader.GetFloat(8);
                string lpath = dbreader.GetString(9);
                string lmd5 = dbreader.GetString(10);
                int llv = dbreader.GetInt32(11);
                int ldiff = dbreader.GetInt32(12);
                string lfname = dbreader.GetString(13);
                string ljacket = dbreader.GetString(14);
                int ltotalnotes = dbreader.GetInt32(15);
                int ltime = dbreader.GetInt32(16);
                int lrank = dbreader.GetInt32(17);
                bool lln = dbreader.GetInt32(18) == 1 ? true : false;
                string lprev = dbreader.GetString(19);

                return new MusicListData(
                    lid, ltitle, lsubtitle, lartist, lsubartist, lgerne,
                    lbpmstart, lbpmmin, lbpmmax,
                    lpath, lmd5, llv, ldiff, lfname,
                    ljacket, ltotalnotes, ltime, lrank,
                    lln, lprev);
            }
            else
            {
                return null;
            }
        }

        public void DeleteBMS(List<string> pathList)
        {
            dbcommand = dbconn.CreateCommand();

            foreach(string s in pathList)
            {
                string path = s.Replace("'", "''");

                string query = "delete from list where path='" +
                    path + "'";
                dbcommand.CommandText = query;
                dbcommand.ExecuteNonQuery();
            }
        }
    }
}
