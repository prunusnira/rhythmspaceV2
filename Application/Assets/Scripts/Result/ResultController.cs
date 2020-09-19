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

        public Sprite ranks;
        public Sprite ranka;
        public Sprite rankb;
        public Sprite rankc;
        public Sprite rankf;
        public Sprite pfmark;
        public Sprite fcmark;

        public Sprite failed;
        private int isClear = 0;

        void Awake()
        {
            // 곡 정보
            PlayData Data = new PlayData();
            BMSAnalyzer analyzer = new BMSAnalyzer();
            analyzer.HeaderAnalyzer(Data.BMS);
            gerne.text = Data.BMS.getGerne();
            title.text = Data.BMS.getTitle();
            artist.text = Data.BMS.getArtist();

            // 판정
            int vgd = Const.GetResultGood();
            int vok = Const.GetResultOk();
            int vmiss = Const.GetResultMiss();
            int vcb = Const.GetResultComboBreak();
            float rate = Const.GetResultAvgRate() / 100;
            float vdiff = Const.GetResultTimeDiff();

            perfect.text = Const.GetResultPerfect().ToString();
            great.text = Const.GetResultGreat().ToString();
            good.text = vgd.ToString();
            ok.text = vok.ToString();
            miss.text = vmiss.ToString();
            combobreak.text = vcb.ToString();
            avgrate.text = (rate * 100).ToString("0.00") + "%";
            exscore.text = Const.GetResultScore().ToString();
            combo.text = Const.GetResultMaxCombo().ToString();
            timediff.text = vdiff.ToString("0.00") + "ms";

            isClear = Const.GetClear();

            if (rate > 98)
                rank.sprite = ranks;
            else if (rate > 93)
                rank.sprite = ranka;
            else if (rate > 85)
                rank.sprite = rankb;
            else
                rank.sprite = rankc;

            if (isClear != 1)
            {
                rank.sprite = rankf;
                cleared.sprite = failed;
                //loop.GetComponent<Renderer>().material = failMaterial;
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
