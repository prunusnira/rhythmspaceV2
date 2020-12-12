using BMSCore;
using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BMSPlayer {
	// 게임 전체를 컨트롤하는 클래스
	/**
	 작업 내용
	 1. scene을 불러올 때 NoteGenerator에서 BMS의 정보를 읽고 노트 데이터를 생성함
	 2. 읽은 노트 데이터를 Scroller로 보냄 (Scroller는 노트 스크롤만 담당)
	 3. 버튼 동작은 여기에서 처리
	 */
	public class PlayController : MonoBehaviour {
        // UI Related
		private PlayData Data;
		private PlayUI UI;
        private HPBarUI HPUI;
        private JudgeUIProcess JudgeUI;
        private BGAControl BGAControl;
        private Graph Graph;
        private CoverLiftControl Cover;
        private StageFailed stageFailed;
        private StageCleared stageCleared;

        // Pause Menu: 편의성을 위해 컨트롤러에서 관리
        public GameObject layerPauseMenu;
        public Button btnRestart;
        public Button btnRestartSame;
        public Button btnExit;

        // Menu btn
        public Sprite normalBtn;
        public Sprite selectBtn;

        // Data Related
        private NoteGenerator generator;
        private Scroller scroller;
        private LNObjConverter LNConverter;
        private HPController hpController;
        private BMSAnalyzer analyzer;
        private Randomizer randomizer;

        // Scene 관리
        private bool gameOverTriggered = false;
        private bool isPaused = false;
        private bool isGameOver = false;
        private bool isBGAMovieExist = false;
        private bool isBMSReady = false;
        private bool isLoadingPhase1 = false;
        private bool isLoadingPhase1Finish = false;
        private bool isLoadingPhase2Ready = false;
        private bool isKeyInfo = true;
        private bool isInfoDisplay = false;
        private bool isPositionFixed = false;

        // Data in thread
        private int encoding = 932;
        private Thread loadingThread1;
        private Thread loadingThread2;

        // 시간 관리
        private double StartTime = 0;
        private double PrevTickTime = 0;
        private double PlayTimePassed = 0;
        private double DeltaTempSum = 0; // 그래프 기록용
        private double PauseStartTime = 0;

        private int pauseSel = 0;

        private bool firstrun = false;
        private bool isPlayAuto;

        private int[] noteLayout;

		void Awake() {
            // Note Generator에서 노트 데이터를 읽어들이고 Scroller에 보내는 과정 까지를 Awake에서 실행
            // Start에서 실행 할 경우 프레임 내에서 작업하게 되므로 좋지 않음
            Keys.LoadButtonSetting();

            // UI 가져오기
            UI = GetComponent<PlayUI>();
            HPUI = GetComponent<HPBarUI>();
            BGAControl = GetComponent<BGAControl>();
            Graph = GetComponent<Graph>();
            Cover = GetComponent<CoverLiftControl>();
            JudgeUI = GetComponent<JudgeUIProcess>();
            stageFailed = GetComponent<StageFailed>();
            stageCleared = GetComponent<StageCleared>();

            encoding = Const.Encoding;

            // HP Controller
            hpController = HPController.Instance;

            // 배속, 라인 수, BMS 로드 등의 기본 데이터 가져오기를 생성자에서 수행
            Data = new PlayData(Const.PlayingBMSPath);

            // 배치 변경 관련 작업
            randomizer = new Randomizer();
            noteLayout = randomizer.GetNoteLayout();

            // BMS 파일 분석
            analyzer = new BMSAnalyzer();

            // 분석된 데이터를 기반으로 스크롤러 준비
            scroller = GetComponent<Scroller>();
            scroller.Init();

            // 분석된 데이터를 바탕으로 노트를 생성
            generator = new NoteGenerator();

            if (Const.Auto == AutoPlayType.ALL)
                isPlayAuto = true;
            else
                isPlayAuto = false;

            // 모든 노트를 한꺼번에 뿌림
            bool[] lnadd = new bool[8];

            for (int i = 0; i < 8; i++)
            {
                lnadd[i] = false;
            }

            // 일시정지 메뉴
            btnRestart.gameObject.GetComponent<Image>().sprite = selectBtn;
            btnRestartSame.gameObject.GetComponent<Image>().sprite = normalBtn;
            btnExit.gameObject.GetComponent<Image>().sprite = normalBtn;

            btnRestart.GetComponentInChildren<Text>().text = Const.ResultTxtRetry[(int)Const.Language];
            btnRestartSame.GetComponentInChildren<Text>().text = Const.ResultTxtRetrySame[(int)Const.Language];
            btnExit.GetComponentInChildren<Text>().text = Const.ResultTxtNext[(int)Const.Language];

            btnRestart.onClick.AddListener(delegate
            {
                Const.ChangeLayout = true;
                RestartGame();
            });
            btnRestartSame.onClick.AddListener(delegate
            {
                Const.ChangeLayout = true;
                RestartGame();
            });
            btnExit.onClick.AddListener(delegate
            {
                double currentTick = Convert.ToDouble(DateTime.Now.Ticks) / 1000000;
                StartTime += currentTick - PauseStartTime;
                Const.Clear = ClearType.FAIL;
                isGameOver = true;
                isPaused = false;
                HidePauseMenu();
            });
        }

        private void Start()
        {
            // Stage 정보 넣기
            UI.SetStageInfo(
                Const.selectedOnList.Info.Gerne,
                Const.selectedOnList.Info.Title,
                Const.selectedOnList.Info.SubTitle,
                Const.selectedOnList.Info.Artist,
                Const.selectedOnList.Info.SubArtist
            );

            // 저장된 커버 이동 수치만큼 이동
            Cover.CoverHiddenUp();
            Cover.CoverHiddenDown();
            Cover.CoverSuddenDown();
            Cover.CoverSuddenUp();
            Cover.CoverLiftUp();
            Cover.CoverLiftDown();
        }

        private void Update ()
        {
            try
            {
                // 플레이 키 (최 우선 체크)
                if(!isGameOver)
                {
                    scroller.ButtonPushState();
                    scroller.Beam();
                }

                // 게임 플레이와 독립적으로 조절 가능해야 하는 부분들
                // 스피드 조절
                if (Input.GetKeyDown(KeyCode.Alpha1) ||
                    (Keys.GetBtn(9) && Keys.GetBtnDown(1)))
                {
                    if (Const.SpdType == SpdType.STANDARD)
                    {
                        scroller.SpeedDownFixed();
                    }
                    if (Const.SpdType == SpdType.CONSTANT)
                    {
                        scroller.SpeedDownFluid();
                    }
                }
                if (Input.GetKeyDown(KeyCode.Alpha2) ||
                    (Keys.GetBtn(9) && Keys.GetBtnDown(2)))
                {
                    if (Const.SpdType == SpdType.STANDARD)
                    {
                        scroller.SpeedUpFixed();
                    }
                    if (Const.SpdType == SpdType.CONSTANT)
                    {
                        scroller.SpeedUpFluid();
                    }
                }

                // 셔터 조정
                // SUD down
                if (Input.GetKey(KeyCode.Alpha3) ||
                    (Keys.GetBtn(9) && Keys.GetBtn(3)))
                {
                    Cover.CoverSuddenDown();
                }
                // SUD up
                if (Input.GetKey(KeyCode.Alpha4) ||
                    (Keys.GetBtn(9) && Keys.GetBtn(4)))
                {
                    Cover.CoverSuddenUp();
                }

                // LIFT down
                if (Input.GetKey(KeyCode.Alpha5) ||
                    (Keys.GetBtn(9) && Keys.GetBtn(5)))
                {
                    Cover.CoverLiftDown();
                }
                // LIFT up
                if (Input.GetKey(KeyCode.Alpha6) ||
                    (Keys.GetBtn(9) && Keys.GetBtn(6)))
                {
                    Cover.CoverLiftUp();
                }

                // HID down
                if (Input.GetKey(KeyCode.Alpha7) ||
                    (Keys.GetBtn(9) && Keys.GetBtn(7)))
                {
                    Cover.CoverHiddenDown();
                }
                // HID up
                if (Input.GetKey(KeyCode.Alpha8) ||
                    (Keys.GetBtn(9) && Keys.GetBtn(10)))
                {
                    Cover.CoverHiddenUp();
                }

                // 데이터 로딩
                if (!isLoadingPhase1)
                {
                    isLoadingPhase1 = true;
                    loadingThread1 = new Thread(new ThreadStart(LoadBMSPhase1));
                    loadingThread1.Start();
                    return;
                }
                else if(isLoadingPhase1Finish)
                {
                    isLoadingPhase1Finish = false;
                    
                    // 사운드 정의(Unity)
                    //soundController.PreloadSound(Data.BMS);

                    if (Data.BMS.BGAPaths.Count > 0)
                    {
                        foreach(string k in Data.BMS.BGAPaths.Keys)
                        {
                            Data.BMS.BGAImages.Add(k, Tools.createSpriteFromFile(Data.BMS.BGAPaths[k]));
                        }
                    }
                    isLoadingPhase2Ready = true;
                    return;
                }
                else if(isLoadingPhase2Ready)
                {
                    isLoadingPhase2Ready = false;
                    loadingThread2 = new Thread(new ThreadStart(LoadBMSPhase2));
                    loadingThread2.Start();
                    return;
                }

                if(isInfoDisplay)
                {
                    // 곡 정보 출력
                    UI.SetMusicInfo(Data.BMS);

                    // 기어 BPM 표시
                    UI.SetGearBPM(
                        Data.BMS.BPMStart,
                        Data.BMS.BPMMin,
                        Data.BMS.BPMMax
                    );
                    isInfoDisplay = false;
                    
                    // BGA 준비
                    if (analyzer.IsVideoExist())
                    {
                        isBGAMovieExist = true;
                        if(Data.BMS.BGAVideoFile.EndsWith(".wmv") || Data.BMS.BGAVideoFile.EndsWith(".avi"))
                        {
                            BGAControl.BGAVideoActivateNM();
                            BGAControl.BGAVideoPreloadNM(Data.BMS.BGAVideoFile);
                            BGAControl.SetVLCNeed(false);
                        }
                        else
                        {
                            BGAControl.BGAVideoActivateVLC();
                            BGAControl.BGAVideoPreloadVLC(Data.BMS.BGAVideoFile);
                            BGAControl.SetVLCNeed(true);
                        }
                    }
                    else
                    {
                        BGAControl.BGAImageActivate();
                    }

                    if (Data.BMS.LayerNote.Count > 0)
                    {
                        BGAControl.LayerImageActivate();
                    }

                    UI.UpdateSpeed();
                }

                if(isPositionFixed)
                {
                    if (Const.GraphType != GraphType.OFFBGA
                        && Const.GraphType != GraphType.OFFGEAR)
                        Graph.SetInitialGraph(Data.TotalNotes);

                    scroller.PlaySetup(Data.TotalNotes, Data.BMS.Rank);

                    UI.UpdateTimerTotal(Data.LastTiming);
                    HPUI.UpdateHP(hpController.CurrentHP);

                    isBMSReady = true;
                    UI.DeactiveLoading();
                    isPositionFixed = false;
                }

                if (!isBMSReady)
                {
                    return;
                }

                if (firstrun && !isPaused)
                {
                    double currentTick = Convert.ToDouble(DateTime.Now.Ticks) / 1000000;
                    PlayTimePassed = currentTick - StartTime;
                    double DeltaTime = PlayTimePassed - PrevTickTime;

                    double bpm = Data.CurrentBPM;
                    double bps = Data.BPS;

                    // 1틱 동안 노트가 움직이는 거리 (시간 * 속도)

                    // BPM이 변경되는 곡 때문에 총 플레이 시간과 bps만으로
                    // 노트위치를 계산하는 것은 불가능함
                    if (Data.Stop > 0)
                    {
                        Data.Stop -= DeltaTime;

                        if (Data.Stop <= 0)
                        {
                            Data.Stop = 0;
                            Data.IsStopOn = false;
                        }
                    }

                    if(!isGameOver)
                    {
                        scroller.MoveNotes(Data, PlayTimePassed, ref bps);
                        scroller.MoveMine(Data, PlayTimePassed, bps);
                        scroller.MoveSplitLine(Data, PlayTimePassed, bps);
                    }

                    if(Const.BGAOnOff == 1)
                    {
                        scroller.PlayBGA(Data.NoteBGA, Data.BMS, PlayTimePassed);
                        scroller.PlayLayer(Data.NoteLayer, Data.BMS, PlayTimePassed);
                    }
                    scroller.PlayBGM(Data.NoteBGM, Data.BMS, PlayTimePassed);
                    scroller.PlayBPM(Data, PlayTimePassed, ref bpm, ref bps);
                    scroller.PlayStop(Data, PlayTimePassed, bps);

                    if (isPlayAuto)
                    {
                        scroller.AutoPlay(Data.NotePlay, Data.NoteLong, Data.BMS,
                            PlayTimePassed, ref bps);
                    }
                    else
                    {
                        scroller.Play(Data.NotePlay, Data.NoteLong, Data.BMS,
                            PlayTimePassed, ref bpm, ref bps);
                    }

                    Data.CurrentBPM = bpm;
                    Data.BPS = bps;

                    // 게임 종료 시 처리
                    // Type 1: 게이지가 0이 되었을 때
                    if (!isGameOver && hpController.IsHpMin())
                    {
                        if (GameOverCheck())
                        {
                            Const.Clear = ClearType.FAIL;
                            isGameOver = true;
                        }
                    }
                    // Type 2: 노트가 다 사용되었을 때
                    if (!isGameOver && scroller.GetProcessedNotes() >= Data.TotalNotes)
                    {
                        stageCleared.ShowClearScoreStat(
                            scroller.GetScore(), Data.TotalNotes);

                        // 풀콤/퍼펙여부 확인 후 화면에 표시
                        if(scroller.GetBreak() == 0)
                        {
                            if(scroller.GetGood() == 0)
                            {
                                // 퍼펙트
                                JudgeUI.ShowFCPFMark(0);
                            }
                            else
                            {
                                // 풀콤보
                                JudgeUI.ShowFCPFMark(1);
                            }
                        }

                        // 게이지 타입과 퍼센트에 따라 클리어 유무 결정
                        switch (Const.GaugeType)
                        {
                            case GaugeType.ASSISTED:
                                if (hpController.CurrentHP >= 6000)
                                {
                                    Const.Clear = ClearType.ASSISTCLEAR;
                                }
                                else
                                {
                                    Const.Clear = ClearType.FAIL;
                                }
                                break;
                            case GaugeType.EASY:
                                if (hpController.CurrentHP >= 8000)
                                {
                                    Const.Clear = ClearType.EASYCLEAR;
                                }
                                else
                                {
                                    Const.Clear = ClearType.FAIL;
                                }
                                break;
                            case GaugeType.NORMAL:
                                if (hpController.CurrentHP >= 8000)
                                {
                                    Const.Clear = ClearType.NORMALCLEAR;
                                }
                                else
                                {
                                    Const.Clear = ClearType.FAIL;
                                }
                                break;
                            case GaugeType.HARD:
                                Const.Clear = ClearType.HARDCLEAR;
                                break;
                            case GaugeType.EXHARD:
                                Const.Clear = ClearType.EXCLEAR;
                                break;
                        }

                        bool isPlayingMusic = SoundControllerFMOD.Instance.CheckSoundPlaying();
                        bool isPlayingBGA = false;

                        if (isBGAMovieExist)
                        {
                            if (BGAControl.IsVLCNeeded())
                            {
                                isPlayingBGA = BGAControl.isBGAPlayingVLC();
                            }
                            else
                            {
                                isPlayingBGA = BGAControl.isBGAPlayingNM();
                            }
                        }

                        if (!isPlayingMusic && !isPlayingBGA)
                        {
                            isGameOver = true;
                        }
                    }
                    // 게임오버 처리
                    if (isGameOver && !gameOverTriggered)
                    {
                        if (Const.Clear == ClearType.FAIL && scroller.GetProcessedNotes() < Data.TotalNotes)
                        {
                            // 스테이지 페일 사운드 및 효과 재생
                            UI.RemovePanels();
                            stageFailed.ShowGameOver();
                        }
                        // 게임오버로 가기 전에 페이드
                        StartCoroutine("GameOver");
                    }
                    else if (isGameOver && gameOverTriggered && UI.GetFadeDone())
                    {
                        Debug.Log("GAMEOVER");
                        // 재생중인 모든 음악 종료
                        SoundControllerFMOD.Instance.StopAll();

                        SoundControllerFMOD.Instance.FreeMemory(Data.BMS);

                        Const.ResultGraph = Data.HPGraph;

                        // 결과 데이터 수집 후 result로 넘기기
                        scroller.GetResultData(Data.TotalNotes);

                        // 결과창으로 이동
                        SceneManager.LoadScene("Result");
                    }

                    UI.UpdateTimerCur(PlayTimePassed, Data.LastTiming);

                    PrevTickTime = PlayTimePassed;

                    if (isKeyInfo && PlayTimePassed >= 50)
                    {
                        isKeyInfo = false;
                        UI.RemoveKeyInfo();
                    }

                    // 1초에 한 번 hp 값을 기록
                    DeltaTempSum += DeltaTime;
                    if (DeltaTempSum >= 10)
                    {
                        Data.HPGraph.Add(hpController.CurrentHP);
                        DeltaTempSum = 0;
                    }
                }
                else if (firstrun && isPaused)
                {
                    // 위 아래 버튼 이동시 메뉴 변경
                    if (Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        PauseMenuMove(ref pauseSel, false);
                    }
                    if (Input.GetKeyDown(KeyCode.DownArrow))
                    {
                        PauseMenuMove(ref pauseSel, true);
                    }
                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        switch(pauseSel)
                        {
                            case 0:
                                Const.ChangeLayout = true;
                                RestartGame();
                                break;
                            case 1:
                                Const.ChangeLayout = false;
                                RestartGame();
                                break;
                            case 2:
                                double currentTick = Convert.ToDouble(DateTime.Now.Ticks) / 1000000;
                                StartTime += currentTick - PauseStartTime;
                                Const.Clear = ClearType.FAIL;
                                isGameOver = true;
                                isPaused = false;
                                HidePauseMenu();
                                break;
                        }
                    }
                }
                else
                {
                    firstrun = true;
                    StartTime = Convert.ToDouble(DateTime.Now.Ticks) / 1000000;
                    PrevTickTime = 0;
                }

                // 일시정지 메뉴 소환
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if(scroller.GetProcessedNotes() == Data.TotalNotes)
                    {
                        // 바로 리절트로 이동
                        isGameOver = true;
                        return;
                    }

                    isPaused = !isPaused;
                    double currentTick = Convert.ToDouble(DateTime.Now.Ticks) / 1000000;

                    if (isPaused)
                    {
                        PauseStartTime = currentTick;
                        SoundControllerFMOD.Instance.PauseAll();
                        if (Data.BMS.BGAVideoFile != null)
                        {
                            if (BGAControl.IsVLCNeeded())
                            {
                                BGAControl.PauseBGAVideoVLC();
                            }
                            else
                            {
                                BGAControl.PauseBGAVideoNM();
                            }
                        }
                        ShowPauseMenu();
                    }
                    else
                    {
                        StartTime += currentTick - PauseStartTime;
                        SoundControllerFMOD.Instance.ResumeAll();
                        if (Data.BMS.BGAVideoFile != null)
                        {
                            if (BGAControl.IsVLCNeeded())
                            {
                                BGAControl.ResumeBGAVideoVLC();
                            }
                            else
                            {
                                BGAControl.ResumeBGAVideoNM();
                            }
                        }
                        HidePauseMenu();
                    }
                }
            }
            catch (Exception e)
            {
                ErrorHandler.LogError(e.Message + " " + e.StackTrace);
            }
        }

        private void LoadBMSPhase1()
        {
            try
            {
                if (Const.selectedOnList != null)
                {
                    scroller.UpdateBPM(Const.selectedOnList.Info.BPMstart);
                    scroller.RecalcSpd();
                }

                analyzer.FullAnalyzer(Data.BMS, encoding);

                // LNOBJ 타입이면 추가로 이식하는 작업을 수행함
                if (Data.BMS.LNType == LNType.Obj)
                {
                    LNConverter = new LNObjConverter();
                    LNConverter.FixLongNoteLNOBJ(Data.BMS);
                }

                // 파일 분석 이후 기본 정보 분석
                Data.CurrentBPM = Data.BMS.BPMStart;
                Data.BPS = Data.CurrentBPM / 240;
                Data.SPB = (double)(240 * 1000) / Data.CurrentBPM;
                scroller.UpdateBPM(Data.CurrentBPM);
                scroller.RecalcSpd();
                // BPM = 분당 비트수, 1분에 1/4박자(bar 1개)의 개수
                // beat per second는 bpm/60, 여기에 4 bar = 1박이므로 4로 추가로 나눈다
                // 모든 시간은 ms 단위로 한다

                isInfoDisplay = true;

                // 사운드 정의(FMOD)
                SoundControllerFMOD.Instance.PreloadSound(Data.BMS);

                isLoadingPhase1Finish = true;
            }
            catch (Exception e)
            {
                ErrorHandler.LogError(e.Message + " " + e.StackTrace);
            }
        }

        private void LoadBMSPhase2()
        {
            try
            {
                // 노트 오브젝트(물리x) 생성
                // GetNoteLayout == null이면 SRAN
                generator.AnalyzeNotes(Data, noteLayout);
                generator.PositionToTiming(Data);
                generator.SortAllNotes(Data);

                isPositionFixed = true;
            }
            catch (Exception e)
            {
                ErrorHandler.LogError(e.Message + " " + e.StackTrace);
            }
        }

        IEnumerator GameOver()
        {
            UI.SetFade();
            gameOverTriggered = true;
            yield return null;
        }

        private bool GameOverCheck()
        {
            switch (Const.GaugeType)
            {
                case GaugeType.ASSISTED:
                case GaugeType.EASY:
                case GaugeType.NORMAL:
                    return false;
                case GaugeType.HARD:
                case GaugeType.EXHARD:
                    return true;
                default:
                    return false;
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
            if (pauseSel == 0)
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
            else if (pauseSel == 1)
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
            switch (newsel)
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

        public void RestartGame()
        {
            SoundControllerFMOD.Instance.StopAll();
            SoundControllerFMOD.Instance.FreeMemory(Data.BMS);
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene("PlayScreen");
        }
    }
}
