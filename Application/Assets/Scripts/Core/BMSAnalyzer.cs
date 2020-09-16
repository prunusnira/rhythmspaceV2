using BMSPlayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace BMSCore
{
    public class BMSAnalyzer
    {
        private bool isWavExist = false;
        private bool isOggExist = false;
        private bool isVideoExist = false;

        // 노래 선택창에서 선택한 BMS 파일의 헤더를 분석
        public void HeaderAnalyzer(BMS bms)
        {
            // input stream 열기 // 기본 Default, 일본어 932, 한국어 949
            int encoding = Const.GetEncoding();

            StreamReader bmsReader = new StreamReader(
                bms.getFilePath(), System.Text.Encoding.GetEncoding(encoding)
            );

            // 한 줄 씩 읽으면서 분석
            string buf = null;
            while ((buf = bmsReader.ReadLine()) != null)
            {
                try
                {
                    string[] line = buf.Split(' ');
                    if (line.Length != 0 && line[0].StartsWith("#"))
                    {
                        // divide tag and other info
                        string tag = line[0];
                        string left = "";
                        string etc;
                        for (int i = 1; i < line.Length; i++)
                        {
                            etc = line[i];
                            if (left.Length != 0)
                            {
                                left += " ";
                            }
                            left += etc;
                        }

                        string chkWav = tag.Substring(0, 4);

                        // For BPM Check
                        int parsedTag = 0;

                        // process each tag
                        if(tag == "#")
                        {
                            // 에러값 예외처리
                            // 여기서 걸러지지 않으면 try-catch로 거름
                            continue;
                        }
                        else if (tag == "#PLAYER")
                        {
                            bms.setPlayer(int.Parse(left));
                        }
                        else if (tag == "#GENRE")
                        {
                            bms.setGerne(left);
                        }
                        else if (tag == "#TITLE")
                        {
                            bms.setTitle(left);
                        }
                        else if (tag == "#ARTIST")
                        {
                            bms.setArtist(left);
                        }
                        else if (tag == "#BPM")
                        {
                            float bpm;
                            float.TryParse(left, out bpm);
                            bms.setBPMStart(bpm);
                            bms.setBPMMin(bpm);
                            bms.setBPMMax(bpm);
                        }
                        else if (tag == "#PLAYLEVEL")
                        {
                            int level;
                            int.TryParse(left, out level);
                            bms.setLevel(level);
                        }
                        else if (tag == "#RANK")
                        {
                            bms.setRank(int.Parse(left));
                        }
                        else if (tag == "#TOTAL")
                        {
                            int note;
                            int.TryParse(left, out note);
                            bms.setTotalNotes(note);
                        }
                        else if (tag == "#STAGEFILE")
                        {
                            bms.setStageFile(left);
                        }
                        else if (tag != "#BPM" && chkWav == "#BPM")
                        {
                            // 16진수로 표현 불가능 한 BPM 표현
                            double bpmC = 0d;
                            bool trial = double.TryParse(left, out bpmC);
                            GetBPMInfoNum(bpmC, ref bms);
                        }

                        // BPM CHECK
                        else if (int.TryParse(tag.Substring(1, 1), out parsedTag))
                        {
                            // 이외의 경우는 tag가 모두 노트 데이터
                            string noteBuf;
                            string[] noteTok = tag.Split(':');

                            noteBuf = noteTok[0];

                            string leftNote = "";
                            leftNote += noteTok[1];

                            int bar = int.Parse(noteBuf.Substring(1, 3));
                            int ch = int.Parse(noteBuf.Substring(4, 2));
                            if (ch == 3 && leftNote != "00")
                            {
                                // BPM
                                GetBPMInfo(leftNote, ref bms);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    ErrorHandler.LogError("===File Error===");
                    ErrorHandler.LogError("Unexpected BMS sentence found, but ignore");
                    ErrorHandler.LogError("from: " + bms.getFilePath());
                    continue;
                }
            }
            bmsReader.Close();
            
        }

        public void FullAnalyzer(BMS bms)
        {
            // wav ogg 체크
            string[] wavfiles = Directory.GetFiles(bms.getFolderPath(), "*.wav");
            string[] oggfiles = Directory.GetFiles(bms.getFolderPath(), "*.ogg");

            if (wavfiles.Length > 0)
                isWavExist = true;
            if (oggfiles.Length > 0)
                isOggExist = true;

            // input stream 열기 // 기본 Default, 일본어 932, 한국어 949
            int encoding = Const.GetEncoding();
            StreamReader bmsReader = new StreamReader(bms.getFilePath(), System.Text.Encoding.GetEncoding(encoding));

            // 한 줄 씩 읽으면서 분석
            string buf = null;
            while ((buf = bmsReader.ReadLine()) != null)
            {
                try
                {
                    string[] line = buf.Split(' ');
                    if (line.Length != 0 && line[0].StartsWith("#"))
                    {
                        // divide tag and other info
                        string tag = line[0];
                        string left = "";
                        string etc;
                        for (int i = 1; i < line.Length; i++)
                        {
                            etc = line[i];
                            if (left.Length != 0)
                            {
                                left += " ";
                            }
                            left += etc;
                        }

                        string chkWav = tag.Substring(0, 4);
                        int parsedTag = 0;

                        // process each tag
                        if(tag == "#")
                        {
                            // 에러값 예외처리
                            // 여기서 걸러지지 않으면 try-catch로 거름
                            continue;
                        }
                        if (tag == "#PLAYER")
                        {
                            bms.setPlayer(int.Parse(left));
                        }
                        else if (tag == "#GENRE")
                        {
                            bms.setGerne(left);
                        }
                        else if (tag == "#TITLE")
                        {
                            bms.setTitle(left);
                        }
                        else if (tag == "#ARTIST")
                        {
                            bms.setArtist(left);
                        }
                        else if (tag == "#BPM")
                        {
                            float bpm;
                            float.TryParse(left, out bpm);
                            bms.setBPMStart(bpm);
                            bms.setBPMMin(bpm);
                            bms.setBPMMax(bpm);
                        }
                        else if (tag == "#PLAYLEVEL")
                        {
                            int level;
                            int.TryParse(left, out level);
                            bms.setLevel(level);
                        }
                        else if (tag == "#RANK")
                        {
                            bms.setRank(int.Parse(left));
                        }
                        else if (tag == "#TOTAL")
                        {
                            int note;
                            int.TryParse(left, out note);
                            bms.setTotalNotes(note);
                        }
                        else if (tag == "#STAGEFILE")
                        {
                            bms.setStageFile(left);
                        }

                        else if (tag == "#STOP")
                        {
                        }

                        // Long Note Type Check
                        else if (tag == "#LNTYPE")
                        {
                            switch (int.Parse(left))
                            {
                                case 1: bms.setLNType(LNType.Type1); break;
                                case 2: bms.setLNType(LNType.Type2); break;
                            }
                        }
                        else if (tag == "#LNOBJ")
                        {
                            bms.setLNType(LNType.Obj);
                            bms.setLNObj(left);
                        }

                        else if (chkWav == "#WAV")
                        {
                            string filename = "";
                            string[] name = left.Split('.');
                            for (int i = 0; i < name.Length - 1; i++)
                            {
                                filename += name[i] + '.';
                            }
                            if (isWavExist && !isOggExist)
                                filename += "wav";
                            else if (!isWavExist && isOggExist)
                                filename += "ogg";
                            else
                            {
                                // 혼돈의 카오스...
                            }

                            bms.mWavList.Add(tag.Substring(4, 2), filename);
                        }

                        else if (chkWav == "#BMP")
                        {
                            if (left.EndsWith(".mpg") || left.EndsWith(".mp4"))
                            {
                                bms.bgaVideoFile = bms.getFolderPath() + left;
                                isVideoExist = true;
                            }
                            else if (left.EndsWith(".bmp") || left.EndsWith(".jpg")
                                || left.EndsWith(".jpeg") || left.EndsWith(".png"))
                            {
                                // 파일 확장자 처리부터 수행
                                string[] filename = left.Split('.');
                                string filenameCheck = "";
                                for(int i = 0; i < filename.Length - 1; i++)
                                {
                                    filenameCheck += filename[i] + ".";
                                }

                                if (File.Exists(bms.getFolderPath() + filenameCheck + "bmp")) filenameCheck += "bmp";
                                else if (File.Exists(bms.getFolderPath() + filenameCheck + "jpg")) filenameCheck += "jpg";
                                else if (File.Exists(bms.getFolderPath() + filenameCheck + "jpeg")) filenameCheck += "jpeg";
                                else if (File.Exists(bms.getFolderPath() + filenameCheck + "png")) filenameCheck += "png";

                                bms.mBGAImages.Add(tag.Substring(4, 2), UnityTools.createSpriteFromFile(bms.getFolderPath() + filenameCheck));
                            }
                        }
                        else if (tag != "#BPM" && chkWav == "#BPM")
                        {
                            // 16진수로 표현 불가능 한 BPM 표현
                            double bpmC = 0d;
                            bool trial = double.TryParse(left, out bpmC);
                            GetBPMInfoNum(bpmC, ref bms);

                            // 등록 필요
                            bms.mBPMNum.Add(tag.Substring(4, 2), bpmC);
                        }

                        // 문자인 경우는 제외해야 함
                        else if (int.TryParse(tag.Substring(1, 1), out parsedTag))
                        {
                            // 이외의 경우는 tag가 모두 노트 데이터
                            string noteBuf;
                            string[] noteTok = tag.Split(':');

                            noteBuf = noteTok[0];

                            string leftNote = "";
                            leftNote += noteTok[1];

                            int bar = int.Parse(noteBuf.Substring(1, 3));
                            int ch = int.Parse(noteBuf.Substring(4, 2));

                            if (ch == 1)
                            {
                                if (leftNote != "00")
                                {
                                    // 베이스 음악 (MUSIC)
                                    List<string> mlist;
                                    if (bms.mMusic.ContainsKey(bar))
                                    {
                                        mlist = bms.mMusic[bar];
                                        bms.mMusic.Remove(bar);
                                    }
                                    else
                                    {
                                        mlist = new List<string>();
                                    }

                                    mlist.Add(leftNote);

                                    bms.mMusic.Add(bar, mlist);
                                }
                            }
                            else if (ch == 2)
                            {
                                bms.mBarLength.Add(bar, double.Parse(leftNote));
                            }
                            else if (ch == 3 && leftNote != "00")
                            {   // BPM
                                bms.mBPMNote.Add(bar, leftNote);
                                GetBPMInfo(leftNote, ref bms);
                            }
                            else if (ch == 4)
                            {
                                // BGA
                                bms.mBGANote.Add(bar, leftNote);
                            }
                            else if (ch == 6)
                            {
                                // LAYER IMG
                            }
                            else if (ch == 7)
                            {
                                // POOR IMG
                            }
                            else if(ch == 8)
                            {
                                // BPM
                                bms.mBPMNoteType2.Add(bar, leftNote);
                            }
                            else
                            {
                                if (bms.lastBar < bar) bms.lastBar = bar;

                                // bar 데이터 유무 체크
                                if (!bms.mNote.ContainsKey(bar))
                                    bms.mNote[bar] = new Dictionary<int, string>();

                                // bar에 키값 있는지 여부 확인
                                if (bms.mNote[bar].ContainsKey(ch) && bms.mNote[bar][ch] == null)
                                {
                                    bms.mNote[bar].Remove(ch);
                                    bms.mNote[bar].Add(ch, leftNote);
                                }
                                else
                                {
                                    bms.mNote[bar].Add(ch, leftNote);
                                }

                                // 비어있는 노트 파트에 빈 공간을 추가해서 채움
                                if (!bms.mNote[bar].ContainsKey(11)) bms.mNote[bar].Add(11, null);
                                if (!bms.mNote[bar].ContainsKey(12)) bms.mNote[bar].Add(12, null);
                                if (!bms.mNote[bar].ContainsKey(13)) bms.mNote[bar].Add(13, null);
                                if (!bms.mNote[bar].ContainsKey(14)) bms.mNote[bar].Add(14, null);
                                if (!bms.mNote[bar].ContainsKey(15)) bms.mNote[bar].Add(15, null);
                                if (!bms.mNote[bar].ContainsKey(16)) bms.mNote[bar].Add(16, null);
                                if (!bms.mNote[bar].ContainsKey(18)) bms.mNote[bar].Add(18, null);
                                if (!bms.mNote[bar].ContainsKey(19)) bms.mNote[bar].Add(19, null);
                                if (!bms.mNote[bar].ContainsKey(51)) bms.mNote[bar].Add(51, null);
                                if (!bms.mNote[bar].ContainsKey(52)) bms.mNote[bar].Add(52, null);
                                if (!bms.mNote[bar].ContainsKey(53)) bms.mNote[bar].Add(53, null);
                                if (!bms.mNote[bar].ContainsKey(54)) bms.mNote[bar].Add(54, null);
                                if (!bms.mNote[bar].ContainsKey(55)) bms.mNote[bar].Add(55, null);
                                if (!bms.mNote[bar].ContainsKey(56)) bms.mNote[bar].Add(56, null);
                                if (!bms.mNote[bar].ContainsKey(58)) bms.mNote[bar].Add(58, null);
                                if (!bms.mNote[bar].ContainsKey(59)) bms.mNote[bar].Add(59, null);
                            }

                            //bms->mNote.insert(std::pair<int, Bar>(bar, mbar));
                        }
                    }
                }
                catch (Exception e)
                {
                    ErrorHandler.LogError("===File Error===");
                    ErrorHandler.LogError("Unexpected BMS sentence found, but ignore");
                    ErrorHandler.LogError("from: " + bms.getFilePath());
                    continue;
                }
            }
            bmsReader.Close();
        }        

        public bool IsVideoExist()
        {
            return isVideoExist;
        }

        public void GetBPMInfo(string notestr, ref BMS bms)
        {
            int bpmsize = notestr.Length / 2;
            for (int n = 0; n < bpmsize; n++)
            {
                string str = notestr.Substring(n * 2, 2);
                if (str != "00")
                {
                    double bpm = Convert.ToInt32(str, 16);
                    if(bpm < bms.getBPMMin())
                    {
                        bms.setBPMMin(bpm);
                    }
                    if(bpm > bms.getBPMMax())
                    {
                        bms.setBPMMax(bpm);
                    }
                }
            }
        }

        public void GetBPMInfoNum(double bpm, ref BMS bms)
        {
            if (bpm < bms.getBPMMin())
            {
                bms.setBPMMin(bpm);
            }
            if (bpm > bms.getBPMMax())
            {
                bms.setBPMMax(bpm);
            }
        }
    }
}
