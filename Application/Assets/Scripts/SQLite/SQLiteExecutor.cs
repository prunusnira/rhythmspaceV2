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
                if(!Instance.checkTableExist()) Instance.initializeTable();
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
            dbcommand.Dispose();
            dbconn.Close();
            dbcommand = null;
            dbconn = null;
        }

        public void initializeTable()
        {
            dbcommand = dbconn.CreateCommand();
            string list = "create table list (" +
                "id integer primary key autoincrement," +
                "title varchar(100) not null," +
                "subtitle varchar(100) not null," +
                "artist varchar (100) not null," +
                "subartist varchar (100) not null," +
                "gerne varchar (100)," +
                "bpmstart float not null," +
                "bpmmin float not null," +
                "bpmmax float not null," +
                "path varchar(1000) not null," +
                "md5hash varchar(1000) not null," +
                "level integer not null," +
                "diff integer default 2," +
                "fname varchar (100) not null," +
                "jacket varchar (100))";
            string record = "create table record (" +
                "id integer primary key autoincrement," +
                "name varhar(100) not null," +
                "rank varchar(2) not null," +
                "score integer(10) not null," +
                "md5hash varchar(1000) not null)";

            dbcommand.CommandText = list;
            dbcommand.ExecuteNonQuery();
            dbcommand.CommandText = record;
            dbcommand.ExecuteNonQuery();
        }

        public void dropList()
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

        public void InsertBMS(string[] param)
        {
            dbcommand = dbconn.CreateCommand();

            for(int i = 0; i < param.Length; i++)
            {
                param[i] = param[i].Replace("'", "''");
                param[i] = param[i].Replace("\"", "\"\"");
            }

            string query =
                "insert into list " +
                    "(title, subtitle, artist, subartist, gerne," +
                    " bpmstart, bpmmin, bpmmax," +
                    " path, md5hash, level, diff," +
                    " fname, jacket)" +
                    "values" +
                    "('" +
                        param[0] + "','" + param[1] + "','" + param[2] + "','" + param[3] + "','" + param[4] + "'," +
                        param[5] + "," + param[6] + "," + param[7] + ",'" +
                        param[8] + "', '" + param[9] + "','" + param[10] + "','" + param[11] + "','" +
                        param[12] + "','" + param[13] + "')"; ;
            
            dbcommand.CommandText = query;
            dbcommand.ExecuteNonQuery();
        }

        public void InsertRecord(string[] param)
        {
            dbcommand = dbconn.CreateCommand();

            for (int i = 0; i < param.Length; i++)
            {
                param[i] = param[i].Replace("'", "''");
                param[i] = param[i].Replace("\"", "\"\"");
            }

            string query =
                "insert into record " +
                    "(name, rank, score) values" +
                    "('" + param[0] + "','" + param[1] + "','" + param[2] + "'," + param[3] + ") " +
                    "on duplicate key update name='" + param[0] + "', rank='" + param[1] + "', score='" + param[2] + "'";
        }

        public bool checkTableExist()
        {
            dbcommand = dbconn.CreateCommand();
            string query = "SELECT name FROM sqlite_master WHERE type='table'";

            dbcommand.CommandText = query;
            dbreader = dbcommand.ExecuteReader();

            bool allexist = false;
            bool existlist = false;
            bool existrec = false;
            while(dbreader.Read())
            {
                string name = dbreader.GetString(0);
                if (name == "list") existlist = true;
                if (name == "record") existrec = true;
            }

            if (existlist && existrec) allexist = true;

            return allexist;
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
            string query = "select * from record";

            if (param != null)
            {
                query += " where md5hash='" + param + "'";
            }

            dbcommand.CommandText = query;
            dbreader = dbcommand.ExecuteReader();
            dbreader.Read();
            int recid = dbreader.GetInt32(0);
            string recname = dbreader.GetString(1);
            string recrank = dbreader.GetString(2);
            int recscore = dbreader.GetInt32(3);
            string recmd5 = dbreader.GetString(4);

            return new RecordData(recid, recname, recrank, recscore, recmd5);
        }
    }
}
