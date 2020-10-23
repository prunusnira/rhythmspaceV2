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
        public Text gerne;
        public Text title;
        public Text artist;

        // Judgement data
        public Text perfect;
        public Text great;
        public Text good;
        public Text ok;
        public Text miss;
        public Text avgrate;
        public Text exscore;
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
        public TextMeshProUGUI comboPrev;
        public TextMeshProUGUI comboNew;
        public TextMeshProUGUI comboDiff;
        public TextMeshProUGUI clearPrev;
        public TextMeshProUGUI clearNew;

        // Rank
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

        void Awake()
        {
            // 곡 정보
            PlayData Data = new PlayData();
            BMSAnalyzer analyzer = new BMSAnalyzer();
            analyzer.HeaderAnalyzer(Data.BMS);
            gerne.text = Data.BMS.Gerne;
            title.text = Data.BMS.Title;
            artist.text = Data.BMS.Artist;

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
            exscore.text = Const.ResultScore.ToString();
            combo.text = Const.ResultMaxCombo.ToString();
            timediff.text = vdiff.ToString("0.00") + "ms";

            isClear = Const.Clear;

            // 차이
            scorePrev.text = Const.MyBestScore.ToString();
            scoreNew.text = Const.ResultScore.ToString();
            missNew.text = vmiss.ToString();
            comboNew.text = Const.ResultMaxCombo.ToString();

            switch (isClear)
            {
                case ClearType.ASSISTCLEAR:
                    clearNew.text = "ASSISTED CLEAR";
                    break;
                case ClearType.EASYCLEAR:
                    clearNew.text = "EASY CLEAR";
                    break;
                case ClearType.NORMALCLEAR:
                    clearNew.text = "NORMAL CLEAR";
                    break;
                case ClearType.HARDCLEAR:
                    clearNew.text = "HARD CLEAR";
                    break;
                case ClearType.EXCLEAR:
                    clearNew.text = "EX CLEAR";
                    break;
                case ClearType.FULLCB:
                    clearNew.text = "FULLCOMBO";
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
                missPrev.text = Const.MyBestPrev.Poor.ToString();
                comboPrev.text = Const.MyBestPrev.MaxCombo.ToString();
                switch (Const.MyBestPrev.Clear)
                {
                    case ClearType.ASSISTCLEAR:
                        clearPrev.text = "ASSISTED CLEAR";
                        break;
                    case ClearType.EASYCLEAR:
                        clearPrev.text = "EASY CLEAR";
                        break;
                    case ClearType.NORMALCLEAR:
                        clearPrev.text = "NORMAL CLEAR";
                        break;
                    case ClearType.HARDCLEAR:
                        clearPrev.text = "HARD CLEAR";
                        break;
                    case ClearType.EXCLEAR:
                        clearPrev.text = "EX CLEAR";
                        break;
                    case ClearType.FULLCB:
                        clearPrev.text = "FULLCOMBO";
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

                int vcombodiff = Const.ResultMaxCombo - Const.MyBestPrev.MaxCombo;
                if (vcombodiff < 0)
                {
                    comboDiff.text = vcombodiff.ToString();
                }
                else
                {
                    comboDiff.text = "+" + vcombodiff.ToString();
                }
            }
            else
            {
                missPrev.text = "0";
                comboPrev.text = "0";
                clearPrev.text = "NO PLAY";
                missDiff.text = "+" + vmiss.ToString();
                scoreDiff.text = "+" + Const.ResultScore.ToString();
                comboDiff.text = "+" + Const.ResultMaxCombo.ToString();
            }

            switch(rankstr)
            {
                case "aaa":
                    rank.sprite = rankaaa;
                    break;
                case "aa":
                    rank.sprite = rankaa;
                    break;
                case "a":
                    rank.sprite = ranka;
                    break;
                case "b":
                    rank.sprite = rankb;
                    break;
                case "c":
                    rank.sprite = rankc;
                    break;
                case "d":
                    rank.sprite = rankd;
                    break;
                case "e":
                    rank.sprite = ranke;
                    break;
                case "f":
                    rank.sprite = rankf;
                    break;
            }

            if (isClear == ClearType.FAIL)
            {
                rank.sprite = rankf;
                cleared.sprite = failed;
            }

            if (vmiss == 0)
            {
                if (vok == 0)
                {
                    if (vgd == 0)
                    {
                        fcpfmark.SetActive(true);
                        fcpfmark.GetComponent<Image>().sprite = pfmark;
                    }
                }
                else
                {
                    fcpfmark.SetActive(true);
                    fcpfmark.GetComponent<Image>().sprite = fcmark;
                }
            }

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
                }
                

                record = new RecordData(
                    hash,
                    nrank, nscore, (int)njtype, (int)nclear,
                    npf, ngr, ngd, nok, npr, ncombo, ncb
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
