using BMSCore;
using DatabaseManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

namespace BMSPlayer
{
    public class MusicListManager
    {
        SQLiteExecutor executor;
        LoadBMSList loader;
        MD5 md5;

        public MusicListManager()
        {
            executor = SQLiteExecutor.getInstance();
            loader = new LoadBMSList();
            md5 = MD5.Create();
        }

        public void close()
        {
            executor.closeDB();
            md5.Clear();
        }

        public MusicListData LoadBMSFromFolder(string path, int index)
        {
            if (path != "")
            {
                // 모든 bms의 목록을 가져와야 함
                return loader.LoadBms(path, index);
            }
            return null;
        }

        public void LoadBMSFromDBOverall()
        {
            executor.select(SQLiteExecutor.TABLETYPE.LIST);
        }

        public void AddDataToDB(List<MusicListData> list)
        {
            if (list.Count != 0)
            {
                executor.dropList();
                // 리스트의 각 파일을 DB에 등록(이 때 MD5 Hash값도 계산)
                foreach (MusicListData d in list)
                {
                    FileStream fstream = File.OpenRead(d.Path + d.FileName);
                    var bytehash = md5.ComputeHash(fstream);

                    string hash = BitConverter.ToString(bytehash);

                    string[] param =
                    {
                            d.Name, d.Artist, d.Gerne,
                            d.BPMstart.ToString(), d.BPMmin.ToString(), d.BPMmax.ToString(),
                            d.Path, hash, d.Level.ToString(), d.FileName, d.Jacket
                        };
                    executor.insertInto(SQLiteExecutor.TABLETYPE.LIST, param);
                    fstream.Close();
                }
            }
        }
    }
}