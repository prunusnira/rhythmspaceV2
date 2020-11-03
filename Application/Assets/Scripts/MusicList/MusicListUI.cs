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
        // Description
        public Button btnDescSystemOp;
        public TextMeshProUGUI txtDescMusicSel;
        public TextMeshProUGUI txtDescSystemOp;
        public TextMeshProUGUI txtDescPlay;
        public TextMeshProUGUI txtDescPlayClick;
        public TextMeshProUGUI txtDescUpperFolder;
        public TextMeshProUGUI txtDescPage;
        public TextMeshProUGUI txtTip;

        // Data store
        private List<ListItemNode> bmslist;
        private RecordDataManager rdm;
        private List<int> listDepth;
        private int listPosition;

        private EventSystem evtsystem;
        private PointerEventData eventdata;
        private GraphicRaycaster raycast;
        private long pressedTime;

        public static bool isTop = true;
        public static bool isLangChanged = false;
        public static bool isKeyChanged = false;
        public static bool isRefreshDone = false;
        public static bool isCustomRandom = false;

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
        public Sprite rampFail;
        public ScrollRectInfinite musicRect;
        public GameObject layerSysOpt;

        public Camera uiCam;

        // Search
        public InputField inputSearch;
        public Button btnSearchSubmit;
        private bool searchMode;

        // Selected info
        public RawImage infoJacket;
        public TextMeshProUGUI infoGerne;
        public TextMeshProUGUI infoTitle;
        public TextMeshProUGUI infoSubTitle;
        public TextMeshProUGUI infoArtist;
        public TextMeshProUGUI infoSubArtist;
        public TextMeshProUGUI infoBpm;
        public GameObject btnStart;

        // Difficulty
        public SpriteRenderer diffBeg;
        public SpriteRenderer diffNor;
        public SpriteRenderer diffHyp;
        public SpriteRenderer diffAno;
        public SpriteRenderer diffIns;
        public TextMeshProUGUI lvBeg;
        public TextMeshProUGUI lvNor;
        public TextMeshProUGUI lvHyp;
        public TextMeshProUGUI lvAno;
        public TextMeshProUGUI lvIns;

        public Sprite begOn;
        public Sprite begOff;
        public Sprite norOn;
        public Sprite norOff;
        public Sprite hypOn;
        public Sprite hypOff;
        public Sprite anoOn;
        public Sprite anoOff;
        public Sprite insOn;
        public Sprite insOff;
        public Sprite unkOn;
        public Sprite unkOff;

        public Text optSpdAppend;

        public TextMeshProUGUI recordScore;
        public TextMeshProUGUI recordPerfect;
        public TextMeshProUGUI recordGreat;
        public TextMeshProUGUI recordGood;
        public TextMeshProUGUI recordOk;
        public TextMeshProUGUI recordPoor;
        public TextMeshProUGUI recordCombo;
        public TextMeshProUGUI recordCBreak;
        public TextMeshProUGUI recordClearStat;
        public TextMeshProUGUI recordRank;

        public Sprite empty;

        // data
        public Text debugText;

        public AudioSource sfxChange;
        public AudioSource bgLoop;
        public AudioClip sfxChangeClip;
        public AudioClip sfxSelect;
        public AudioClip[] loop;

        private int selectedIdx = 0;

        void Awake()
        {
            // Initialize
            Application.targetFrameRate = 3000;
            bmslist = new List<ListItemNode>();
            rdm = new RecordDataManager();
            if(File.Exists(Const.JSONPath))
            {
                ListTreeGenerator();
                SelectListGenerator();
            }

            musicRect.Init(bmslist, ObjectSetup);

            int cnt = Const.ListPos;
            for (int i = 0; i < cnt; i++)
            {
                musicRect.AddItemBottom(ObjectSetup);
            }

            // Description
            btnDescSystemOp.onClick.AddListener(OpenSystemOption);
            UpdateDescription();

            // Search
            btnSearchSubmit.onClick.AddListener(SearchResult);
            searchMode = false;
        }

        public void Start()
        {
            // 리프레시를 해줘야 standalone에서 배경이 재생됨
            Screen.SetResolution(
                Const.ScrWidth,
                Const.ScrHeight,
                Const.ScreenMode
            );

            evtsystem = GetComponent<EventSystem>();
            raycast = GetComponent<GraphicRaycaster>();

            if(musicRect.GetItemCount() > 0)
            {
                showInfo(musicRect.GetCurrent());
            }

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
            if(isLangChanged)
            {
                UpdateDescription();
                isLangChanged = false;
            }

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

                if(Input.GetKey(KeyCode.DownArrow) ||
                    Input.mouseScrollDelta.y < 0)
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

                if (Input.GetKey(KeyCode.UpArrow) ||
                    Input.mouseScrollDelta.y > 0)
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
                    OpenSystemOption();
                }

                if (Input.GetKeyDown(KeyCode.Return))
                {
                    if(Const.selectedOnList.Type == ItemType.DIRECTORY)
                    {
                        MoveIntoFolder();
                    }
                    else if (Const.selectedOnList.Type == ItemType.BMS)
                    {
                        sfxChange.PlayOneShot(sfxSelect);
                        StartGame();
                    }
                }

                if(Input.GetKeyDown(KeyCode.Backspace)
                    || Input.GetMouseButtonDown(1))
                {
                    if(searchMode)
                    {
                        // 본래대로 돌아감
                        searchMode = false;
                        musicRect.Clear();
                        musicRect.ResetIndex();
                        SelectListGenerator();
                        musicRect.Init(bmslist, ObjectSetup);
                        showInfo(musicRect.GetCurrent());
                    }
                    if(Const.ListDepth.Count > 0)
                    {
                        Const.ListDepth.RemoveAt(Const.ListDepth.Count - 1);
                        Const.ListPos = 0;
                        musicRect.Clear();
                        musicRect.ResetIndex();
                        SelectListGenerator();
                        musicRect.Init(bmslist, ObjectSetup);
                        showInfo(musicRect.GetCurrent());
                    }
                    sfxChange.PlayOneShot(sfxSelect);
                }
            }
            
            if(isRefreshDone)
            {
                isRefreshDone = false;
                // 목록을 리셋하고 새로 리프레시
                musicRect.Clear();
                bmslist.Clear();

                ListTreeGenerator();
                SelectListGenerator();
                musicRect.Init(bmslist, ObjectSetup);

                showInfo(musicRect.GetCurrent());
            }
        }

        public void OpenSystemOption()
        {
            layerSysOpt.SetActive(true);
            sfxChange.PlayOneShot(sfxSelect);
            SetNotOnTop();
            GetComponent<SystemSetting>().EnableWindow();
        }

        public void SearchResult()
        {
            if (inputSearch.text != "")
            {
                string text = inputSearch.text;
                List<MusicListData> searchResult = MusicListManager.Instance.FindBMSWithName(text);
                if (searchResult.Count > 0)
                {
                    musicRect.Clear();
                    bmslist.Clear();
                    foreach (MusicListData d in searchResult)
                    {
                        ListItemNode bmsNode = new ListItemNode
                        {
                            Display = d.Title,
                            Info = d,
                            Type = ItemType.BMS,
                            Path = d.Path
                        };
                        bmslist.Add(bmsNode);
                    }
                    musicRect.Init(bmslist, ObjectSetup);
                    showInfo(musicRect.GetCurrent());
                    searchMode = true;
                }
                else
                {
                    // 검색 결과가 없음을 알림
                    inputSearch.text = "";
                    inputSearch.placeholder.GetComponent<Text>().text
                        = "NO SEARCH RESULT";
                }
            }
            else
            {
                inputSearch.placeholder.GetComponent<Text>().text
                    = "INPUT KEYWORD HERE";
            }
        }

        private void OnSearchActivated()
        {
            isTop = false;
        }

        private void OnSearchDeactivated()
        {
            isTop = true;
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
                    infoBpm.text = "BPM " + bms.BPMstart.ToString("0.##");
                }
                else
                {
                    infoBpm.text = "BPM " + bms.BPMmin.ToString("0.##") +
                        "~" + bms.BPMmax.ToString("0.##") +
                        " (" + bms.BPMstart.ToString("0.##") + " start)";
                }

                if (Const.SpdType == SpdType.STANDARD)
                {
                    int spdfl = (int)(bms.BPMstart * Const.SpeedStd / 100);
                    Const.SpeedCon = spdfl;
                    optSpdAppend.text = "CON " + spdfl.ToString("0");
                }
                else
                {
                    int spd = (int)(Const.SpeedCon / bms.BPMstart * 100);
                    Const.SpeedStd = spd;
                    optSpdAppend.text = "STD " + ((float)spd / 100).ToString("0.00") + "x";
                }

                // 난이도 숫자 표기
                switch(bms.Difficulty)
                {
                    case 1:
                        diffBeg.sprite = begOn;
                        diffNor.sprite = norOff;
                        diffHyp.sprite = hypOff;
                        diffAno.sprite = anoOff;
                        diffIns.sprite = insOff;

                        lvBeg.text = bms.Level.ToString("00");
                        lvNor.text = "00";
                        lvHyp.text = "00";
                        lvAno.text = "00";
                        lvIns.text = "00";
                        break;
                    case 2:
                        diffBeg.sprite = begOff;
                        diffNor.sprite = norOn;
                        diffHyp.sprite = hypOff;
                        diffAno.sprite = anoOff;
                        diffIns.sprite = insOff;

                        lvBeg.text = "00";
                        lvNor.text = bms.Level.ToString("00");
                        lvHyp.text = "00";
                        lvAno.text = "00";
                        lvIns.text = "00";
                        break;
                    case 3:
                        diffBeg.sprite = begOff;
                        diffNor.sprite = norOff;
                        diffHyp.sprite = hypOn;
                        diffAno.sprite = anoOff;
                        diffIns.sprite = insOff;

                        lvBeg.text = "00";
                        lvNor.text = "00";
                        lvHyp.text = bms.Level.ToString("00");
                        lvAno.text = "00";
                        lvIns.text = "00";
                        break;
                    case 4:
                        diffBeg.sprite = begOff;
                        diffNor.sprite = norOff;
                        diffHyp.sprite = hypOff;
                        diffAno.sprite = anoOn;
                        diffIns.sprite = insOff;

                        lvBeg.text = "00";
                        lvNor.text = "00";
                        lvHyp.text = "00";
                        lvAno.text = bms.Level.ToString("00");
                        lvIns.text = "00";
                        break;
                    case 5:
                        diffBeg.sprite = begOff;
                        diffNor.sprite = norOff;
                        diffHyp.sprite = hypOff;
                        diffAno.sprite = anoOff;
                        diffIns.sprite = insOn;

                        lvBeg.text = "00";
                        lvNor.text = "00";
                        lvHyp.text = "00";
                        lvAno.text = "00";
                        lvIns.text = bms.Level.ToString("00");
                        lvIns.color = new Color(127f / 256, 46f / 256, 178f / 256);
                        break;
                    default:
                        diffBeg.sprite = begOff;
                        diffNor.sprite = norOff;
                        diffHyp.sprite = hypOff;
                        diffAno.sprite = anoOff;
                        diffIns.sprite = unkOn;

                        lvBeg.text = "00";
                        lvNor.text = "00";
                        lvHyp.text = "00";
                        lvAno.text = "00";
                        lvIns.text = bms.Level.ToString("00");
                        lvIns.color = Color.white;
                        break;
                }

                // 기록 값 표기
                RecordData data = rdm.GetFullClearStat(bms.MD5Hash);
                Const.MyBestPrev = data;
                if(data != null)
                {
                    Const.MyBestScore = data.Score;
                    Const.MyBestRank = data.Rank;
                    recordScore.text = data.Score.ToString();
                    recordPerfect.text = data.Perfect.ToString();
                    recordGreat.text = data.Great.ToString();
                    recordGood.text = data.Good.ToString();
                    recordOk.text = data.OK.ToString();
                    recordPoor.text = data.Poor.ToString();
                    recordCombo.text = data.MaxCombo.ToString();
                    recordCBreak.text = data.CBreak.ToString();
                    recordRank.text = data.Rank.ToUpper();

                    switch (data.Clear)
                    {
                        case ClearType.ASSISTCLEAR:
                            recordClearStat.text = "ASSISTED CLEAR";
                            break;
                        case ClearType.EASYCLEAR:
                            recordClearStat.text = "EASY CLEAR";
                            break;
                        case ClearType.NORMALCLEAR:
                            recordClearStat.text = "NORMAL CLEAR";
                            break;
                        case ClearType.HARDCLEAR:
                            recordClearStat.text = "HARD CLEAR";
                            break;
                        case ClearType.EXCLEAR:
                            recordClearStat.text = "EX CLEAR";
                            break;
                        case ClearType.FULLCB:
                            recordClearStat.text = "FULLCOMBO";
                            break;
                        case ClearType.PERFECT:
                            recordClearStat.text = "PERFECT";
                            break;
                        case ClearType.FAIL:
                            recordClearStat.text = "FAILED";
                            break;
                    }
                }
                else
                {
                    Const.MyBestScore = 0;
                    Const.MyBestRank = "";
                    recordScore.text = "0";
                    recordPerfect.text = "0";
                    recordGreat.text = "0";
                    recordGood.text = "0";
                    recordOk.text = "0";
                    recordPoor.text = "0";
                    recordCombo.text = "0";
                    recordCBreak.text = "0";
                    recordClearStat.text = "NO PLAY";
                    recordRank.text = "";
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

                recordScore.text = "0";
                recordPerfect.text = "0";
                recordGreat.text = "0";
                recordGood.text = "0";
                recordOk.text = "0";
                recordPoor.text = "0";
                recordCombo.text = "0";
                recordCBreak.text = "0";
                recordClearStat.text = "NO PLAY";
                recordRank.text = "";
            }
        }

        public GameObject ObjectSetup(ListItemNode n, int i)
        {
            if (n.Type == ItemType.BMS)
            {
                MusicListData d = n.Info;
                GameObject music = Instantiate(patternPrefab) as GameObject;

                SelectedInfo info = music.GetComponent<SelectedInfo>();
                info.index = i;
                info.musicData = d;

                Transform c = music.transform;
                Button btn = c.GetComponent<Button>();
                btn.onClick.AddListener(delegate
                {
                    if(selectedIdx == i)
                    {
                        Const.ListPos = i;
                        StartGame();
                    }
                    else
                    {
                        selectedIdx = i;
                        Const.selectedOnList = n;
                        showInfo(n);
                    }
                });

                TextMeshProUGUI level = c.GetChild(0).GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI title = c.GetChild(1).GetComponent<TextMeshProUGUI>();
                Image ramp = c.GetChild(2).GetComponent<Image>();

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

                // 기록을 가져옴
                rdm = new RecordDataManager();
                switch((ClearType)rdm.GetClearStat(d.MD5Hash))
                {
                    case ClearType.ASSISTCLEAR:
                        ramp.sprite = rampAssisted;
                        break;
                    case ClearType.EASYCLEAR:
                        ramp.sprite = rampEasy;
                        break;
                    case ClearType.NORMALCLEAR:
                        ramp.sprite = rampNormal;
                        break;
                    case ClearType.HARDCLEAR:
                        ramp.sprite = rampHard;
                        break;
                    case ClearType.EXCLEAR:
                        ramp.sprite = rampEX;
                        break;
                    case ClearType.FULLCB:
                        ramp.sprite = rampFC;
                        break;
                    case ClearType.PERFECT:
                        ramp.sprite = rampPFC;
                        break;
                    case ClearType.FAIL:
                        ramp.sprite = rampFail;
                        break;
                }

                return music;
            }
            else if (n.Type == ItemType.DIRECTORY)
            {
                GameObject folder = Instantiate(folderPrefab) as GameObject;

                SelectedInfo info = folder.GetComponent<SelectedInfo>();
                info.index = i;

                Transform c = folder.transform;
                Button btn = c.GetComponent<Button>();
                btn.onClick.AddListener(delegate {
                    musicRect.ChangeCurrentIdx(i);
                    MoveIntoFolder();
                });
                TextMeshProUGUI title = c.GetChild(0).GetComponent<TextMeshProUGUI>();
                title.text = n.Display;

                return folder;
            }
            else return null;
        }

        public void MoveIntoFolder()
        {
            Const.ListDepth.Add(musicRect.GetCurrentIdx());
            Const.ListPos = 0;
            musicRect.Clear();
            musicRect.ResetIndex();
            SelectListGenerator();
            musicRect.Init(bmslist, ObjectSetup);
            showInfo(musicRect.GetCurrent());
            sfxChange.PlayOneShot(sfxSelect);
        }

        public void StartGame()
        {
            MusicListData pattern = Const.selectedOnList.Info;
            if (pattern != null)
            {
                // 선택한 패턴의 정보를 표시하고 플레이 시작이 가능
                Debug.Log(pattern.Title + " " + pattern.Artist);

                OnFinish();
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
                    List<MusicListData> list = MusicListManager.Instance.LoadBMSFromFolder(child.Path);
                    for (int j = 0; j < list.Count; j++)
                    {
                        MusicListData d = list[j];
                        ListItemNode bmsNode = new ListItemNode
                        {
                            Display = d.Title,
                            Info = d,
                            Type = ItemType.BMS,
                            Path = d.Path
                        };
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

        public void UpdateDescription()
        {
            LanguageType lang = Const.Language;
            txtDescMusicSel.text = Const.listSelect[(int)lang];
            txtDescSystemOp.text = Const.listSystemOp[(int)lang];
            txtDescPlay.text = Const.listPlayEnter[(int)lang];
            txtDescPlayClick.text = Const.listPlayClick[(int)lang];
            txtDescUpperFolder.text = Const.listUpper[(int)lang];
            txtDescPage.text = Const.playOpPage[(int)lang];
            txtTip.text = "* TIP: "+Const.listTip[(int)lang];
        }

        public void OnFinish()
        {
            MusicListManager.Instance.Close();
            rdm.Close();
        }
    }
}
