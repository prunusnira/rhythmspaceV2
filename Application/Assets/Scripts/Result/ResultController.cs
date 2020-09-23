using BMSCore;
using System.Collections;
using System.Collections.Generic;
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

            if (isClear != ClearType.FAIL)
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
