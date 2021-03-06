﻿using BMSCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BMSPlayer
{
    public class ResultController : MonoBehaviour
    {
        // Music data
        [Header("Title")]
        public TextMeshProUGUI title;
        public TextMeshProUGUI subtitle;
        public TextMeshProUGUI artist;
        public TextMeshProUGUI subartist;

        // UI
        [Header("Retry desc")]
        public Text txtRetry;
        public Text txtRetrySame;
        public Text txtNext;
        public Text txtRetrySameDesc;

        // Judgement data
        [Header("Judge details")]
        public Text perfect;
        public Text great;
        public Text good;
        public Text ok;
        public Text miss;
        public Text epoor;
        public Text avgrate;
        public Text combo;
        public Text combobreak;
        public Text timediff;
        public Text fast;
        public Text slow;

        [Header("Rank, FC mark")]
        public SpriteRenderer rank;
        public SpriteRenderer fcpfmark;
        public Sprite pfmark;
        public Sprite fcmark;

        // Difference
        [Header("Difference from prev score")]
        public TextMeshProUGUI scorePrev;
        public TextMeshProUGUI scoreNew;
        public TextMeshProUGUI scoreDiff;
        public TextMeshProUGUI scoreDiffR;
        public TextMeshProUGUI missPrev;
        public TextMeshProUGUI missNew;
        public TextMeshProUGUI missDiff;
        public TextMeshProUGUI rankPrev;
        public TextMeshProUGUI rankNew;
        public TextMeshProUGUI targetScore;
        public TextMeshProUGUI targetDiff;
        public Image clearPrev;
        public Image clearNew;

        // Clear Sprite
        [Header("Clear status sprite")]
        public Sprite clearNP;
        public Sprite clearAC;
        public Sprite clearEC;
        public Sprite clearNC;
        public Sprite clearHC;
        public Sprite clearEXC;
        public Sprite clearFC;
        public Sprite clearPF;
        public Sprite clearFail;

        // BG Sound
        [Header("BG Sound")]
        public AudioSource bgLoop;
        public AudioClip[] loop;

        // Record
        private RecordDataManager rdm;
        private RecordData record;

        // Graph
        [Header("Graph")]
        public GraphDrawer Graph;
        public SpriteRenderer GraphBG;
        public Sprite GraphAS;
        public Sprite GraphNR;
        public Sprite GraphHD;
        public Sprite GraphEX;

        // Others
        [Header("Play status desc")]
        public Text Judgement;
        public Text PatternType;
        public Text PatternDifficulty;
        public Text PatternLevel;
        public TextMeshProUGUI noteLayout;

        void Awake()
        {
            Keys.LoadButtonSetting();

            // UI
            txtRetry.text = Const.ResultTxtRetry[(int)Const.Language];
            txtRetrySame.text = Const.ResultTxtRetrySame[(int)Const.Language];
            txtNext.text = Const.ResultTxtNext[(int)Const.Language];
            txtRetrySameDesc.text = Const.ResultTxtRetrySameDesc[(int)Const.Language];

            // 곡 정보
            PlayData Data = new PlayData(Const.PlayingBMSPath);
            BMSAnalyzer analyzer = new BMSAnalyzer();
            analyzer.HeaderAnalyzer(Data.BMS, Const.Encoding);
            title.text = Data.BMS.Title;
            subtitle.text = Data.BMS.SubTitle;
            artist.text = Data.BMS.Artist;
            subartist.text = Data.BMS.SubArtist;

            // 점수 차이
            int ex = Const.ResultScore;
            int total = Const.ResultTotalNote;
            string scoreDiffStr = "";
            if (ex >= 2 * total * 8.5f / 9)
            {
                // MAX-
                int val = total * 2 - ex;
                scoreDiffStr += "MAX-" + val;
            }
            else if (ex >= 2 * total * 8f / 9)
            {
                // AAA+
                int val = ex - total * 2 * 8 / 9;
                scoreDiffStr += "AAA+" + val;
            }
            else if (ex >= 2 * total * 7.5f / 9)
            {
                // AAA-
                int val = total * 2 * 8 / 9 - ex;
                scoreDiffStr += "AAA-" + val;
            }
            else if (ex >= 2 * total * 7f / 9)
            {
                // AA+
                int val = ex - total * 2 * 7 / 9;
                scoreDiffStr += "AA+" + val;
            }
            else if (ex >= 2 * total * 6.5f / 9)
            {
                // AA-
                int val = total * 2 * 7 / 9 - ex;
                scoreDiffStr += "AA-" + val;
            }
            else if (ex >= 2 * total * 6f / 9)
            {
                // A+
                int val = ex - total * 2 * 6 / 9;
                scoreDiffStr += "A+" + val;
            }
            else if (ex >= 2 * total * 5.5f / 9)
            {
                // A-
                int val = total * 2 * 6 / 9 - ex;
                scoreDiffStr += "A-" + val;
            }
            else if (ex >= 2 * total * 5f / 9)
            {
                // B+
                int val = ex - total * 2 * 5 / 9;
                scoreDiffStr += "B+" + val;
            }
            else if (ex >= 2 * total * 4.5f / 9)
            {
                // B-
                int val = total * 2 * 5 / 9 - ex;
                scoreDiffStr += "B-" + val;
            }
            else if (ex >= 2 * total * 4f / 9)
            {
                // C+
                int val = ex - total * 2 * 4 / 9;
                scoreDiffStr += "C+" + val;
            }
            else
            {
                // C-
                int val = total * 2 * 4 / 9 - ex;
                scoreDiffStr += "C-" + val;
            }

            // 패턴 정보
            string difficulty = "";
            switch (Data.BMS.Difficulty)
            {
                case 1:
                    difficulty = "BEGINNER";
                    PatternDifficulty.color = new Color(127f / 255, 1, 97f / 255);
                    break;
                case 2:
                    difficulty = "NORMAL";
                    PatternDifficulty.color = new Color(119f / 255, 251f / 255, 1);
                    break;
                case 3:
                    difficulty = "HYPER";
                    PatternDifficulty.color = new Color(1, 244f / 255, 134f / 255);
                    break;
                case 4:
                    difficulty = "ANOTHER";
                    PatternDifficulty.color = new Color(1, 124f / 255, 124f / 255);
                    break;
                case 5:
                    difficulty = "INSANE";
                    PatternDifficulty.color = new Color(127f / 255, 46f / 255, 178f / 255);
                    break;
                default:
                    difficulty = "UNKNOWN";
                    PatternDifficulty.color = Color.white;
                    break;
            }

            string layout = "";
            switch(Const.NoteLayout)
            {
                case NoteLayout.NORMAL:
                    layout = "NORMAL";
                    break;
                case NoteLayout.RANDOM:
                    layout = "RANDOM";
                    break;
                case NoteLayout.MIRROR:
                    layout = "MIRROR";
                    break;
                case NoteLayout.SRAN:
                    layout = "S-RANDOM";
                    break;
                case NoteLayout.CRAN:
                    layout = "C-RANDOM";
                    break;
                case NoteLayout.CUSTOM:
                    layout = "CUSTOM";
                    break;
            }

            PatternType.text = "SP";
            PatternDifficulty.text = difficulty;
            PatternLevel.text = Data.BMS.Level.ToString();

            switch(Data.BMS.Player)
            {
                case 1:
                    PatternType.color = new Color(1, 51f / 255, 51f / 255);
                    break;
                case 3:
                    PatternType.color = new Color(0, 102f / 255, 1);
                    break;
            }

            PatternLevel.color = new Color(1, 204f / 255, 0);

            // 판정 설정
            switch(Const.JudgeType)
            {
                case JudgeType.ARCADE:
                    Judgement.text = "ARCADE";
                    break;
                case JudgeType.ORIGINAL:
                    switch(Data.BMS.Rank)
                    {
                        case 0:
                            Judgement.text = "ORIGINAL VERY HARD";
                            break;
                        case 1:
                            Judgement.text = "ORIGINAL HARD";
                            break;
                        case 3:
                            Judgement.text = "ORIGINAL EASY";
                            break;
                        case 2:
                        default:
                            Judgement.text = "ORIGINAL NORMAL";
                            break;
                    }
                    break;
                case JudgeType.LR2:
                    switch (Data.BMS.Rank)
                    {
                        case 0:
                            Judgement.text = "LR2 VERY HARD";
                            break;
                        case 1:
                            Judgement.text = "LR2 HARD";
                            break;
                        case 3:
                            Judgement.text = "LR2 EASY";
                            break;
                        case 2:
                        default:
                            Judgement.text = "LR2 NORMAL";
                            break;
                    }
                    break;
                case JudgeType.BEATORAJA:
                    switch (Data.BMS.Rank)
                    {
                        case 0:
                            Judgement.text = "BEATORAJA VERY HARD";
                            break;
                        case 1:
                            Judgement.text = "BEATORAJA HARD";
                            break;
                        case 3:
                            Judgement.text = "BEATORAJA EASY";
                            break;
                        case 2:
                        default:
                            Judgement.text = "BEATORAJA NORMAL";
                            break;
                    }
                    break;
            }

            // 노트 배치
            string noteLayoutStr = layout + " ";
            if(Const.CurrentLayout != null)
            {
                foreach (int i in Const.CurrentLayout)
                {
                    noteLayoutStr += i.ToString() + " ";
                }
            }
            noteLayout.text = noteLayoutStr;

            // 판정
            int vgr = Const.ResultGreat;
            int vgd = Const.ResultGood;
            int vok = Const.ResultOk;
            int vmiss = Const.ResultMiss;
            int vep = Const.ResultEPoor;
            int vcb = Const.ResultComboBreak;
            float rate = Const.ResultAvgRate / 100;
            float vdiff = Const.ResultTimeDiff;
            string rankstr = Const.ResultRank;

            perfect.text = Const.ResultPerfect.ToString();
            great.text = Const.ResultGreat.ToString();
            good.text = vgd.ToString();
            ok.text = vok.ToString();
            miss.text = (vmiss+vep).ToString();
            epoor.text = vep.ToString();
            combobreak.text = vcb.ToString();
            fast.text = Const.ResultFast.ToString();
            slow.text = Const.ResultSlow.ToString();

            avgrate.text = (rate * 100).ToString("0.00") + "%";
            combo.text = Const.ResultMaxCombo.ToString();
            timediff.text = (vdiff * 100).ToString("0.00") + "ms";

            // 차이
            scoreDiffR.text = scoreDiffStr;
            scorePrev.text = Const.MyBestScore.ToString();
            scoreNew.text = Const.ResultScore.ToString();
            missNew.text = vcb.ToString();
            targetScore.text = Const.ResultTarget.ToString();
            int scorediff = Const.ResultScore - Const.ResultTarget;

            if (scorediff > 0)
            {
                targetDiff.text = "+" + (scorediff).ToString();
            }
            else
            {
                targetDiff.text = scorediff.ToString();
            }

            if(Const.ResultProcessedNote < Const.ResultTotalNote)
            {
                Const.Clear = ClearType.FAIL;
            }
            else
            {
                if (vcb == 0)
                {
                    if (vgd == 0 && vgr == 0)
                    {
                        fcpfmark.gameObject.SetActive(true);
                        fcpfmark.sprite = pfmark;
                        Const.Clear = ClearType.PERFECT;
                    }
                    else
                    {
                        fcpfmark.gameObject.SetActive(true);
                        fcpfmark.sprite = fcmark;
                        Const.Clear = ClearType.FULLCB;
                    }
                }
            }

            switch (Const.Clear)
            {
                case ClearType.ASSISTCLEAR:
                    clearNew.sprite = clearAC;
                    break;
                case ClearType.EASYCLEAR:
                    clearNew.sprite = clearEC;
                    break;
                case ClearType.NORMALCLEAR:
                    clearNew.sprite = clearNC;
                    break;
                case ClearType.HARDCLEAR:
                    clearNew.sprite = clearHC;
                    break;
                case ClearType.EXCLEAR:
                    clearNew.sprite = clearEXC;
                    break;
                case ClearType.FULLCB:
                    clearNew.sprite = clearFC;
                    break;
                case ClearType.PERFECT:
                    clearNew.sprite = clearPF;
                    break;
                case ClearType.FAIL:
                    clearNew.sprite = clearFail;
                    break;
            }

            if (Const.MyBestPrev != null)
            {
                rankPrev.text = Const.MyBestRank.ToUpper();
                missPrev.text = Const.MyBestPrev.CBreak.ToString();
                switch (Const.MyBestPrev.Clear)
                {
                    case ClearType.ASSISTCLEAR:
                        clearPrev.sprite = clearAC;
                        break;
                    case ClearType.EASYCLEAR:
                        clearPrev.sprite = clearEC;
                        break;
                    case ClearType.NORMALCLEAR:
                        clearPrev.sprite = clearNC;
                        break;
                    case ClearType.HARDCLEAR:
                        clearPrev.sprite = clearHC;
                        break;
                    case ClearType.EXCLEAR:
                        clearPrev.sprite = clearEXC;
                        break;
                    case ClearType.FULLCB:
                        clearPrev.sprite = clearFC;
                        break;
                    case ClearType.PERFECT:
                        clearPrev.sprite = clearPF;
                        break;
                    case ClearType.FAIL:
                        clearPrev.sprite = clearFail;
                        break;
                }

                int vscorediff = Const.ResultScore - Const.MyBestScore;
                if(vscorediff < 0)
                {
                    scoreDiff.text = vscorediff.ToString();
                }
                else
                {
                    scoreDiff.text = "+" + vscorediff.ToString();
                }

                int vmissdiff = vmiss - Const.MyBestPrev.Poor;
                if(vmissdiff < 0)
                {
                    missDiff.text = vmissdiff.ToString();
                }
                else
                {
                    missDiff.text = "+" + vmissdiff.ToString();
                }
            }
            else
            {
                missPrev.text = "-";
                clearPrev.sprite = clearNP;
                rankPrev.text = "-";
                missDiff.text = "+" + vmiss.ToString();
                scoreDiff.text = "+" + Const.ResultScore.ToString();
            }

            switch(rankstr)
            {
                case "aaa":
                    rank.sprite = SkinSetting.RankAAA;
                    rankNew.text = "AAA";
                    break;
                case "aa":
                    rank.sprite = SkinSetting.RankAA;
                    rankNew.text = "AA";
                    break;
                case "a":
                    rank.sprite = SkinSetting.RankA;
                    rankNew.text = "A";
                    break;
                case "b":
                    rank.sprite = SkinSetting.RankB;
                    rankNew.text = "B";
                    break;
                case "c":
                    rank.sprite = SkinSetting.RankC;
                    rankNew.text = "C";
                    break;
                case "d":
                    rank.sprite = SkinSetting.RankD;
                    rankNew.text = "D";
                    break;
                case "e":
                    rank.sprite = SkinSetting.RankE;
                    rankNew.text = "E";
                    break;
                case "f":
                    rank.sprite = SkinSetting.RankF;
                    rankNew.text = "F";
                    break;
            }

            // 데이터 기록
            if(Const.Auto == AutoPlayType.OFF)
            {
                rdm = new RecordDataManager();
                
                // 테스트용 리셋
                /*rdm.DropTable();
                rdm.CreateNewTable();*/

                string nrank;
                int nscore;
                GaugeType njtype;
                ClearType nclear;
                int npf;
                int ngr;
                int ngd;
                int nok;
                int npr;
                int ncombo;
                int ncb;
                int ncnt;

                // 기존 등록된 스코어와 비교 수행
                // 1. 스코어가 높다면 - 스코어와 판정을 갱신
                // 2. 게이지 레벨이 높다면 - 게이지만 갱신
                // 3. 스코어 무관하게 풀콤을 했다면 - 풀콤 여부 갱신

                FileStream fstream = File.OpenRead(Data.BMS.FilePath);
                MD5 md5 = new MD5CryptoServiceProvider();
                var bytehash = md5.ComputeHash(fstream);
                fstream.Close();
                md5.Clear();

                string hash = BitConverter.ToString(bytehash).Replace("-", "").ToLower();

                // 기존 기록 값 가져오기
                RecordData prev = rdm.GetFullClearStat(hash);

                if(prev != null)
                {
                    ncnt = prev.PlayCnt + 1;
                    // 1. 스코어 비교
                    if (Const.ResultScore > prev.Score)
                    {
                        nrank = Const.ResultRank;
                        nscore = Const.ResultScore;
                        npf = Const.ResultPerfect;
                        ngr = Const.ResultGreat;
                        ngd = Const.ResultGood;
                        nok = Const.ResultOk;
                        npr = Const.ResultMiss;
                        ncb = Const.ResultComboBreak;
                    }
                    else
                    {
                        nrank = prev.Rank;
                        nscore = prev.Score;
                        npf = prev.Perfect;
                        ngr = prev.Great;
                        ngd = prev.Good;
                        nok = prev.OK;
                        npr = prev.Poor;
                        ncombo = prev.MaxCombo;
                        ncb = prev.CBreak;
                    }

                    // 2. 게이지 레벨 비교
                    if(Const.Clear < prev.Clear)
                    {
                        njtype = Const.GaugeType;
                        nclear = Const.Clear;
                    }
                    else
                    {
                        njtype = prev.GaugeType;
                        nclear = prev.Clear;
                    }
                    // Auto Turntable은 무조건 Assist
                    if(Const.Auto == AutoPlayType.TURNTABLE)
                    {
                        nclear = ClearType.ASSISTCLEAR;
                    }

                    // 3. 콤보
                    if(Const.ResultMaxCombo > prev.MaxCombo)
                    {
                        ncombo = Const.ResultMaxCombo;
                    }
                    else
                    {
                        ncombo = prev.MaxCombo;
                    }
                }
                else
                {
                    // 새 기록 바로 적용
                    nrank = Const.ResultRank;
                    nscore = Const.ResultScore;
                    npf = Const.ResultPerfect;
                    ngr = Const.ResultGreat;
                    ngd = Const.ResultGood;
                    nok = Const.ResultOk;
                    npr = Const.ResultMiss;
                    ncombo = Const.ResultMaxCombo;
                    ncb = Const.ResultComboBreak;
                    njtype = Const.GaugeType;
                    nclear = Const.Clear;
                    ncnt = 0;
                }
                

                record = new RecordData(
                    hash,
                    nrank, nscore, (int)njtype, (int)nclear,
                    npf, ngr, ngd, nok, npr, ncombo, ncb, ncnt
                );

                rdm.RegisterRecord(record);
                rdm.Close();
            }
        }

        private void Start()
        {
            // MusicLoop on
            System.Random rand = new System.Random();
            AudioClip loopMusic = loop[rand.Next(0, loop.Length)];
            bgLoop.clip = loopMusic;
            bgLoop.loop = true;
            bgLoop.playOnAwake = true;
            bgLoop.Play();

            switch(Const.GaugeType)
            {
                case GaugeType.ASSISTED:
                    GraphBG.sprite = GraphAS;
                    break;
                case GaugeType.EASY:
                case GaugeType.NORMAL:
                    GraphBG.sprite = GraphNR;
                    break;
                case GaugeType.HARD:
                    GraphBG.sprite = GraphHD;
                    break;
                case GaugeType.EXHARD:
                    GraphBG.sprite = GraphEX;
                    break;
            }

            // 그래프 그리기
            if(Const.ResultGraph != null)
            {
                Graph.AddVertexes(Const.ResultGraph);
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.R) || GetBtnDown(2))
            {
                Const.ChangeLayout = true;
                SceneManager.LoadScene("PlayScreen", LoadSceneMode.Single);
            }

            if (Input.GetKeyDown(KeyCode.T) || GetBtnDown(6))
            {
                Const.ChangeLayout = false;
                SceneManager.LoadScene("PlayScreen", LoadSceneMode.Single);
            }

            if (Input.GetKeyDown(KeyCode.Return) || GetBtnWhite())
            {
                SceneManager.LoadScene("MusicSelect", LoadSceneMode.Single);
            }
        }

        private bool GetBtnWhite()
        {
            return
                GetBtnDown(1) ||
                GetBtnDown(3) ||
                GetBtnDown(5) ||
                GetBtnDown(7);
        }

        private bool GetBtnDown(int i)
        {
            return
                (Keys.btnAxisSet1[i] && Keys.btnSet1[i].EndsWith("p") &&
                    Keys.GetAxisValue(Keys.btnSet1[i]) > 0.5) ||
                (Keys.btnAxisSet1[i] && Keys.btnSet1[i].EndsWith("m") &&
                    Keys.GetAxisValue(Keys.btnSet1[i]) < -0.5) ||
                (Keys.btnAxisSet2[i] && Keys.btnSet2[i].EndsWith("p") &&
                    Keys.GetAxisValue(Keys.btnSet2[i]) > 0.5) ||
                (Keys.btnAxisSet2[i] && Keys.btnSet2[i].EndsWith("m") &&
                    Keys.GetAxisValue(Keys.btnSet2[i]) < -0.5) ||
                (!Keys.btnAxisSet1[i] && Keys.GetKeyDown(Keys.btnSet1[i])) ||
                (!Keys.btnAxisSet2[i] && Keys.GetKeyDown(Keys.btnSet2[i]));
        }
    }
}
