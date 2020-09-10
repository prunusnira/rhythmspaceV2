using monoflow;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

namespace BMSPlayer
{
	// 플레이 화면의 모든 UI 요소 컨트롤
	public class PlayUI : MonoBehaviour {
        // UI Object
        public GameObject layerJudgeAll;
        public GameObject layerPauseMenu;

        // Pause Menu
        public Button btnRestart;
        public Button btnExit;

        public Sprite cb0;
        public Sprite cb1;
        public Sprite cb2;
        public Sprite cb3;
        public Sprite cb4;
        public Sprite cb5;
        public Sprite cb6;
        public Sprite cb7;
        public Sprite cb8;
        public Sprite cb9;

        // Judge type ED
        public GameObject judgeTypeED;

        // Judge sprite renderer
        public GameObject comboLayerED;
        public SpriteRenderer judgeSpriteED;
        public TextMesh txtTimingED;

        // Judge sprite
        public Sprite spPerfect;
        public Sprite spGreat;
        public Sprite spGood;
        public Sprite spOk;
        public Sprite spMiss;

        // Judge type BM
        public GameObject judgeTypeBM;
        public TextMesh txtTimingPercentBM;
        public TextMesh txtTimingMsBM;
        public TextMesh txtJudgeBM;

        private long timeLastComboPopup;
        private long timeLastTimingPopup;

        // Side judge
        public Text txtAvgDiff;
        public Text txtAvgRate;
        public Text txtPerfect;
        public Text txtGreat;
        public Text txtGood;
        public Text txtOk;
        public Text txtMiss;

        // Option
        public Text txtSpeed;
        public Text txtBPM;
        public Text txtCalcSpd;

        // Gear
        public GameObject playArea;
        public TextMesh gearCombo;
        public TextMesh gearScore;
        public TextMesh gearExScore;
        public TextMesh gearSpeed;
        public TextMesh gearHP;
        public TextMesh gearGuageType;

        // Beam
        public GameObject beam1;
        public GameObject beam2;
        public GameObject beam3;
        public GameObject beam4;
        public GameObject beam5;
        public GameObject beam6;
        public GameObject beam7;
        public GameObject beam8;

        // HPBar
        private HPController hpController;
        public TextMesh hpBarType;
        public SpriteRenderer hpBar;
        public Sprite hpBarAssisted;
        public Sprite hpBarEasy;
        public Sprite hpBarNormal;
        public Sprite hpBarHard;
        public Sprite hpBarExHard;
        
        // Game Object
        public GameObject startLine; // 노트가 생성되는 위치
        public GameObject lastLine; // 판정선
        public GameObject noteWhite;
        public GameObject noteBlue;
        public GameObject notePink;
        public GameObject noteEmpty;
        public GameObject noteNouse;
        
        public GameObject playerPanel;
        public GameObject btnWhite;
        public GameObject btnBlue;

        public MPMP bgaVideo;
        public SpriteRenderer bgaImage;
        public GameObject[] touches;

        public GameObject fadeCube;
        private bool isFading = false;

        private int pauseSel = 0;
        public Sprite normalBtn;
        public Sprite selectBtn;

        public void Awake()
        {
            if (Const.GetPJudge() == 0) layerJudgeAll.SetActive(false);
            hpController = GetComponent<HPController>();

            switch(Const.GetJudgeType())
            {
                case JudgeType.ASSISTED:
                    hpBarType.text = "ASSISTED";
                    hpBarType.color = new Color(206f / 255, 159f / 255, 1f);
                    hpBar.sprite = hpBarAssisted;
                    break;
                case JudgeType.EASY:
                    hpBarType.text = "EASY";
                    hpBarType.color = new Color(159f / 255, 1f, 180f / 255);
                    hpBar.sprite = hpBarEasy;
                    break;
                case JudgeType.NORMAL:
                    hpBarType.text = "NORMAL";
                    hpBarType.color = new Color(159f / 255, 215f / 255, 1f);
                    hpBar.sprite = hpBarNormal;
                    break;
                case JudgeType.HARD:
                    hpBarType.text = "HARD";
                    hpBarType.color = new Color(1f, 159f / 255, 159f / 255);
                    hpBar.sprite = hpBarHard;
                    break;
                case JudgeType.EXHARD:
                    hpBarType.text = "EX-HARD";
                    hpBarType.color = new Color(246f / 255, 1f, 159f / 255);
                    hpBar.sprite = hpBarExHard;
                    break;
                default:
                    hpBarType.text = "NORMAL";
                    hpBarType.color = new Color(159f / 255, 215f / 255, 1f);
                    hpBar.sprite = hpBarNormal;
                    break;
            }

            btnRestart.gameObject.GetComponent<Image>().sprite = normalBtn;
            btnExit.gameObject.GetComponent<Image>().sprite = normalBtn;

            if(Const.GetJudgeUIType() == JudgeUIType.BM)
            {
                judgeTypeED.SetActive(false);
            }
            else
            {
                judgeTypeBM.SetActive(false);
            }
            UpdateHP(hpController.GetHP());
        }

		public void UpdateSpeed(double bpm)
        {
			int speed = Const.GetSpeed();
            txtSpeed.text = ((float)speed / 100).ToString("0.00");
            txtBPM.text = bpm.ToString("0.00");
            txtCalcSpd.text = ((bpm * speed) / 100).ToString("0.00");

            gearSpeed.text = ((float)speed / 100).ToString("0.00") + "x";
        }

		public void UpdateHP(int hp)
        {
            float chp = (float)hp / hpController.GetHPMax();
            hpBar.material.SetFloat("_Progress", chp);
            gearHP.text = (chp * 100).ToString("0.00") + "%";
        }

        public void SetFade()
        {
            isFading = true;
            fadeCube.SetActive(true);
        }

        public float GetFaderAlpha()
        {
            return fadeCube.GetComponent<Renderer>().material.color.a;
        }

        private void OnGUI()
        {
            if (isFading)
            {
                StartCoroutine("FadeOut");
            }
        }

        private void Update()
        {
            if (DateTime.Now.Ticks / 10000 - timeLastComboPopup > 1000)
            {
                if(Const.GetJudgeUIType() == JudgeUIType.ED)
                {
                    comboLayerED.SetActive(false);
                    judgeSpriteED.gameObject.SetActive(false);
                    txtTimingED.gameObject.SetActive(false);
                }
                else
                {
                    txtJudgeBM.gameObject.SetActive(false);
                    txtTimingPercentBM.gameObject.SetActive(false);
                    txtTimingMsBM.gameObject.SetActive(false);
                }
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

        public void UpdateScore(int score)
        {
            gearScore.text = score.ToString();
        }

        public void UpdateExScore(int score)
        {
            gearExScore.text = score.ToString();
        }

        public void UpdateMaxCombo(int combo)
        {
            gearCombo.text = combo.ToString();
        }

        public void UpdateJudge(TimingType judgetype, int combo, string accuracy, int ms)
        {
            string judgeStr = "";
            // Judge update
            switch(judgetype)
            {
                case TimingType.PERFECT:
                    judgeSpriteED.sprite = spPerfect;
                    judgeStr = "PERFECT";
                    break;
                case TimingType.GREAT:
                    judgeSpriteED.sprite = spGreat;
                    judgeStr = "GREAT";
                    break;
                case TimingType.GOOD:
                    judgeSpriteED.sprite = spGood;
                    judgeStr = "GOOD";
                    break;
                case TimingType.BAD:
                    judgeSpriteED.sprite = spOk;
                    judgeStr = "BAD";
                    break;
                case TimingType.POOR:
                    judgeSpriteED.sprite = spMiss;
                    judgeStr = "POOR";
                    break;
            }

            // Combo update

            if (Const.GetJudgeUIType() == JudgeUIType.ED)
            {
                foreach (Transform t in comboLayerED.transform)
                {
                    Destroy(t.gameObject);
                }
                if (combo > 0)
                {
                    char[] numToChar = combo.ToString().ToCharArray();
                    float fixval;
                    switch (numToChar.Length)
                    {
                        case 1: fixval = 0f; break;
                        case 2: fixval = -62.5f; break;
                        case 3: fixval = -125f; break;
                        case 4: fixval = -187.5f; break;
                        case 5: fixval = -250f; break;
                        default: fixval = 0; break;
                    }
                    int idx = 0;
                    foreach (char c in numToChar)
                    {
                        GameObject piclayer = new GameObject();
                        piclayer.transform.SetParent(comboLayerED.transform);
                        piclayer.transform.localPosition = new Vector3(idx * 110 + fixval, 0f, 0f);
                        piclayer.transform.localRotation = new Quaternion();
                        piclayer.transform.localScale = new Vector3(2f, 2f, 2f);
                        SpriteRenderer renderer = piclayer.AddComponent<SpriteRenderer>();
                        switch (c)
                        {
                            case '0':
                                renderer.sprite = cb0;
                                break;
                            case '1':
                                renderer.sprite = cb1;
                                break;
                            case '2':
                                renderer.sprite = cb2;
                                break;
                            case '3':
                                renderer.sprite = cb3;
                                break;
                            case '4':
                                renderer.sprite = cb4;
                                break;
                            case '5':
                                renderer.sprite = cb5;
                                break;
                            case '6':
                                renderer.sprite = cb6;
                                break;
                            case '7':
                                renderer.sprite = cb7;
                                break;
                            case '8':
                                renderer.sprite = cb8;
                                break;
                            case '9':
                                renderer.sprite = cb9;
                                break;
                        }
                        idx++;
                    }
                }

                // accuracy update
                txtTimingED.text = accuracy;

                Vector3 ccp = comboLayerED.transform.localPosition;
                comboLayerED.SetActive(true);
                comboLayerED.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
                StartCoroutine(comboPopAni());
                judgeSpriteED.gameObject.SetActive(true);
                judgeSpriteED.transform.localScale = new Vector3(1.56f, 1.56f, 1.56f);
                txtTimingED.gameObject.SetActive(true);
                txtTimingED.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                StartCoroutine(timingPopAni());
            }
            else
            {
                txtTimingPercentBM.text = accuracy;
                if(judgetype == TimingType.BAD || judgetype == TimingType.POOR)
                {
                    txtJudgeBM.text = judgeStr;
                }
                else
                {
                    txtJudgeBM.text = judgeStr + " " + combo;
                }

                if(ms > 0)
                {
                    txtTimingMsBM.color = Color.red;
                    txtTimingMsBM.text = "+" + ms.ToString() + "ms";
                }
                else if(ms < 0)
                {
                    txtTimingMsBM.color = new Color(135f/255, 206f/255, 235f/255);
                    txtTimingMsBM.text = ms.ToString() + "ms";
                }
                else
                {
                    txtTimingMsBM.text = "";
                }

                txtTimingPercentBM.gameObject.SetActive(true);
                txtJudgeBM.gameObject.SetActive(true);
                txtTimingMsBM.gameObject.SetActive(true);
            }

            timeLastComboPopup = DateTime.Now.Ticks / 10000;
            timeLastTimingPopup = DateTime.Now.Ticks / 10000;
        }

        public void UpdateSideJudge(int p, int gr, int gd, int o, int m, string a, string d)
        {
            txtPerfect.text = p.ToString();
            txtGreat.text = gr.ToString();
            txtGood.text = gd.ToString();
            txtOk.text = o.ToString();
            txtMiss.text = m.ToString();
            txtAvgRate.text = a;
            txtAvgDiff.text = d;
        }

        public void TextToImg(int number, int type)
        {
            
        }

        // Beam 보이기
        public void ShowAndHideBeam(int line, bool onoff)
        {
            switch(line)
            {
                case 0:
                    if(onoff) beam1.SetActive(true);
                    else beam1.SetActive(false);
                    break;
                case 1:
                    if (onoff) beam2.SetActive(true);
                    else beam2.SetActive(false);
                    break;
                case 2:
                    if (onoff) beam3.SetActive(true);
                    else beam3.SetActive(false);
                    break;
                case 3:
                    if (onoff) beam4.SetActive(true);
                    else beam4.SetActive(false);
                    break;
                case 4:
                    if (onoff) beam5.SetActive(true);
                    else beam5.SetActive(false);
                    break;
                case 5:
                    if (onoff) beam6.SetActive(true);
                    else beam6.SetActive(false);
                    break;
                case 6:
                    if (onoff) beam7.SetActive(true);
                    else beam7.SetActive(false);
                    break;
                case 7:
                    if (onoff) beam8.SetActive(true);
                    else beam8.SetActive(false);
                    break;
            }
        }

        // Animation Effect Coroutine
        IEnumerator comboPopAni()
        {
            yield return new WaitForSeconds(0.08f);
            comboLayerED.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        IEnumerator timingPopAni()
        {
            yield return new WaitForSeconds(0.08f);
            judgeSpriteED.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
            txtTimingED.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        public void BGAVideoActivate()
        {
            bgaVideo.gameObject.SetActive(true);
        }

        // BGA Control
        public void BGAVideoSetting(string videoFile)
        {
            bgaVideo.OnInit = (mpmp) =>
            {
                mpmp.volume = 0f;
                mpmp.preventFlicker = true;
                mpmp.autoPlay = false;
                mpmp.Load(videoFile);

                mpmp.OnLoaded = (mp) =>
                {
                    mp.Play();

                    // 여기에 Callback을 넣으면 다시 재생시켜도 콜백으로 인해
                    // 항상 일시정지가 되므로 코루틴으로 멈춤
                    // 애초에 로딩이 안되면 Callback 자체를 부르지 못하니 상관없음
                    StartCoroutine(CheckMovieStart());
                };
            };
        }

        IEnumerator CheckMovieStart()
        {
            while(!bgaVideo.IsPlaying())
            {
                yield return null;
            }

            bgaVideo.Pause();
            bgaVideo.SeekTo(0f);

            // 스스로 코루틴을 멈출 때 쓰는 구문
            yield break;
        }

        public void BGAVideoPlay()
        {
            bgaVideo.Play();
        }

        public void BGAImageSetting(Sprite img)
        {
            bgaImage.sprite = img;

            float width = bgaImage.sprite.bounds.size.x;
            float height = bgaImage.sprite.bounds.size.y;

            float worldScreenHeight = Camera.main.orthographicSize * 2.0f;
            float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

            bgaImage.gameObject.transform.localScale =
                new Vector3(640, 360, 1);
                    //worldScreenWidth / width / 1,
                    //worldScreenHeight / height / 1);
        }

        public void ShowPauseMenu()
        {
            layerPauseMenu.SetActive(true);
        }
        
        public void HidePauseMenu()
        {
            layerPauseMenu.SetActive(false);
        }

        public void PauseMenuMove()
        {
            if(pauseSel == 0)
            {
                pauseSel = 1;
                btnRestart.gameObject.GetComponent<Image>().sprite = normalBtn;
                btnExit.gameObject.GetComponent<Image>().sprite = selectBtn;
            }
            else
            {
                pauseSel = 0;
                btnRestart.gameObject.GetComponent<Image>().sprite = selectBtn;
                btnExit.gameObject.GetComponent<Image>().sprite = normalBtn;
            }
        }

        public void PauseMenuExec()
        {
            if(pauseSel == 0)
            {
                RestartGame();
            }
            else
            {
                ExitGame();
            }
        }

        public void RestartGame()
        {
            Scene scene = SceneManager.GetActiveScene();
            Loading.StartLoading("PlayScreen");
        }

        public void ExitGame()
        {
            Loading.StartLoading("MusicSelect");
        }
    }
}
