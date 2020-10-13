﻿using UnityEngine;
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
        private List<ListItemNode> bmslist;
        private MusicListManager mlm;
        private List<int> listDepth;
        private int listPosition;

        private EventSystem evtsystem;
        private PointerEventData eventdata;
        private GraphicRaycaster raycast;
        private long pressedTime;

        public static bool isTop = true;

        // Unity object
        public GameObject patternPrefab;
        public GameObject folderPrefab;
        public Sprite rampNoPlay;
        public Sprite rampAssisted;
        public Sprite rampEasy;
        public Sprite rampNormal;
        public Sprite rampHard;
        public Sprite rampEX;
        public Sprite rampFC;
        public Sprite rampPFC;
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
            bmslist = new List<ListItemNode>();
            mlm = new MusicListManager();
            ListTreeGenerator();
            SelectListGenerator();
            musicRect.Init(bmslist, Const.ListPos, ObjectSetup);
            mlm.close();
        }

        public void Start()
        {
            evtsystem = GetComponent<EventSystem>();
            raycast = GetComponent<GraphicRaycaster>();

            for(int i = 0; i < 8; i++)
            {
                musicRect.AddItemTop(ObjectSetup);
            }

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
                    if(Const.selectedOnList.Type == ItemType.DIRECTORY)
                    {
                        mlm = new MusicListManager();
                        Const.ListDepth.Add(musicRect.GetCurrentIdx());
                        Const.ListPos = 0;
                        musicRect.Clear();
                        SelectListGenerator();
                        musicRect.Init(bmslist, Const.ListPos, ObjectSetup);
                        mlm.close();

                        for (int i = 0; i < 8; i++)
                        {
                            musicRect.AddItemTop(ObjectSetup);
                        }
                        showInfo(musicRect.GetCurrent());
                    }
                    else if (Const.selectedOnList.Type == ItemType.BMS)
                    {
                        StartGame();
                    }
                }

                if(Input.GetKeyDown(KeyCode.Backspace))
                {
                    if(Const.ListDepth.Count > 0)
                    {
                        mlm = new MusicListManager();
                        Const.ListDepth.RemoveAt(Const.ListDepth.Count - 1);
                        Const.ListPos = 0;
                        musicRect.Clear();
                        SelectListGenerator();
                        musicRect.Init(bmslist, Const.ListPos, ObjectSetup);
                        mlm.close();

                        for (int i = 0; i < 8; i++)
                        {
                            musicRect.AddItemTop(ObjectSetup);
                        }
                        showInfo(musicRect.GetCurrent());
                    }
                }
            }
            
            if(Const.isRefreshDone)
            {
                Const.isRefreshDone = false;
                // 목록을 리셋하고 새로 리프레시
                musicRect.Clear();
                bmslist.Clear();
                
                mlm = new MusicListManager();
                ListTreeGenerator();
                SelectListGenerator();
                musicRect.Init(bmslist, Const.ListPos, ObjectSetup);
                mlm.close();

                showInfo(musicRect.GetCurrent());
            }
        }

        public void showInfo(ListItemNode node)
        {
            Const.selectedOnList = node;

            if(node.Type == ItemType.BMS)
            {
                MusicListData bms = node.Info;

                if (File.Exists(bms.Path + bms.Jacket))
                {
                    infoJacket.texture = Tools.LoadTexture(bms.Path + bms.Jacket);
                }
                else
                {
                    infoJacket.texture = empty.texture;
                }
                infoGerne.text = bms.Gerne;
                infoTitle.text = bms.Title;
                infoSubTitle.text = bms.SubTitle;
                infoArtist.text = bms.Artist;
                infoSubArtist.text = bms.SubArtist;

                // BPM 표시 설정
                if (bms.BPMmin == bms.BPMmax)
                {
                    infoBpm.text = "BPM " + bms.BPMstart;
                }
                else
                {
                    infoBpm.text = "BPM " + bms.BPMmin +
                        "~" + bms.BPMmax +
                        " (" + bms.BPMstart + " start)";
                }

                if (Const.SpdType == SpdType.FIXED)
                {
                    int spdfl = (int)(bms.BPMstart * Const.SpeedFixed / 100);
                    Const.SpeedFluid = spdfl;
                    optSpdAppend.text = "FLUID " + spdfl.ToString("0");
                }
                else
                {
                    int spd = (int)(Const.SpeedFluid / bms.BPMstart * 100);
                    Const.SpeedFixed = spd;
                    optSpdAppend.text = "FIXED " + ((float)spd / 100).ToString("0.00") + "x";
                }
            }
            else if(node.Type == ItemType.DIRECTORY)
            {
                infoJacket.texture = empty.texture;
                infoGerne.text = "";
                infoTitle.text = node.Display;
                infoSubTitle.text = "";
                infoArtist.text = "";
                infoSubArtist.text = "";
                infoBpm.text = "";
            }
        }

        public GameObject ObjectSetup(ListItemNode n)
        {
            if (n.Type == ItemType.BMS)
            {
                MusicListData d = n.Info;
                GameObject music = Instantiate(patternPrefab) as GameObject;
                Transform c = music.transform;
                TextMeshProUGUI level = c.GetChild(0).GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI title = c.GetChild(1).GetComponent<TextMeshProUGUI>();

                level.text = d.Level.ToString();
                title.text = d.Title;

                // Difficulty에 따른 색상 변경
                switch (d.Difficulty)
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
            else if (n.Type == ItemType.DIRECTORY)
            {
                GameObject folder = Instantiate(folderPrefab) as GameObject;
                Transform c = folder.transform;
                TextMeshProUGUI title = c.GetChild(0).GetComponent<TextMeshProUGUI>();
                title.text = n.Display;

                return folder;
            }
            else return null;
        }

        public void StartGame()
        {
            MusicListData pattern = Const.selectedOnList.Info;
            if (pattern != null)
            {
                // 선택한 패턴의 정보를 표시하고 플레이 시작이 가능
                Debug.Log(pattern.Title + " " + pattern.Artist);

                Const.PlayingBMSPath = pattern.Path + pattern.FileName;

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

        // Initialize Music List
        private void ListTreeGenerator()
        {
            // 해야 할 일
            // 1. JSON 파일을 읽는다
            // 2. Depth 정보에 따라서 폴더 내부로 들어간다
            // 3. pos에 따라 리스트 내에서 위치를 이동시킨다

            // 1. JSON 파일 읽기
            string jsonStr = File.ReadAllText(Const.JSONPath);

            // JSON 파싱
            JSONObject jsonHead = new JSONObject(jsonStr);

            // JSON to FolderNode
            // 헤드 노드 만들기
            Const.BMSTree = new ListItemTree(jsonHead);
        }

        // 현재 리스트 불러오기
        // 불러오는 방법
        // 1) depth만큼 이동
        // 2) child의 bms == true이면 -> bms를 가져옴
        // 3) child의 bms == false이면 -> child의 children의
        //    노드 수를 확인하고 0이 아니면 해당 child의 폴더 명을 가져옴
        public void SelectListGenerator()
        {
            // 2. Depth 정보에 따라 불러들일 폴더 선택
            // depth 정보는 이미 child 폴더 노드에 완성되어 있음
            // bms 노드 추가할 때만 해당 parent 노드로 등록하면 됨
            bmslist.Clear();
            ListItemNode current = Const.BMSTree.Head;
            List<int> depth = Const.ListDepth;

            foreach(int d in depth)
            {
                current = current.Children[d];
            }

            for (int i = 0; i < current.Children.Count; i++)
            {
                ListItemNode child = current.Children[i];

                if (child.HasBMS)
                {
                    // child의 children에서 BMS 정보 가져오기
                    // 해당 폴더의 서브폴더는 신경쓰지 않음
                    List<MusicListData> list = mlm.LoadBMSFromFolder(child.Path);
                    for (int j = 0; j < list.Count; j++)
                    {
                        MusicListData d = list[j];
                        ListItemNode bmsNode = new ListItemNode();
                        bmsNode.Display = d.Title;
                        bmsNode.Info = d;
                        bmsNode.Type = ItemType.BMS;
                        bmsNode.Path = d.Path;
                        bmslist.Add(bmsNode);
                    }
                }
                else
                {
                    // 해당 폴더 자체를 목록에 등록함
                    string dirname = Path.GetFileName(child.Path);
                    child.Display = dirname;
                    child.Path = child.Path;
                    child.Type = ItemType.DIRECTORY;
                    bmslist.Add(child);
                }
            }
        }
    }
}
