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

        // Scene 관리
        private double musicStartTime;
        private double timePrevFrame;
        private bool switchResult;
        private double totalTime = 0;
        private bool gameOverTriggered = false;
        private int noteCount;
        private int totalNotes;
        private double totalLen = 0; // Bar만 따진것이 아닌 가변 마디를 고려한 이전 마디까지의 길이
        private bool isPaused = false;

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

            // 배속, 라인 수, BMS 로드 등의 기본 데이터 가져오기를 생성자에서 수행
            Data = new PlayData();

            // BMS 파일 분석
            BMSAnalyzer analyzer = new BMSAnalyzer();
            analyzer.FullAnalyzer(Data.BMS);

            // LNOBJ 타입이면 추가로 이식하는 작업을 수행함
            if(Data.BMS.getLNType() == LNType.Obj)
            {
                LNConverter = new LNObjConverter();
                LNConverter.FixLongNoteLNOBJ(Data.BMS);
            }

            // 파일 분석 이후 기본 정보 분석
            Data.CurrentBPM = Data.BMS.getBPMStart();
            Data.PrevBPM = Data.CurrentBPM;
            Data.BPS = Data.CurrentBPM / 240;
            Data.SPB = (double)(240 * 1000) / Data.CurrentBPM;
            // BPM = 분당 비트수, 1분에 1/4박자(bar 1개)의 개수
            // beat per second는 bpm/60, 여기에 4 bar = 1박이므로 4로 추가로 나눈다
            // 모든 시간은 ms 단위로 한다

            // 곡 정보 출력
            UI.SetMusicInfo(
                Data.BMS.getGerne(),
                Data.BMS.getTitle(),
                Data.BMS.getArtist()
            );

            // 기어 BPM 표시
            UI.SetGearBPM(
                Data.BMS.getBPMStart(),
                Data.BMS.getBPMMin(),
                Data.BMS.getBPMMax()
            );

            if(analyzer.IsVideoExist())
            {
                UI.BGAVideoActivate();
                UI.BGAVideoSetting(Data.BMS.bgaVideoFile);
            }

            // 분석된 데이터를 바탕으로 노트를 생성
            generator = GetComponent<NoteGenerator>();

            // 사운드 컨트롤러 정의
            if(Const.GetAudio() == 0)
            {
                soundController = GetComponent<SoundControllerUnity>();
            }
            else
            {
                soundController = GetComponent<SoundControllerFMOD>();
            }

            // 사운드 정의
            soundController.Initialize();
            soundController.InitSoundChannels();
            soundController.PreloadSound(Data.BMS);

			generator.AnalyzeNotes(Data.BMS, Data.Notes, Data.BMS.lastBar, Data.SPB,
                ref totalLen, ref noteCount, ref totalNotes, ref totalTime);

            Debug.Log("File Loaded: " + Data.BMS.getFilePath());
            Debug.Log("Music: " + Data.BMS.getTitle());
            Debug.Log("Artist: " + Data.BMS.getArtist());
            Debug.Log("BPM: " + Data.BMS.getBPMStart());
            Debug.Log("Total: " + totalNotes.ToString());

            UI.UpdateSpeed(Data.CurrentBPM);

            // 분석된 데이터를 기반으로 스크롤러 준비
            scroller = GetComponent<Scroller>();
            scroller.Init(noteCount);

            if (Const.GetAuto() == 1)
                isPlayAuto = true;
            else
                isPlayAuto = false;

            // 모든 노트를 한꺼번에 뿌림
            bool[] lnadd = new bool[8];

            for (int i = 0; i < 8; i++)
            {
                lnadd[i] = false;
            }

            // 라인 벗어나는 롱노트 END 삭제용
            //List<Note> remove = new List<Note>();

            // 일단 그냥 다 추가하고 롱노트만 따로 처리하면 될거 같은데
            /*foreach (Note n in Data.Notes)
            {
                if (n.getNotetype() == Note.NOTETYPE.PLAYABLE)
                {
                    //if (n.getLane() <= Const.GetPlayline())
                    //{
                        GameObject noteObj = generator.AddNewNote(n.getLane(), Data.PlayLine, n.getPosition(), UI.playArea.transform);
                        noteObj.transform.parent = UI.playArea.transform;
                        n.setNote(noteObj);
                    //}
                    //else
                    //{
                        // 플레이 라인을 벗어나는 경우 MUSIC 타입으로 변경
                    //    n.changeToMusic();
                    //}
                }
            }*/

            foreach (Note n in Data.Notes)
            {
                if (!n.isLong())
                {
                    if (n.getNotetype() == Note.NOTETYPE.PLAYABLE)
                    {
                        //if(n.getLane() <= Const.GetPlayline())
                        //{
                            GameObject noteObj = generator.AddNewNote(n.getLane(), Data.PlayLine, n.getPosition(), UI.playArea.transform);
                            noteObj.transform.parent = UI.playArea.transform;
                            n.setNote(noteObj);
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
                        int cline = n.getLane();
                        if (lnadd[cline])
                        {
                            // 이미 롱노트가 추가중인 상태이면 현재 라인의 lnlist를 갱신하고 노트 표시 추가
                            for (int i = 0; i < Data.LongNotes.Count; i++)
                            {
                                if (Data.LongNotes[i].getLane() == cline && Data.LongNotes[i].getEnd() == null)
                                {
                                    GameObject noteObj = generator.AddNewNote(n.getLane(), Data.PlayLine, n.getPosition(), UI.playArea.transform);
                                    noteObj.transform.parent = UI.playArea.transform;
                                    n.setNote(noteObj);
                                    Data.LongNotes[i].setEnd(n);
                                    Data.LongNotes[i].setEndPos(n.getPosition());
                                    Data.LongNotes[i].getMiddle().setPosition(
                                        (Data.LongNotes[i].getStart().getPosition()+n.getPosition()) / 2
                                    );
                                    lnadd[cline] = false;
                                }
                            }
                        }
                        else
                        {
                            // 아직 롱노트가 없으면 일반 노트를 하나 추가하고
                            // 롱노트를 이 노트의 위치와 시작 위치 사이에 추가할 수 있도록 함
                            // 이 동작은 Scroller.moveNotes()에서 isLong()을 확인해서 표기한다
                            GameObject noteObj = generator.AddNewNote(n.getLane(), Data.PlayLine, n.getPosition(), UI.playArea.transform);
                            noteObj.transform.parent = UI.playArea.transform;
                            n.setNote(noteObj);

                            Note lnNote = new Note(n.getPosition() + 1, "L#", n.getBar(), n.getLane(), Note.NOTETYPE.LONGNOTE, true);
                            GameObject lnObj = generator.AddNewNote(lnNote.getLane(), Data.PlayLine, lnNote.getPosition(), UI.playArea.transform);
                            lnObj.transform.parent = UI.playArea.transform;
                            lnNote.setNote(lnObj);

                            Data.LongNotes.Add(new Longnote(cline, n, n.getPosition(), lnNote));
                            // 롱노트 길이 표기용 노트는 L# 이라는 WAV 명을 표기하고 노트 타입도 LONGNOTE
                            // (단 롱노트의 시작과 끝은 일반 Note이다)
                            lnadd[cline] = true;
                        }
                    //}
                }
            }

            /*foreach (Note r in remove)
            {
                Data.Notes.Remove(r);
            }*/
        }

        void Update ()
        {
            // 플레이 키 (최 우선 체크)
            scroller.ProcessBeamOnTouch();

            if (firstrun && !isPaused)
            {
                double deltaTime = DateTime.Now.Ticks / 10000 - timePrevFrame;
                timePrevFrame = DateTime.Now.Ticks / 10000;

                double bpm = Data.CurrentBPM;
                double bps = Data.BPS;
                double spb = Data.SPB;

                // 변속곡 때문에 총 플레이 시간과 bps만으로 계산하는 것은 불가능함
                scroller.moveNotes(deltaTime, Data.BPS, Data.SPB,
                            Data.LongNotes, Data.Notes);

                scroller.play(Data.Notes, UI.touches, Data.LongNotes,
                        //ref unityAudio, ref channelGroup, ref fmodChannel,
                        Data.BMS, ref bpm, ref bps, ref spb,
                        totalNotes, Data.PlayLine, totalLen, isPlayAuto);

                Data.CurrentBPM = bpm;
                Data.BPS = bps;
                Data.SPB = spb;

                if (scroller.CheckGameOver() && !gameOverTriggered)
                {
                    // 게임오버로 가기 전에 페이드
                    StartCoroutine("GameOver");
                }
                else
                {
                    if (GameObject.FindGameObjectsWithTag("Note").Length == 0)
                    {
                        Const.SetClear(1);
                        bool isPlaying = soundController.CheckSoundPlaying();

                        if (!isPlaying)
                            scroller.SetGameOver();
                    }
                }
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
                musicStartTime = DateTime.Now.Ticks / 10000;
                timePrevFrame = musicStartTime;
            }

            if (scroller.CheckGameOver() && gameOverTriggered)
            {
                Loading.StartLoading("Result");
            }

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                isPaused = !isPaused;

                if(isPaused)
                {
                    // 메뉴 보여주기
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
            if (switchResult)
            {
                Debug.Log("GAMEOVER");
                // 재생중인 모든 음악 종료
                //channelGroup.stop();
                soundController.StopAll();

                // 결과 데이터 수집 후 result로 넘기기
                scroller.GetResultData(totalNotes);

                UI.SetFade();
                gameOverTriggered = true;
            }
            else
            {
                yield return new WaitForSeconds(0.0f);
                switchResult = true;
            }
        }
    }
}
