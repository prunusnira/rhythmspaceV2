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
using TMPro;

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
        public TextMeshProUGUI infoGerne;
        public TextMeshProUGUI infoTitle;
        public TextMeshProUGUI infoSubTitle;
        public TextMeshProUGUI infoArtist;
        public TextMeshProUGUI infoSubArtist;
        public TextMeshProUGUI infoBpm;
        public SelectedMusicInfo info;
        public GameObject btnStart;

        public Text optSpdAppend;

        public Sprite empty;

        // data
        public Text debugText;

        public AudioSource sfxChange;
        public AudioSource bgLoop;
        public AudioClip sfxChangeClip;
        public AudioClip[] loop;

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
        }

        public void Start()
        {
            evtsystem = GetComponent<EventSystem>();
            raycast = GetComponent<GraphicRaycaster>();
            
            musicRect.Init(bmslist, ObjectSetup);
            showInfo(musicRect.GetCurrent());

            // MusicLoop on
            System.Random rand = new System.Random();
            AudioClip loopMusic = loop[rand.Next(0, loop.Length)];
            bgLoop.clip = loopMusic;
            bgLoop.loop = true;
            bgLoop.playOnAwake = true;
            bgLoop.Play();
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

                if (Input.GetKeyDown(KeyCode.F12))
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
            infoTitle.text = Const.selectedMusic.Title;
            infoSubTitle.text = Const.selectedMusic.SubTitle;
            infoArtist.text = Const.selectedMusic.Artist;
            infoSubArtist.text = Const.selectedMusic.SubArtist;

            // BPM 표시 설정
            if (Const.selectedMusic.BPMmin == Const.selectedMusic.BPMmax)
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
            TextMeshProUGUI level = c.GetChild(0).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI title = c.GetChild(1).GetComponent<TextMeshProUGUI>();

            level.text = d.Level.ToString();
            title.text = d.Title;

            // Difficulty에 따른 색상 변경
            switch(d.Difficulty)
            {
                case 1:
                    level.color = new Color(119f / 255, 255f / 255, 145f / 255);
                    break;
                case 2:
                    level.color = new Color(92f / 255, 120f / 255, 229f / 255);
                    break;
                case 3:
                    level.color = new Color(255f / 255, 242f / 255, 146f / 255);
                    break;
                case 4:
                    level.color = new Color(212f / 255, 95f / 255, 95f / 255);
                    break;
                case 5:
                    level.color = new Color(172f / 255, 46f / 255, 178f / 255);
                    break;
                default:
                    level.color = new Color(1f, 1f, 1f);
                    break;
            }

            return music;
        }

        public void StartGame()
        {
            if (Const.selectedMusic != null)
            {
                // 선택한 패턴의 정보를 표시하고 플레이 시작이 가능
                Debug.Log(Const.selectedMusic.Title + " " + Const.selectedMusic.Artist);

                Const.PlayingBMSPath = Const.selectedMusic.Path + Const.selectedMusic.FileName;

                Debug.Log("READY TO RUN");
                SceneManager.LoadScene("Loading");
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
