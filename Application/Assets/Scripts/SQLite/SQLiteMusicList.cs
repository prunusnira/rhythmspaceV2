using BMSCore;
using BMSPlayer;
using Mono.Data.Sqlite;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace DatabaseManager
{
    public class SQLiteMusicList
    {
        public IDbConnection dbconn;
        public IDbCommand dbcommand;
        public IDataReader dbreader;

        private static SQLiteMusicList instance;

        public static SQLiteMusicList Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SQLiteMusicList();
                    if (!instance.CheckListTableExist()) instance.InitListTable();
                }
                else
                {
                    if (!instance.checkDBConn())
                    {
                        instance = new SQLiteMusicList();
                    }
                }
                return instance;
            }
        }

        private SQLiteMusicList()
        {
            connectDB(Const.ListDBPath);
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
                musiclist.Add(GetMusicListData(dbreader));
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
                musiclist.Add(GetMusicListData(dbreader));
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
                return GetMusicListData(dbreader);
            }

            return null;
        }

        public List<MusicListData> FindBMSListWithMD5(List<string> md5list)
        {
            string str = "";
            for(int i = 0; i < md5list.Count; i++)
            {
                str += "'" + md5list[i] + "'";
                if (i != md5list.Count - 1) str += ",";
            }
            dbcommand = dbconn.CreateCommand();

            string query = "select * from list where md5hash in (" + str + ")";

            dbcommand.CommandText = query;
            dbreader = dbcommand.ExecuteReader();

            List<MusicListData> list = new List<MusicListData>();

            while(dbreader.Read())
            {
                list.Add(GetMusicListData(dbreader));
            }

            return list;
        }

        public List<MusicListData> FindBMSListWithName(NameType type)
        {
            dbcommand = dbconn.CreateCommand();

            string query = "select * from list where ";

            switch(type)
            {
                case NameType.ABCD:
                    query += "title like 'a%' or " +
                        "title like 'b%' or " +
                        "title like 'c%' or " +
                        "title like 'd%' " +
                        "collate nocase";
                    break;
                case NameType.EFGH:
                    query += "title like 'e%' or " +
                        "title like 'f%' or " +
                        "title like 'g%' or " +
                        "title like 'h%' " +
                        "collate nocase";
                    break;
                case NameType.IJKL:
                    query += "title like 'i%' or " +
                        "title like 'j%' or " +
                        "title like 'k%' or " +
                        "title like 'l%' " +
                        "collate nocase";
                    break;
                case NameType.MNOP:
                    query += "title like 'm%' or " +
                        "title like 'n%' or " +
                        "title like 'o%' or " +
                        "title like 'p%' " +
                        "collate nocase";
                    break;
                case NameType.QRST:
                    query += "title like 'q%' or " +
                        "title like 'r%' or " +
                        "title like 's%' or " +
                        "title like 't%' " +
                        "collate nocase";
                    break;
                case NameType.UVWXYZ:
                    query += "title like 'u%' or " +
                        "title like 'v%' or " +
                        "title like 'w%' or " +
                        "title like 'x%' or " +
                        "title like 'y%' or " +
                        "title like 'z%' " +
                        "collate nocase";
                    break;
                case NameType.NUMBERS:
                    query += "title like '0%' or " +
                        "title like '1%' or " +
                        "title like '2%' or " +
                        "title like '3%' or " +
                        "title like '4%' or " +
                        "title like '5%' or " +
                        "title like '6%' or " +
                        "title like '7%' or " +
                        "title like '8%' or " +
                        "title like '9%'";
                    break;
                case NameType.OTHERS:
                    query += "title not like 'a%' and " +
                        "title not like 'b%' and " +
                        "title not like 'c%' and " +
                        "title not like 'd%' and " +
                        "title not like 'e%' and " +
                        "title not like 'f%' and " +
                        "title not like 'g%' and " +
                        "title not like 'h%' and " +
                        "title not like 'i%' and " +
                        "title not like 'j%' and " +
                        "title not like 'k%' and " +
                        "title not like 'l%' and " +
                        "title not like 'm%' and " +
                        "title not like 'n%' and " +
                        "title not like 'o%' and " +
                        "title not like 'p%' and " +
                        "title not like 'q%' and " +
                        "title not like 'r%' and " +
                        "title not like 's%' and " +
                        "title not like 't%' and " +
                        "title not like 'u%' and " +
                        "title not like 'v%' and " +
                        "title not like 'w%' and " +
                        "title not like 'x%' and " +
                        "title not like 'y%' and " +
                        "title not like 'z%' and " +
                        "title not like '0%' and " +
                        "title not like '1%' and " +
                        "title not like '2%' and " +
                        "title not like '3%' and " +
                        "title not like '4%' and " +
                        "title not like '5%' and " +
                        "title not like '6%' and " +
                        "title not like '7%' and " +
                        "title not like '8%' and " +
                        "title not like '9%' " +
                        "collate nocase";
                    break;
            }

            dbcommand.CommandText = query;
            dbreader = dbcommand.ExecuteReader();

            List<MusicListData> list = new List<MusicListData>();

            while (dbreader.Read())
            {
                list.Add(GetMusicListData(dbreader));
            }

            return list;
        }

        public MusicListData GetMusicOfMD5Hash(string hash)
        {
            dbcommand = dbconn.CreateCommand();

            string query = "select * from list where md5hash='" + hash + "'";

            dbcommand.CommandText = query;
            dbreader = dbcommand.ExecuteReader();

            if(dbreader.Read())
            {
                return GetMusicListData(dbreader);
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
            string query = "select * from list where replace(coalesce(title, subtitle), ' ', '') = replace('" + title + "', ' ', '')";

            dbcommand.CommandText = query;
            dbreader = dbcommand.ExecuteReader();

            if (dbreader.Read())
            {
                return GetMusicListData(dbreader);
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

        private MusicListData GetMusicListData(IDataReader dbreader)
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
    }
}
