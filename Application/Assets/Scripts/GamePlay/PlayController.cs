using BMSCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BMSPlayer {
	// 게임 전체를 컨트롤하는 클래스
	/**
	 작업 내용
	 1. scene을 불러올 때 NoteGenerator에서 BMS의 정보를 읽고 노트 데이터를 생성함
	 2. 읽은 노트 데이터를 Scroller로 보냄 (Scroller는 노트 스크롤만 담당)
	 3. 버튼 동작은 여기에서 처리
	 */
	public class PlayController : MonoBehaviour {
		private PlayData Data;
		private PlayUI UI;
		private NoteGenerator generator;
        private Scroller scroller;
        private LNObjConverter LNConverter;
        private HPController hpController;
        private BMSAnalyzer analyzer;
        private Randomizer randomizer;

        public GameObject startLine; // 노트가 생성되는 위치

        // Scene 관리
        private bool gameOverTriggered = false;
        private bool isPaused = false;
        private bool isGameOver = false;
        private bool isBGAMovieExist = false;
        private bool isBMSReady = false;
        private bool isLoading = false;
        private bool isKeyInfo = true;

        // 시간 관리
        private double StartTime = 0;
        private double PrevTickTime = 0;
        private double PlayTimePassed = 0;

        public double BarLengthPrevbar = 0;
        private int pauseSel = 0;

        // Audio Management
        private ISoundController soundController;

        private bool firstrun = false;
        public bool isPlayAuto;

		void Awake() {
            // Note Generator에서 노트 데이터를 읽어들이고 Scroller에 보내는 과정 까지를 Awake에서 실행
            // Start에서 실행 할 경우 프레임 내에서 작업하게 되므로 좋지 않음

            // UI 가져오기
            UI = GetComponent<PlayUI>();

            // HP Controller
            hpController = GetComponent<HPController>();

            // 배속, 라인 수, BMS 로드 등의 기본 데이터 가져오기를 생성자에서 수행
            Data = new PlayData();

            // 배치 변경 관련 작업
            randomizer = new Randomizer();

            // BMS 파일 분석
            analyzer = new BMSAnalyzer();

            // 분석된 데이터를 기반으로 스크롤러 준비
            scroller = GetComponent<Scroller>();

            // 분석된 데이터를 바탕으로 노트를 생성
            generator = GetComponent<NoteGenerator>();

            // 사운드 컨트롤러 초기화
            soundController = GetComponent<SoundControllerFMOD>();

            soundController.Initialize();
            soundController.InitSoundChannels();

            if (Const.Auto == AutoPlayType.ON)
                isPlayAuto = true;
            else
                isPlayAuto = false;

            // 모든 노트를 한꺼번에 뿌림
            bool[] lnadd = new bool[8];

            for (int i = 0; i < 8; i++)
            {
                lnadd[i] = false;
            }
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
            UI.CoverHiddenUp();
            UI.CoverHiddenDown();
            UI.CoverSuddenDown();
            UI.CoverSuddenUp();
            UI.CoverLiftUp();
            UI.CoverLiftDown();
        }

        private void Update ()
        {
            // 데이터 로딩
            if(!isLoading)
            {
                StartCoroutine("LoadBMS");
                return;
            }

            if(!isBMSReady)
            {
                return;
            }

            // 플레이 키 (최 우선 체크)
            scroller.ButtonPushState();

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

                scroller.MoveNotes(Data, PlayTimePassed, ref bps);
                scroller.MoveMine(Data, PlayTimePassed, bps);

                scroller.PlayBGA(Data.NoteBGA, Data.BMS, PlayTimePassed);
                scroller.PlayBGM(Data.NoteBGM, Data.BMS, PlayTimePassed);
                scroller.PlayBPM(Data, PlayTimePassed, ref bpm, ref bps);
                scroller.PlayStop(Data, PlayTimePassed, bps);

                scroller.SpeedChangeAndBeam(bpm);

                if (isPlayAuto)
                {
                    scroller.AutoPlay(Data.NotePlay, Data.NoteLong, Data.BMS,
                        PlayTimePassed, ref bps, Data.TotalNotes);
                }
                else
                {
                    scroller.Play(Data.NotePlay, Data.NoteLong, Data.BMS,
                        PlayTimePassed,
                        ref bpm, ref bps, Data.TotalNotes);
                }

                Data.CurrentBPM = bpm;
                Data.BPS = bps;

                // 게임 종료 시 처리
                // Type 1: 게이지가 0이 되었을 때
                if(hpController.IsHpMin())
                {
                    if (GameOverCheck())
                    {
                        Const.Clear = ClearType.FAIL;
                        isGameOver = true;
                    }
                }
                // Type 2: 노트가 다 사용되었을 때
                if (scroller.GetProcessedNotes() >= Data.TotalNotes)
                {
                    // 게이지 타입과 퍼센트에 따라 클리어 유무 결정
                    switch(Const.JudgeType)
                    {
                        case JudgeType.ASSISTED:
                            if(hpController.CurrentHP >= 6000)
                            {
                                Const.Clear = ClearType.ASSISTCLEAR;
                            }
                            else
                            {
                                Const.Clear = ClearType.FAIL;
                            }
                            break;
                        case JudgeType.EASY:
                            if (hpController.CurrentHP >= 8000)
                            {
                                Const.Clear = ClearType.EASYCLEAR;
                            }
                            else
                            {
                                Const.Clear = ClearType.FAIL;
                            }
                            break;
                        case JudgeType.NORMAL:
                            if (hpController.CurrentHP >= 8000)
                            {
                                Const.Clear = ClearType.NORMALCLEAR;
                            }
                            else
                            {
                                Const.Clear = ClearType.FAIL;
                            }
                            break;
                        case JudgeType.HARD:
                            Const.Clear = ClearType.HARDCLEAR;
                            break;
                        case JudgeType.EXHARD:
                            Const.Clear = ClearType.EXCLEAR;
                            break;
                    }

                    bool isPlayingMusic = soundController.CheckSoundPlaying();
                    bool isPlayingBGA = false;

                    if(isBGAMovieExist)
                    {
                        isPlayingBGA = UI.isBGAPlaying();
                    }

                    if (!isPlayingMusic && !isPlayingBGA)
                    {
                        isGameOver = true;
                    }
                }
                // 게임오버 처리
                if (isGameOver && !gameOverTriggered)
                {
                    // 게임오버로 가기 전에 페이드
                    StartCoroutine("GameOver");
                }
                else if (isGameOver && gameOverTriggered && UI.GetFadeDone())
                {
                    Debug.Log("GAMEOVER");
                    // 재생중인 모든 음악 종료
                    soundController.StopAll();

                    soundController.FreeMemory(Data.BMS);

                    // 결과 데이터 수집 후 result로 넘기기
                    scroller.GetResultData(Data.TotalNotes);

                    // 결과창으로 이동
                    SceneManager.LoadScene("Result");
                }

                UI.UpdateTimerCur(PlayTimePassed);

                PrevTickTime = PlayTimePassed;

                if (isKeyInfo && PlayTimePassed > 50)
                {
                    isKeyInfo = false;
                    UI.RemoveKeyInfo();
                }
            }
            else if(firstrun && isPaused)
            {
                // 위 아래 버튼 이동시 메뉴 변경
                if(Input.GetKeyDown(KeyCode.UpArrow))
                {
                    UI.PauseMenuMove(ref pauseSel, true);
                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    UI.PauseMenuMove(ref pauseSel, false);
                }
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    bool isEnd = UI.PauseMenuExec(pauseSel);
                    if(isEnd)
                    {
                        Const.Clear = ClearType.FAIL;
                        isGameOver = true;
                        isPaused = false;
                        UI.HidePauseMenu();
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
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                isPaused = !isPaused;

                if(isPaused)
                {
                    UI.ShowPauseMenu();
                }
                else
                {
                    UI.HidePauseMenu();
                }
            }

            // 셔터 조정
            // SUD down
            if(Input.GetKey(KeyCode.Alpha3))
            {
                UI.CoverSuddenDown();
            }
            // SUD up
            else if (Input.GetKey(KeyCode.Alpha4))
            {
                UI.CoverSuddenUp();
            }

            // LIFT down
            if (Input.GetKey(KeyCode.Alpha5))
            {
                UI.CoverLiftDown();
            }
            // LIFT up
            else if (Input.GetKey(KeyCode.Alpha6))
            {
                UI.CoverLiftUp();
            }

            // HID down
            if (Input.GetKey(KeyCode.Alpha7))
            {
                UI.CoverHiddenDown();
            }
            // HID up
            else if (Input.GetKey(KeyCode.Alpha8))
            {
                UI.CoverHiddenUp();
            }
        }

        IEnumerator LoadBMS()
        {
            isLoading = true;
            yield return null;

            analyzer.FullAnalyzer(Data.BMS);
            yield return null;

            // LNOBJ 타입이면 추가로 이식하는 작업을 수행함
            if (Data.BMS.LNType == LNType.Obj)
            {
                LNConverter = new LNObjConverter();
                LNConverter.FixLongNoteLNOBJ(Data.BMS);
            }
            yield return null;

            // 파일 분석 이후 기본 정보 분석
            Data.CurrentBPM = Data.BMS.BPMStart;
            Data.BPS = Data.CurrentBPM / 240;
            Data.SPB = (double)(240 * 1000) / Data.CurrentBPM;
            // BPM = 분당 비트수, 1분에 1/4박자(bar 1개)의 개수
            // beat per second는 bpm/60, 여기에 4 bar = 1박이므로 4로 추가로 나눈다
            // 모든 시간은 ms 단위로 한다
            yield return null;

            // 곡 정보 출력
            UI.SetMusicInfo(Data.BMS.Title);
            yield return null;

            // 기어 BPM 표시
            UI.SetGearBPM(
                Data.BMS.BPMStart,
                Data.BMS.BPMMin,
                Data.BMS.BPMMax
            );
            yield return null;

            if (analyzer.IsVideoExist())
            {
                isBGAMovieExist = true;
                UI.BGAVideoActivate();
            }
            else
            {
                UI.BGAImageActivate();
            }
            yield return null;

            // 사운드 정의
            soundController.PreloadSound(Data.BMS);
            yield return null;

            // 노트 오브젝트(물리x) 생성
            // GetNoteLayout == null이면 SRAN
            generator.AnalyzeNotes(Data, randomizer.GetNoteLayout());
            yield return null;
            generator.PositionToTiming(Data);
            yield return null;
            generator.SortAllNotes(Data);
            yield return null;

            UI.UpdateSpeed();
            yield return null;

            scroller.Init(Data.TotalNotes);
            yield return null;

            UI.UpdateTimerTotal(Data.LastTiming);
            UI.UpdateHP(hpController.CurrentHP);
            yield return null;

            isBMSReady = true;
            UI.DeactiveLoading();
            yield return null;
        }

        IEnumerator GameOver()
        {
            UI.SetFade();
            gameOverTriggered = true;
            yield return null;
        }

        private bool GameOverCheck()
        {
            switch (Const.JudgeType)
            {
                case JudgeType.ASSISTED:
                case JudgeType.EASY:
                case JudgeType.NORMAL:
                    return false;
                case JudgeType.HARD:
                case JudgeType.EXHARD:
                    return true;
                default:
                    return false;
            }
        }
    }
}
