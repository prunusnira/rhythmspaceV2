using BMSCore;
using BMSPlayer;
using Mono.Data.Sqlite;
using System;
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

        public enum TABLETYPE { LIST, RECORD }

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
                "name varchar(100) not null," +
                "artist varchar (100) not null," +
                "gerne varchar (100)," +
                "bpmstart float not null," +
                "bpmmin float not null," +
                "bpmmax float not null," +
                "path varchar(1000) not null," +
                "md5hash varchar(1000) not null," +
                "level integer not null," +
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

        public void insertInto(TABLETYPE type, string[] param)
        {
            dbcommand = dbconn.CreateCommand();

            for(int i = 0; i < param.Length; i++)
            {
                param[i] = param[i].Replace("'", "''");
                param[i] = param[i].Replace("\"", "\"\"");
            }

            string query = "";
            switch (type)
            {
                case TABLETYPE.LIST:
                    // ����Ʈ�� ���� ���鶩 ������ ��Ű�� �ؾ� ��
                    query = "insert into list " +
                        "(name, artist, gerne," +
                        " bpmstart, bpmmin, bpmmax," +
                        " path, md5hash, level," +
                        " fname, jacket)" +
                        "values" +
                        "('" +
                            param[0] +"','" + param[1] +"','" + param[2] +"'," +
                            param[3] + "," + param[4] + "," + param[5] + ",'" +
                            param[6] + "', '" + param[7] + "','" +param[8] + "','" +
                            param[9] + "','" + param[10] + "')";
                    break;
                case TABLETYPE.RECORD:
                    query = "insert into record " +
                        "(name, rank, score) values" +
                        "('" + param[0] + "','" + param[1] + "','" + param[2] + "'," + param[3] + ") " +
                        "on duplicate key update name='"+param[0]+"', rank='"+param[1]+"', score='"+param[2]+"'";
                    break;
            }
            dbcommand.CommandText = query;
            dbcommand.ExecuteNonQuery();
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

        public void select(TABLETYPE type, string name = null)
        {
            dbcommand = dbconn.CreateCommand();
            string table = null;
            if (type == TABLETYPE.LIST) table = "list";
            else if (type == TABLETYPE.RECORD) table = "record";

            string query = "select * from " + table;

            if(type == TABLETYPE.RECORD)
            {
                query += " where name='"+name+"'";
            }

            dbcommand.CommandText = query;
            dbreader = dbcommand.ExecuteReader();

            while(dbreader.Read())
            {
                if(type == TABLETYPE.RECORD)
                {
                    int recid = dbreader.GetInt32(0);
                    string recname = dbreader.GetString(1);
                    string recrank = dbreader.GetString(2);
                    int recscore = dbreader.GetInt32(3);
                    string recmd5 = dbreader.GetString(4);

                    Const.record.Add(new RecordData(recid, recname, recrank, recscore, recmd5));
                }
                else
                {
                    int lid = dbreader.GetInt32(0);
                    string lname = dbreader.GetString(1);
                    string lartist = dbreader.GetString(2);
                    string lgerne = dbreader.GetString(3);
                    float lbpmstart = dbreader.GetFloat(4);
                    float lbpmmin = dbreader.GetFloat(5);
                    float lbpmmax = dbreader.GetFloat(6);
                    string lpath = dbreader.GetString(7);
                    string lmd5 = dbreader.GetString(8);
                    int llv = dbreader.GetInt32(9);
                    string lfname = dbreader.GetString(10);
                    string ljacket = dbreader.GetString(11);

                    MusicListData data = new MusicListData(
                        lid, lname, lartist, lgerne, lbpmstart, lbpmmin, lbpmmax,
                        lpath, lmd5, llv, lfname, ljacket);

                    Const.list.Add(data);
                }
            }
        }
    }
}