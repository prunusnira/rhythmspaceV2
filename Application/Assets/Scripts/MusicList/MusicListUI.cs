using UnityEngine;
using UnityEngine.UI;
using BMSCore;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using TMPro;
using System.Threading;
using System.Linq;

namespace BMSPlayer
{
    public class MusicListUI : MonoBehaviour
    {
        // Description
        [Header("Option Buttons")]
        public Button btnDescPlayOp;
        public Button btnDescSystemOp;

        [Header("Option Layers")]
        public GameObject layerPlayOpt;
        public GameObject layerSysOpt;

        [Header("Description")]
        public TextMeshProUGUI txtDescMusicSel;
        public TextMeshProUGUI txtDescPlayOp;
        public TextMeshProUGUI txtDescSystemOp;
        public TextMeshProUGUI txtDescPlay;
        public TextMeshProUGUI txtDescUpperFolder;
        public TextMeshProUGUI txtTip;
        public TextMeshProUGUI txtTip2;

        public GameObject[] DescSet;
        private int curDesc = 0;
        private float DescDeltaTime = 0;

        // Data store
        private List<ListItem> bmslist;
        public TextMeshProUGUI txtPath;
        public TextMeshProUGUI txtPathGenoInfo;
        public Button btnSort;
        public TextMeshProUGUI txtSortBtn;
        private SortType sortType;
        private bool isFolderChanged = true;
        private long pressedTime;
        private string bmsPath;
        private int encoding;

        public static bool isTop = true;
        public static bool isLangChanged = false;
        public static bool isKeyChanged = false;
        public static bool isRefreshDone = false;
        public static bool isCustomRandom = false;
        public static bool isPathChanged = false;

        // List object
        [Header("List Objects")]
        public GameObject patternPrefab;
        public GameObject folderPrefab;
        public GameObject tablePrefab;
        public GameObject clearPrefab;
        public GameObject namePrefab;
        public ScrollRectInfinite musicRect;

        // Search
        [Header("Search")]
        public InputField inputSearch;
        public Button btnSearchSubmit;

        // Selected info
        [Header("BMS Info")]
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
        [Header("Difficulty")]
        public Image diffBeg;
        public Image diffNor;
        public Image diffHyp;
        public Image diffAno;
        public Image diffIns;
        public TextMeshProUGUI lvBeg;
        public TextMeshProUGUI lvNor;
        public TextMeshProUGUI lvHyp;
        public TextMeshProUGUI lvAno;
        public TextMeshProUGUI lvIns;

        [Header("Record")]
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

        [Header("Clear Txt Image")]
        public Sprite clearNP;
        public Sprite clearAC;
        public Sprite clearEC;
        public Sprite clearNC;
        public Sprite clearHC;
        public Sprite clearEXC;
        public Sprite clearFC;
        public Sprite clearPF;
        public Sprite clearFail;

        public Sprite empty;

        [Header("Audio")]
        public AudioSource sfxChange;
        public AudioSource bgLoop;
        public AudioSource prevLoop;
        public AudioClip sfxChangeClip;
        public AudioClip sfxSelect;
        public AudioClip[] loop;

        private int selectedIdx = 0;
        private Thread listLoadThread = null;
        private bool isLoading = false;
        private bool isLoadingFinish = false;

        [Header("Loading")]
        public GameObject loadingList;
        public GameObject loadingEmpty;
        public TextMeshProUGUI emptyMsg;

        // Dialog
        [Header("Dialog")]
        public GameObject dlgNoPattern;
        public GameObject dlgQuit;

        // Preview player
        private PreviewController prevController;
        private bool isPrevCon = false;
        private bool isPrevConReady = false;
        private bool isPrevPlay = false;
        private string prevConPath = "";
        private string prevFilePath = "";

        void Awake()
        {
            // Initialize
            prevController = GetComponent<PreviewController>();
            Application.targetFrameRate = Const.FrameRate;
            bmslist = new List<ListItem>();
            sortType = Const.SortType;
            bmsPath = Const.BMSFolderPath;
            encoding = Const.Encoding;

            if (Const.SearchMode)
            {
                SearchResult(Const.SearchText);
            }
            else
            {
                ListLoadThread();
                UpdatePathText();
            }

            // Description
            btnDescPlayOp.onClick.AddListener(OpenPlayOption);
            btnDescSystemOp.onClick.AddListener(OpenSystemOption);
            btnSort.onClick.AddListener(ChangeSort);
            UpdateDescription();

            // Search
            inputSearch.onEndEdit.AddListener(delegate
            {
                if(inputSearch.text != "")
                {
                    SearchResult(null);
                }
            });
            btnSearchSubmit.onClick.AddListener(delegate {
                SearchResult(null);
            });
        }

        public void Start()
        {
            // 리프레시를 해줘야 standalone에서 배경이 재생됨
            Screen.SetResolution(
                Const.ScrWidth,
                Const.ScrHeight,
                Const.ScreenMode
            );

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

            if(isPathChanged)
            {
                bmsPath = Const.BMSFolderPath;
                isPathChanged = false;
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

                if (Input.GetKeyDown(KeyCode.F9))
                {
                    OpenPlayOption();
                }

                if (Input.GetKeyDown(KeyCode.F10))
                {
                    OpenSystemOption();
                }

                if(Input.GetKeyDown(KeyCode.Escape))
                {
                    OpenQuitDialog();
                }

                // 전용 커스텀 버튼 체크
                if (Input.GetKeyDown(KeyCode.Return) || GetBtnWhite())
                {
                    if (Const.selectedOnList.Type == ItemType.BMS)
                    {
                        sfxChange.PlayOneShot(sfxSelect);
                        StartGame();
                    }
                    else
                    {
                        MoveIntoFolder();
                    }
                }

                // 전용 커스텀 버튼 체크
                if (Input.GetKeyDown(KeyCode.Backspace) ||
                    Input.GetMouseButtonDown(1) ||
                    GetBtnBlue())
                {
                    if(Const.SearchMode)
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
                        Const.SearchMode = false;
                        musicRect.Clear();
                        musicRect.ResetIndex();
                        ListLoadThread();
                    }
                    else if(Const.ListDepth.Count > 0)
                    {
                        Const.ListPos = Const.ListDepth[Const.ListDepth.Count - 1];
                        Const.ListDepth.RemoveAt(Const.ListDepth.Count - 1);
                        Const.ListPath.RemoveAt(Const.ListPath.Count - 1);
                        musicRect.Clear();
                        musicRect.ResetIndex();
                        ListLoadThread();

                        txtPathGenoInfo.gameObject.SetActive(false);
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
                if(isFolderChanged)
                {
                    txtPath.text += "(Total " + bmslist.Count + ")";
                    isFolderChanged = false;
                }
            }

            if(isPrevConReady)
            {
                isPrevConReady = false;
                isPrevCon = true;
            }
            if(isPrevCon && !isPrevPlay)
            {
                isPrevPlay = true;
                prevController.Setup(prevConPath, delegate {
                    bgLoop.volume = 0.1f;
                });
            }
            else if(!isPrevCon && isPrevPlay)
            {
                isPrevPlay = false;
                prevController.StopPlaying();
            }
        }

        public void OpenPlayOption()
        {
            layerPlayOpt.SetActive(true);
            sfxChange.PlayOneShot(sfxSelect);
            SetNotOnTop();
            GetComponent<PlayOptionSetting>().EnableWindow();
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

        public void SearchResult(string keyword)
        {
            string searchStr = keyword;
            if (searchStr == null) searchStr = inputSearch.text.Replace(" ", "");
            if (searchStr != "")
            {
                List<MusicListData> searchResult = MusicDataManager.Instance.LoadBMSWithName(searchStr);
                if (searchResult.Count > 0)
                {
                    Const.SearchText = searchStr;
                    txtPath.text = Const.musiclistSearchMode[(int)Const.Language];
                    musicRect.Clear();
                    bmslist.Clear();
                    foreach (MusicListData d in searchResult)
                    {
                        ListItem bmsNode = new ListItem
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
                    Const.SearchMode = true;
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

        public void showInfo(ListItem node)
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
                    infoBpm.text = bms.BPMstart.ToString("0.##");
                }
                else
                {
                    infoBpm.text = bms.BPMmin.ToString("0.##") +
                        "~" + bms.BPMmax.ToString("0.##") +
                        "\n(" + bms.BPMstart.ToString("0.##") + " start)";
                }

                if (Const.SpdType == SpdType.STANDARD)
                {
                    int spdfl = (int)(bms.BPMstart * Const.SpeedStd / 100);
                    Const.SpeedCon = spdfl;
                }
                else
                {
                    int spd = (int)(Const.SpeedCon / bms.BPMstart * 100);
                    Const.SpeedStd = spd;
                }

                // 난이도 숫자 표기
                switch(bms.Difficulty)
                {
                    case 1:
                        diffBeg.sprite = SkinSetting.DiffBegOn;
                        diffNor.sprite = SkinSetting.DiffNorOff;
                        diffHyp.sprite = SkinSetting.DiffHypOff;
                        diffAno.sprite = SkinSetting.DiffAnoOff;
                        diffIns.sprite = SkinSetting.DiffInsOff;

                        lvBeg.text = bms.Level.ToString("00");
                        lvNor.text = "00";
                        lvHyp.text = "00";
                        lvAno.text = "00";
                        lvIns.text = "00";
                        break;
                    case 2:
                        diffBeg.sprite = SkinSetting.DiffBegOff;
                        diffNor.sprite = SkinSetting.DiffNorOn;
                        diffHyp.sprite = SkinSetting.DiffHypOff;
                        diffAno.sprite = SkinSetting.DiffAnoOff;
                        diffIns.sprite = SkinSetting.DiffInsOff;

                        lvBeg.text = "00";
                        lvNor.text = bms.Level.ToString("00");
                        lvHyp.text = "00";
                        lvAno.text = "00";
                        lvIns.text = "00";
                        break;
                    case 3:
                        diffBeg.sprite = SkinSetting.DiffBegOff;
                        diffNor.sprite = SkinSetting.DiffNorOff;
                        diffHyp.sprite = SkinSetting.DiffHypOn;
                        diffAno.sprite = SkinSetting.DiffAnoOff;
                        diffIns.sprite = SkinSetting.DiffInsOff;

                        lvBeg.text = "00";
                        lvNor.text = "00";
                        lvHyp.text = bms.Level.ToString("00");
                        lvAno.text = "00";
                        lvIns.text = "00";
                        break;
                    case 4:
                        diffBeg.sprite = SkinSetting.DiffBegOff;
                        diffNor.sprite = SkinSetting.DiffNorOff;
                        diffHyp.sprite = SkinSetting.DiffHypOff;
                        diffAno.sprite = SkinSetting.DiffAnoOn;
                        diffIns.sprite = SkinSetting.DiffInsOff;

                        lvBeg.text = "00";
                        lvNor.text = "00";
                        lvHyp.text = "00";
                        lvAno.text = bms.Level.ToString("00");
                        lvIns.text = "00";
                        break;
                    case 5:
                        diffBeg.sprite = SkinSetting.DiffBegOff;
                        diffNor.sprite = SkinSetting.DiffNorOff;
                        diffHyp.sprite = SkinSetting.DiffHypOff;
                        diffAno.sprite = SkinSetting.DiffAnoOff;
                        diffIns.sprite = SkinSetting.DiffInsOn;

                        lvBeg.text = "00";
                        lvNor.text = "00";
                        lvHyp.text = "00";
                        lvAno.text = "00";
                        lvIns.text = bms.Level.ToString("00");
                        lvIns.color = new Color(127f / 256, 46f / 256, 178f / 256);
                        break;
                    default:
                        diffBeg.sprite = SkinSetting.DiffBegOff;
                        diffNor.sprite = SkinSetting.DiffNorOff;
                        diffHyp.sprite = SkinSetting.DiffHypOff;
                        diffAno.sprite = SkinSetting.DiffAnoOff;
                        diffIns.sprite = SkinSetting.DiffUnkOn;

                        lvBeg.text = "00";
                        lvNor.text = "00";
                        lvHyp.text = "00";
                        lvAno.text = "00";
                        lvIns.text = bms.Level.ToString("00");
                        lvIns.color = Color.white;
                        break;
                }

                // 기록 값 표기
                RecordData data = RecordDataManager.Instance.GetFullClearStat(bms.MD5Hash);
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
                            recordRank.sprite = SkinSetting.RankAAA;
                            break;
                        case "aa":
                            recordRank.sprite = SkinSetting.RankAA;
                            break;
                        case "a":
                            recordRank.sprite = SkinSetting.RankA;
                            break;
                        case "b":
                            recordRank.sprite = SkinSetting.RankB;
                            break;
                        case "c":
                            recordRank.sprite = SkinSetting.RankC;
                            break;
                        case "d":
                            recordRank.sprite = SkinSetting.RankD;
                            break;
                        case "e":
                            recordRank.sprite = SkinSetting.RankE;
                            break;
                        case "f":
                            recordRank.sprite = SkinSetting.RankF;
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

                // 프리뷰 파일이 있으면 재생함
                if(bms.Preview != null && bms.Preview.Length > 0)
                {
                    prevController.StopPlaying();
                    prevConPath = "";

                    if (prevFilePath != bms.Preview)
                    {
                        prevLoop.Stop();
                        isPrevCon = false;
                        prevFilePath = bms.Preview;

                        bgLoop.volume = 0.1f;
                        WWW www = new WWW("file://"+bms.Preview);
                        prevLoop.clip = www.GetAudioClip(false, true);
                        prevLoop.Play();
                    }
                }
                else
                {
                    prevLoop.Stop();
                    prevFilePath = "";
                    bgLoop.volume = 0.5f;

                    isPrevCon = false;
                    isPrevPlay = false;
                    prevController.StopPlaying();

                    prevConPath = bms.Path + bms.FileName;

                    //isPrevConReady = true;
                }
            }
            else
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

                bgLoop.volume = 0.5f;
                prevLoop.Stop();
                isPrevCon = false;
            }
        }

        public GameObject ObjectSetup(ListItem n, int i)
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

                Image img = c.GetComponent<Image>();
                TextMeshProUGUI level = c.GetChild(0).GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI title = c.GetChild(1).GetComponent<TextMeshProUGUI>();
                Image ramp = c.GetChild(2).GetComponent<Image>();

                level.text = d.Level.ToString();
                title.text = d.Title;

                if (!d.LNExist) c.GetChild(3).gameObject.SetActive(false);

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
                switch((ClearType)RecordDataManager.Instance.GetClearStat(d.MD5Hash))
                {
                    case ClearType.ASSISTCLEAR:
                        ramp.sprite = SkinSetting.RampAssist;
                        break;
                    case ClearType.EASYCLEAR:
                        ramp.sprite = SkinSetting.RampEasy;
                        break;
                    case ClearType.NORMALCLEAR:
                        ramp.sprite = SkinSetting.RampNormal;
                        break;
                    case ClearType.HARDCLEAR:
                        ramp.sprite = SkinSetting.RampHard;
                        break;
                    case ClearType.EXCLEAR:
                        ramp.sprite = SkinSetting.RampEXHard;
                        break;
                    case ClearType.FULLCB:
                        ramp.sprite = SkinSetting.RampFullCombo;
                        break;
                    case ClearType.PERFECT:
                        ramp.sprite = SkinSetting.RampPerfect;
                        break;
                    case ClearType.FAIL:
                        ramp.sprite = SkinSetting.RampFailed;
                        break;
                }

                if(n.IsFromTable && !n.Exist)
                {
                    music.GetComponent<Image>().sprite = SkinSetting.PatternNotExistImg;
                }
                else
                {
                    music.GetComponent<Image>().sprite = SkinSetting.PatternImg;
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
            else if (n.Type == ItemType.CLEAR)
            {
                GameObject table = Instantiate(clearPrefab) as GameObject;

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
            else if (n.Type == ItemType.NAME)
            {
                GameObject table = Instantiate(namePrefab) as GameObject;

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
            ListItem node = musicRect.GetCurrent();
            Const.ListPath.Add(node.Path);
            Const.ListPos = 0;
            isFolderChanged = true;
            musicRect.Clear();
            musicRect.ResetIndex();

            ListLoadThread();

            UpdatePathText();
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

        // 현재 리스트 불러오기
        public void SelectListGenerator()
        {
            bmslist.Clear();

            // Table 유무에 따른 리스트 표기 분화
            if(Const.ListPath.Count > 0 &&
                Const.ListPath[0].StartsWith("table://"))
            {
                // 난이도 표에서 리스트를 표기하는 경우
                MusicListAdderTable();
            }
            else if (Const.ListPath.Count > 0 &&
                Const.ListPath[0].StartsWith("clear://"))
            {
                // 난이도 표에서 리스트를 표기하는 경우
                MusicListAdderClear();
            }
            else if (Const.ListPath.Count > 0 &&
                Const.ListPath[0].StartsWith("name://"))
            {
                // 난이도 표에서 리스트를 표기하는 경우
                MusicListAdderName();
            }
            else if(Const.ListPath.Count == 0)
            {
                // 첫 페이지
                MusicListZero();
            }
            else
            {
                // 일반적으로 표기하는 경우
                MusicListAdderNormal();
            }

            if (Const.ListPath.Count != 0) SortMusicList();
            else isLoadingFinish = true;
        }

        public void MusicListAdderTable()
        {
            if (Const.ListPath.Count == 1)
            {
                // 난이도 목록 표시
                string name = Const.ListPath[0].Split(
                    new string[] { "//" }, StringSplitOptions.None)[1];

                DiffTableMode type = 0;
                switch (name)
                {
                    case "satellite": type = DiffTableMode.SATELLITE; break;
                    case "stella": type = DiffTableMode.STELLA; break;
                    case "genonm": type = DiffTableMode.GENONM; break;
                    case "genoins": type = DiffTableMode.GENOINS; break;
                }
                List<int> levelList = TableDataManager.Instance.GetTableLevelList(type);

                foreach (int i in levelList)
                {
                    bmslist.Add(
                        new ListItem
                        {
                            Display = "Level " + (i < 10 ? "0"+i : i.ToString()),
                            Type = ItemType.TABLE,
                            Path = Const.ListPath[0] + "/" + i.ToString(),
                            Info = new MusicListData
                            {
                                Level = -9999
                            }
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
                    case "genonm":
                        listData = TableDataManager.Instance.LoadTableGeByLevel(lv, DiffTableMode.GENONM);
                        break;
                    case "genoins":
                        listData = TableDataManager.Instance.LoadTableGeByLevel(lv, DiffTableMode.GENOINS);
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

                        ListItem bmsNode = new ListItem
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
        }

        public void MusicListAdderClear()
        {
            if (Const.ListPath.Count == 1)
            {
                // 난이도 목록 표시
                string rank = Const.ListPath[0].Split(
                    new string[] { "//" }, StringSplitOptions.None)[1];

                List<string> md5List = RecordDataManager.Instance.GetRankHash(rank);
                List<MusicListData> musicList = MusicDataManager.Instance.LoadBMSListWithMD5(md5List);
                
                foreach (MusicListData d in musicList)
                {
                    bmslist.Add(
                        new ListItem
                        {
                            Display = d.Title,
                            Info = d,
                            Type = ItemType.BMS,
                            Path = d.Path,
                            IsFromTable = false,
                            Exist = true
                        }
                    );
                }
            }
        }

        public void MusicListAdderName()
        {
            if (Const.ListPath.Count == 1)
            {
                // 난이도 목록 표시
                string name = Const.ListPath[0].Split(
                    new string[] { "//" }, StringSplitOptions.None)[1];

                NameType type = NameType.ABCD;
                switch(name)
                {
                    case "abcd":
                        type = NameType.ABCD;
                        break;
                    case "efgh":
                        type = NameType.EFGH;
                        break;
                    case "ijkl":
                        type = NameType.IJKL;
                        break;
                    case "mnop":
                        type = NameType.MNOP;
                        break;
                    case "qrst":
                        type = NameType.QRST;
                        break;
                    case "uvwxyz":
                        type = NameType.UVWXYZ;
                        break;
                    case "num":
                        type = NameType.NUMBERS;
                        break;
                    case "others":
                        type = NameType.OTHERS;
                        break;
                }

                List<MusicListData> musicList =
                    MusicDataManager.Instance.LoadBMSListWithName(type);

                foreach (MusicListData d in musicList)
                {
                    bmslist.Add(
                        new ListItem
                        {
                            Display = d.Title,
                            Info = d,
                            Type = ItemType.BMS,
                            Path = d.Path,
                            IsFromTable = false,
                            Exist = true
                        }
                    );
                }
            }
        }

        public void MusicListZero()
        {
            // 최상위 폴더
            bmslist.Add(new ListItem
            {
                Display = "BMS",
                Type = ItemType.DIRECTORY,
                Path = bmsPath,
                IsFromTable = false,
                Exist = true
            });

            // 클리어 랭크
            bmslist.Add(new ListItem
            {
                Display = "FULL COMBO",
                Type = ItemType.CLEAR,
                Path = "clear://fc"
            });
            bmslist.Add(new ListItem
            {
                Display = "CLEAR RANK AAA",
                Type = ItemType.CLEAR,
                Path = "clear://aaa"
            });
            bmslist.Add(new ListItem
            {
                Display = "CLEAR RANK AA",
                Type = ItemType.CLEAR,
                Path = "clear://aa"
            });
            bmslist.Add(new ListItem
            {
                Display = "CLEAR RANK A",
                Type = ItemType.CLEAR,
                Path = "clear://a"
            });
            bmslist.Add(new ListItem
            {
                Display = "CLEAR RANK B to F",
                Type = ItemType.CLEAR,
                Path = "clear://b"
            });

            // 이름
            bmslist.Add(new ListItem
            {
                Display = "NAME ABCD",
                Type = ItemType.NAME,
                Path = "name://abcd"
            });
            bmslist.Add(new ListItem
            {
                Display = "NAME EFGH",
                Type = ItemType.NAME,
                Path = "name://efgh"
            });
            bmslist.Add(new ListItem
            {
                Display = "NAME IJKL",
                Type = ItemType.NAME,
                Path = "name://ijkl"
            });
            bmslist.Add(new ListItem
            {
                Display = "NAME MNOP",
                Type = ItemType.NAME,
                Path = "name://mnop"
            });
            bmslist.Add(new ListItem
            {
                Display = "NAME QRST",
                Type = ItemType.NAME,
                Path = "name://qrst"
            });
            bmslist.Add(new ListItem
            {
                Display = "NAME UVWXYZ",
                Type = ItemType.NAME,
                Path = "name://uvwxyz"
            });
            bmslist.Add(new ListItem
            {
                Display = "NAME Number",
                Type = ItemType.NAME,
                Path = "name://num"
            });
            bmslist.Add(new ListItem
            {
                Display = "NAME OTHERS",
                Type = ItemType.NAME,
                Path = "name://others"
            });

            // 난이도 표
            bmslist.Add(new ListItem
            {
                Display = "Satellite",
                Type = ItemType.TABLE,
                Path = "table://satellite"
            });
            bmslist.Add(new ListItem
            {
                Display = "Stella",
                Type = ItemType.TABLE,
                Path = "table://stella"
            });
            bmslist.Add(new ListItem
            {
                Display = "Genocide Normal",
                Type = ItemType.TABLE,
                Path = "table://genonm"
            });
            bmslist.Add(new ListItem
            {
                Display = "Genocide Insane",
                Type = ItemType.TABLE,
                Path = "table://genoins"
            });
        }

        public void MusicListAdderNormal()
        {
            List<int> depth = Const.ListDepth;

            // Tree 유무와 관계없이 DB 검색으로 처리하기
            string path = Const.ListPath[Const.ListPath.Count - 1];

            // 1. 하위 폴더를 탐색하면서 BMS 파일이 있는지 확인
            if(path != "")
            {
                // 2-1. 현재 폴더에 bms파일이 있는지 확인
                string[] bmsfiles = Directory.EnumerateFiles(path, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(s =>
                            s.ToLower().EndsWith(".bms") ||
                            s.ToLower().EndsWith(".bme") ||
                            s.ToLower().EndsWith(".bml")
                        ).ToArray();

                foreach(string s in bmsfiles)
                {
                    MusicListData d = MusicDataManager.Instance.LoadBMSFromPath(s, encoding);
                    if(d != null)
                    {
                        ListItem bmsNode = new ListItem
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

                foreach (string s in Directory.EnumerateDirectories(path, "*", SearchOption.TopDirectoryOnly))
                {
                    // 2-2. 폴더 하위에 bms 파일이 있다면
                    List<MusicListData> list =
                        MusicDataManager.Instance.LoadBMSFromFolder(s);

                    if (list != null)
                    {
                        foreach (MusicListData d in list)
                        {
                            ListItem bmsNode = new ListItem
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

                    // 2-3. 폴더 하위에 다른 폴더가 있다면
                    List<string> dirlist =
                        new List<string>(Directory.EnumerateDirectories(s, "*", SearchOption.TopDirectoryOnly));
                    if (dirlist.Count > 0)
                    {
                        DirectoryInfo dir = new DirectoryInfo(s);
                        ListItem dirNode = new ListItem
                        {
                            Display = dir.Name,
                            Path = s,
                            Type = ItemType.DIRECTORY,
                            Info = new MusicListData
                            {
                                Level = -9999
                            }
                        };
                        bmslist.Add(dirNode);
                    }
                }
            }
        }

        public void UpdateDescription()
        {
            LanguageType lang = Const.Language;
            txtDescMusicSel.text = Const.listSelect[(int)lang];
            txtDescPlayOp.text = Const.listPlayOp[(int)lang];
            txtDescSystemOp.text = Const.listSystemOp[(int)lang];
            txtDescPlay.text = Const.listPlay[(int)lang];
            txtDescUpperFolder.text = Const.listUpper[(int)lang];
            txtTip.text = Const.listTip[(int)lang];
            txtTip2.text = Const.listTip2[(int)lang];
            txtPathGenoInfo.text = Const.musiclistGenoTableInfo[(int)lang];

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
            RecordDataManager.Instance.Close();
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

                    // GENOCIDE 테이블 관련 알림
                    if(pathlist[0].StartsWith("geno"))
                    {
                        txtPathGenoInfo.gameObject.SetActive(true);
                    }
                }
                else if (Const.ListPath[0].StartsWith("clear://"))
                {
                    string paths = Const.ListPath[Const.ListPath.Count - 1].Substring(8);
                    string[] pathlist = paths.Split(new string[] { "/" }, StringSplitOptions.None);

                    foreach (string s in pathlist)
                    {
                        txtPath.text += "CLEAR "+s.ToUpper() + " > ";
                    }
                }
                else if (Const.ListPath[0].StartsWith("name://"))
                {
                    string paths = Const.ListPath[Const.ListPath.Count - 1].Substring(7);
                    string[] pathlist = paths.Split(new string[] { "/" }, StringSplitOptions.None);

                    foreach (string s in pathlist)
                    {
                        txtPath.text += "NAME "+s.ToUpper() + " > ";
                    }
                }
                else
                {
                    foreach (string s in Const.ListPath)
                    {
                        string[] paths = s.Split(new string[] { "\\" }, StringSplitOptions.None);
                        txtPath.text += paths[paths.Length-1] + " > ";
                    }
                }
            }
        }

        private void ChangeSort()
        {
            switch (Const.SortType)
            {
                case SortType.TITLE:
                    Const.SortType = SortType.LEVEL;
                    txtSortBtn.text = "LEVEL";
                    break;
                case SortType.LEVEL:
                    Const.SortType = SortType.TITLE;
                    txtSortBtn.text = "TITLE";
                    break;
            }
            sortType = Const.SortType;
            sfxChange.PlayOneShot(sfxSelect);

            if (Const.ListDepth.Count > 0)
            {
                musicRect.Clear();
                SortMusicList();
            }
        }

        private void SortMusicList()
        {
            if (sortType == SortType.TITLE)
                bmslist.Sort((ListItem a, ListItem b) =>
                    a.Display.CompareTo(b.Display));
            else if (sortType == SortType.LEVEL)
                bmslist.Sort((ListItem a, ListItem b) =>
                    a.Info.Level.CompareTo(b.Info.Level));

            isLoadingFinish = true;
        }
    }
}
