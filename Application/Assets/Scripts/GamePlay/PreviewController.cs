using BMSCore;
using System;
using System.Collections;
using System.Threading;
using UnityEngine;

namespace BMSPlayer {
	// PlayController를 간략화하여 미리듣기만 수행하는 클래스
	public class PreviewController: MonoBehaviour {
        // Data Related
        private PlayData Data;
        private NoteGenerator generator;
        private ScrollerPreviewPlayer scroller;
        private LNObjConverter LNConverter;
        private BMSAnalyzer analyzer;

        // Scene 관리
        private bool isBMSReady = false;
        private bool isLoadingPhase1 = false;
        private bool isLoadingPhase1Finish = false;
        private bool isLoadingPhase2Ready = false;
        private bool isGoodToGo = false;

        // Data in thread
        private int encoding = 932;

        // 시간 관리
        private double StartTime = 0;
        private double PrevTickTime = 0;
        private double PlayTimePassed = 0;

        // Audio Management
        private bool firstrun = false;

        // callback
        public delegate void MusicReadyCallback();
        private MusicReadyCallback Callback;

		private void Awake() {
            // BMS 파일 분석
            analyzer = new BMSAnalyzer();

            // 분석된 데이터를 기반으로 스크롤러 준비
            scroller = new ScrollerPreviewPlayer();

            // 분석된 데이터를 바탕으로 노트를 생성
            generator = new NoteGenerator();

            encoding = Const.Encoding;
        }

        private void Update ()
        {
            try
            {
                // 데이터 로딩
                if (isGoodToGo && !isLoadingPhase1)
                {
                    isLoadingPhase1 = true;
                    Thread loadingThread = new Thread(new ThreadStart(LoadBMSPhase1));
                    loadingThread.Start();
                    return;
                }
                else if(isLoadingPhase1Finish)
                {
                    isLoadingPhase1Finish = false;
                    //StartCoroutine(PreloadSound());
                    isLoadingPhase2Ready = true;
                    Callback();
                    return;
                }
                else if(isLoadingPhase2Ready)
                {
                    isLoadingPhase2Ready = false;
                    Thread loadingThread = new Thread(new ThreadStart(LoadBMSPhase2));
                    loadingThread.Start();
                    return;
                }

                if (!isBMSReady)
                {
                    return;
                }
                else
                {
                    MusicListUI.isPrevLoaded = true;
                }

                if (firstrun)
                {
                    double currentTick = Convert.ToDouble(DateTime.Now.Ticks) / 1000000;
                    PlayTimePassed = currentTick - StartTime;
                    double DeltaTime = PlayTimePassed - PrevTickTime;

                    scroller.PlayPreviewSound(
                        Data.NotePlay, Data.NoteLong,
                        Data.BMS, PlayTimePassed);

                    scroller.PlayBGM(Data.NoteBGM, Data.BMS, PlayTimePassed);
                    
                    if (scroller.GetProcessedNotes() >= Data.TotalNotes)
                    {
                        StopPlaying();
                    }

                    PrevTickTime = PlayTimePassed;
                }
                else
                {
                    firstrun = true;
                    StartTime = Convert.ToDouble(DateTime.Now.Ticks) / 1000000;
                    PrevTickTime = 0;
                    PlayTimePassed = 0;
                }
            }
            catch (Exception e)
            {
                ErrorHandler.LogError(e.Message + " " + e.StackTrace);
            }
        }

        public bool Setup(string path, MusicReadyCallback cb)
        {
            Data = new PlayData(path);
            isGoodToGo = true;
            firstrun = false;
            Callback = cb;

            StartTime = 0;
            PrevTickTime = 0;
            PlayTimePassed = 0;
            return true;
        }

        private void LoadBMSPhase1()
        {
            try
            {
                analyzer.FullAnalyzer(Data.BMS, encoding);

                // LNOBJ 타입이면 추가로 이식하는 작업을 수행함
                if (Data.BMS.LNType == LNType.Obj)
                {
                    LNConverter = new LNObjConverter();
                    LNConverter.FixLongNoteLNOBJ(Data.BMS);
                }

                // 사운드 정의(FMOD)
                //soundController.PreloadSound(Data.BMS);
                SoundControllerBASS.Instance.PreloadSound(Data.BMS);
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
                generator.AnalyzeNotes(Data, new int[] { 1, 2, 3, 4, 5, 6, 7 });
                generator.PositionToTiming(Data);
                generator.SortAllNotes(Data);

                isBMSReady = true;
            }
            catch (Exception e)
            {
                ErrorHandler.LogError(e.Message + " " + e.StackTrace);
            }
        }

        public void StopPlaying()
        {
            if(Data != null)
            {
                // 재생중인 모든 음악 종료
                SoundControllerBASS.Instance.StopAll();
                SoundControllerBASS.Instance.FreeMemory(Data.BMS);

                Data = null;
                isGoodToGo = false;
                isBMSReady = false;

                isLoadingPhase1 = false;
                isLoadingPhase1Finish = false;
                isLoadingPhase2Ready = false;

                StartTime = 0;
                PrevTickTime = 0;
                PlayTimePassed = 0;
            }
        }

        IEnumerator PreloadSound()
        {
            SoundControllerFMOD.Instance.PreloadSound(Data.BMS);
            yield return null;
            isLoadingPhase2Ready = true;
            Callback();
        }
    }
}
