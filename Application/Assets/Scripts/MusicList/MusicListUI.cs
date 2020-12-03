using UnityEngine;
using UnityEngine.UI;
using BMSCore;
using UnityEngine.EventSystems;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using TMPro;
using System.Threading;

namespace BMSPlayer
{
    public class MusicListUI : MonoBehaviour
    {
        // Description
        public Button btnDescSystemOp;
        public TextMeshProUGUI txtDescMusicSel;
        public TextMeshProUGUI txtDescSystemOp;
        public TextMeshProUGUI txtDescPlay;
        public TextMeshProUGUI txtDescUpperFolder;
        public TextMeshProUGUI txtDescPage;
        public TextMeshProUGUI txtTip;
        public TextMeshProUGUI txtTip2;

        public GameObject[] DescSet;
        private int curDesc = 0;
        private float DescDeltaTime = 0;

        // Data store
        private List<ListItemNode> bmslist;
        private RecordDataManager rdm;
        private List<int> listDepth;
        private int listPosition;
        public TextMeshProUGUI txtPath;

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
        public GameObject tablePrefab;
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
        public TextMeshProUGUI infoTotalNote;
        public TextMeshProUGUI infoPlayTime;
        public TextMeshProUGUI infoJudgeRank;

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
        public Image recordRank;
        public Image recordClearStat;
        public Sprite clearNP;
        public Sprite clearAC;
        public Sprite clearEC;
        public Sprite clearNC;
        public Sprite clearHC;
        public Sprite clearEXC;
        public Sprite clearFC;
        public Sprite clearPF;
        public Sprite clearFail;
        public Sprite rankaaa;
        public Sprite rankaa;
        public Sprite ranka;
        public Sprite rankb;
        public Sprite rankc;
        public Sprite rankd;
        public Sprite ranke;
        public Sprite rankf;

        public Sprite empty;

        public AudioSource sfxChange;
        public AudioSource bgLoop;
        public AudioClip sfxChangeClip;
        public AudioClip sfxSelect;
        public AudioClip[] loop;

        private int selectedIdx = 0;
        private Thread listLoadThread = null;
        private bool isLoading = false;
        private bool isLoadingFinish = false;
        public GameObject loadingList;
        public GameObject loadingEmpty;
        public TextMeshProUGUI emptyMsg;

        // Dialog
        public GameObject dlgNoPattern;
        public GameObject dlgQuit;

        void Awake()
        {
            // Initialize
            Application.targetFrameRate = Const.FrameRate;
            bmslist = new List<ListItemNode>();
            rdm = new RecordDataManager();
            if(File.Exists(Const.JSONPath))
            {
                ListTreeGenerator();
            }

            ListLoadThread();

            // Description
            btnDescSystemOp.onClick.AddListener(OpenSystemOption);
            UpdateDescription();

            // Search
            inputSearch.onEndEdit.AddListener(delegate
            {
                if(inputSearch.text != "")
                {
                    SearchResult();
                }
            });
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
            if(DescDeltaTime > 3)
            {
                SwitchDesc();
            }
            else
            {
                DescDeltaTime += Time.deltaTime;
            }

            if(isLangChanged)
            {
                UpdateDescription();
                isLangChanged = false;
            }

            // 곡 선택이 front인 경우에만 동작
            if(isTop && !isLoading)
            {
                // 키보드를 움직였을 때 메뉴가 움직이도록 설정
                if (Input.GetKeyDown(KeyCode.DownArrow) ||
                    Keys.GetTurnTableLeftDown())
                {
                    pressedTime = DateTime.Now.Ticks / 1000;
                    musicRect.AddItemBottom(ObjectSetup);
                    showInfo(musicRect.GetCurrent());
                    sfxChange.PlayOneShot(sfxChangeClip);
                }

                if(Input.GetKey(KeyCode.DownArrow) ||
                    Input.mouseScrollDelta.y < 0 ||
                    Keys.GetTurnTableLeftKeep())
                {
                    if(DateTime.Now.Ticks / 1000 - pressedTime > 5000)
                    {
                        musicRect.AddItemBottom(ObjectSetup);
                        showInfo(musicRect.GetCurrent());
                        sfxChange.PlayOneShot(sfxChangeClip);
                    }
                }

                if (Input.GetKeyDown(KeyCode.UpArrow) ||
                    Keys.GetTurnTableRightDown())
                {
                    pressedTime = DateTime.Now.Ticks / 1000;
                    musicRect.AddItemTop(ObjectSetup);
                    showInfo(musicRect.GetCurrent());
                    sfxChange.PlayOneShot(sfxChangeClip);
                }

                if (Input.GetKey(KeyCode.UpArrow) ||
                    Input.mouseScrollDelta.y > 0 ||
                    Keys.GetTurnTableRightKeep())
                {
                    if (DateTime.Now.Ticks / 1000 - pressedTime > 5000)
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

                if(Input.GetKeyDown(KeyCode.Escape))
                {
                    OpenQuitDialog();
                }

                // 전용 커스텀 버튼 체크
                if (Input.GetKeyDown(KeyCode.Return) ||
                    GetBtnWhite())
                {
                    if(Const.selectedOnList.Type == ItemType.DIRECTORY
                        || Const.selectedOnList.Type == ItemType.TABLE)
                    {
                        MoveIntoFolder();
                    }
                    else if (Const.selectedOnList.Type == ItemType.BMS)
                    {
                        sfxChange.PlayOneShot(sfxSelect);
                        StartGame();
                    }
                }

                // 전용 커스텀 버튼 체크
                if (Input.GetKeyDown(KeyCode.Backspace) ||
                    Input.GetMouseButtonDown(1) ||
                    GetBtnBlue())
                {
                    if(searchMode)
                    {
                        // 본래대로 돌아감
                        if(Const.ListDepth.Count > 0)
                        {
                            Const.ListPos = Const.ListDepth[Const.ListDepth.Count - 1];
                        }
                        else
                        {
                            Const.ListPos = 0;
                        }
                        searchMode = false;
                        musicRect.Clear();
                        musicRect.ResetIndex();
                        ListLoadThread();
                    }
                    if(Const.ListDepth.Count > 0)
                    {
                        Const.ListPos = Const.ListDepth[Const.ListDepth.Count - 1];
                        Const.ListDepth.RemoveAt(Const.ListDepth.Count - 1);
                        Const.ListPath.RemoveAt(Const.ListPath.Count - 1);
                        musicRect.Clear();
                        musicRect.ResetIndex();
                        ListLoadThread();
                    }
                    UpdatePathText();
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
                Const.ListPos = 0;
                Const.ListDepth.Clear();
                ListLoadThread();
            }

            if(isLoadingFinish)
            {
                isLoadingFinish = false;

                musicRect.Init(bmslist, Const.ListPos, ObjectSetup);
                if (musicRect.GetItemCount() > 0)
                {
                    showInfo(musicRect.GetCurrent());
                }
                loadingList.SetActive(false);

                if(bmslist.Count == 0)
                {
                    loadingEmpty.SetActive(true);
                }

                isLoading = false;
            }
        }

        public void OpenSystemOption()
        {
            layerSysOpt.SetActive(true);
            sfxChange.PlayOneShot(sfxSelect);
            SetNotOnTop();
            GetComponent<SystemSetting>().EnableWindow();
        }

        public void OpenQuitDialog()
        {
            dlgQuit.SetActive(true);
            sfxChange.PlayOneShot(sfxSelect);
            SetNotOnTop();
            GetComponent<DialogQuit>().EnableWindow();
        }

        public void SearchResult()
        {
            if (inputSearch.text != "")
            {
                string text = inputSearch.text;
                List<MusicListData> searchResult = MusicDataManager.Instance.FindBMSWithName(text);
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
                            Path = d.Path,
                            Exist = true
                        };
                        bmslist.Add(bmsNode);
                    }
                    musicRect.Init(bmslist, 0, ObjectSetup);
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
                infoTotalNote.text = bms.TotalNotes.ToString();

                switch (bms.Rank)
                {
                    case 0:
                        infoJudgeRank.text = "VERY HARD";
                        break;
                    case 1:
                        infoJudgeRank.text = "HARD";
                        break;
                    case 3:
                        infoJudgeRank.text = "EASY";
                        break;
                    case 2:
                    default:
                        infoJudgeRank.text = "NORMAL";
                        break;
                }

                int time = bms.PlayTime;
                string min = (Convert.ToInt32(time) / 600).ToString("00");
                string sec = (Convert.ToInt32(time) % 600 / 10).ToString("00");
                infoPlayTime.text = min + ":" + sec;

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

                    switch (data.Rank)
                    {
                        case "aaa":
                            recordRank.sprite = rankaaa;
                            break;
                        case "aa":
                            recordRank.sprite = rankaa;
                            break;
                        case "a":
                            recordRank.sprite = ranka;
                            break;
                        case "b":
                            recordRank.sprite = rankb;
                            break;
                        case "c":
                            recordRank.sprite = rankc;
                            break;
                        case "d":
                            recordRank.sprite = rankd;
                            break;
                        case "e":
                            recordRank.sprite = ranke;
                            break;
                        case "f":
                            recordRank.sprite = rankf;
                            break;
                    }

                    switch (data.Clear)
                    {
                        case ClearType.ASSISTCLEAR:
                            recordClearStat.sprite = clearAC;
                            break;
                        case ClearType.EASYCLEAR:
                            recordClearStat.sprite = clearEC;
                            break;
                        case ClearType.NORMALCLEAR:
                            recordClearStat.sprite = clearNC;
                            break;
                        case ClearType.HARDCLEAR:
                            recordClearStat.sprite = clearHC;
                            break;
                        case ClearType.EXCLEAR:
                            recordClearStat.sprite = clearEXC;
                            break;
                        case ClearType.FULLCB:
                            recordClearStat.sprite = clearFC;
                            break;
                        case ClearType.PERFECT:
                            recordClearStat.sprite = clearPF;
                            break;
                        case ClearType.FAIL:
                            recordClearStat.sprite = clearFail;
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
                    recordClearStat.sprite = clearNP;
                    recordRank.sprite = empty;
                }
            }
            else if(node.Type == ItemType.DIRECTORY ||
                    node.Type == ItemType.TABLE)
            {
                infoJacket.texture = empty.texture;
                infoGerne.text = "";
                infoTitle.text = node.Display;
                infoSubTitle.text = "";
                infoArtist.text = "";
                infoSubArtist.text = "";
                infoBpm.text = "";
                infoTotalNote.text = "";
                infoPlayTime.text = "";
                infoJudgeRank.text = "";

                recordScore.text = "0";
                recordPerfect.text = "0";
                recordGreat.text = "0";
                recordGood.text = "0";
                recordOk.text = "0";
                recordPoor.text = "0";
                recordCombo.text = "0";
                recordCBreak.text = "0";
                recordClearStat.sprite = clearNP;
                recordRank.sprite = empty;
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

                if(n.IsFromTable && !n.Exist)
                {
                    music.GetComponent<Image>().color =
                        new Color(130f / 255, 0, 31f / 255);
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
            else if(n.Type == ItemType.TABLE)
            {
                GameObject table = Instantiate(tablePrefab) as GameObject;

                SelectedInfo info = table.GetComponent<SelectedInfo>();
                info.index = i;

                Transform c = table.transform;
                Button btn = c.GetComponent<Button>();
                btn.onClick.AddListener(delegate {
                    musicRect.ChangeCurrentIdx(i);
                    MoveIntoFolder();
                });
                TextMeshProUGUI title = c.GetChild(0).GetComponent<TextMeshProUGUI>();
                title.text = n.Display;

                return table;
            }
            else return null;
        }

        public void MoveIntoFolder()
        {
            Const.ListDepth.Add(musicRect.GetCurrentIdx());
            ListItemNode node = musicRect.GetCurrent();
            if(node.Type == ItemType.TABLE)
            {
                Const.ListPath.Add(node.Path);
            }
            else
            {
                Const.ListPath.Add(node.Display);
            }
            Const.ListPos = 0;
            musicRect.Clear();
            musicRect.ResetIndex();

            UpdatePathText();

            ListLoadThread();
            sfxChange.PlayOneShot(sfxSelect);
        }

        public void StartGame()
        {
            if(Const.selectedOnList.Exist)
            {
                MusicListData pattern = Const.selectedOnList.Info;
                if (pattern != null)
                {
                    OnFinish();
                    Const.PlayingBMSPath = pattern.Path + pattern.FileName;

                    Debug.Log("READY TO RUN");
                    SceneManager.LoadScene("Loading");
                }
            }
            else
            {
                // 패턴 없음에 대한 메시지 표시
                dlgNoPattern.SetActive(true);
                sfxChange.PlayOneShot(sfxSelect);
                SetNotOnTop();
                GetComponent<DialogNoPattern>().EnableWindow();
                GetComponent<DialogNoPattern>().SetURL(Const.selectedOnList.Url);
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

            // Table 유무에 따른 리스트 표기 분화
            if(Const.ListPath.Count > 0 &&
                Const.ListPath[0].StartsWith("table://"))
            {
                // 난이도 표에서 리스트를 표기하는 경우
                MusicListAdderTable();
            }
            else
            {
                // 일반적으로 표기하는 경우
                MusicListAdderNormal();
            }
        }

        public void MusicListAdderTable()
        {
            if (Const.ListPath.Count == 1)
            {
                // 난이도 목록 표시
                string name = Const.ListPath[0].Split(
                    new string[] { "//" }, StringSplitOptions.None)[1];

                int type = 0;
                switch (name)
                {
                    case "satellite": type = 0; break;
                    case "stella": type = 1; break;
                    case "genocide": type = 2; break;
                }
                List<int> levelList = TableDataManager.Instance.GetTableLevelList(type);

                foreach (int i in levelList)
                {
                    bmslist.Add(
                        new ListItemNode
                        {
                            Display = "Level "+i.ToString(),
                            Type = ItemType.TABLE,
                            Path = Const.ListPath[0] + "/" + i.ToString()
                        }
                    );
                }
            }
            else if (Const.ListDepth.Count == 2)
            {
                // 각 난이도별 모든 곡 리스트 표시
                string[] data = Const.ListPath[1].Split(
                    new string[] { "//", "/" }, StringSplitOptions.None);

                string name = data[1];
                int lv = Convert.ToInt32(data[2]);

                List<DiffTableData> listData = null;
                switch (name)
                {
                    case "satellite":
                        listData = TableDataManager.Instance.LoadTableStSlByLevel(lv, DiffTableMode.SATELLITE);
                        break;
                    case "stella":
                        listData = TableDataManager.Instance.LoadTableStSlByLevel(lv, DiffTableMode.STELLA);
                        break;
                    case "genocide":
                        listData = TableDataManager.Instance.LoadTableGeByLevel(lv);
                        break;
                }

                if(listData != null)
                {
                    foreach(DiffTableData d in listData)
                    {
                        bool exist = true;
                        MusicListData mdata = new MusicListData();
                        if (d.MD5.Length > 0)
                        {
                            mdata = TableDataManager.Instance.GetMusicOfMD5Hash(d.MD5);
                        }
                        else if(d.MD5.Length == 0)
                        {
                            mdata = TableDataManager.Instance.GetMusicOfTitle(d.Title);
                        }

                        if (mdata == null)
                        {
                            mdata = new MusicListData
                            {
                                Title = d.Title,
                                Level = lv,
                                MD5Hash = d.MD5
                            };
                            exist = false;
                        }

                        ListItemNode bmsNode = new ListItemNode
                        {
                            Display = d.Title,
                            Type = ItemType.BMS,
                            Url = d.URL,
                            Info = mdata,
                            IsFromTable = true,
                            Exist = exist
                        };
                        bmslist.Add(bmsNode);
                    }
                }
            }
            isLoadingFinish = true;
        }

        public void MusicListAdderNormal()
        {
            List<int> depth = Const.ListDepth;
            if (Const.BMSTree != null)
            {
                ListItemNode current = Const.BMSTree.Head;

                foreach (int d in depth)
                {
                    current = current.Children[d];
                }

                if (current.Type == ItemType.DIRECTORY &&
                    current.HaveBMS)
                {
                    List<MusicListData> list = MusicDataManager.Instance.LoadBMSFromFolder(current.Path);
                    for (int j = 0; j < list.Count; j++)
                    {
                        MusicListData d = list[j];
                        ListItemNode bmsNode = new ListItemNode
                        {
                            Display = d.Title,
                            Info = d,
                            Type = ItemType.BMS,
                            Path = d.Path,
                            IsFromTable = false,
                            Exist = true
                        };
                        bmslist.Add(bmsNode);
                    }
                }

                for (int i = 0; i < current.Children.Count; i++)
                {
                    ListItemNode child = current.Children[i];

                    if(child.HaveBMS)
                    {
                        // child의 children에서 BMS 정보 가져오기
                        // 해당 폴더의 서브폴더는 신경쓰지 않음
                        List<MusicListData> list = MusicDataManager.Instance.LoadBMSFromFolder(child.Path);
                        for (int j = 0; j < list.Count; j++)
                        {
                            MusicListData d = list[j];
                            ListItemNode bmsNode = new ListItemNode
                            {
                                Display = d.Title,
                                Info = d,
                                Type = ItemType.BMS,
                                Path = d.Path,
                                IsFromTable = false,
                                Exist = true
                            };
                            bmslist.Add(bmsNode);
                        }
                    }
                    else
                    {
                        // 해당 폴더 자체를 목록에 등록함
                        if(child.Type == ItemType.DIRECTORY)
                        {
                            string dirname = Path.GetFileName(child.Path);
                            child.Display = dirname;
                            child.Path = child.Path;
                            child.Type = ItemType.DIRECTORY;
                            bmslist.Add(child);
                        }
                    }
                }
            }

            // Depth 0일 경우에 난이도 표를 추가
            if (depth.Count == 0)
            {
                bmslist.Add(new ListItemNode
                {
                    Display = "Satellite",
                    Type = ItemType.TABLE,
                    Path = "table://satellite"
                });
                bmslist.Add(new ListItemNode
                {
                    Display = "Stella",
                    Type = ItemType.TABLE,
                    Path = "table://stella"
                });
                bmslist.Add(new ListItemNode
                {
                    Display = "Genocide",
                    Type = ItemType.TABLE,
                    Path = "table://genocide"
                });
            }
            isLoadingFinish = true;
        }

        public void UpdateDescription()
        {
            LanguageType lang = Const.Language;
            txtDescMusicSel.text = Const.listSelect[(int)lang];
            txtDescSystemOp.text = Const.listSystemOp[(int)lang];
            txtDescPlay.text = Const.listPlay[(int)lang];
            txtDescUpperFolder.text = Const.listUpper[(int)lang];
            txtDescPage.text = Const.playOpPage[(int)lang];
            txtTip.text = Const.listTip[(int)lang];
            txtTip2.text = Const.listTip2[(int)lang];

            emptyMsg.text = Const.musiclistEmpty[(int)lang];
        }

        private void SwitchDesc()
        {
            if(curDesc == 4)
            {
                curDesc = 0;
            }
            else
            {
                curDesc++;
            }

            for(int i = 0; i < DescSet.Length; i++)
            {
                if(i == curDesc)
                {
                    DescSet[i].SetActive(true);
                }
                else
                {
                    DescSet[i].SetActive(false);
                }
            }
            DescDeltaTime = 0;
        }

        public void ListLoadThread()
        {
            isLoading = true;
            loadingEmpty.SetActive(false);
            loadingList.SetActive(true);
            listLoadThread = new Thread(new ThreadStart(delegate
            {
                SelectListGenerator();
            }));
            listLoadThread.Start();
        }

        public void OnFinish()
        {
            MusicDataManager.Instance.Close();
            rdm.Close();
        }

        public static bool GetBtnWhite()
        {
            return
                GetBtnDown(1) ||
                GetBtnDown(3) ||
                GetBtnDown(5) ||
                GetBtnDown(7);
        }

        public static bool GetBtnBlue()
        {
            return
                GetBtnDown(2) ||
                GetBtnDown(4) ||
                GetBtnDown(6);
        }

        public static bool GetBtnDown(int i)
        {
            return
                (!Keys.btnAxisSet1[i] && Keys.GetKeyDown(Keys.btnSet1[i])) ||
                (!Keys.btnAxisSet2[i] && Keys.GetKeyDown(Keys.btnSet2[i]));
        }

        private void UpdatePathText()
        {
            txtPath.text = "ROOT > ";
            if(Const.ListPath.Count > 0)
            {
                if(Const.ListPath[0].StartsWith("table://"))
                {
                    string paths = Const.ListPath[Const.ListPath.Count - 1].Substring(8);
                    string[] pathlist = paths.Split(new string[] { "/" }, StringSplitOptions.None);

                    foreach(string s in pathlist)
                    {
                        txtPath.text += s.ToUpper() + " > ";
                    }
                }
                else
                {
                    foreach (string s in Const.ListPath)
                    {
                        txtPath.text += s + " > ";
                    }
                }
            }
        }
    }
}
