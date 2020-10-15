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
	// 플레이 화면의 모든 UI 요소 컨트롤
	public class PlayUI : MonoBehaviour {
        // UI Object
        private List<GameObject> NoteOnScreen;
        public GameObject layerJudgeAll;
        public GameObject layerPauseMenu;
        public Text FPSCounter;

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
        private TimingType currentJudge = TimingType.NONE;

        // Judge type BM
        public GameObject judgeTypeBM;
        public TextMesh txtTimingPercentBM;
        public TextMesh txtTimingMsBM;
        public TextMeshPro txtJudgeBM;

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

        // Gear
        public GameObject noteParentObj;
        public TextMesh gearCombo;
        public TextMesh gearExScore;
        public TextMesh gearSpeed;
        public TextMesh gearSpeedFluid;
        public TextMesh gearHP;
        public TextMesh gearGuageType;
        public TextMesh gearBPM;
        public TextMesh gearBPMmin;
        public TextMesh gearBPMmax;
        public TextMesh txtAutoPlay;
        public TextMesh txtLoading;
        public GameObject[] gearBtnPress;

        // Cover
        public GameObject coverSud;
        public GameObject coverHid;
        public GameObject playArea;
        private int coverSudPos;
        private int coverHidPos;
        private int playAreaPos;

        // Beam
        public GameObject[] beam;

        // Note Effect
        private Coroutine[] effectCoroutine;
        public SpriteRenderer[] noteEffects;
        public GameObject[] touches;
        private float[] effectRotation = new float[8];

        // Play Graph
        public SpriteRenderer graphCurrent;
        public SpriteRenderer graphMyBest;
        public SpriteRenderer graphTarget;
        public SpriteRenderer graphBestBase;
        public SpriteRenderer graphTargetBase;
        public SpriteRenderer rankCurrent;
        public SpriteRenderer rankBest;
        public SpriteRenderer rankTarget;
        public TextMesh scoreCurrent;
        public TextMesh scoreBest;
        public TextMesh scoreTarget;
        public Sprite rankAAA;
        public Sprite rankAA;
        public Sprite rankA;
        public Sprite rankB;
        public Sprite rankC;
        public Sprite rankD;
        public Sprite rankE;
        public Sprite rankF;

        // HPBar
        private HPController hpController;
        public TextMesh hpBarType;
        public SpriteRenderer hpBar;
        public Sprite hpBarAssisted;
        public Sprite hpBarEasy;
        public Sprite hpBarNormal;
        public Sprite hpBarHard;
        public Sprite hpBarExHard;

        // BGA
        public SpriteRenderer bgaImage;
        public RectTransform bgaRect;
        public VideoPlayer bgaVideo;
        public GameObject bgaVideoLayer;
        public GameObject bgaErrorLayer;

        // Fader
        public Image Fader;
        private bool FadeDone = false;
        private bool FadeStart = false;
        private bool FadeReady = false;

        private int pauseSel = 0;
        public Sprite normalBtn;
        public Sprite selectBtn;

        // Music Info
        public TextMeshPro infoTtile;

        // Display Note
        private NoteGenerator generator;
        private bool[] lnadd = new bool[8];

        public void Awake()
        {
            // 노트 표시용 설정
            generator = GetComponent<NoteGenerator>();
            for (int i = 0; i < 8; i++)
            {
                lnadd[i] = false;
                effectCoroutine = new Coroutine[8];
                effectRotation[i] = 0f;
            }

            // 판정 패널 표시 설정
            if (Const.DisplayJudge == 0) layerJudgeAll.SetActive(false);

            // HP bar 설정
            SetInitialHPBar();

            // HP 기본 수치 설정
            hpController = GetComponent<HPController>();
            UpdateHP(hpController.CurrentHP);

            // 일시정지 메뉴
            btnRestart.gameObject.GetComponent<Image>().sprite = normalBtn;
            btnExit.gameObject.GetComponent<Image>().sprite = normalBtn;

            // 판정 표시 타입 변경
            if(Const.JudgeUIType == JudgeUIType.BM)
            {
                judgeTypeED.SetActive(false);
            }
            else
            {
                judgeTypeBM.SetActive(false);
            }

            // 오토 플레이 표기
            if(Const.Auto == AutoPlayType.ON)
            {
                txtAutoPlay.gameObject.SetActive(true);
            }

            // 그래프 초기화
            SetInitialGraph();

            // 커버 포지션 변경
            coverSudPos = Const.CoverSudPos;
            coverHidPos = Const.CoverHidPos;
            playAreaPos = Const.AreaLiftPos;

            NoteOnScreen = new List<GameObject>();
        }

        private void Update()
        {
            FPSCounter.text = "FPS " + ((int)(1f / Time.unscaledDeltaTime)).ToString();
            double timing = (double)DateTime.Now.Ticks / 1000000 - timeLastComboPopup;
            if (timing > 10)
            {
                if(Const.JudgeUIType == JudgeUIType.ED)
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

            if (txtJudgeBM.gameObject.activeSelf && timing % 1.5 < 0.1)
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
            gearBPM.text = bpm.ToString("0.00");
            gearBPMmin.text = min.ToString("0.00");
            gearBPMmax.text = max.ToString("0.00");
        }

        public void SetGearCurBPM(double bpm)
        {
            gearBPM.text = bpm.ToString("0.00");
            gearSpeedFluid.text = ((int)(bpm * Const.SpeedFixed / 100)).ToString();
        }

        private void SetInitialHPBar()
        {
            switch (Const.JudgeType)
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
        }

        private void SetInitialGraph()
        {
            graphCurrent.material.SetFloat("_Progress", 0f);
            graphMyBest.material.SetFloat("_Progress", 0f);
            graphTarget.material.SetFloat("_Progress", 0f);

            graphBestBase.material.SetFloat("_Progress", 0f);
            rankBest.sprite = rankF;

            switch(Const.GraphTarget)
            {
                case GraphTargetType.A:
                    graphTargetBase.material.SetFloat("_Progress", 6f/9);
                    rankTarget.sprite = rankA;
                    break;
                case GraphTargetType.AA:
                    graphTargetBase.material.SetFloat("_Progress", 7f/9);
                    rankTarget.sprite = rankAA;
                    break;
                case GraphTargetType.AAA:
                    graphTargetBase.material.SetFloat("_Progress", 8f/9);
                    rankTarget.sprite = rankAAA;
                    break;
                case GraphTargetType.MAX:
                    graphTargetBase.material.SetFloat("_Progress", 1f);
                    rankTarget.sprite = rankAAA;
                    break;
                default:
                    graphTargetBase.material.SetFloat("_Progress", 0f);
                    rankTarget.sprite = rankF;
                    break;
            }
        }

        public void SetMusicInfo(string name)
        {
            // 곡 정보 설정
            infoTtile.text = name;
        }

        public void UpdateSpeed()
        {
            gearSpeed.text = ((float)Const.SpeedFixed / 100).ToString("0.00") + "x";
            gearSpeedFluid.text = Const.SpeedFluid.ToString();
        }

        public void UpdateHP(int hp)
        {
            float chp = (float)hp / hpController.HPMax;
            hpBar.material.SetFloat("_Progress", chp);
            gearHP.text = (chp * 100).ToString("0.00") + "%";
        }

        public void UpdateExScore(int score)
        {
            gearExScore.text = score.ToString();
            scoreCurrent.text = score.ToString();
        }

        public void UpdateGraph(int ex, int procNotes, int totalNotes)
        {
            Debug.Log(ex + " " + procNotes + " " + totalNotes);
            graphCurrent.material.SetFloat("_Progress", ((float)ex) / (totalNotes * 2));

            // 현재 자기 랭크 글자 변경
            string rank = GetRank(ex, procNotes);
            switch(rank)
            {
                case "aaa":
                    rankCurrent.sprite = rankAAA;
                    break;
                case "aa":
                    rankCurrent.sprite = rankAA;
                    break;
                case "a":
                    rankCurrent.sprite = rankA;
                    break;
                case "b":
                    rankCurrent.sprite = rankB;
                    break;
                case "c":
                    rankCurrent.sprite = rankC;
                    break;
                case "d":
                    rankCurrent.sprite = rankD;
                    break;
                case "e":
                    rankCurrent.sprite = rankE;
                    break;
                case "f":
                    rankCurrent.sprite = rankF;
                    break;
            }

            // 타겟 그래프 상승
            switch (Const.GraphTarget)
            {
                case GraphTargetType.A:
                    graphTarget.material.SetFloat("_Progress", ((float)procNotes) / totalNotes * 6 / 9);
                    scoreTarget.text = (procNotes * 2 * 6 / 9).ToString();
                    break;
                case GraphTargetType.AA:
                    graphTarget.material.SetFloat("_Progress", ((float)procNotes) / totalNotes * 7 / 9);
                    scoreTarget.text = (procNotes * 2 * 7 / 9).ToString();
                    break;
                case GraphTargetType.AAA:
                    graphTarget.material.SetFloat("_Progress", ((float)procNotes) / totalNotes * 8 / 9);
                    scoreTarget.text = (procNotes * 2 * 8 / 9).ToString();
                    break;
                case GraphTargetType.MAX:
                    graphTarget.material.SetFloat("_Progress", ((float)procNotes) / totalNotes);
                    scoreTarget.text = (procNotes * 2).ToString();
                    break;
            }
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
            gearCombo.text = combo.ToString();
        }

        public void UpdateJudge(TimingType judgetype, int combo, string accuracy, int ms)
        {
            string judgeStr = "";
            currentJudge = judgetype;
            // Judge update
            switch(judgetype)
            {
                case TimingType.PERFECT:
                    judgeSpriteED.sprite = spPerfect;
                    judgeStr = "GREAT";
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
                case TimingType.EPOOR:
                    judgeSpriteED.sprite = spMiss;
                    judgeStr = "POOR";
                    break;
            }

            // Combo update

            if (Const.JudgeUIType == JudgeUIType.ED)
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
                    txtTimingMsBM.color = new Color(135f / 255, 206f / 255, 235f / 255);
                    txtTimingMsBM.text = "FAST" + ms.ToString() + "ms";
                }
                else if(ms < 0)
                {
                    txtTimingMsBM.color = Color.red;
                    txtTimingMsBM.text = "SLOW " + Math.Abs(ms).ToString() + "ms";
                }
                else
                {
                    txtTimingMsBM.text = "";
                }

                txtTimingPercentBM.gameObject.SetActive(true);
                txtJudgeBM.gameObject.SetActive(true);
                txtTimingMsBM.gameObject.SetActive(true);
            }

            timeLastComboPopup = (double)DateTime.Now.Ticks / 1000000;
            timeLastTimingPopup = (double)DateTime.Now.Ticks / 1000000;
        }

        public void UpdateSideJudge(
            int p, int gr, int gd, int o, int m, int cb,
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
        }

        // Beam 보이기
        public void ShowAndHideBeam(int line, bool onoff)
        {
            if (onoff)
            {
                beam[line].SetActive(true);
                gearBtnPress[line].SetActive(true);
            }
            else
            {
                beam[line].SetActive(false);
                gearBtnPress[line].SetActive(false);
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
            float x = 1f;
            float y = 1f;
            sprite.transform.localScale = new Vector3(x, y, 1f);
            for(int i = 0; i < 12; i++)
            {
                yield return new WaitForSeconds(0.02f);
                sprite.transform.rotation = Quaternion.Euler(new Vector3(90f, 15f * i, 0f));
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
            sprite.transform.rotation = Quaternion.Euler(new Vector3(90f, effectRotation[pos], 0f));
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

        IEnumerator comboChangeBM(TimingType type)
        {
            if(type == TimingType.PERFECT)
            {
                txtJudgeBM.colorGradient = new VertexGradient(
                    new Color(56f / 255, 122f / 255, 208f / 255),
                    new Color(56f / 255, 122f / 255, 208f / 255),
                    new Color(183f / 255, 196f / 255, 255f / 255),
                    new Color(183f / 255, 196f / 255, 255f / 255)
                    );
                yield return new WaitForSeconds(0.05f);
                txtJudgeBM.colorGradient = new VertexGradient(
                    new Color(232f / 255, 86f / 255, 155f / 255),
                    new Color(232f / 255, 86f / 255, 155f / 255),
                    new Color(234f / 255, 164f / 255, 179f / 255),
                    new Color(234f / 255, 164f / 255, 179f / 255)
                    );
                yield return new WaitForSeconds(0.05f);
                txtJudgeBM.colorGradient = new VertexGradient(
                    new Color(175f / 255, 232f / 255, 197f / 255)
                    );
                yield return new WaitForSeconds(0.05f);
            }
            else
            {
                txtJudgeBM.colorGradient = new VertexGradient(
                    new Color(217f / 255, 150f / 255, 0f),
                    new Color(217f / 255, 150f / 255, 0f),
                    new Color(255f / 255, 225f / 255, 196f / 255),
                    new Color(255f / 255, 225f / 255, 196f / 255)
                    );
                yield return new WaitForSeconds(0.1f);
                txtJudgeBM.colorGradient = new VertexGradient(
                    new Color(0f, 0f, 0f, 0f)
                    );
                yield return new WaitForSeconds(0.05f);
            }
        }

        public void BGAVideoActivate()
        {
            bgaVideoLayer.SetActive(true);
        }

        public void BGAImageActivate()
        {
            bgaImage.gameObject.SetActive(true);
        }

        // BGA Control
        public void BGAVideoPlay(string file)
        {
            bgaVideo.url = "file://"+file;
            bgaVideo.errorReceived += BGAErrorLayer;
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

        public void DisplayMineNote(MineNote note)
        {
            GameObject noteObj = generator.AddNewMineNote(note.Line, note.Timing, noteParentObj.transform);
            noteObj.transform.SetParent(noteParentObj.transform, false);
            note.OnScreen = true;
            note.NoteObject = noteObj;
        }

        public void DisplayPlayNote(PlayNote note, List<LongNote> lnlist)
        {
            if (note.PlayNoteType == NoteType.SINGLE)
            {
                GameObject noteObj = generator.AddNewNote(note.Line, note.Timing, noteParentObj.transform);
                noteObj.transform.SetParent(noteParentObj.transform, false);
                note.OnScreen = true;
                note.NoteObject = noteObj;
            }
            else
            {
                // 아직 롱노트가 없으면 일반 노트를 하나 추가하고
                // 롱노트를 이 노트의 위치와 시작 위치 사이에 추가할 수 있도록 함
                // 이 동작은 Scroller.moveNotes()에서 isLong()을 확인해서 표기한다

                // 시작노트
                if(note.PlayNoteType == NoteType.LNSTART)
                {
                    GameObject noteObj = generator.AddNewNote(note.Line, note.Timing, noteParentObj.transform);
                    noteObj.transform.SetParent(noteParentObj.transform, false);
                    note.OnScreen = true;
                    note.NoteObject = noteObj;

                    // 끝노트도 같이 추가한다
                    for (int i = 0; i < lnlist.Count; i++)
                    {
                        if (lnlist[i].Start == note)
                        {
                        // 가운데노트
                            PlayNote lnNote = lnlist[i].Mid;
                            GameObject lnObj = generator.AddNewNote(lnNote.Line, lnNote.Timing, noteParentObj.transform);
                            lnObj.transform.SetParent(noteParentObj.transform, false);
                            lnNote.OnScreen = true;
                            lnNote.NoteObject = lnObj;

                            // 끝노트
                            PlayNote endNote = lnlist[i].End;
                            GameObject endObj = generator.AddNewNote(endNote.Line, endNote.Timing, noteParentObj.transform);
                            endObj.transform.SetParent(noteParentObj.transform, false);
                            endNote.OnScreen = true;
                            endNote.NoteObject = endObj;

                            lnlist[i].Mid.Position = (note.Position + endNote.Position) / 2;
                        }
                    }
                }
            }
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
            SceneManager.LoadScene("PlayScreen");
        }

        public void ExitGame()
        {
            SceneManager.LoadScene("MusicSelect");
        }

        public void DeactiveLoading()
        {
            txtLoading.gameObject.SetActive(false);
        }

        public void CoverSuddenDown()
        {
            float posx = coverSud.transform.localPosition.x;
            float posy = coverSud.transform.localPosition.y;

            coverSudPos -= 10;
            if(coverSudPos < 0)
            {
                coverSudPos = 0;
            }

            coverSud.transform.localPosition = new Vector3(posx, posy, coverSudPos);

            Const.CoverSudPos = coverSudPos;
        }

        public void CoverSuddenUp()
        {
            float posx = coverSud.transform.localPosition.x;
            float posy = coverSud.transform.localPosition.y;

            coverSudPos += 10;
            if (coverSudPos > 2000)
            {
                coverSudPos = 2000;
            }

            coverSud.transform.localPosition = new Vector3(posx, posy, coverSudPos);

            Const.CoverSudPos = coverSudPos;
        }

        public void CoverHiddenDown()
        {
            float posx = coverHid.transform.localPosition.x;
            float posy = coverHid.transform.localPosition.y;

            coverHidPos += 10;
            if (coverHidPos > 2000)
            {
                coverHidPos = 2000;
            }

            coverHid.transform.localPosition = new Vector3(posx, posy, -1f * coverHidPos);

            Const.CoverHidPos = coverHidPos;
        }

        public void CoverHiddenUp()
        {
            float posx = coverHid.transform.localPosition.x;
            float posy = coverHid.transform.localPosition.y;

            coverHidPos -= 10;
            if (coverHidPos < 0)
            {
                coverHidPos = 0;
            }

            coverHid.transform.localPosition = new Vector3(posx, posy, -1f * coverHidPos);

            Const.CoverHidPos = coverHidPos;
        }

        public void CoverLiftDown()
        {
            float posx = playArea.transform.localPosition.x;
            float posy = playArea.transform.localPosition.y;

            playAreaPos -= 10;
            if (playAreaPos < 0)
            {
                playAreaPos = 0;
            }

            playArea.transform.localPosition = new Vector3(posx, posy, playAreaPos);

            Const.AreaLiftPos = playAreaPos;
        }

        public void CoverLiftUp()
        {
            float posx = playArea.transform.localPosition.x;
            float posy = playArea.transform.localPosition.y;

            playAreaPos += 10;
            if (playAreaPos > 2000)
            {
                playAreaPos = 2000;
            }

            playArea.transform.localPosition = new Vector3(posx, posy, playAreaPos);

            Const.AreaLiftPos = playAreaPos;
        }
    }
}
