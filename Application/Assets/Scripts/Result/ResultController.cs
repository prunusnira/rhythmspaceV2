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

        public GameObject fcpfmark;
        public Material failMaterial;
        public Image rank;
        public Image cleared;

        // Difference
        public TextMeshProUGUI scorePrev;
        public TextMeshProUGUI scoreNew;
        public TextMeshProUGUI scoreDiff;
        public TextMeshProUGUI missPrev;
        public TextMeshProUGUI missNew;
        public TextMeshProUGUI missDiff;
        public TextMeshProUGUI clearPrev;
        public TextMeshProUGUI clearNew;
        public TextMeshProUGUI rankPrev;
        public TextMeshProUGUI rankNew;
        public TextMeshProUGUI targetScore;
        public TextMeshProUGUI targetDiff;

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
        private ClearType isClear;

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
            // 곡 정보
            PlayData Data = new PlayData();
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

            isClear = Const.Clear;

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

            switch (isClear)
            {
                case ClearType.ASSISTCLEAR:
                    clearNew.text = "ASSIST CLEAR";
                    break;
                case ClearType.EASYCLEAR:
                    clearNew.text = "EASY CLEAR";
                    break;
                case ClearType.NORMALCLEAR:
                    clearNew.text = "CLEAR";
                    break;
                case ClearType.HARDCLEAR:
                    clearNew.text = "HARD CLEAR";
                    break;
                case ClearType.EXCLEAR:
                    clearNew.text = "EX CLEAR";
                    break;
                case ClearType.FULLCB:
                    clearNew.text = "FULL COMBO";
                    break;
                case ClearType.PERFECT:
                    clearNew.text = "PERFECT";
                    break;
                case ClearType.FAIL:
                    clearNew.text = "FAILED";
                    break;
            }

            if (Const.MyBestPrev != null)
            {
                rankPrev.text = Const.MyBestRank.ToUpper();
                missPrev.text = Const.MyBestPrev.Poor.ToString();
                switch (Const.MyBestPrev.Clear)
                {
                    case ClearType.ASSISTCLEAR:
                        clearPrev.text = "ASSIST CLEAR";
                        break;
                    case ClearType.EASYCLEAR:
                        clearPrev.text = "EASY CLEAR";
                        break;
                    case ClearType.NORMALCLEAR:
                        clearPrev.text = "CLEAR";
                        break;
                    case ClearType.HARDCLEAR:
                        clearPrev.text = "HARD CLEAR";
                        break;
                    case ClearType.EXCLEAR:
                        clearPrev.text = "EX CLEAR";
                        break;
                    case ClearType.FULLCB:
                        clearPrev.text = "FULL COMBO";
                        break;
                    case ClearType.PERFECT:
                        clearPrev.text = "PERFECT";
                        break;
                    case ClearType.FAIL:
                        clearPrev.text = "FAILED";
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
                clearPrev.text = "NO PLAY";
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

            if (isClear == ClearType.FAIL)
            {
                cleared.sprite = failed;
            }

            if (vcb == 0 && Const.Auto == AutoPlayType.OFF)
            {
                if (vgd == 0)
                {
                    fcpfmark.SetActive(true);
                    fcpfmark.GetComponent<Image>().sprite = pfmark;
                }
                else
                {
                    fcpfmark.SetActive(true);
                    fcpfmark.GetComponent<Image>().sprite = fcmark;
                }
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

                string hash = BitConverter.ToString(bytehash);

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
            if (Input.GetKeyDown(KeyCode.R))
            {
                Const.ChangeLayout = true;
                SceneManager.LoadScene("PlayScreen", LoadSceneMode.Single);
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                Const.ChangeLayout = false;
                SceneManager.LoadScene("PlayScreen", LoadSceneMode.Single);
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                SceneManager.LoadScene("MusicSelect", LoadSceneMode.Single);
            }
        }

        public GameObject fadeCube;
        private bool isFading = false;

        private void OnGUI()
        {
            if (isFading)
            {
                StartCoroutine("FadeOut");
            }
        }

        IEnumerator FadeOut()
        {
            if (fadeCube.GetComponent<Renderer>().material.color.a < 1.0f)
            {
                Color c = fadeCube.GetComponent<Renderer>().material.color;
                c.a += 0.05f;
                fadeCube.GetComponent<Renderer>().material.color = c;
                yield return new WaitForSeconds(0.2f);
            }
            else
            {
                isFading = false;
                yield return null;
            }
        }
    }
}
