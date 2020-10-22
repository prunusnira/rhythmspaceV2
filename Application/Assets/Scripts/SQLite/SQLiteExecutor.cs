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

        public static SQLiteExecutor Instance;

        public static SQLiteExecutor getInstance()
        {
            if (Instance == null)
            {
                Instance = new SQLiteExecutor();
                if (!Instance.CheckListTableExist()) Instance.InitListTable();
                if (!Instance.CheckRecordTableExist()) Instance.InitRecordTable();
            }
            else
            {
                if(!Instance.checkDBConn())
                {
                    Instance = new SQLiteExecutor();
                }
                Debug.Log("DB instance exist");
            }
            return Instance;
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
                title varchar(100) not null,
                subtitle varchar(100) not null,
                artist varchar (100) not null,
                subartist varchar (100) not null,
                gerne varchar (100),
                bpmstart float not null,
                bpmmin float not null,
                bpmmax float not null,
                path varchar(1000) not null,
                md5hash varchar(1000) not null,
                level integer not null,
                diff integer default 2,
                fname varchar (100) not null,
                jacket varchar (100))";

            dbcommand.CommandText = list;
            dbcommand.ExecuteNonQuery();
        }

        public void InitRecordTable()
        {
            dbcommand = dbconn.CreateCommand();
            string record =
                @"create table record (
                id integer primary key autoincrement,
                md5hash varchar(100) unique not null,
                rank varchar(2) not null,
                score integer(5) not null,
                judge integer(1) not null,
                clear integer(1) not null,
                pf integer(5) not null,
                gr integer(5) not null,
                gd integer(5) not null,
                ok integer(5) not null,
                pr integer(5) not null,
                combo integer(5) not null,
                cb integer(5) not null)";

            dbcommand.CommandText = record;
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

        public void InsertBMS(List<string[]> paramList)
        {
            dbcommand = dbconn.CreateCommand();

            for(int i = 0; i < paramList.Count; i++)
            {
                string[] param = paramList[i];
                for(int j = 0; j < param.Length; j++)
                {
                    param[j] = param[j].Replace("'", "''");
                    param[j] = param[j].Replace("\"", "\"\"");
                }
            }

            string query =
                @"insert into list
                    (title, subtitle, artist, subartist, gerne,
                    bpmstart, bpmmin, bpmmax,
                    path, md5hash, level, diff,
                    fname, jacket)
                    values";

            foreach(string[] param in paramList)
            {
                query += "('" +
                        param[0] + "','" + param[1] + "','" + param[2] + "','" + param[3] + "','" + param[4] + "'," +
                        param[5] + "," + param[6] + "," + param[7] + ",'" +
                        param[8] + "', '" + param[9] + "','" + param[10] + "','" + param[11] + "','" +
                        param[12] + "','" + param[13] + "')";

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
            int ok, int pr, int combo, int cb
            )
        {
            dbcommand = dbconn.CreateCommand();

            string query =
                @"insert into record
                    (md5hash, rank, score, judge,
                        clear, pf, gr, gd, ok, pr, combo, cb) values
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
                        cb + ") " +
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
                        " cb=" + cb;

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

                MusicListData data = new MusicListData(
                    lid, ltitle, lsubtitle, lartist, lsubartist, lgerne,
                    lbpmstart, lbpmmin, lbpmmax,
                    lpath, lmd5, llv, ldiff, lfname, ljacket);

                musiclist.Add(data);
            }

            return musiclist;
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

                return new RecordData(recmd5, recrank,
                    recscore, recjudge,
                    recclear, recpf, recgr, recgd, recok, recpr, reccombo, reccb);
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
    }
}
