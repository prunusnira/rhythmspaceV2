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
        public TextMesh FPSCounter;

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
        public GameObject[] Gear1P;
        public GameObject[] Gear2P;
        public GameObject[] Area1P;
        public GameObject[] Area2P;

        // Text display
        public TextMesh Combo;
        public TextMesh Score;
        public TextMesh SpeedStandard;
        public TextMesh SpeedConstant;
        public TextMesh HP;
        public TextMesh BPMcur;
        public TextMesh BPMmin;
        public TextMesh BPMmax;
        public TextMesh Difficulty;
        public TextMesh Level;
        public TextMesh GaugeType;

        public TextMesh[] txtAutoPlayNr;
        public TextMesh[] txtAutoPlayW125;
        public TextMesh[] txtAutoPlayW150;
        public TextMesh[] txtLoadingNr;
        public TextMesh[] txtLoadingW125;
        public TextMesh[] txtLoadingW150;
        private TextMesh txtAutoPlay;
        private TextMesh txtLoading;

        // Skin
        public SpriteRenderer[] skinGear;
        public Sprite[] skinGearNormal;
        public Sprite[] skinGearDark;

        // Note Effect
        private Coroutine[] effectCoroutine;
        public SpriteRenderer[] noteEffects;
        private float[] effectRotation = new float[8];

        // BGA
        public RectTransform bgaRect;
        public RectTransform layerRect;
        public RectTransform bgaVideoRect;
        public RectTransform bgaFollowingObj;

        // Fader
        public Image Fader;
        private bool FadeDone = false;
        private bool FadeStart = false;
        private bool FadeReady = false;

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
        public SpriteRenderer Upper;
        public SpriteRenderer Lower;
        public TextMeshPro UpperTitle;
        public Sprite upperDark;
        public Sprite lowerDark;

        public void Awake()
        {
            try
            {
                // Dark Skin
                if(Const.GearSkin == SkinType.DARK)
                {
                    Upper.sprite = upperDark;
                    Lower.sprite = lowerDark;
                }

                // Reset
                Const.ResultTarget = 0;

                // 사용자 설정에 따른 기어-BGA-그래프 위치 변경
                ObjectPositionSetup();
                FrameMove();

                for (int i = 0; i < 8; i++)
                {
                    effectCoroutine = new Coroutine[8];
                    effectRotation[i] = 0f;
                }

                // 판정 패널 표시 설정
                if (Const.DisplayJudge == 0) layerJudgeAll.SetActive(false);

                // 오토 플레이 표기
                if (Const.Auto == AutoPlayType.ALL)
                {
                    txtAutoPlay.gameObject.SetActive(true);
                }

                NoteOnScreen = new List<GameObject>();

                // 기어에 표시하는 정보의 위치와 개수 확인
                //SideInfoDisplayPosition();
            }
            catch (Exception e)
            {
                ErrorHandler.LogError(e.Message + " " + e.StackTrace);
            }
        }

        private void Update()
        {
            FPSCounter.text = "FPS " + ((int)(1f / Time.unscaledDeltaTime)).ToString();
            
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
            BPMcur.text = bpm.ToString("0.##");
            BPMmin.text = min.ToString("0.##");
            BPMmax.text = max.ToString("0.##");
        }

        public void SetGearCurBPM(double bpm)
        {
            BPMcur.text = bpm.ToString("0.##");
            SpeedConstant.text = ((int)(bpm * Const.SpeedStd / 100)).ToString();
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
            SpeedStandard.text = ((float)Const.SpeedStd / 100).ToString("0.00") + "x";
            SpeedConstant.text = Const.SpeedCon.ToString();
        }

        public void UpdateScore(int score)
        {
            Score.text = score.ToString();
        }

        public void UpdateMaxCombo(int combo)
        {
            Combo.text = combo.ToString();
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
                noteEffects[pos].transform.localScale = new Vector3(1f, 1f, 1f);
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
            float x = 3f;
            float y = 3f;
            sprite.transform.localScale = new Vector3(x, y, 1f);
            for(int i = 0; i < 12; i++)
            {
                yield return new WaitForSeconds(0.01f);
                sprite.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 15f * i));
                sprite.transform.localScale = new Vector3(x * 0.8f, y * 0.8f, 1f);
                x *= 0.8f;
                y *= 0.8f;
            }
            yield return new WaitForSeconds(0.01f);
            sprite.gameObject.SetActive(false);
        }

        IEnumerator noteEffectLN(SpriteRenderer sprite, int pos)
        {
            yield return new WaitForSeconds(0.01f);
            effectRotation[pos] += 15f;
            if (effectRotation[pos] % 360 == 0) effectRotation[pos] = 0f;
            sprite.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, effectRotation[pos]));
        }

        public void DeactiveLoading()
        {
            txtLoading.gameObject.SetActive(false);
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

        #region Gear/BGA/Graph/Frame/Text Position
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

            switch(Const.GearSize)
            {
                case SkinSize.STANDARD:
                    txtLoading = txtLoadingNr[Const.PlayerSide];
                    txtAutoPlay = txtAutoPlayNr[Const.PlayerSide];
                    break;
                case SkinSize.WIDE125:
                    txtLoading = txtLoadingW125[Const.PlayerSide];
                    txtAutoPlay = txtAutoPlayW125[Const.PlayerSide];
                    break;
                case SkinSize.WIDE150:
                    txtLoading = txtLoadingW150[Const.PlayerSide];
                    txtAutoPlay = txtAutoPlayW150[Const.PlayerSide];
                    break;
            }

            if (Const.GearSkin == SkinType.NORMAL)
            {
                skinGear[Const.PlayerSide].sprite = skinGearNormal[Const.PlayerSide];
            }
            else if (Const.GearSkin == SkinType.DARK)
            {
                skinGear[Const.PlayerSide].sprite = skinGearDark[Const.PlayerSide];
            }

            int PlaySide = Const.PlayerSide;
            if(PlaySide == 0)
            {
                Gear1P[(int)Const.GearSize].SetActive(true);
                Area1P[(int)Const.GearSize].SetActive(true);

                if(Const.GraphType == GraphType.OFFGEAR)
                {
                    Gear2P[(int)Const.GearSize].SetActive(true);
                    Area2P[(int)Const.GearSize].SetActive(true);
                }
            }
            else
            {
                Gear2P[(int)Const.GearSize].SetActive(true);
                Area2P[(int)Const.GearSize].SetActive(true);

                if (Const.GraphType == GraphType.OFFGEAR)
                {
                    Gear1P[(int)Const.GearSize].SetActive(true);
                    Area1P[(int)Const.GearSize].SetActive(true);
                }
            }

            int invert = Const.PlayerSide == 0 ? 1 : -1;

            HP.transform.localPosition =
                new Vector3(
                    Const.HPTextPosX[(int)Const.GearSize] * invert,
                    Const.HPTextPosY[(int)Const.GearSize],
                    HP.transform.localPosition.z);
            GaugeType.transform.localPosition =
                new Vector3(
                    Const.GaugeTypeTextPosX[(int)Const.GearSize] * invert,
                    Const.GaugeTypeTextPosY[(int)Const.GearSize],
                    GaugeType.transform.localPosition.z);
            SpeedStandard.transform.localPosition =
                new Vector3(
                    Const.SpdStdTextPosX[(int)Const.GearSize] * invert,
                    Const.SpdStdTextPosY[(int)Const.GearSize],
                    SpeedStandard.transform.localPosition.z);
            SpeedConstant.transform.localPosition =
                new Vector3(
                    Const.SpdConTextPosX[(int)Const.GearSize] * invert,
                    Const.SpdConTextPosY[(int)Const.GearSize],
                    SpeedStandard.transform.localPosition.z);
            Difficulty.transform.localPosition =
                new Vector3(
                    Const.DiffTextPosX[(int)Const.GearSize] * invert,
                    Const.DiffTextPosY[(int)Const.GearSize],
                    Difficulty.transform.localPosition.z);
            Level.transform.localPosition =
                new Vector3(
                    Const.LvTextPosX[(int)Const.GearSize] * invert,
                    Const.LvTextPosY[(int)Const.GearSize],
                    Level.transform.localPosition.z);

            switch (Const.GraphType)
            {
                case GraphType.SMALL:
                    if (isTypeA)
                    {
                        if (PlaySide == 0)
                        {
                            // 1P Left BGA
                            bgaRect.sizeDelta = new Vector2(
                                Const.BGA_GraphSm_Wid[(int)Const.GearSize],
                                Const.BGA_GraphSm_Hei[(int)Const.GearSize]
                            );
                            bgaRect.localPosition = new Vector3(
                                Const.BGA_1P_GraphSm_Left_PosX[(int)Const.GearSize],
                                Const.BGA_1P_GraphSm_Left_PosY[(int)Const.GearSize],
                                bgaRect.localPosition.z
                            );
                            layerRect.sizeDelta = new Vector2(
                                Const.BGA_GraphSm_Wid[(int)Const.GearSize],
                                Const.BGA_GraphSm_Hei[(int)Const.GearSize]
                            );
                            layerRect.localPosition = new Vector3(
                                Const.BGA_1P_GraphSm_Left_PosX[(int)Const.GearSize],
                                Const.BGA_1P_GraphSm_Left_PosY[(int)Const.GearSize],
                                layerRect.localPosition.z
                            );
                            bgaVideoRect.sizeDelta = new Vector2(
                                Const.BGA_GraphSm_Wid[(int)Const.GearSize],
                                Const.BGA_GraphSm_Hei[(int)Const.GearSize]
                            );
                            bgaVideoRect.localPosition = new Vector3(
                                Const.BGA_1P_GraphSm_Left_PosX[(int)Const.GearSize],
                                Const.BGA_1P_GraphSm_Left_PosY[(int)Const.GearSize],
                                bgaVideoRect.localPosition.z
                            );
                            bgaFollowingObj.localPosition = new Vector3(
                                Const.BGA_1P_GraphSm_Left_PosX[(int)Const.GearSize],
                                bgaFollowingObj.localPosition.y,
                                bgaFollowingObj.localPosition.z
                            );
                        }
                        else
                        {
                            // 2P Right BGA
                            bgaRect.sizeDelta = new Vector2(
                                Const.BGA_GraphSm_Wid[(int)Const.GearSize],
                                Const.BGA_GraphSm_Hei[(int)Const.GearSize]
                            );
                            bgaRect.localPosition = new Vector3(
                                Const.BGA_2P_GraphSm_Right_PosX[(int)Const.GearSize],
                                Const.BGA_2P_GraphSm_Right_PosY[(int)Const.GearSize],
                                bgaRect.localPosition.z
                            );
                            layerRect.sizeDelta = new Vector2(
                                Const.BGA_GraphSm_Wid[(int)Const.GearSize],
                                Const.BGA_GraphSm_Hei[(int)Const.GearSize]
                            );
                            layerRect.localPosition = new Vector3(
                                Const.BGA_2P_GraphSm_Right_PosX[(int)Const.GearSize],
                                Const.BGA_2P_GraphSm_Right_PosY[(int)Const.GearSize],
                                layerRect.localPosition.z
                            );
                            bgaVideoRect.sizeDelta = new Vector2(
                                Const.BGA_GraphSm_Wid[(int)Const.GearSize],
                                Const.BGA_GraphSm_Hei[(int)Const.GearSize]
                            );
                            bgaVideoRect.localPosition = new Vector3(
                                Const.BGA_2P_GraphSm_Right_PosX[(int)Const.GearSize],
                                Const.BGA_2P_GraphSm_Right_PosY[(int)Const.GearSize],
                                bgaVideoRect.localPosition.z
                            );
                            bgaFollowingObj.localPosition = new Vector3(
                                Const.BGA_2P_GraphSm_Right_PosX[(int)Const.GearSize],
                                bgaFollowingObj.localPosition.y,
                                bgaFollowingObj.localPosition.z
                            );
                        }
                    }
                    else
                    {
                        if (PlaySide == 0)
                        {
                            // 1P Right BGA
                            bgaRect.sizeDelta = new Vector2(
                                Const.BGA_GraphSm_Wid[(int)Const.GearSize],
                                Const.BGA_GraphSm_Hei[(int)Const.GearSize]
                            );
                            bgaRect.localPosition = new Vector3(
                                Const.BGA_1P_GraphSm_Right_PosX[(int)Const.GearSize],
                                Const.BGA_1P_GraphSm_Right_PosY[(int)Const.GearSize],
                                bgaRect.localPosition.z
                            );
                            layerRect.sizeDelta = new Vector2(
                                Const.BGA_GraphSm_Wid[(int)Const.GearSize],
                                Const.BGA_GraphSm_Hei[(int)Const.GearSize]
                            );
                            layerRect.localPosition = new Vector3(
                                Const.BGA_1P_GraphSm_Right_PosX[(int)Const.GearSize],
                                Const.BGA_1P_GraphSm_Right_PosY[(int)Const.GearSize],
                                layerRect.localPosition.z
                            );
                            bgaVideoRect.sizeDelta = new Vector2(
                                Const.BGA_GraphSm_Wid[(int)Const.GearSize],
                                Const.BGA_GraphSm_Hei[(int)Const.GearSize]
                            );
                            bgaVideoRect.localPosition = new Vector3(
                                Const.BGA_1P_GraphSm_Right_PosX[(int)Const.GearSize],
                                Const.BGA_1P_GraphSm_Right_PosY[(int)Const.GearSize],
                                bgaVideoRect.localPosition.z
                            );
                            bgaFollowingObj.localPosition = new Vector3(
                                Const.BGA_1P_GraphSm_Right_PosX[(int)Const.GearSize],
                                bgaFollowingObj.localPosition.y,
                                bgaFollowingObj.localPosition.z
                            );
                        }
                        else
                        {
                            // 2P Left BGA
                            bgaRect.sizeDelta = new Vector2(
                                Const.BGA_GraphSm_Wid[(int)Const.GearSize],
                                Const.BGA_GraphSm_Hei[(int)Const.GearSize]
                            );
                            bgaRect.localPosition = new Vector3(
                                Const.BGA_2P_GraphSm_Left_PosX[(int)Const.GearSize],
                                Const.BGA_2P_GraphSm_Left_PosY[(int)Const.GearSize],
                                bgaRect.localPosition.z
                            );
                            layerRect.sizeDelta = new Vector2(
                                Const.BGA_GraphSm_Wid[(int)Const.GearSize],
                                Const.BGA_GraphSm_Hei[(int)Const.GearSize]
                            );
                            layerRect.localPosition = new Vector3(
                                Const.BGA_2P_GraphSm_Left_PosX[(int)Const.GearSize],
                                Const.BGA_2P_GraphSm_Left_PosY[(int)Const.GearSize],
                                layerRect.localPosition.z
                            );
                            bgaVideoRect.sizeDelta = new Vector2(
                                Const.BGA_GraphSm_Wid[(int)Const.GearSize],
                                Const.BGA_GraphSm_Hei[(int)Const.GearSize]
                            );
                            bgaVideoRect.localPosition = new Vector3(
                                Const.BGA_2P_GraphSm_Left_PosX[(int)Const.GearSize],
                                Const.BGA_2P_GraphSm_Left_PosY[(int)Const.GearSize],
                                bgaVideoRect.localPosition.z
                            );
                            bgaFollowingObj.localPosition = new Vector3(
                                Const.BGA_2P_GraphSm_Left_PosX[(int)Const.GearSize],
                                bgaFollowingObj.localPosition.y,
                                bgaFollowingObj.localPosition.z
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
                            Const.BGA_GraphMini_Wid[(int)Const.GearSize],
                            Const.BGA_GraphMini_Hei[(int)Const.GearSize]
                        );
                        bgaRect.localPosition = new Vector3(
                            Const.BGA_1P_GraphMini_PosX[(int)Const.GearSize],
                            Const.BGA_1P_GraphMini_PosY[(int)Const.GearSize],
                            bgaRect.localPosition.z
                        );
                        layerRect.sizeDelta = new Vector2(
                            Const.BGA_GraphMini_Wid[(int)Const.GearSize],
                            Const.BGA_GraphMini_Hei[(int)Const.GearSize]
                        );
                        layerRect.localPosition = new Vector3(
                            Const.BGA_1P_GraphMini_PosX[(int)Const.GearSize],
                            Const.BGA_1P_GraphMini_PosY[(int)Const.GearSize],
                            layerRect.localPosition.z
                        );
                        bgaVideoRect.sizeDelta = new Vector2(
                            Const.BGA_GraphMini_Wid[(int)Const.GearSize],
                            Const.BGA_GraphMini_Hei[(int)Const.GearSize]
                        );
                        bgaVideoRect.localPosition = new Vector3(
                            Const.BGA_1P_GraphMini_PosX[(int)Const.GearSize],
                            Const.BGA_1P_GraphMini_PosY[(int)Const.GearSize],
                            bgaVideoRect.localPosition.z
                        );
                        bgaFollowingObj.localPosition = new Vector3(
                            Const.BGA_1P_GraphMini_PosX[(int)Const.GearSize],
                            bgaFollowingObj.localPosition.y,
                            bgaFollowingObj.localPosition.z
                        );
                    }
                    else
                    {
                        // 2P BGA
                        bgaRect.sizeDelta = new Vector2(
                            Const.BGA_GraphMini_Wid[(int)Const.GearSize],
                            Const.BGA_GraphMini_Hei[(int)Const.GearSize]
                        );
                        bgaRect.localPosition = new Vector3(
                            Const.BGA_2P_GraphMini_PosX[(int)Const.GearSize],
                            Const.BGA_2P_GraphMini_PosY[(int)Const.GearSize],
                            bgaRect.localPosition.z
                        );
                        layerRect.sizeDelta = new Vector2(
                            Const.BGA_GraphMini_Wid[(int)Const.GearSize],
                            Const.BGA_GraphMini_Hei[(int)Const.GearSize]
                        );
                        layerRect.localPosition = new Vector3(
                            Const.BGA_2P_GraphMini_PosX[(int)Const.GearSize],
                            Const.BGA_2P_GraphMini_PosY[(int)Const.GearSize],
                            layerRect.localPosition.z
                        );
                        bgaVideoRect.sizeDelta = new Vector2(
                            Const.BGA_GraphMini_Wid[(int)Const.GearSize],
                            Const.BGA_GraphMini_Hei[(int)Const.GearSize]
                        );
                        bgaVideoRect.localPosition = new Vector3(
                            Const.BGA_2P_GraphMini_PosX[(int)Const.GearSize],
                            Const.BGA_2P_GraphMini_PosY[(int)Const.GearSize],
                            bgaVideoRect.localPosition.z
                        );
                        bgaFollowingObj.localPosition = new Vector3(
                            Const.BGA_2P_GraphMini_PosX[(int)Const.GearSize],
                            bgaFollowingObj.localPosition.y,
                            bgaFollowingObj.localPosition.z
                        );
                    }
                    break;
                case GraphType.OFFBGA:
                    if (PlaySide == 0)
                    {
                        // 1P BGA
                        bgaRect.sizeDelta = new Vector2(
                            Const.BGA_GraphOff_Wid[(int)Const.GearSize],
                            Const.BGA_GraphOff_Hei[(int)Const.GearSize]
                        );
                        bgaRect.localPosition = new Vector3(
                            Const.BGA_1P_GraphOff_PosX[(int)Const.GearSize],
                            Const.BGA_1P_GraphOff_PosY[(int)Const.GearSize],
                            bgaRect.localPosition.z
                        );
                        layerRect.sizeDelta = new Vector2(
                            Const.BGA_GraphOff_Wid[(int)Const.GearSize],
                            Const.BGA_GraphOff_Hei[(int)Const.GearSize]
                        );
                        layerRect.localPosition = new Vector3(
                            Const.BGA_1P_GraphOff_PosX[(int)Const.GearSize],
                            Const.BGA_1P_GraphOff_PosY[(int)Const.GearSize],
                            layerRect.localPosition.z
                        );
                        bgaVideoRect.sizeDelta = new Vector2(
                            Const.BGA_GraphOff_Wid[(int)Const.GearSize],
                            Const.BGA_GraphOff_Hei[(int)Const.GearSize]
                        );
                        bgaVideoRect.localPosition = new Vector3(
                            Const.BGA_1P_GraphOff_PosX[(int)Const.GearSize],
                            Const.BGA_1P_GraphOff_PosY[(int)Const.GearSize],
                            bgaVideoRect.localPosition.z
                        );
                        bgaFollowingObj.localPosition = new Vector3(
                            Const.BGA_1P_GraphOff_PosX[(int)Const.GearSize],
                            bgaFollowingObj.localPosition.y,
                            bgaFollowingObj.localPosition.z
                        );
                    }
                    else
                    {
                        // 2P BGA
                        bgaRect.sizeDelta = new Vector2(
                            Const.BGA_GraphOff_Wid[(int)Const.GearSize],
                            Const.BGA_GraphOff_Hei[(int)Const.GearSize]
                        );
                        bgaRect.localPosition = new Vector3(
                            Const.BGA_2P_GraphOff_PosX[(int)Const.GearSize],
                            Const.BGA_2P_GraphOff_PosY[(int)Const.GearSize],
                            bgaRect.localPosition.z
                        );
                        layerRect.sizeDelta = new Vector2(
                            Const.BGA_GraphOff_Wid[(int)Const.GearSize],
                            Const.BGA_GraphOff_Hei[(int)Const.GearSize]
                        );
                        layerRect.localPosition = new Vector3(
                            Const.BGA_2P_GraphOff_PosX[(int)Const.GearSize],
                            Const.BGA_2P_GraphOff_PosY[(int)Const.GearSize],
                            layerRect.localPosition.z
                        );
                        bgaVideoRect.sizeDelta = new Vector2(
                            Const.BGA_GraphOff_Wid[(int)Const.GearSize],
                            Const.BGA_GraphOff_Hei[(int)Const.GearSize]
                        );
                        bgaVideoRect.localPosition = new Vector3(
                            Const.BGA_2P_GraphOff_PosX[(int)Const.GearSize],
                            Const.BGA_2P_GraphOff_PosY[(int)Const.GearSize],
                            bgaVideoRect.localPosition.z
                        );
                        bgaFollowingObj.localPosition = new Vector3(
                            Const.BGA_2P_GraphOff_PosX[(int)Const.GearSize],
                            bgaFollowingObj.localPosition.y,
                            bgaFollowingObj.localPosition.z
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
                                Const.BGA_GraphBig_Wid[(int)Const.GearSize],
                                Const.BGA_GraphBig_Hei[(int)Const.GearSize]
                            );
                            bgaRect.localPosition = new Vector3(
                                Const.BGA_1P_GraphBig_Right_PosX[(int)Const.GearSize],
                                Const.BGA_1P_GraphBig_Right_PosY[(int)Const.GearSize],
                                bgaRect.localPosition.z
                            );
                            layerRect.sizeDelta = new Vector2(
                                Const.BGA_GraphBig_Wid[(int)Const.GearSize],
                                Const.BGA_GraphBig_Hei[(int)Const.GearSize]
                            );
                            layerRect.localPosition = new Vector3(
                                Const.BGA_1P_GraphBig_Right_PosX[(int)Const.GearSize],
                                Const.BGA_1P_GraphBig_Right_PosY[(int)Const.GearSize],
                                layerRect.localPosition.z
                            );
                            bgaVideoRect.sizeDelta = new Vector2(
                                Const.BGA_GraphBig_Wid[(int)Const.GearSize],
                                Const.BGA_GraphBig_Hei[(int)Const.GearSize]
                            );
                            bgaVideoRect.localPosition = new Vector3(
                                Const.BGA_1P_GraphBig_Right_PosX[(int)Const.GearSize],
                                Const.BGA_1P_GraphBig_Right_PosY[(int)Const.GearSize],
                                bgaVideoRect.localPosition.z
                            );
                            bgaFollowingObj.localPosition = new Vector3(
                                Const.BGA_1P_GraphBig_Right_PosX[(int)Const.GearSize],
                                bgaFollowingObj.localPosition.y,
                                bgaFollowingObj.localPosition.z
                            );
                        }
                        else
                        {
                            // 2P Left BGA
                            bgaRect.sizeDelta = new Vector2(
                                Const.BGA_GraphBig_Wid[(int)Const.GearSize],
                                Const.BGA_GraphBig_Hei[(int)Const.GearSize]
                            );
                            bgaRect.localPosition = new Vector3(
                                Const.BGA_2P_GraphBig_Left_PosX[(int)Const.GearSize],
                                Const.BGA_2P_GraphBig_Left_PosY[(int)Const.GearSize],
                                bgaRect.localPosition.z
                            );
                            layerRect.sizeDelta = new Vector2(
                                Const.BGA_GraphBig_Wid[(int)Const.GearSize],
                                Const.BGA_GraphBig_Hei[(int)Const.GearSize]
                            );
                            layerRect.localPosition = new Vector3(
                                Const.BGA_2P_GraphBig_Left_PosX[(int)Const.GearSize],
                                Const.BGA_2P_GraphBig_Left_PosY[(int)Const.GearSize],
                                layerRect.localPosition.z
                            );
                            bgaVideoRect.sizeDelta = new Vector2(
                                Const.BGA_GraphBig_Wid[(int)Const.GearSize],
                                Const.BGA_GraphBig_Hei[(int)Const.GearSize]
                            );
                            bgaVideoRect.localPosition = new Vector3(
                                Const.BGA_2P_GraphBig_Left_PosX[(int)Const.GearSize],
                                Const.BGA_2P_GraphBig_Left_PosY[(int)Const.GearSize],
                                bgaVideoRect.localPosition.z
                            );
                            bgaFollowingObj.localPosition = new Vector3(
                                Const.BGA_2P_GraphBig_Left_PosX[(int)Const.GearSize],
                                bgaFollowingObj.localPosition.y,
                                bgaFollowingObj.localPosition.z
                            );
                        }
                    }
                    break;
            }
        }

        private void FrameMove()
        {
            int UpperMove = 0;
            int LowerMove = 0;
            if (Const.PlayerSide == 0)
            {
                if (Const.LayoutType == UILayoutType.TYPEB)
                {
                    if (Const.GraphType == GraphType.NORMAL)
                    {
                        UpperMove = Const.UpperRightPosX[(int)Const.GearSize];
                        LowerMove = Const.LowerRightPosX[(int)Const.GearSize];
                    }
                    else
                    {
                        UpperMove = Const.UpperCenter_1PPosX[(int)Const.GearSize];
                        LowerMove = Const.LowerCenter_1PPosX[(int)Const.GearSize];
                    }
                }
                else
                {
                    UpperMove = Const.UpperCenter_1PPosX[(int)Const.GearSize];
                    LowerMove = Const.LowerCenter_1PPosX[(int)Const.GearSize];
                }
            }
            else
            {
                if (Const.LayoutType == UILayoutType.TYPEB)
                {
                    if (Const.GraphType == GraphType.NORMAL)
                    {
                        UpperMove = Const.UpperLeftPosX[(int)Const.GearSize];
                        LowerMove = Const.LowerLeftPosX[(int)Const.GearSize];
                    }
                    else
                    {
                        UpperMove = Const.UpperCenter_2PPosX[(int)Const.GearSize];
                        LowerMove = Const.LowerCenter_2PPosX[(int)Const.GearSize];
                    }
                }
                else
                {
                    UpperMove = Const.UpperCenter_2PPosX[(int)Const.GearSize];
                    LowerMove = Const.LowerCenter_2PPosX[(int)Const.GearSize];
                }
            }

            Upper.transform.localPosition = new Vector3(
                Upper.transform.localPosition.x + UpperMove,
                Upper.transform.localPosition.y,
                Upper.transform.localPosition.z
            );
            UpperTitle.transform.localPosition = new Vector3(
                UpperTitle.transform.localPosition.x + UpperMove,
                UpperTitle.transform.localPosition.y,
                UpperTitle.transform.localPosition.z
            );

            Lower.transform.localPosition = new Vector2(
                Lower.transform.localPosition.x + LowerMove,
                Lower.transform.localPosition.y
            );
            Score.transform.localPosition = new Vector2(
                Score.transform.localPosition.x + LowerMove,
                Score.transform.localPosition.y
            );
            Combo.transform.localPosition = new Vector2(
                Combo.transform.localPosition.x + LowerMove,
                Combo.transform.localPosition.y
            );
            BPMcur.transform.localPosition = new Vector2(
                BPMcur.transform.localPosition.x + LowerMove,
                BPMcur.transform.localPosition.y
            );
            BPMmin.transform.localPosition = new Vector2(
                BPMmin.transform.localPosition.x + LowerMove,
                BPMmin.transform.localPosition.y
            );
            BPMmax.transform.localPosition = new Vector2(
                BPMmax.transform.localPosition.x + LowerMove,
                BPMmax.transform.localPosition.y
            );
            txtCurrentTime.transform.localPosition = new Vector2(
                txtCurrentTime.transform.localPosition.x + LowerMove,
                txtCurrentTime.transform.localPosition.y
            );
            txtTotalTime.transform.localPosition = new Vector2(
                txtTotalTime.transform.localPosition.x + LowerMove,
                txtTotalTime.transform.localPosition.y
            );
            FPSCounter.transform.localPosition = new Vector2(
                FPSCounter.transform.localPosition.x + LowerMove,
                FPSCounter.transform.localPosition.y
            );
        }
        #endregion
    }
}
