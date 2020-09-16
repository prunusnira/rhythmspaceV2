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

        // Gear
        public GameObject playArea;
        public TextMesh gearCombo;
        public TextMesh gearExScore;
        public TextMesh gearSpeed;
        public TextMesh gearSpeedFluid;
        public TextMesh gearHP;
        public TextMesh gearGuageType;
        public TextMesh gearBPM;
        public TextMesh gearBPMmin;
        public TextMesh gearBPMmax;

        // Beam
        public GameObject beam1;
        public GameObject beam2;
        public GameObject beam3;
        public GameObject beam4;
        public GameObject beam5;
        public GameObject beam6;
        public GameObject beam7;
        public GameObject beam8;

        // Play Graph
        public SpriteRenderer graphCurrent;
        public SpriteRenderer graphMyBest;
        public SpriteRenderer graphTarget;
        public SpriteRenderer graphBestBase;
        public SpriteRenderer graphTargetBase;
        public TextMesh rankCurrent;
        public TextMesh rankBest;
        public TextMesh rankTarget;
        public TextMesh scoreCurrent;
        public TextMesh scoreBest;
        public TextMesh scoreTarget;

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
        public MPMP bgaVideo;
        public SpriteRenderer bgaImage;
        public GameObject[] touches;

        public GameObject fadeCube;
        private bool isFading = false;

        private int pauseSel = 0;
        public Sprite normalBtn;
        public Sprite selectBtn;

        // Music Info
        public TextMesh infoTtile;

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
            }

            // 판정 패널 표시 설정
            if (Const.GetPJudge() == 0) layerJudgeAll.SetActive(false);

            // HP bar 설정
            SetInitialHPBar();

            // HP 기본 수치 설정
            hpController = GetComponent<HPController>();
            UpdateHP(hpController.GetHP());

            // 일시정지 메뉴
            btnRestart.gameObject.GetComponent<Image>().sprite = normalBtn;
            btnExit.gameObject.GetComponent<Image>().sprite = normalBtn;

            // 판정 표시 타입 변경
            if(Const.GetJudgeUIType() == JudgeUIType.BM)
            {
                judgeTypeED.SetActive(false);
            }
            else
            {
                judgeTypeBM.SetActive(false);
            }

            // 그래프 초기화
            SetInitialGraph();
        }

        private void Update()
        {
            FPSCounter.text = "FPS " + ((int)(1f / Time.unscaledDeltaTime)).ToString();
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

        public void SetGearBPM(double bpm, double min, double max)
        {
            gearBPM.text = bpm.ToString("0.00");
            gearBPMmin.text = min.ToString("0.00");
            gearBPMmax.text = max.ToString("0.00");
        }

        public void SetGearCurBPM(double bpm)
        {
            gearBPM.text = bpm.ToString("0.00");
            gearSpeedFluid.text = ((int)(bpm * Const.GetSpeedFixed() / 100)).ToString();
        }

        private void SetInitialHPBar()
        {
            switch (Const.GetJudgeType())
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
            rankBest.text = "F";

            switch(Const.GetGraphTarget())
            {
                case GraphTargetType.A:
                    graphTargetBase.material.SetFloat("_Progress", 6f/9);
                    rankTarget.text = "A";
                    break;
                case GraphTargetType.AA:
                    graphTargetBase.material.SetFloat("_Progress", 7f/9);
                    rankTarget.text = "AA";
                    break;
                case GraphTargetType.AAA:
                    graphTargetBase.material.SetFloat("_Progress", 8f/9);
                    rankTarget.text = "AAA";
                    break;
                case GraphTargetType.MAX:
                    graphTargetBase.material.SetFloat("_Progress", 1f);
                    rankTarget.text = "AAA";
                    break;
                default:
                    graphTargetBase.material.SetFloat("_Progress", 0f);
                    rankTarget.text = "F";
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
            gearSpeed.text = ((float)Const.GetSpeedFixed() / 100).ToString("0.00") + "x";
            gearSpeedFluid.text = Const.GetSpeedFluid().ToString();
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

        public void UpdateExScore(int score)
        {
            gearExScore.text = score.ToString();
            scoreCurrent.text = score.ToString();
        }

        public void UpdateGraph(int ex, int procNotes, int totalNotes)
        {
            graphCurrent.material.SetFloat("_Progress", ((float)ex) / (totalNotes * 2));

            // 현재 자기 랭크 글자 변경
            float currentRankState = (float)ex / (procNotes * 2);
            if(currentRankState >= 8f / 9)
            {
                rankCurrent.text = "AAA";
            }
            else if (currentRankState >= 7f / 9)
            {
                rankCurrent.text = "AA";
            }
            else if (currentRankState >= 6f / 9)
            {
                rankCurrent.text = "A";
            }
            else if (currentRankState >= 5f / 9)
            {
                rankCurrent.text = "B";
            }
            else if (currentRankState >= 4f / 9)
            {
                rankCurrent.text = "C";
            }
            else if (currentRankState >= 3f / 9)
            {
                rankCurrent.text = "D";
            }
            else if (currentRankState >= 2f / 9)
            {
                rankCurrent.text = "E";
            }
            else
            {
                rankCurrent.text = "F";
            }

            // 타겟 그래프 상승
            switch (Const.GetGraphTarget())
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

        public void displayNote(ref Note note, ref List<Longnote> lnlist)
        {
            if (!note.isLong())
            {
                if (note.getNotetype() == Note.NOTETYPE.PLAYABLE)
                {
                    //if(n.getLane() <= Const.GetPlayline())
                    //{
                    GameObject noteObj = generator.AddNewNote(note.getLane(), note.getPosition(), playArea.transform);
                    noteObj.transform.SetParent(playArea.transform, false);
                    note.setNote(noteObj);
                    note.setReleased(true);
                    //}
                    //else
                    //{
                    // 플레이 라인을 벗어나는 경우 MUSIC 타입으로 변경
                    //    n.changeToMusic();
                    //}
                }
            }
            else
            {
                // 롱노트 중에 일단 라인을 벗어나는지 검사
                /*if (n.getLane() > Const.GetPlayline())
                {
                    // 라인을 벗어나면 롱노트로 추가하지 않고
                    // 시작 노트를 MUSIC 타입으로 변경 후 종료
                    int cline = n.getLane();

                    if (lnadd[cline])
                    {
                        remove.Add(n);
                        lnadd[cline] = false;
                    }
                    else
                    {
                        n.changeToMusic();
                        lnadd[cline] = true;
                    }
                }
                else
                {*/
                // 롱노트 처리
                //int cline = note.getLane();
                /*if (lnadd[cline])
                {
                    // 이미 롱노트가 추가중인 상태이면 현재 라인의 lnlist를 갱신하고 노트 표시 추가
                    for (int i = 0; i < lnlist.Count; i++)
                    {
                        if (lnlist[i].getLane() == cline && lnlist[i].getEnd() == null)
                        {
                            GameObject noteObj = generator.AddNewNote(note.getLane(), note.getPosition(), playArea.transform);
                            noteObj.transform.parent = playArea.transform;
                            note.setNote(noteObj); // 끝노트
                            note.setReleased(true);

                            lnlist[i].setEnd(note);
                            lnlist[i].setEndPos(note.getPosition());
                            lnlist[i].getMiddle().setPosition(
                                (lnlist[i].getStart().getPosition() + note.getPosition()) / 2
                            );
                            lnadd[cline] = false;
                        }
                    }
                }
                else
                {*/
                    // 아직 롱노트가 없으면 일반 노트를 하나 추가하고
                    // 롱노트를 이 노트의 위치와 시작 위치 사이에 추가할 수 있도록 함
                    // 이 동작은 Scroller.moveNotes()에서 isLong()을 확인해서 표기한다

                    // 시작노트
                    GameObject noteObj = generator.AddNewNote(note.getLane(), note.getPosition(), playArea.transform);
                    noteObj.transform.SetParent(playArea.transform, false);
                    note.setNote(noteObj);
                    note.setReleased(true);

                    // 끝노트도 같이 추가한다
                    for (int i = 0; i < lnlist.Count; i++)
                    {
                        if (lnlist[i].getStart() == note)
                        {
                        // 가운데노트
                            Note lnNote = lnlist[i].getMiddle();
                            GameObject lnObj = generator.AddNewNote(lnNote.getLane(), lnNote.getPosition(), playArea.transform);
                            lnObj.transform.SetParent(playArea.transform, false);
                            lnNote.setNote(lnObj);
                            lnNote.setReleased(true);

                            // 끝노트
                            Note endNote = lnlist[i].getEnd();
                            GameObject endObj = generator.AddNewNote(endNote.getLane(), endNote.getPosition(), playArea.transform);
                            endObj.transform.SetParent(playArea.transform, false);
                            endNote.setNote(endObj); // 끝노트
                            endNote.setReleased(true);

                            lnlist[i].setEnd(endNote);
                            lnlist[i].setEndPos(endNote.getPosition());
                            lnlist[i].getMiddle().setPosition(
                                (lnlist[i].getStart().getPosition() + endNote.getPosition()) / 2
                            );
                            //lnadd[cline] = false;
                        }
                    }

                    

                    //lnlist.Add(new Longnote(cline, note, note.getPosition(), lnNote));
                    // 롱노트 길이 표기용 노트는 L# 이라는 WAV 명을 표기하고 노트 타입도 LONGNOTE
                    // (단 롱노트의 시작과 끝은 일반 Note이다)
                    //lnadd[cline] = true;
                //}
                //}
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
            Loading.StartLoading("PlayScreen");
        }

        public void ExitGame()
        {
            Loading.StartLoading("MusicSelect");
        }
    }
}
