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
        public GameObject startLine; // 노트가 생성되는 위치

        // Scene 관리
        private bool gameOverTriggered = false;
        private double totalLen = 0; // Bar만 따진것이 아닌 가변 마디를 고려한 이전 마디까지의 길이
        private bool isPaused = false;
        private bool isGameOver = false;
        private bool isBGAMovieExist = false;

        // 시간 관리
        private double StartTime = 0;
        private double PrevTickTime = 0;
        private double PlayTimePassed = 0;

        public double BarLengthPrevbar = 0;

        // Audio Management
        private ISoundController soundController;
        /*
        // Unity Audio
        private AudioSource[] unityAudio = new AudioSource[Const.CHANNEL];

		// FMOD 관리
		private FMOD.ChannelGroup channelGroup;
        private FMOD.Channel[] fmodChannel = new FMOD.Channel[Const.CHANNEL];*/

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

            // BMS 파일 분석
            BMSAnalyzer analyzer = new BMSAnalyzer();
            analyzer.FullAnalyzer(Data.BMS);

            // LNOBJ 타입이면 추가로 이식하는 작업을 수행함
            if(Data.BMS.LNType == LNType.Obj)
            {
                LNConverter = new LNObjConverter();
                LNConverter.FixLongNoteLNOBJ(Data.BMS);
            }

            // 파일 분석 이후 기본 정보 분석
            Data.CurrentBPM = Data.BMS.BPMStart;
            Data.BPS = Data.CurrentBPM / 240;
            Data.SPB = (double)(240 * 1000) / Data.CurrentBPM;
            // BPM = 분당 비트수, 1분에 1/4박자(bar 1개)의 개수
            // beat per second는 bpm/60, 여기에 4 bar = 1박이므로 4로 추가로 나눈다
            // 모든 시간은 ms 단위로 한다

            // 곡 정보 출력
            UI.SetMusicInfo(Data.BMS.Title);

            // 기어 BPM 표시
            UI.SetGearBPM(
                Data.BMS.BPMStart,
                Data.BMS.BPMMin,
                Data.BMS.BPMMax
            );

            if(analyzer.IsVideoExist())
            {
                isBGAMovieExist = true;
                UI.BGAVideoActivate();
                UI.BGAVideoSetting(Data.BMS.BGAVideoFile);
            }

            // 분석된 데이터를 바탕으로 노트를 생성
            generator = GetComponent<NoteGenerator>();

            // 사운드 컨트롤러 정의
            /*if(Const.GetAudio() == 0)
            {
                soundController = GetComponent<SoundControllerUnity>();
            }
            else
            {*/
                soundController = GetComponent<SoundControllerFMOD>();
            //}

            // 사운드 정의
            soundController.Initialize();
            soundController.InitSoundChannels();
            soundController.PreloadSound(Data.BMS);

			generator.AnalyzeNotes(Data);
            generator.PositionToTiming(Data);
            generator.SortAllNotes(Data);

            Debug.Log("File Loaded: " + Data.BMS.FilePath);
            Debug.Log("Music: " + Data.BMS.Title);
            Debug.Log("Artist: " + Data.BMS.Artist);
            Debug.Log("BPM: " + Data.BMS.BPMStart);
            Debug.Log("Total: " + Data.TotalNotes);

            UI.UpdateSpeed();

            // 분석된 데이터를 기반으로 스크롤러 준비
            scroller = GetComponent<Scroller>();
            scroller.Init(Data.TotalNotes);

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

        private void Update ()
        {
            // 플레이 키 (최 우선 체크)
            scroller.ProcessBeamOnTouch();

            if (firstrun && !isPaused)
            {
                double currentTick = Convert.ToDouble(DateTime.Now.Ticks) / 1000000;
                PlayTimePassed = currentTick - StartTime;
                double DeltaTime = PlayTimePassed - PrevTickTime;

                double bpm = Data.CurrentBPM;
                double bps = Data.BPS;

                // 1틱 동안 노트가 움직이는 거리 (시간 * 속도)

                // 변속곡 때문에 총 플레이 시간과 bps만으로 계산하는 것은 불가능함
                if(Data.Stop > 0)
                {
                    Debug.Log("TIME: " + PlayTimePassed + " / Stop: " + Data.Stop + " (-" + (DeltaTime)*10+")");
                    Data.Stop -= DeltaTime;
                    if (Data.Stop <= 0)
                    {
                        Data.Stop = 0;
                    }
                }

                // Stop 시간이 0이 되었을 때에는 이동 가능?
                if(Data.Stop == 0)
                {
                    scroller.MoveNotes(Data.NotePlay, Data.NoteLong, PlayTimePassed, DeltaTime, ref bps);
                    scroller.MoveMine(Data.NoteMine, DeltaTime, bps);
                }

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

                scroller.PlayBGA(Data.NoteBGA, Data.BMS, PlayTimePassed);
                scroller.PlayBGM(Data.NoteBGM, Data.BMS, PlayTimePassed);
                scroller.PlayBPM(Data, PlayTimePassed, ref bpm, ref bps);
                scroller.PlayStop(Data, PlayTimePassed, bps);

                Data.CurrentBPM = bpm;
                Data.BPS = bps;

                if(Data.IsBPMChanged)
                {
                    UI.FixAllNotePositionOnScreen(Data, Data.BPMNum - 1);
                    Data.IsBPMChanged = false;
                }

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
                    Const.Clear = ClearType.CLEAR;
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
                else if (isGameOver && gameOverTriggered)
                {
                    Debug.Log("GAMEOVER");
                    // 재생중인 모든 음악 종료
                    soundController.StopAll();

                    // 결과 데이터 수집 후 result로 넘기기
                    scroller.GetResultData(Data.TotalNotes);

                    // 결과창으로 이동
                    Loading.StartLoading("Result");
                }

                PrevTickTime = PlayTimePassed;
            }
            else if(firstrun && isPaused)
            {
                // 위 아래 버튼 이동시 메뉴 변경
                if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
                {
                    UI.PauseMenuMove();
                }
                if(Input.GetKeyDown(KeyCode.Return))
                {
                    UI.PauseMenuExec();
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
