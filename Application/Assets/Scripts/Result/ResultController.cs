using BMSCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
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

        private RecordDataManager rdm;
        private RecordData record;

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

                string nrank;
                int nscore;
                JudgeType njtype;
                ClearType nclear;
                int npf;
                int ngr;
                int ngd;
                int nok;
                int npr;
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
                        ncb = prev.CBreak;
                    }

                    // 2. 게이지 레벨 비교
                    if(Const.Clear > prev.Clear)
                    {
                        njtype = Const.JudgeType;
                        nclear = Const.Clear;
                    }
                    else
                    {
                        njtype = prev.GaugeType;
                        nclear = prev.Clear;
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
                    ncb = Const.ResultComboBreak;
                    njtype = Const.JudgeType;
                    nclear = Const.Clear;
                }
                

                record = new RecordData(
                    hash,
                    nrank, nscore, (int)njtype, (int)nclear,
                    npf, ngr, ngd, nok, npr, ncb
                );

                // 테스트용 리셋
                /*rdm.DropTable();
                rdm.CreateNewTable();*/

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
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene("PlayScreen");
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                SceneManager.LoadScene("MusicSelect");
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
