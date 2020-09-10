using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace BMSCore
{
    public class LoadBMSList
    {
        public List<MusicListData> LoadBms(string path)
        {
            List<MusicListData> musiclist = new List<MusicListData>();
            string[] subdirs = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
            // check bms directory is not empty
            if (subdirs != null)
            {
                // get all the files and directories within directory
                for (int i = 0; i < subdirs.Length; i++)
                {
                    string[] subbms = Directory.GetFiles(subdirs[i], "*.*", SearchOption.TopDirectoryOnly)
                        .Where(s => s.ToLower().EndsWith(".bms") || s.ToLower().EndsWith(".bme")).ToArray<string>();
                    
                    List<string> bmsfiles = new List<string>();
                    foreach (string s in subbms)
                    {
                        if(s.EndsWith(".bms") || s.EndsWith(".BMS") || s.EndsWith(".bme") || s.EndsWith(".BME"))
                        {
                            bmsfiles.Add(s);
                        }
                    }

                    if (bmsfiles.Count > 0)
                    {
                        // 파일 하나 분석해서 정보 추가하기
                        for (int k = 0; k < bmsfiles.Count; k++)
                        {
                            string name = null;
                            string fpath = null;
                            string artist = null;
                            string gerne = null;
                            string fname = null;
                            string jacket = null;
                            double bpmstart = 0;
                            double bpmmin = 0;
                            double bpmmax = 0;
                            int player = 0;
                            int level = 0;

                            BMS bms = new BMS(bmsfiles[k]);
                            BMSAnalyzer analyzer = new BMSAnalyzer();
                            analyzer.HeaderAnalyzer(bms);
                            if (bms.getPlayer() != 1) continue;
                            fpath = bms.getFolderPath();
                            artist = bms.getArtist();
                            name = bms.getTitle();
                            gerne = bms.getGerne();
                            player = bms.getPlayer();
                            bpmstart = bms.getBPMStart();
                            bpmmin = bms.getBPMMin();
                            bpmmax = bms.getBPMMax();
                            level = bms.getLevel();
                            fname = bms.getFileName();
                            jacket = bms.getStageFile();
                            if (name != null) musiclist.Add(
                                new MusicListData(
                                    musiclist.Count, name, artist, gerne,
                                    bpmstart, bpmmin, bpmmax,
                                    fpath, level, fname, jacket
                                )
                            );
                        }
                    }
                }
            }
            return musiclist;
        }
    }
}
