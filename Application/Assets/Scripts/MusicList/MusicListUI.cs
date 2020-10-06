using UnityEngine;
using UnityEngine.UI;
using BMSCore;
using DatabaseManager;
using UnityEngine.EventSystems;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

namespace BMSPlayer
{
    public class MusicListUI : MonoBehaviour
    {
        // Data store
        private List<MusicListData> bmslist;
        private MusicListManager mlm;

        private EventSystem evtsystem;
        private PointerEventData eventdata;
        private GraphicRaycaster raycast;
        private long pressedTime;

        public static bool isTop = true;

        // Unity object
        public GameObject patternPrefab;
        public ScrollRectInfinite musicRect;
        public GameObject layerSysOpt;

        public Camera uiCam;

        // Selected info
        public RawImage infoJacket;
        public Text infoGerne;
        public Text infoName;
        public Text infoArtist;
        public Text infoBpm;
        public SelectedMusicInfo info;
        public GameObject btnStart;

        public Text optSpdAppend;

        public Sprite empty;

        // data
        public Text debugText;

        public AudioSource sfxChange;
        public AudioClip sfxChangeClip;

        // Fader
        /*public GameObject fadeCube;
        private bool isFading = false;

        private void OnGUI()
        {
            if (isFading)
            {
                StartCoroutine("FadeOut");
            }
        }*/

        void Awake()
        {
            // Initialize
            bmslist = new List<MusicListData>();
            mlm = new MusicListManager();

            mlm.LoadBMSFromDBOverall();

            // Const의 리스트 데이터 확인
            foreach (MusicListData d in Const.list)
            {
                bmslist.Add(d);
            }

            mlm.close();

            //isFading = true;
        }

        public void Start()
        {
            evtsystem = GetComponent<EventSystem>();
            raycast = GetComponent<GraphicRaycaster>();
            
            musicRect.Init(bmslist, ObjectSetup);
            showInfo(musicRect.GetCurrent());
        }

        public void Update()
        {
            // 곡 선택이 front인 경우에만 동작
            if(isTop)
            {
                // 키보드를 움직였을 때 메뉴가 움직이도록 설정
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    pressedTime = DateTime.Now.Ticks / 1000;
                    musicRect.AddItemBottom(ObjectSetup);
                    showInfo(musicRect.GetCurrent());
                    sfxChange.PlayOneShot(sfxChangeClip);
                }

                if(Input.GetKey(KeyCode.DownArrow))
                {
                    if(DateTime.Now.Ticks / 1000 - pressedTime > 2000)
                    {
                        musicRect.AddItemBottom(ObjectSetup);
                        showInfo(musicRect.GetCurrent());
                        sfxChange.PlayOneShot(sfxChangeClip);
                    }
                }

                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    pressedTime = DateTime.Now.Ticks / 1000;
                    musicRect.AddItemTop(ObjectSetup);
                    showInfo(musicRect.GetCurrent());
                    sfxChange.PlayOneShot(sfxChangeClip);
                }

                if (Input.GetKey(KeyCode.UpArrow))
                {
                    if (DateTime.Now.Ticks / 1000 - pressedTime > 2000)
                    {
                        musicRect.AddItemTop(ObjectSetup);
                        showInfo(musicRect.GetCurrent());
                        sfxChange.PlayOneShot(sfxChangeClip);
                    }
                }

                if (Input.GetKeyDown(KeyCode.F10))
                {
                    layerSysOpt.SetActive(true);
                    SetNotOnTop();
                    GetComponent<SystemSetting>().EnableWindow();
                }

                if (Input.GetKeyDown(KeyCode.Return))
                {
                    StartGame();
                }
            }
            
            if(Const.isRefreshDone)
            {
                Const.isRefreshDone = false;
                // 목록을 리셋하고 새로 리프레시
                Const.list.Clear();
                musicRect.Clear();
                bmslist.Clear();

                mlm = new MusicListManager();
                mlm.LoadBMSFromDBOverall();
                mlm.close();

                // Const의 리스트 데이터 확인
                foreach (MusicListData d in Const.list)
                {
                    bmslist.Add(d);
                }
                musicRect.Init(bmslist, ObjectSetup);
                showInfo(musicRect.GetCurrent());
            }
        }

        public void showInfo(int idx)
        {
            int count = musicRect.content.childCount;
            Const.musiclistIdx = idx;
            Const.selectedMusic = bmslist[idx];

            if (File.Exists(Const.selectedMusic.Path + Const.selectedMusic.Jacket))
            {
                infoJacket.texture = Tools.LoadTexture(Const.selectedMusic.Path + Const.selectedMusic.Jacket);
            }
            else
            {
                infoJacket.texture = empty.texture;
            }
            infoGerne.text = Const.selectedMusic.Gerne;
            infoName.text = Const.selectedMusic.Name;
            infoArtist.text = Const.selectedMusic.Artist;

            // BPM 표시 설정
            if(Const.selectedMusic.BPMmin == Const.selectedMusic.BPMmax)
            {
                infoBpm.text = "BPM " + Const.selectedMusic.BPMstart;
            }
            else
            {
                infoBpm.text = "BPM " + Const.selectedMusic.BPMmin +
                    "~" + Const.selectedMusic.BPMmax +
                    " (" + Const.selectedMusic.BPMstart + " start)";
            }

            if(Const.SpdType == SpdType.FIXED)
            {
                int spdfl = (int)(Const.selectedMusic.BPMstart * Const.SpeedFixed / 100);
                Const.SpeedFluid = spdfl;
                optSpdAppend.text = "FLUID " + spdfl.ToString("0");
            }
            else
            {
                int spd = (int)(Const.SpeedFluid / Const.selectedMusic.BPMstart * 100);
                Const.SpeedFixed = spd;
                optSpdAppend.text = "FIXED " + ((float)spd/100).ToString("0.00")+"x";
            }
        }

        public GameObject ObjectSetup(MusicListData d)
        {
            GameObject music = Instantiate(patternPrefab) as GameObject;
            Transform c = music.transform;
            Text levelnum = c.GetChild(0).GetComponent<Text>();
            Text name = c.GetChild(1).GetComponent<Text>();

            levelnum.text = d.Level.ToString();
            name.text = d.Name;

            return music;
        }

        public void StartGame()
        {
            if (Const.selectedMusic != null)
            {
                // 선택한 패턴의 정보를 표시하고 플레이 시작이 가능
                Debug.Log(Const.selectedMusic.Name + " " + Const.selectedMusic.Artist);

                Const.PlayingBMSPath = Const.selectedMusic.Path + Const.selectedMusic.FileName;

                Debug.Log("READY TO RUN");
                Loading.StartLoading("PlayScreen");
            }
        }

        public static void SetOnTop()
        {
            isTop = true;
        }

        public static void SetNotOnTop()
        {
            isTop = false;
        }

        /*IEnumerator FadeOut()
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
        }*/
    }
}
