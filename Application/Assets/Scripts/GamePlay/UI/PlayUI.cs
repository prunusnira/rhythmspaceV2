using BMSCore;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

namespace BMSPlayer
{
	// 플레이 화면의 UI 요소 컨트롤
	public class PlayUI : MonoBehaviour {
        // UI Object
        private List<GameObject> NoteOnScreen;
        public GameObject layerJudgeAll;
        public GameObject layerPauseMenu;
        public TextMesh FPSCounter;

        // Pause Menu
        public Button btnRestart;
        public Button btnRestartSame;
        public Button btnExit;

        // Judge sprite
        private TimingType currentJudge = TimingType.NONE;

        // Judge type BM
        public GameObject[] judgeTypeBM;
        public TextMesh[] txtInfo1C;
        public TextMesh[] txtInfo2C;
        public TextMesh[] txtInfo3C;
        public TextMesh[] txtInfo1S;
        public TextMesh[] txtInfo2S;
        public TextMesh[] txtInfo3S;
        private TextMesh txtInfoFS;
        private TextMesh txtInfoTarget;
        private TextMesh txtInfoRate;
        public TextMeshPro[] txtJudgeBM;
        private int infoNumA = 0;

        private double timeLastComboPopup;
        private double timeLastTimingPopup;

        // Side judge
        public Text txtAvgDiff;
        public Text txtAvgRate;
        public Text txtPerfect;
        public Text txtGreat;
        public Text txtGood;
        public Text txtOk;
        public Text txtMiss;
        public Text txtComboBreak;
        public Text txtFast;
        public Text txtSlow;

        // Timer
        public TextMesh txtCurrentTime;
        public TextMesh txtTotalTime;

        // Gear and Area
        public GameObject Gear1P;
        public GameObject Gear2P;
        public GameObject Area1P;
        public GameObject Area2P;

        // Text display
        public TextMesh Combo;
        public TextMesh Score;
        public TextMesh SpeedFixed;
        public TextMesh SpeedFluid;
        public TextMesh HP;
        public TextMesh BPMcur;
        public TextMesh BPMmin;
        public TextMesh BPMmax;
        public TextMesh Difficulty;
        public TextMesh Level;

        public TextMesh[] txtAutoPlay;
        public TextMesh[] txtLoading;
        public GameObject[] gearBtnPress1P;
        public GameObject[] gearBtnPress2P;

        // Skin
        public SpriteRenderer[] skinGear;
        public Sprite[] skinGearBlack;
        public Sprite[] skinGearWhite;
        public Sprite[] skinGearDark;

        // Cover
        public GameObject coverSud;
        public GameObject coverHid;
        public GameObject playArea;
        private int coverSudPos;
        private int coverHidPos;
        private int playAreaPos;

        // Beam
        public GameObject[] beam1P;
        public GameObject[] beam2P;

        // Note Effect
        private Coroutine[] effectCoroutine;
        public SpriteRenderer[] noteEffects;
        public GameObject[] touches;
        private float[] effectRotation = new float[8];

        // HPBar
        private HPController hpController;
        public TextMesh hpBarType;
        public SpriteRenderer[] hpBar;
        public Sprite hpBarAssisted;
        public Sprite hpBarEasy;
        public Sprite hpBarNormal;
        public Sprite hpBarHard;
        public Sprite hpBarExHard;

        // BGA
        public SpriteRenderer bgaImage;
        public RectTransform bgaRect;
        public SpriteRenderer layerImage;
        public RectTransform layerRect;
        public VideoPlayer bgaVideo;
        public GameObject bgaVideoLayer;
        public RectTransform bgaVideoRect;
        public GameObject bgaErrorLayer;
        public RectTransform bgaFollowingObj;

        // Fader
        public Image Fader;
        private bool FadeDone = false;
        private bool FadeStart = false;
        private bool FadeReady = false;

        // Menu btn
        public Sprite normalBtn;
        public Sprite selectBtn;

        // Key info
        public GameObject keyInfo;

        // Music Info
        public GameObject stagePanel;
        public TextMeshProUGUI stageGerne;
        public TextMeshProUGUI stageTitle;
        public TextMeshProUGUI stageSubtitle;
        public TextMeshProUGUI stageArtist;
        public TextMeshProUGUI stageSubartist;

        // frame
        public GameObject Upper;
        public GameObject Lower;
        public TextMeshPro UpperTitle;


        public void Awake()
        {
            // 사용자 설정에 따른 기어-BGA-그래프 위치 변경
            ObjectPositionSetup();

            for (int i = 0; i < 8; i++)
            {
                effectCoroutine = new Coroutine[8];
                effectRotation[i] = 0f;
            }

            // 판정 패널 표시 설정
            if (Const.DisplayJudge == 0) layerJudgeAll.SetActive(false);

            // HP bar 설정
            SetInitialHPBar();

            // HP 기본 수치 설정
            hpController = GetComponent<HPController>();

            // 일시정지 메뉴
            btnRestart.gameObject.GetComponent<Image>().sprite = selectBtn;
            btnRestartSame.gameObject.GetComponent<Image>().sprite = normalBtn;
            btnExit.gameObject.GetComponent<Image>().sprite = normalBtn;

            // 판정 표시 타입 변경
            judgeTypeBM[Const.PlayerSide].SetActive(true);

            // 오토 플레이 표기
            if(Const.Auto == AutoPlayType.ALL)
            {
                txtAutoPlay[Const.PlayerSide].gameObject.SetActive(true);
            }

            // 커버 포지션 변경
            coverSudPos = Const.CoverSudPos;
            coverHidPos = Const.CoverHidPos;
            playAreaPos = Const.AreaLiftPos;

            NoteOnScreen = new List<GameObject>();

            // 기어에 표시하는 정보의 위치와 개수 확인
            SideInfoDisplayPosition();
        }

        private void Update()
        {
            FPSCounter.text = "FPS " + ((int)(1f / Time.unscaledDeltaTime)).ToString();
            double timing = (double)DateTime.Now.Ticks / 1000000 - timeLastComboPopup;
            if (timing > 10)
            {
                txtJudgeBM[Const.PlayerSide].gameObject.SetActive(false);
                if (txtInfoFS != null)
                    txtInfoFS.gameObject.SetActive(false);
                if (txtInfoRate != null)
                    txtInfoRate.gameObject.SetActive(false);
                if (txtInfoTarget != null)
                    txtInfoTarget.gameObject.SetActive(false);
            }

            if (txtJudgeBM[Const.PlayerSide].gameObject.activeSelf && timing % 1.5 < 0.1)
            {
                // 판정별 색상 변경 처리
                StartCoroutine(comboChangeBM(currentJudge));
            }

            if(FadeReady && !FadeStart)
            {
                StartCoroutine("FadeOut");
            }
        }

        public void SetFade()
        {
            FadeReady = true;
        }

        public bool GetFadeDone()
        {
            return FadeDone;
        }

        IEnumerator FadeOut()
        {
            FadeStart = true;
            Fader.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);

            for (float time = 0; time <= 1f; time += 1f / 6)
            {
                Fader.color = new Color(0f, 0f, 0f, time);
                yield return new WaitForSeconds(0.03f);
            }

            FadeDone = true;
        }

        public void SetGearBPM(double bpm, double min, double max)
        {
            BPMcur.text = bpm.ToString("0.00");
            BPMmin.text = min.ToString("0.00");
            BPMmax.text = max.ToString("0.00");
        }

        public void SetGearCurBPM(double bpm)
        {
            BPMcur.text = bpm.ToString("0.00");
            SpeedFluid.text = ((int)(bpm * Const.SpeedFixed / 100)).ToString();
        }

        private void SetInitialHPBar()
        {
            switch (Const.GaugeType)
            {
                case GaugeType.ASSISTED:
                    hpBarType.text = "ASSISTED";
                    hpBarType.color = new Color(206f / 255, 159f / 255, 1f);
                    hpBar[Const.PlayerSide].sprite = hpBarAssisted;
                    break;
                case GaugeType.EASY:
                    hpBarType.text = "EASY";
                    hpBarType.color = new Color(159f / 255, 1f, 180f / 255);
                    hpBar[Const.PlayerSide].sprite = hpBarEasy;
                    break;
                case GaugeType.NORMAL:
                    hpBarType.text = "NORMAL";
                    hpBarType.color = new Color(159f / 255, 215f / 255, 1f);
                    hpBar[Const.PlayerSide].sprite = hpBarNormal;
                    break;
                case GaugeType.HARD:
                    hpBarType.text = "HARD";
                    hpBarType.color = new Color(1f, 159f / 255, 159f / 255);
                    hpBar[Const.PlayerSide].sprite = hpBarHard;
                    break;
                case GaugeType.EXHARD:
                    hpBarType.text = "EX-HARD";
                    hpBarType.color = new Color(246f / 255, 1f, 159f / 255);
                    hpBar[Const.PlayerSide].sprite = hpBarExHard;
                    break;
                default:
                    hpBarType.text = "NORMAL";
                    hpBarType.color = new Color(159f / 255, 215f / 255, 1f);
                    hpBar[Const.PlayerSide].sprite = hpBarNormal;
                    break;
            }
        }

        public void SetMusicInfo(BMS bms)
        {
            // 곡 정보 설정
            UpperTitle.text = bms.Title;
            Level.text = bms.Level.ToString();
            switch(bms.Difficulty)
            {
                case 1:
                    Difficulty.text = "BEGINNER";
                    break;
                case 2:
                    Difficulty.text = "NORMAL";
                    break;
                case 3:
                    Difficulty.text = "HYPER";
                    break;
                case 4:
                    Difficulty.text = "ANOTHER";
                    break;
                case 5:
                    Difficulty.text = "INSANE";
                    break;
                default:
                    Difficulty.text = "UNKNOWN";
                    break;
            }
        }

        public void UpdateSpeed()
        {
            SpeedFixed.text = ((float)Const.SpeedFixed / 100).ToString("0.00") + "x";
            SpeedFluid.text = Const.SpeedFluid.ToString();
        }

        public void UpdateHP(int hp)
        {
            float chp = (float)hp / hpController.HPMax;
            hpBar[Const.PlayerSide].material.SetFloat("_Progress", chp);
            HP.text = (chp * 100).ToString("0.00") + "%";
        }

        public void UpdateScore(int score)
        {
            Score.text = score.ToString();
        }

        public string GetRank(int ex, int proc)
        {
            string rank = "f";
            float currentRankState = (float)ex / (proc * 2);

            if (currentRankState >= 8f / 9)
            {
                rank = "aaa";
            }
            else if (currentRankState >= 7f / 9)
            {
                rank = "aa";
            }
            else if (currentRankState >= 6f / 9)
            {
                rank = "a";
            }
            else if (currentRankState >= 5f / 9)
            {
                rank = "b";
            }
            else if (currentRankState >= 4f / 9)
            {
                rank = "c";
            }
            else if (currentRankState >= 3f / 9)
            {
                rank = "d";
            }
            else if (currentRankState >= 2f / 9)
            {
                rank = "e";
            }
            else
            {
                rank = "f";
            }

            return rank;
        }

        public void UpdateMaxCombo(int combo)
        {
            Combo.text = combo.ToString();
        }

        public void UpdateJudge(TimingType judgetype, int combo, string accuracy, int ms)
        {
            string judgeStr = "";
            currentJudge = judgetype;
            // Judge update
            switch(judgetype)
            {
                case TimingType.PERFECT:
                    judgeStr = "GREAT";
                    break;
                case TimingType.GREAT:
                    judgeStr = "GREAT";
                    break;
                case TimingType.GOOD:
                    judgeStr = "GOOD";
                    break;
                case TimingType.BAD:
                    judgeStr = "BAD";
                    break;
                case TimingType.POOR:
                case TimingType.EPOOR:
                    judgeStr = "POOR";
                    break;
            }

            // Combo update
            if (judgetype == TimingType.BAD || judgetype == TimingType.POOR)
            {
                txtJudgeBM[Const.PlayerSide].text = judgeStr;
            }
            else
            {
                txtJudgeBM[Const.PlayerSide].text = judgeStr + " " + combo;
            }
            txtJudgeBM[Const.PlayerSide].gameObject.SetActive(true);

            if (txtInfoRate != null)
            {
                txtInfoRate.text = accuracy;
                txtInfoRate.gameObject.SetActive(true);
            }

            if(txtInfoFS != null)
            {
                if (ms > 0)
                {
                    txtInfoFS.color = new Color(135f / 255, 206f / 255, 235f / 255);
                    txtInfoFS.text = "FAST " + ms.ToString() + "ms";
                }
                else if (ms < 0)
                {
                    txtInfoFS.color = Color.red;
                    txtInfoFS.text = "SLOW " + Math.Abs(ms).ToString() + "ms";
                }
                else
                {
                    txtInfoFS.text = "";
                }
                txtInfoFS.gameObject.SetActive(true);
            }

            timeLastComboPopup = (double)DateTime.Now.Ticks / 1000000;
            timeLastTimingPopup = (double)DateTime.Now.Ticks / 1000000;
        }

        public void UpdateSideJudge(
            int p, int gr, int gd, int o, int m, int cb, int fast, int slow,
            string a, string d)
        {
            txtPerfect.text = p.ToString();
            txtGreat.text = gr.ToString();
            txtGood.text = gd.ToString();
            txtOk.text = o.ToString();
            txtMiss.text = m.ToString();
            txtComboBreak.text = cb.ToString();
            txtAvgRate.text = a;
            txtAvgDiff.text = d;
            txtFast.text = fast.ToString();
            txtSlow.text = slow.ToString();
        }

        // Beam 보이기
        public void ShowAndHideBeam(int line, bool onoff)
        {
            if (onoff)
            {
                if(Const.PlayerSide == 0)
                {
                    beam1P[line].SetActive(true);
                    gearBtnPress1P[line].SetActive(true);
                }
                else
                {
                    beam2P[line].SetActive(true);
                    gearBtnPress2P[line].SetActive(true);
                }
            }
            else
            {
                if (Const.PlayerSide == 0)
                {
                    beam1P[line].SetActive(false);
                    gearBtnPress1P[line].SetActive(false);
                }
                else
                {
                    beam2P[line].SetActive(false);
                    gearBtnPress2P[line].SetActive(false);
                }
            }
        }

        public void TurnOnNoteEffect(int pos)
        {
            if (effectCoroutine[pos] != null) StopCoroutine(effectCoroutine[pos]);
            noteEffects[pos].gameObject.SetActive(true);
            effectCoroutine[pos] = StartCoroutine("noteEffect", noteEffects[pos]);
            
        }

        public void TurnOnNoteEffectLN(int pos, bool onoff)
        {
            if (effectCoroutine[pos] != null) StopCoroutine(effectCoroutine[pos]);
            noteEffects[pos].gameObject.SetActive(true);
            if (onoff)
            {
                noteEffects[pos].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                StartCoroutine(noteEffectLN(noteEffects[pos], pos));
            }
            else
            {
                StartCoroutine("noteEffect", noteEffects[pos]);
            }
        }

        // Note Effect Coroutine
        IEnumerator noteEffect(SpriteRenderer sprite)
        {
            float x = 0.3f;
            float y = 0.3f;
            sprite.transform.localScale = new Vector3(x, y, 1f);
            for(int i = 0; i < 12; i++)
            {
                yield return new WaitForSeconds(0.02f);
                sprite.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 15f * i));
                sprite.transform.localScale = new Vector3(x * 0.8f, y * 0.8f, 1f);
                x *= 0.8f;
                y *= 0.8f;
            }
            yield return new WaitForSeconds(0.02f);
            sprite.gameObject.SetActive(false);
        }

        IEnumerator noteEffectLN(SpriteRenderer sprite, int pos)
        {
            yield return new WaitForSeconds(0.02f);
            effectRotation[pos] += 15f;
            if (effectRotation[pos] % 360 == 0) effectRotation[pos] = 0f;
            sprite.transform.rotation = Quaternion.Euler(new Vector3(0f, effectRotation[pos], 0f));
        }

        IEnumerator comboChangeBM(TimingType type)
        {
            if(type == TimingType.PERFECT)
            {
                txtJudgeBM[Const.PlayerSide].colorGradient = new VertexGradient(
                    new Color(56f / 255, 122f / 255, 208f / 255),
                    new Color(56f / 255, 122f / 255, 208f / 255),
                    new Color(183f / 255, 196f / 255, 255f / 255),
                    new Color(183f / 255, 196f / 255, 255f / 255)
                    );
                yield return new WaitForSeconds(0.05f);
                txtJudgeBM[Const.PlayerSide].colorGradient = new VertexGradient(
                    new Color(232f / 255, 86f / 255, 155f / 255),
                    new Color(232f / 255, 86f / 255, 155f / 255),
                    new Color(234f / 255, 164f / 255, 179f / 255),
                    new Color(234f / 255, 164f / 255, 179f / 255)
                    );
                yield return new WaitForSeconds(0.05f);
                txtJudgeBM[Const.PlayerSide].colorGradient = new VertexGradient(
                    new Color(175f / 255, 232f / 255, 197f / 255)
                    );
                yield return new WaitForSeconds(0.05f);
            }
            else
            {
                txtJudgeBM[Const.PlayerSide].colorGradient = new VertexGradient(
                    new Color(217f / 255, 150f / 255, 0f),
                    new Color(217f / 255, 150f / 255, 0f),
                    new Color(255f / 255, 225f / 255, 196f / 255),
                    new Color(255f / 255, 225f / 255, 196f / 255)
                    );
                yield return new WaitForSeconds(0.1f);
                txtJudgeBM[Const.PlayerSide].colorGradient = new VertexGradient(
                    new Color(0f, 0f, 0f, 0f)
                    );
                yield return new WaitForSeconds(0.05f);
            }
        }

        public void BGAVideoActivate()
        {
            bgaVideoLayer.SetActive(true);
        }

        public void BGAVideoPreload(string file)
        {
            bgaVideo.url = "file://" + file;
            bgaVideo.errorReceived += BGAErrorLayer;
            bgaVideo.Prepare();
        }

        public void BGAImageActivate()
        {
            bgaImage.gameObject.SetActive(true);
        }

        public void LayerImageActivate()
        {
            layerImage.gameObject.SetActive(true);
        }

        // BGA Control
        public void BGAVideoPlay()
        {
            bgaVideo.Play();
        }

        public bool isBGAPlaying()
        {
            return bgaVideo.isPlaying;
        }

        public void BGAErrorLayer(VideoPlayer source, string msg)
        {
            // 메모리 릭 방지
            bgaVideo.errorReceived -= BGAErrorLayer;

            // 레이어 띄우기
            bgaErrorLayer.SetActive(true);
            bgaErrorLayer.GetComponentsInChildren<TextMeshPro>()[0].text =
                Const.videoCodecMsg[(int)Const.Language];
        }

        public void BGAImageSetting(Sprite img)
        {
            if(img != null)
            {
                bgaImage.sprite = img;

                float width = bgaImage.sprite.bounds.size.x;
                float height = bgaImage.sprite.bounds.size.y;

                float rectWidth = bgaRect.sizeDelta.x;
                float rectHeight = bgaRect.sizeDelta.y;

                bgaImage.gameObject.transform.localScale =
                    new Vector3(
                        rectWidth / width,
                        rectHeight / height
                    );
                Debug.Log("BGA Changed");
            }
        }

        public void LayerImageSetting(Sprite img)
        {
            if (img != null)
            {
                layerImage.sprite = img;

                float width = layerImage.sprite.bounds.size.x;
                float height = layerImage.sprite.bounds.size.y;

                float rectWidth = layerRect.sizeDelta.x;
                float rectHeight = layerRect.sizeDelta.y;

                layerImage.gameObject.transform.localScale =
                    new Vector3(
                        rectWidth / width,
                        rectHeight / height
                    );
                Debug.Log("Layer Changed");
            }
        }

        public void ShowPauseMenu()
        {
            layerPauseMenu.SetActive(true);
        }
        
        public void HidePauseMenu()
        {
            layerPauseMenu.SetActive(false);
        }

        public void PauseMenuMove(ref int pauseSel, bool down)
        {
            if(pauseSel == 0)
            {
                if (down)
                {
                    pauseSel = 1;
                }
                else
                {
                    pauseSel = 2;
                }
            }
            else if(pauseSel == 1)
            {
                if (down)
                {
                    pauseSel = 2;
                }
                else
                {
                    pauseSel = 0;
                }
            }
            else
            {
                if (down)
                {
                    pauseSel = 0;
                }
                else
                {
                    pauseSel = 1;
                }
            }
            SetPauseMenu(pauseSel);
        }

        public void SetPauseMenu(int newsel)
        {
            switch(newsel)
            {
                case 0:
                    btnRestart.gameObject.GetComponent<Image>().sprite = selectBtn;
                    btnRestartSame.gameObject.GetComponent<Image>().sprite = normalBtn;
                    btnExit.gameObject.GetComponent<Image>().sprite = normalBtn;
                    break;
                case 1:
                    btnRestart.gameObject.GetComponent<Image>().sprite = normalBtn;
                    btnRestartSame.gameObject.GetComponent<Image>().sprite = selectBtn;
                    btnExit.gameObject.GetComponent<Image>().sprite = normalBtn;
                    break;
                case 2:
                    btnRestart.gameObject.GetComponent<Image>().sprite = normalBtn;
                    btnRestartSame.gameObject.GetComponent<Image>().sprite = normalBtn;
                    btnExit.gameObject.GetComponent<Image>().sprite = selectBtn;
                    break;
            }
        }

        public bool PauseMenuExec(int pauseSel)
        {
            if(pauseSel == 0)
            {
                Const.ChangeLayout = true;
                RestartGame();
            }
            else if (pauseSel == 1) {
                Const.ChangeLayout = false;
                RestartGame();
            }
            else if (pauseSel == 2)
            {
                return true;
            }
            return false;
        }

        public void RestartGame()
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene("PlayScreen");
        }

        public void DeactiveLoading()
        {
            txtLoading[Const.PlayerSide].gameObject.SetActive(false);
            stagePanel.SetActive(false);
        }

        public void SetStageInfo(
            string gerne, string title,
            string subt, string art, string subart)
        {
            stageGerne.text = gerne;
            stageTitle.text = title;
            stageSubtitle.text = subt;
            stageArtist.text = art;
            stageSubartist.text = subart;
        }

        public void RemoveKeyInfo()
        {
            keyInfo.SetActive(false);
        }

        public void UpdateTimerCur(double time)
        {
            string min = (Convert.ToInt32(time) / 600).ToString("00");
            string sec = (Convert.ToInt32(time) % 600 / 10).ToString("00");

            txtCurrentTime.text = min + ":" + sec;
        }

        public void UpdateTimerTotal(double time)
        {
            string min = (Convert.ToInt32(time) / 600).ToString("00");
            string sec = (Convert.ToInt32(time) % 600 / 10).ToString("00");

            txtTotalTime.text = min + ":" + sec;
        }

        #region Combo/Side Info Display Position
        private void SideInfoDisplayPosition()
        {
            // 각 포지션 개수 확인
            switch (Const.FastSlow)
            {
                case DisplayPosType.OFF:
                    txtInfoFS = null;
                    break;
                case DisplayPosType.TYPEA:
                    infoNumA++;
                    break;
                case DisplayPosType.TYPEB:
                    txtInfoFS = txtInfo3S[(int)Const.Language];
                    break;
            }
            switch (Const.TargetDiff)
            {
                case DisplayPosType.OFF:
                    txtInfoTarget = null;
                    break;
                case DisplayPosType.TYPEA:
                    infoNumA++;
                    break;
                case DisplayPosType.TYPEB:
                    txtInfoTarget = txtInfo2S[(int)Const.Language];
                    break;
            }
            switch (Const.RateDiff)
            {
                case DisplayPosType.OFF:
                    txtInfoRate = null;
                    break;
                case DisplayPosType.TYPEA:
                    infoNumA++;
                    break;
                case DisplayPosType.TYPEB:
                    txtInfoRate = txtInfo1S[(int)Const.Language];
                    break;
            }

            // 개수 별 스타일 처리
            switch (infoNumA)
            {
                case 1:
                    if (Const.FastSlow == DisplayPosType.TYPEA)
                    {
                        txtInfoFS = txtInfo2C[(int)Const.Language];
                    }
                    if (Const.RateDiff == DisplayPosType.TYPEA)
                    {
                        txtInfoRate = txtInfo2C[(int)Const.Language];
                    }
                    if (Const.TargetDiff == DisplayPosType.TYPEA)
                    {
                        txtInfoTarget = txtInfo2C[(int)Const.Language];
                    }
                    break;
                case 2:
                    if (Const.FastSlow == DisplayPosType.TYPEA)
                    {
                        if(Const.RateDiff == DisplayPosType.TYPEA)
                        {
                            txtInfoRate = txtInfo1C[(int)Const.Language];
                            txtInfoFS = txtInfo3C[(int)Const.Language];
                        }
                        else if(Const.TargetDiff == DisplayPosType.TYPEA)
                        {
                            txtInfoTarget = txtInfo1C[(int)Const.Language];
                            txtInfoFS = txtInfo3C[(int)Const.Language];
                        }
                    }
                    if (Const.RateDiff == DisplayPosType.TYPEA)
                    {
                        if (Const.TargetDiff == DisplayPosType.TYPEA)
                        {
                            txtInfoRate = txtInfo1C[(int)Const.Language];
                            txtInfoTarget = txtInfo3C[(int)Const.Language];
                        }
                    }
                    break;
                case 3:
                    txtInfoTarget = txtInfo1C[(int)Const.Language];
                    txtInfoRate = txtInfo2C[(int)Const.Language];
                    txtInfoFS = txtInfo3C[(int)Const.Language];
                    break;
            }
        }
        #endregion

        #region Gear/BGA/Graph/Frame Position
        private void ObjectPositionSetup()
        {
            bool isTypeA = true;
            switch(Const.LayoutType)
            {
                case UILayoutType.TYPEA:
                    isTypeA = true;
                    break;
                case UILayoutType.TYPEB:
                    isTypeA = false;
                    break;
            }

            if (Const.GearSkin == "" || Const.GearSkin == "black")
            {
                skinGear[Const.PlayerSide].sprite = skinGearBlack[Const.PlayerSide];
            }
            else if (Const.GearSkin == "white")
            {
                skinGear[Const.PlayerSide].sprite = skinGearWhite[Const.PlayerSide];
            }
            else if (Const.GearSkin == "dark")
            {
                skinGear[Const.PlayerSide].sprite = skinGearDark[Const.PlayerSide];
            }

            int PlaySide = Const.PlayerSide;
            if(PlaySide == 0)
            {
                Gear1P.SetActive(true);
                Area1P.SetActive(true);
            }
            else
            {
                Gear2P.SetActive(true);
                Area2P.SetActive(true);

                InvertObjectX(HP.gameObject);
                InvertObjectX(hpBarType.gameObject);
                InvertObjectX(SpeedFixed.gameObject);
                InvertObjectX(SpeedFluid.gameObject);
                InvertObjectX(Difficulty.gameObject);
                InvertObjectX(Level.gameObject);
            }

            switch (Const.GraphType)
            {
                case GraphType.SMALL:
                    if (isTypeA)
                    {
                        if (PlaySide == 0)
                        {
                            // 1P Left BGA
                            bgaRect.sizeDelta = new Vector2(
                                Const.BGA_1P_GraphSm_Left_Wid,
                                Const.BGA_1P_GraphSm_Left_Hei
                            );
                            bgaRect.localPosition = new Vector2(
                                Const.BGA_1P_GraphSm_Left_PosX,
                                Const.BGA_1P_GraphSm_Left_PosY
                            );
                            layerRect.sizeDelta = new Vector2(
                                Const.BGA_1P_GraphSm_Left_Wid,
                                Const.BGA_1P_GraphSm_Left_Hei
                            );
                            layerRect.localPosition = new Vector3(
                                Const.BGA_1P_GraphSm_Left_PosX,
                                Const.BGA_1P_GraphSm_Left_PosY, -1
                            );
                            bgaVideoRect.sizeDelta = new Vector2(
                                Const.BGA_1P_GraphSm_Left_Wid,
                                Const.BGA_1P_GraphSm_Left_Hei
                            );
                            bgaVideoRect.localPosition = new Vector2(
                                Const.BGA_1P_GraphSm_Left_PosX,
                                Const.BGA_1P_GraphSm_Left_PosY
                            );
                            bgaFollowingObj.localPosition = new Vector2(
                                Const.BGA_1P_GraphSm_Left_PosX,
                                bgaFollowingObj.localPosition.y
                            );
                        }
                        else
                        {
                            // 2P Right BGA
                            bgaRect.sizeDelta = new Vector2(
                                Const.BGA_2P_GraphSm_Right_Wid,
                                Const.BGA_2P_GraphSm_Right_Hei
                            );
                            bgaRect.localPosition = new Vector2(
                                Const.BGA_2P_GraphSm_Right_PosX,
                                Const.BGA_2P_GraphSm_Right_PosY
                            );
                            layerRect.sizeDelta = new Vector2(
                                Const.BGA_2P_GraphSm_Right_Wid,
                                Const.BGA_2P_GraphSm_Right_Hei
                            );
                            layerRect.localPosition = new Vector3(
                                Const.BGA_2P_GraphSm_Right_PosX,
                                Const.BGA_2P_GraphSm_Right_PosY, -1
                            );
                            bgaVideoRect.sizeDelta = new Vector2(
                                Const.BGA_2P_GraphSm_Right_Wid,
                                Const.BGA_2P_GraphSm_Right_Hei
                            );
                            bgaVideoRect.localPosition = new Vector2(
                                Const.BGA_2P_GraphSm_Right_PosX,
                                Const.BGA_2P_GraphSm_Right_PosY
                            );
                            bgaFollowingObj.localPosition = new Vector2(
                                Const.BGA_2P_GraphSm_Right_PosX,
                                bgaFollowingObj.localPosition.y
                            );
                        }
                    }
                    else
                    {
                        if (PlaySide == 0)
                        {
                            // 1P Right BGA
                            bgaRect.sizeDelta = new Vector2(
                                Const.BGA_1P_GraphSm_Right_Wid,
                                Const.BGA_1P_GraphSm_Right_Hei
                            );
                            bgaRect.localPosition = new Vector2(
                                Const.BGA_1P_GraphSm_Right_PosX,
                                Const.BGA_1P_GraphSm_Right_PosY
                            );
                            layerRect.sizeDelta = new Vector2(
                                Const.BGA_1P_GraphSm_Right_Wid,
                                Const.BGA_1P_GraphSm_Right_Hei
                            );
                            layerRect.localPosition = new Vector3(
                                Const.BGA_1P_GraphSm_Right_PosX,
                                Const.BGA_1P_GraphSm_Right_PosY, -1
                            );
                            bgaVideoRect.sizeDelta = new Vector2(
                                Const.BGA_1P_GraphSm_Right_Wid,
                                Const.BGA_1P_GraphSm_Right_Hei
                            );
                            bgaVideoRect.localPosition = new Vector2(
                                Const.BGA_1P_GraphSm_Right_PosX,
                                Const.BGA_1P_GraphSm_Right_PosY
                            );
                            bgaFollowingObj.localPosition = new Vector2(
                                Const.BGA_1P_GraphSm_Right_PosX,
                                bgaFollowingObj.localPosition.y
                            );
                        }
                        else
                        {
                            // 2P Left BGA
                            bgaRect.sizeDelta = new Vector2(
                                Const.BGA_2P_GraphSm_Left_Wid,
                                Const.BGA_2P_GraphSm_Left_Hei
                            );
                            bgaRect.localPosition = new Vector2(
                                Const.BGA_2P_GraphSm_Left_PosX,
                                Const.BGA_2P_GraphSm_Left_PosY
                            );
                            layerRect.sizeDelta = new Vector2(
                                Const.BGA_2P_GraphSm_Left_Wid,
                                Const.BGA_2P_GraphSm_Left_Hei
                            );
                            layerRect.localPosition = new Vector3(
                                Const.BGA_2P_GraphSm_Left_PosX,
                                Const.BGA_2P_GraphSm_Left_PosY, -1
                            );
                            bgaVideoRect.sizeDelta = new Vector2(
                                Const.BGA_2P_GraphSm_Left_Wid,
                                Const.BGA_2P_GraphSm_Left_Hei
                            );
                            bgaVideoRect.localPosition = new Vector2(
                                Const.BGA_2P_GraphSm_Left_PosX,
                                Const.BGA_2P_GraphSm_Left_PosY
                            );
                            bgaFollowingObj.localPosition = new Vector2(
                                Const.BGA_2P_GraphSm_Left_PosX,
                                bgaFollowingObj.localPosition.y
                            );
                        }
                    }
                    break;
                case GraphType.MINI:
                    // Mini는 타입 상관없이 고정
                    if (PlaySide == 0)
                    {
                        // 1P BGA
                        bgaRect.sizeDelta = new Vector2(
                            Const.BGA_1P_GraphMini_Wid,
                            Const.BGA_1P_GraphMini_Hei
                        );
                        bgaRect.localPosition = new Vector2(
                            Const.BGA_1P_GraphMini_PosX,
                            Const.BGA_1P_GraphMini_PosY
                        );
                        layerRect.sizeDelta = new Vector2(
                            Const.BGA_1P_GraphMini_Wid,
                            Const.BGA_1P_GraphMini_Hei
                        );
                        layerRect.localPosition = new Vector3(
                            Const.BGA_1P_GraphMini_PosX,
                            Const.BGA_1P_GraphMini_PosY, -1
                        );
                        bgaVideoRect.sizeDelta = new Vector2(
                            Const.BGA_1P_GraphMini_Wid,
                            Const.BGA_1P_GraphMini_Hei
                        );
                        bgaVideoRect.localPosition = new Vector2(
                            Const.BGA_1P_GraphMini_PosX,
                            Const.BGA_1P_GraphMini_PosY
                        );
                        bgaFollowingObj.localPosition = new Vector2(
                            Const.BGA_1P_GraphMini_PosX,
                            bgaFollowingObj.localPosition.y
                        );
                    }
                    else
                    {
                        // 2P BGA
                        bgaRect.sizeDelta = new Vector2(
                            Const.BGA_2P_GraphMini_Wid,
                            Const.BGA_2P_GraphMini_Hei
                        );
                        bgaRect.localPosition = new Vector2(
                            Const.BGA_2P_GraphMini_PosX,
                            Const.BGA_2P_GraphMini_PosY
                        );
                        layerRect.sizeDelta = new Vector2(
                            Const.BGA_2P_GraphMini_Wid,
                            Const.BGA_2P_GraphMini_Hei
                        );
                        layerRect.localPosition = new Vector3(
                            Const.BGA_2P_GraphMini_PosX,
                            Const.BGA_2P_GraphMini_PosY, -1
                        );
                        bgaVideoRect.sizeDelta = new Vector2(
                            Const.BGA_2P_GraphMini_Wid,
                            Const.BGA_2P_GraphMini_Hei
                        );
                        bgaVideoRect.localPosition = new Vector2(
                            Const.BGA_2P_GraphMini_PosX,
                            Const.BGA_2P_GraphMini_PosY
                        );
                        bgaFollowingObj.localPosition = new Vector2(
                            Const.BGA_2P_GraphMini_PosX,
                            bgaFollowingObj.localPosition.y
                        );
                    }
                    break;
                case GraphType.OFFBGA:
                    if (PlaySide == 0)
                    {
                        // 1P BGA
                        bgaRect.sizeDelta = new Vector2(
                            Const.BGA_1P_GraphOff_Wid,
                            Const.BGA_1P_GraphOff_Hei
                        );
                        bgaRect.localPosition = new Vector2(
                            Const.BGA_1P_GraphOff_PosX,
                            Const.BGA_1P_GraphOff_PosY
                        );
                        layerRect.sizeDelta = new Vector2(
                            Const.BGA_1P_GraphOff_Wid,
                            Const.BGA_1P_GraphOff_Hei
                        );
                        layerRect.localPosition = new Vector3(
                            Const.BGA_1P_GraphOff_PosX,
                            Const.BGA_1P_GraphOff_PosY, -1
                        );
                        bgaVideoRect.sizeDelta = new Vector2(
                            Const.BGA_1P_GraphOff_Wid,
                            Const.BGA_1P_GraphOff_Hei
                        );
                        bgaVideoRect.localPosition = new Vector2(
                            Const.BGA_1P_GraphOff_PosX,
                            Const.BGA_1P_GraphOff_PosY
                        );
                        bgaFollowingObj.localPosition = new Vector2(
                            Const.BGA_1P_GraphOff_PosX,
                            bgaFollowingObj.localPosition.y
                        );
                    }
                    else
                    {
                        // 2P BGA
                        bgaRect.sizeDelta = new Vector2(
                            Const.BGA_2P_GraphOff_Wid,
                            Const.BGA_2P_GraphOff_Hei
                        );
                        bgaRect.localPosition = new Vector2(
                            Const.BGA_2P_GraphOff_PosX,
                            Const.BGA_2P_GraphOff_PosY
                        );
                        layerRect.sizeDelta = new Vector2(
                            Const.BGA_2P_GraphOff_Wid,
                            Const.BGA_2P_GraphOff_Hei
                        );
                        layerRect.localPosition = new Vector3(
                            Const.BGA_2P_GraphOff_PosX,
                            Const.BGA_2P_GraphOff_PosY, -1
                        );
                        bgaVideoRect.sizeDelta = new Vector2(
                            Const.BGA_2P_GraphOff_Wid,
                            Const.BGA_2P_GraphOff_Hei
                        );
                        bgaVideoRect.localPosition = new Vector2(
                            Const.BGA_2P_GraphOff_PosX,
                            Const.BGA_2P_GraphOff_PosY
                        );
                        bgaFollowingObj.localPosition = new Vector2(
                            Const.BGA_2P_GraphOff_PosX,
                            bgaFollowingObj.localPosition.y
                        );
                    }
                    break;
                case GraphType.NORMAL:
                case GraphType.OFFGEAR:
                default:
                    // Type A일때는 가운데임
                    if (!isTypeA)
                    {
                        if (PlaySide == 0)
                        {
                            // 1P Right BGA
                            bgaRect.sizeDelta = new Vector2(
                                Const.BGA_1P_GraphBig_Right_Wid,
                                Const.BGA_1P_GraphBig_Right_Hei
                            );
                            bgaRect.localPosition = new Vector2(
                                Const.BGA_1P_GraphBig_Right_PosX,
                                Const.BGA_1P_GraphBig_Right_PosY
                            );
                            layerRect.sizeDelta = new Vector2(
                                Const.BGA_1P_GraphBig_Right_Wid,
                                Const.BGA_1P_GraphBig_Right_Hei
                            );
                            layerRect.localPosition = new Vector3(
                                Const.BGA_1P_GraphBig_Right_PosX,
                                Const.BGA_1P_GraphBig_Right_PosY, -1
                            );
                            bgaVideoRect.sizeDelta = new Vector2(
                                Const.BGA_1P_GraphBig_Right_Wid,
                                Const.BGA_1P_GraphBig_Right_Hei
                            );
                            bgaVideoRect.localPosition = new Vector2(
                                Const.BGA_1P_GraphBig_Right_PosX,
                                Const.BGA_1P_GraphBig_Right_PosY
                            );
                            bgaFollowingObj.localPosition = new Vector2(
                                Const.BGA_1P_GraphBig_Right_PosX,
                                bgaFollowingObj.localPosition.y
                            );

                            FrameMoveRight();
                        }
                        else
                        {
                            // 2P Left BGA
                            bgaRect.sizeDelta = new Vector2(
                                Const.BGA_2P_GraphBig_Left_Wid,
                                Const.BGA_2P_GraphBig_Left_Hei
                            );
                            bgaRect.localPosition = new Vector2(
                                Const.BGA_2P_GraphBig_Left_PosX,
                                Const.BGA_2P_GraphBig_Left_PosY
                            );
                            layerRect.sizeDelta = new Vector2(
                                Const.BGA_2P_GraphBig_Left_Wid,
                                Const.BGA_2P_GraphBig_Left_Hei
                            );
                            layerRect.localPosition = new Vector3(
                                Const.BGA_2P_GraphBig_Left_PosX,
                                Const.BGA_2P_GraphBig_Left_PosY, -1
                            );
                            bgaVideoRect.sizeDelta = new Vector2(
                                Const.BGA_2P_GraphBig_Left_Wid,
                                Const.BGA_2P_GraphBig_Left_Hei
                            );
                            bgaVideoRect.localPosition = new Vector2(
                                Const.BGA_2P_GraphBig_Left_PosX,
                                Const.BGA_2P_GraphBig_Left_PosY
                            );
                            bgaFollowingObj.localPosition = new Vector2(
                                Const.BGA_2P_GraphBig_Left_PosX,
                                bgaFollowingObj.localPosition.y
                            );

                            FrameMoveLeft();
                        }
                    }
                    break;
            }
        }

        public void InvertObjectX(GameObject obj)
        {
            obj.transform.localPosition = new Vector3(
                obj.transform.localPosition.x * -1,
                obj.transform.localPosition.y,
                obj.transform.localPosition.z
            );
        }

        public void FrameMoveLeft()
        {
            Upper.transform.localPosition = new Vector2(
                Upper.transform.localPosition.x - 503,
                Upper.transform.localPosition.y
            );
            Lower.transform.localPosition = new Vector2(
                Lower.transform.localPosition.x - 503,
                Lower.transform.localPosition.y
            );
            Score.transform.localPosition = new Vector2(
                Score.transform.localPosition.x - 503,
                Score.transform.localPosition.y
            );
            Combo.transform.localPosition = new Vector2(
                Combo.transform.localPosition.x - 503,
                Combo.transform.localPosition.y
            );
            BPMcur.transform.localPosition = new Vector2(
                BPMcur.transform.localPosition.x - 503,
                BPMcur.transform.localPosition.y
            );
            BPMmin.transform.localPosition = new Vector2(
                BPMmin.transform.localPosition.x - 503,
                BPMmin.transform.localPosition.y
            );
            BPMmax.transform.localPosition = new Vector2(
                BPMmax.transform.localPosition.x - 503,
                BPMmax.transform.localPosition.y
            );
            UpperTitle.transform.localPosition = new Vector2(
                UpperTitle.transform.localPosition.x - 503,
                UpperTitle.transform.localPosition.y
            );
            txtCurrentTime.transform.localPosition = new Vector2(
                txtCurrentTime.transform.localPosition.x - 503,
                txtCurrentTime.transform.localPosition.y
            );
            txtTotalTime.transform.localPosition = new Vector2(
                txtTotalTime.transform.localPosition.x - 503,
                txtTotalTime.transform.localPosition.y
            );
            FPSCounter.transform.localPosition = new Vector2(
                FPSCounter.transform.localPosition.x - 503,
                FPSCounter.transform.localPosition.y
            );
        }

        public void FrameMoveRight()
        {
            Upper.transform.localPosition = new Vector2(
                Upper.transform.localPosition.x + 503,
                Upper.transform.localPosition.y
            );
            Lower.transform.localPosition = new Vector2(
                Lower.transform.localPosition.x + 503,
                Lower.transform.localPosition.y
            );
            Score.transform.localPosition = new Vector2(
                Score.transform.localPosition.x + 503,
                Score.transform.localPosition.y
            );
            Combo.transform.localPosition = new Vector2(
                Combo.transform.localPosition.x + 503,
                Combo.transform.localPosition.y
            );
            BPMcur.transform.localPosition = new Vector2(
                BPMcur.transform.localPosition.x + 503,
                BPMcur.transform.localPosition.y
            );
            BPMmin.transform.localPosition = new Vector2(
                BPMmin.transform.localPosition.x + 503,
                BPMmin.transform.localPosition.y
            );
            BPMmax.transform.localPosition = new Vector2(
                BPMmax.transform.localPosition.x + 503,
                BPMmax.transform.localPosition.y
            );
            UpperTitle.transform.localPosition = new Vector2(
                UpperTitle.transform.localPosition.x + 503,
                UpperTitle.transform.localPosition.y
            );
            txtCurrentTime.transform.localPosition = new Vector2(
                txtCurrentTime.transform.localPosition.x + 503,
                txtCurrentTime.transform.localPosition.y
            );
            txtTotalTime.transform.localPosition = new Vector2(
                txtTotalTime.transform.localPosition.x + 503,
                txtTotalTime.transform.localPosition.y
            );
            FPSCounter.transform.localPosition = new Vector2(
                FPSCounter.transform.localPosition.x + 503,
                FPSCounter.transform.localPosition.y
            );
        }
        #endregion

        #region Play Area Cover
        public void CoverSuddenDown()
        {
            float posx = coverSud.transform.localPosition.x;
            float posz = coverSud.transform.localPosition.z;

            coverSudPos -= 1;
            if(coverSudPos < 0)
            {
                coverSudPos = 0;
            }

            coverSud.transform.localPosition = new Vector3(posx, coverSudPos, posz);

            Const.CoverSudPos = coverSudPos;
        }

        public void CoverSuddenUp()
        {
            float posx = coverSud.transform.localPosition.x;
            float posz = coverSud.transform.localPosition.z;

            coverSudPos += 1;
            if (coverSudPos > 725)
            {
                coverSudPos = 725;
            }

            coverSud.transform.localPosition = new Vector3(posx, coverSudPos, posz);

            Const.CoverSudPos = coverSudPos;
        }

        public void CoverHiddenDown()
        {
            float posx = coverHid.transform.localPosition.x;
            float posz = coverHid.transform.localPosition.z;

            coverHidPos += 1;
            if (coverHidPos > 725)
            {
                coverHidPos = 725;
            }

            coverHid.transform.localPosition = new Vector3(posx, -1f * coverHidPos, posz);

            Const.CoverHidPos = coverHidPos;
        }

        public void CoverHiddenUp()
        {
            float posx = coverHid.transform.localPosition.x;
            float posz = coverHid.transform.localPosition.z;

            coverHidPos -= 1;
            if (coverHidPos < 0)
            {
                coverHidPos = 0;
            }

            coverHid.transform.localPosition = new Vector3(posx, -1f * coverHidPos, posz);

            Const.CoverHidPos = coverHidPos;
        }

        public void CoverLiftDown()
        {
            float posx = playArea.transform.localPosition.x;
            float posz = playArea.transform.localPosition.z;

            playAreaPos -= 1;
            if (playAreaPos < 0)
            {
                playAreaPos = 0;
            }

            playArea.transform.localPosition = new Vector3(posx, playAreaPos, posz);

            Const.AreaLiftPos = playAreaPos;
        }

        public void CoverLiftUp()
        {
            float posx = playArea.transform.localPosition.x;
            float posz = playArea.transform.localPosition.z;

            playAreaPos += 1;
            if (playAreaPos > 725)
            {
                playAreaPos = 725;
            }

            playArea.transform.localPosition = new Vector3(posx, playAreaPos, posz);

            Const.AreaLiftPos = playAreaPos;
        }
        #endregion
    }
}
