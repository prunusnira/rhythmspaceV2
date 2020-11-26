using BMSCore;
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
        public TextMeshProUGUI title;
        public TextMeshProUGUI subtitle;

        // UI
        public Text txtRetry;
        public Text txtRetrySame;
        public Text txtNext;
        public Text txtRetrySameDesc;

        // Judgement data
        public Text perfect;
        public Text great;
        public Text good;
        public Text ok;
        public Text miss;
        public Text avgrate;
        public Text combo;
        public Text combobreak;
        public Text timediff;
        public Text fast;
        public Text slow;

        public SpriteRenderer rank;
        public SpriteRenderer fcpfmark;

        // Difference
        public TextMeshProUGUI scorePrev;
        public TextMeshProUGUI scoreNew;
        public TextMeshProUGUI scoreDiff;
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
        public Sprite clearNP;
        public Sprite clearAC;
        public Sprite clearEC;
        public Sprite clearNC;
        public Sprite clearHC;
        public Sprite clearEXC;
        public Sprite clearFC;
        public Sprite clearPF;
        public Sprite clearFail;

        // Rank Sprite
        public Sprite rankaaa;
        public Sprite rankaa;
        public Sprite ranka;
        public Sprite rankb;
        public Sprite rankc;
        public Sprite rankd;
        public Sprite ranke;
        public Sprite rankf;
        public Sprite pfmark;
        public Sprite fcmark;

        public Sprite failed;

        // BG Sound
        public AudioSource bgLoop;
        public AudioClip[] loop;

        // Record
        private RecordDataManager rdm;
        private RecordData record;

        // Graph
        public GraphDrawer Graph;
        public SpriteRenderer GraphBG;
        public Sprite GraphAS;
        public Sprite GraphNR;
        public Sprite GraphHD;
        public Sprite GraphEX;

        // Others
        public Text Judgement;
        public Text Pattern;

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

            // 패턴 정보
            string difficulty = "";
            switch (Data.BMS.Difficulty)
            {
                case 1:
                    difficulty = "BEGINNER";
                    break;
                case 2:
                    difficulty = "NORMAL";
                    break;
                case 3:
                    difficulty = "HYPER";
                    break;
                case 4:
                    difficulty = "ANOTHER";
                    break;
                case 5:
                    difficulty = "INSANE";
                    break;
                default:
                    difficulty = "UNKNOWN";
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
                    layout = "S-RAN";
                    break;
                case NoteLayout.CRAN:
                    layout = "C-RAN";
                    break;
                case NoteLayout.CUSTOM:
                    layout = "CUSTOM";
                    break;
            }

            Pattern.text =
                "SP " + difficulty +
                " " + Data.BMS.Level +
                "     |     LAYOUT: " + layout;

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

            // 판정
            int vgr = Const.ResultGreat;
            int vgd = Const.ResultGood;
            int vok = Const.ResultOk;
            int vmiss = Const.ResultMiss;
            int vcb = Const.ResultComboBreak;
            float rate = Const.ResultAvgRate / 100;
            float vdiff = Const.ResultTimeDiff;
            string rankstr = Const.ResultRank;

            perfect.text = Const.ResultPerfect.ToString();
            great.text = Const.ResultGreat.ToString();
            good.text = vgd.ToString();
            ok.text = vok.ToString();
            miss.text = vmiss.ToString();
            combobreak.text = vcb.ToString();
            fast.text = Const.ResultFast.ToString();
            slow.text = Const.ResultSlow.ToString();

            avgrate.text = (rate * 100).ToString("0.00") + "%";
            combo.text = Const.ResultMaxCombo.ToString();
            timediff.text = (vdiff * 100).ToString("0.00") + "ms";

            // 차이
            scorePrev.text = Const.MyBestScore.ToString();
            scoreNew.text = Const.ResultScore.ToString();
            missNew.text = vmiss.ToString();
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
                missPrev.text = Const.MyBestPrev.Poor.ToString();
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
                    rank.sprite = rankaaa;
                    rankNew.text = "AAA";
                    break;
                case "aa":
                    rank.sprite = rankaa;
                    rankNew.text = "AA";
                    break;
                case "a":
                    rank.sprite = ranka;
                    rankNew.text = "A";
                    break;
                case "b":
                    rank.sprite = rankb;
                    rankNew.text = "B";
                    break;
                case "c":
                    rank.sprite = rankc;
                    rankNew.text = "C";
                    break;
                case "d":
                    rank.sprite = rankd;
                    rankNew.text = "D";
                    break;
                case "e":
                    rank.sprite = ranke;
                    rankNew.text = "E";
                    break;
                case "f":
                    rank.sprite = rankf;
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
                MD5 md5 = MD5.Create();
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
                    if(Const.Clear > prev.Clear)
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
