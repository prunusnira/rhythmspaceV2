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
    public class MusicDataRegister
    {
        public static int AddBMSFromPath()
        {
            SQLiteExecutor executor = SQLiteExecutor.getInstance();
            // BMS Loader
            LoadBMSList loader = new LoadBMSList();
            // MD5 Hash checker
            MD5 md5 = MD5.Create();
            List<MusicListData> list;

            string path = Const.GetBMSFolderPath();
            string hash;
            if (path != "")
            {
                // 모든 bms의 목록을 가져와야 함
                list = loader.LoadBms(path);
                if (list.Count != 0)
                {
                    executor.dropList();
                    // 리스트의 각 파일을 DB에 등록(이 때 MD5 Hash값도 계산)
                    foreach (MusicListData d in list)
                    {
                        FileStream fstream = File.OpenRead(d.Path + d.FileName);
                        var bytehash = md5.ComputeHash(fstream);

                        hash = BitConverter.ToString(bytehash);

                        string[] param =
                        {
                            d.Name, d.Artist, d.Gerne,
                            d.BPMstart.ToString(), d.BPMmin.ToString(), d.BPMmax.ToString(),
                            d.Path, hash, d.Level.ToString(), d.FileName, d.Jacket
                        };
                        executor.insertInto(SQLiteExecutor.TABLETYPE.LIST, param);
                        fstream.Close();
                    }
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                return 2;
            }
        }
    }
}