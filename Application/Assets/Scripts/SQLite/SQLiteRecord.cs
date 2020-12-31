using BMSPlayer;
using Mono.Data.Sqlite;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace DatabaseManager
{
    public class SQLiteRecord
    {
        public IDbConnection dbconn;
        public IDbCommand dbcommand;
        public IDataReader dbreader;

        private static SQLiteRecord instance;

        public static SQLiteRecord Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SQLiteRecord();
                    if (!instance.CheckRecordTableExist()) instance.InitRecordTable();
                }
                else
                {
                    if (!instance.checkDBConn())
                    {
                        instance = new SQLiteRecord();
                    }
                }
                return instance;
            }
        }

        private SQLiteRecord()
        {
            connectDB(Const.RecordDBPath);
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

        public void DropRecord()
        {
            dbcommand = dbconn.CreateCommand();
            string query = @"drop table if exists record";
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
                return GetRecordData(dbreader);
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

        public List<string> SelectRankHash(string rank)
        {
            dbcommand = dbconn.CreateCommand();

            string query = "select md5hash from record";
            if (rank == "fc")
            {
                query += " where clear=0 or clear == '1'";
            }
            else if(rank == "b")
            {
                query += " where rank='b' "+
                    "or rank='c' " +
                    "or rank='d' " +
                    "or rank='e' " +
                    "or rank='f'";
            }
            else
            {
                query += " where rank='"+rank+"'";
            }

            dbcommand.CommandText = query;
            dbreader = dbcommand.ExecuteReader();

            List<string> list = new List<string>();

            while(dbreader.Read())
            {
                list.Add(dbreader.GetString(0));
            }

            return list;
        }

        private RecordData GetRecordData(IDataReader dbreader)
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
    }
}
