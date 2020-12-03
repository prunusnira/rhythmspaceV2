using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.IO;

namespace BMSPlayer
{
    public class PlayOptionSetting: MonoBehaviour
    {
        // Page
        private int Page = 0;
        public Button btnPageChanger;
        public GameObject PageSet0;
        public GameObject PageSet1;
        public GameObject PageSet2;

        // Speed
        public Button btnSpdDown;
        public Button btnSpdUp;
        public Text txtSpeed;
        public Text txtSpdAnother;
        private int speed;
        private int speedfl;

        // Speed Type
        public Button btnSpdType;
        public Text txtSpdType;

        // Auto
        public Button btnAuto;
        public Text txtAuto;

        // Judge
        public Button btnJudgePanel;
        public Text txtJudgePanel;

        // Gauge Type
        public Button btnGauge;
        public Text txtGauge;

        // Target Graph Type
        public Button btnTarget;
        public Text txtTarget;

        // Note Layout
        public Button btnNoteLayout;
        public Text txtNoteLayout;
        public Text txtNoteDesc;

        // Custom Random
        public Button btnCustomRandom;
        public GameObject customRandom;

        // Judgement Type
        public Button btnJudgeType;
        public Text txtJudgeType;

        // GearSkin
        public Button btnGearSkin;
        public Text txtGearSkin;

        // GearSize
        public Button btnGearSize;
        public Text txtGearSize;

        // NoteSkin
        public Button btnNoteSkin;
        public Text txtNoteSkin;

        // NoteSize
        public Button btnNoteSize;
        public Text txtNoteSize;

        // GraphSize
        public Button btnGraphType;
        public Text txtGraphType;

        // Layout
        public Button btnLayoutType;
        public Text txtLayoutType;
        public Text txtLayoutDesc;

        // BGA ON/OFF
        public Button btnBGA;
        public Text txtBGAOnOff;

        // PlaySide
        public Button btnPlaySide;
        public Text txtPlaySide;

        // Fast/Slow
        public Button btnFastSlow;
        public Text txtFastSlow;
        public Text txtFastSlowDesc;

        // Target Difference
        public Button btnTargetDiff;
        public Text txtTargetDiff;
        public Text txtTargetDiffDesc;

        // Rate diff
        public Button btnRate;
        public Text txtRateDiff;
        public Text txtRateDiffDesc;

        // SFX Play
        public AudioSource sfxPlay;
        public AudioClip sfxSource;

        // Initial Setting
        public static bool gearSizeInit = false;
        public static bool judgeInit = false;

        public void Awake()
        {
            // 버튼 설정
            btnPageChanger.onClick.AddListener(PageChange);

            // Page 1
            btnSpdDown.onClick.AddListener(delegate
            {
                if (Const.SpdType == SpdType.STANDARD)
                {
                    SpeedDownFixed();
                }
                else if (Const.SpdType == SpdType.CONSTANT)
                {
                    SpeedDownFluid();
                }
            });
            btnSpdUp.onClick.AddListener(delegate
            {
                if (Const.SpdType == SpdType.STANDARD)
                {
                    SpeedUpFixed();
                }
                else if (Const.SpdType == SpdType.CONSTANT)
                {
                    SpeedUpFluid();
                }
            });
            btnSpdType.onClick.AddListener(SpeedTypeChange);
            btnAuto.onClick.AddListener(AutoOnOff);
            btnJudgePanel.onClick.AddListener(JudgeUIOnOff);
            btnGauge.onClick.AddListener(GaugeTypeChange);
            btnTarget.onClick.AddListener(GraphTargetChange);
            btnNoteLayout.onClick.AddListener(NoteLayoutChange);
            btnCustomRandom.onClick.AddListener(OpenCustomRandom);
            btnJudgeType.onClick.AddListener(JudgeTypeChange);

            // Page 2
            btnGearSkin.onClick.AddListener(GearSkinChange);
            btnGearSize.onClick.AddListener(GearSizeChange);
            btnNoteSkin.onClick.AddListener(NoteSkinChange);
            btnNoteSize.onClick.AddListener(NoteSizeChange);
            btnGraphType.onClick.AddListener(GraphSizeChange);
            btnLayoutType.onClick.AddListener(LayoutChange);
            btnBGA.onClick.AddListener(BGAChange);

            // Page 3
            btnPlaySide.onClick.AddListener(PlaySideChange);
            btnFastSlow.onClick.AddListener(FastSlowChange);
            btnTargetDiff.onClick.AddListener(TargetDiffChange);
            btnRate.onClick.AddListener(RateChange);

            // 오토 설정
            switch (Const.Auto)
            {
                case AutoPlayType.OFF:
                    txtAuto.text = "OFF";
                    txtAuto.color = Color.white;
                    break;
                case AutoPlayType.TURNTABLE:
                    txtAuto.text = "TURNTABLE";
                    txtAuto.color = Color.magenta;
                    break;
                case AutoPlayType.ALL:
                    txtAuto.text = "ALL";
                    txtAuto.color = Color.red;
                    break;
            }

            // 판정 표시 설정
            if(Const.DisplayJudge == 0)
            {
                txtJudgePanel.text = "OFF";
                txtJudgePanel.color = Color.white;
            }
            else
            {
                txtJudgePanel.text = "ON";
                txtJudgePanel.color = new Color(1f, 120f / 255, 0f);
            }

            // 속도
            if(Const.SpdType == SpdType.STANDARD)
            {
                speed = Const.SpeedStd;
                txtSpdType.text = "STANDARD";
                txtSpdType.color = new Color(80f / 255, 201f / 255, 125f / 255);
                txtSpeed.text = ((float)speed / 100).ToString("0.00") + "x";
            }
            else
            {
                speedfl = Const.SpeedCon;
                txtSpdType.text = "CONSTANT";
                txtSpdType.color = new Color(80f / 255, 201f / 255, 125f / 255);
                txtSpeed.text = speedfl.ToString();
            }

            // 게이지 타입
            switch(Const.GaugeType)
            {
                case GaugeType.ASSISTED:
                    txtGauge.text = "ASSISTED";
                    txtGauge.color = new Color(206f / 255, 159f / 255, 1f);
                    break;
                case GaugeType.EASY:
                    txtGauge.text = "EASY";
                    txtGauge.color = new Color(159f / 255, 1f, 180f / 255);
                    break;
                case GaugeType.NORMAL:
                    txtGauge.text = "NORMAL";
                    txtGauge.color = new Color(159f / 255, 215f / 255, 1f);
                    break;
                case GaugeType.HARD:
                    txtGauge.text = "HARD";
                    txtGauge.color = new Color(1f, 159f / 255, 159f / 255);
                    break;
                case GaugeType.EXHARD:
                    txtGauge.text = "EX-HARD";
                    txtGauge.color = new Color(1f, 207f / 255, 79f / 255);
                    break;
            }

            // 그래프
            switch(Const.GraphTarget)
            {
                case GraphTargetType.A:
                    txtTarget.text = "A";
                    txtTarget.color = new Color(196f / 255, 52f / 255, 174f / 255);
                    break;
                case GraphTargetType.AA:
                    txtTarget.text = "AA";
                    txtTarget.color = new Color(1f, 133f / 255, 45f / 255);
                    break;
                case GraphTargetType.AAA:
                    txtTarget.text = "AAA";
                    txtTarget.color = new Color(1f, 64f / 255, 64f / 255);
                    break;
                case GraphTargetType.MAX:
                    txtTarget.text = "MAX";
                    txtTarget.color = new Color(164f / 255, 71f / 255, 117f / 255);
                    break;
                case GraphTargetType.OFF:
                default:
                    txtTarget.text = "OFF";
                    txtTarget.color = Color.white;
                    break;
            }

            // 판정
            InitJudge();

            // 배치
            switch(Const.NoteLayout)
            {
                case NoteLayout.RANDOM:
                    txtNoteLayout.text = "RANDOM";
                    txtNoteLayout.color = new Color(1f, 231f / 255, 142f / 255);
                    txtNoteDesc.text = Const.RandomDescRD[(int)Const.Language];
                    break;
                case NoteLayout.MIRROR:
                    txtNoteLayout.text = "MIRROR";
                    txtNoteLayout.color = new Color(1f, 231f / 255, 142f / 255);
                    txtNoteDesc.text = Const.RandomDescMR[(int)Const.Language];
                    break;
                case NoteLayout.SRAN:
                    txtNoteLayout.text = "S-RAN";
                    txtNoteLayout.color = new Color(1f, 231f / 255, 142f / 255);
                    txtNoteDesc.text = Const.RandomDescSR[(int)Const.Language];
                    break;
                case NoteLayout.CRAN:
                    txtNoteLayout.text = "C-RAN";
                    txtNoteLayout.color = new Color(1f, 231f / 255, 142f / 255);
                    txtNoteDesc.text = Const.RandomDescCR[(int)Const.Language];
                    break;
                case NoteLayout.CUSTOM:
                    txtNoteLayout.text = "CUSTOM";
                    txtNoteLayout.color = new Color(1f, 231f / 255, 142f / 255);
                    txtNoteDesc.text = Const.RandomDescCU[(int)Const.Language];
                    break;
                case NoteLayout.NORMAL:
                default:
                    txtNoteLayout.text = "NORMAL";
                    txtNoteLayout.color = Color.white;
                    txtNoteDesc.text = Const.RandomDescNR[(int)Const.Language];
                    break;
            }

            // 기어 스킨
            switch (Const.GearSkin)
            {
                case SkinType.NORMAL:
                    txtGearSkin.text = "NORMAL";
                    break;
                case SkinType.DARK:
                    txtGearSkin.text = "DARK";
                    break;
            }

            // 기어 사이즈
            InitSkinSize();

            // 노트 스킨
            switch (Const.NoteSkin)
            {
                case NoteSkin.NORMAL:
                    txtNoteSkin.text = "NORMAL";
                    break;
                case NoteSkin.PASTEL:
                    txtNoteSkin.text = "PASTEL";
                    break;
            }

            // 노트 사이즈
            switch (Const.NoteSize)
            {
                case NoteSize.NORMAL:
                    txtNoteSize.text = "NORMAL";
                    break;
                case NoteSize.SLIM:
                    txtNoteSize.text = "SLIM";
                    break;
                case NoteSize.FAT:
                    txtNoteSize.text = "FAT";
                    break;
            }

            // 그래프 사이즈
            switch (Const.GraphType)
            {
                case GraphType.NORMAL:
                    txtGraphType.text = "NORMAL";
                    break;
                case GraphType.SMALL:
                    txtGraphType.text = "SMALL";
                    break;
                case GraphType.MINI:
                    txtGraphType.text = "MINI";
                    break;
                case GraphType.OFFGEAR:
                    txtGraphType.text = "OFF(GEAR)";
                    break;
                case GraphType.OFFBGA:
                    txtGraphType.text = "OFF(BGA)";
                    break;
            }

            // UI 레이아웃
            switch (Const.LayoutType)
            {
                case UILayoutType.TYPEA:
                    txtLayoutType.text = "TYPE A";
                    txtLayoutDesc.text = Const.LayoutDesc[0, (int)Const.Language];
                    break;
                case UILayoutType.TYPEB:
                    txtLayoutType.text = "TYPE B";
                    txtLayoutDesc.text = Const.LayoutDesc[1, (int)Const.Language];
                    break;
            }

            // BGA
            switch(Const.BGAOnOff)
            {
                case 0:
                    txtBGAOnOff.text = "OFF";
                    break;
                case 1:
                    txtBGAOnOff.text = "ON";
                    break;
            }

            // 플레이사이드
            switch (Const.PlayerSide)
            {
                case 0:
                    txtPlaySide.text = "1P";
                    break;
                case 1:
                    txtPlaySide.text = "2P";
                    break;
            }

            // 패슬
            switch(Const.FastSlow)
            {
                case DisplayPosType.OFF:
                    txtFastSlow.text = "OFF";
                    break;
                case DisplayPosType.TYPEA:
                    txtFastSlow.text = "TYPE A";
                    break;
                case DisplayPosType.TYPEB:
                    txtFastSlow.text = "TYPE B";
                    break;
            }
            txtFastSlowDesc.text =
                Const.FSDiffDisplayDesc[(int)Const.FastSlow, (int)Const.Language];

            // 타겟 비교
            switch (Const.TargetDiff)
            {
                case DisplayPosType.OFF:
                    txtTargetDiff.text = "OFF";
                    break;
                case DisplayPosType.TYPEA:
                    txtTargetDiff.text = "TYPE A";
                    break;
                case DisplayPosType.TYPEB:
                    txtTargetDiff.text = "TYPE B";
                    break;
            }
            txtTargetDiffDesc.text =
                Const.FSDiffDisplayDesc[(int)Const.TargetDiff, (int)Const.Language];

            // 정확도
            switch (Const.RateDiff)
            {
                case DisplayPosType.OFF:
                    txtRateDiff.text = "OFF";
                    break;
                case DisplayPosType.TYPEA:
                    txtRateDiff.text = "TYPE A";
                    break;
                case DisplayPosType.TYPEB:
                    txtRateDiff.text = "TYPE B";
                    break;
            }
            txtRateDiffDesc.text =
                Const.FSDiffDisplayDesc[(int)Const.RateDiff, (int)Const.Language];
        }

        public void Start()
        {
            Page = 0;
            PageSet0.SetActive(true);
            PageSet1.SetActive(false);
        }

        public void Update()
        {
            if(MusicListUI.isCustomRandom)
            {
                MusicListUI.isCustomRandom = false;
                Const.NoteLayout = NoteLayout.CRAN;
                NoteLayoutChange();
            }

            if(Input.GetKeyDown(KeyCode.Tab))
            {
                PageChange();
            }

            if(Page == 0)
            {

                if (Input.GetKeyDown(KeyCode.F1))
                {
                    if (Const.SpdType == SpdType.STANDARD)
                    {
                        SpeedDownFixed();
                    }
                    else if (Const.SpdType == SpdType.CONSTANT)
                    {
                        SpeedDownFluid();
                    }
                }
                else if (Input.GetKeyDown(KeyCode.F2))
                {
                    if (Const.SpdType == SpdType.STANDARD)
                    {
                        SpeedUpFixed();
                    }
                    else if (Const.SpdType == SpdType.CONSTANT)
                    {
                        SpeedUpFluid();
                    }
                }
                else if (Input.GetKeyDown(KeyCode.F3))
                {
                    SpeedTypeChange();
                }
                else if (Input.GetKeyDown(KeyCode.F4))
                {
                    AutoOnOff();
                }
                else if (Input.GetKeyDown(KeyCode.F5))
                {
                    JudgeUIOnOff();
                }
                else if (Input.GetKeyDown(KeyCode.F6))
                {
                    GaugeTypeChange();
                }
                else if (Input.GetKeyDown(KeyCode.F7))
                {
                    GraphTargetChange();
                }
                else if (Input.GetKeyDown(KeyCode.F8))
                {
                    NoteLayoutChange();
                }
                else if (Input.GetKeyDown(KeyCode.F9))
                {
                    OpenCustomRandom();
                }
                else if (Input.GetKeyDown(KeyCode.F10))
                {
                    JudgeTypeChange();
                }
            }
            else if(Page == 1)
            {
                if (Input.GetKeyDown(KeyCode.F1))
                {
                    GearSkinChange();
                }
                else if (Input.GetKeyDown(KeyCode.F2))
                {
                    GraphSizeChange();
                }
                else if (Input.GetKeyDown(KeyCode.F3))
                {
                    NoteSkinChange();
                }
                else if (Input.GetKeyDown(KeyCode.F4))
                {
                    NoteSizeChange();
                }
                else if (Input.GetKeyDown(KeyCode.F5))
                {
                    GraphSizeChange();
                }
                else if (Input.GetKeyDown(KeyCode.F6))
                {
                    LayoutChange();
                }
                else if (Input.GetKeyDown(KeyCode.F7))
                {
                    BGAChange();
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.F1))
                {
                    PlaySideChange();
                }
                else if (Input.GetKeyDown(KeyCode.F2))
                {
                    FastSlowChange();
                }
                else if (Input.GetKeyDown(KeyCode.F3))
                {
                    TargetDiffChange();
                }
                else if (Input.GetKeyDown(KeyCode.F4))
                {
                    RateChange();
                }
            }

            if(gearSizeInit)
            {
                gearSizeInit = false;
                InitSkinSize();
            }

            if(judgeInit)
            {
                judgeInit = false;
                InitJudge();
            }
        }
        
        // Init
        private void InitSkinSize()
        {
            switch (Const.GearSize)
            {
                case SkinSize.STANDARD:
                    txtGearSize.text = "STANDARD";
                    break;
                case SkinSize.WIDE125:
                    txtGearSize.text = "WIDE(1.25)";
                    break;
                case SkinSize.WIDE150:
                    txtGearSize.text = "WIDE(1.50)";
                    break;
            }
        }

        private void InitJudge()
        {
            switch (Const.JudgeType)
            {
                case JudgeType.ARCADE:
                    txtJudgeType.text = "ARCADE";
                    break;
                case JudgeType.ORIGINAL:
                    txtJudgeType.text = "ORIGINAL";
                    break;
                case JudgeType.LR2:
                    txtJudgeType.text = "LR2";
                    break;
                case JudgeType.BEATORAJA:
                    txtJudgeType.text = "BEATORAJA";
                    break;
            }
        }

        // Changes
        private void PageChange()
        {
            if(Page == 0)
            {
                Page = 1;
                PageSet0.SetActive(false);
                PageSet1.SetActive(true);
                PageSet2.SetActive(false);
            }
            else if(Page == 1)
            {
                Page = 2;
                PageSet0.SetActive(false);
                PageSet1.SetActive(false);
                PageSet2.SetActive(true);
            }
            else if(Page == 2)
            {
                Page = 0;
                PageSet0.SetActive(true);
                PageSet1.SetActive(false);
                PageSet2.SetActive(false);
            }
            sfxPlay.PlayOneShot(sfxSource);
        }

        private void SpeedUpFixed()
        {
            if(speed < 2000) speed += 25;
            Const.SpeedStd = speed;

            // 폴더일 때는 변경 없이 처리
            if (Const.selectedOnList.Type == ItemType.BMS)
            {
                speedfl = (int)(Const.selectedOnList.Info.BPMstart * speed / 100);
                Const.SpeedCon = speedfl;
            }

            txtSpeed.text = ((float)speed/100).ToString("0.00") + "x";
            txtSpdAnother.text = "CON " + speedfl.ToString();
        }

        private void SpeedDownFixed()
        {
            if (speed > 50) speed -= 25;
            Const.SpeedStd = speed;

            // 폴더일 때는 변경 없이 처리
            if (Const.selectedOnList.Type == ItemType.BMS)
            {
                speedfl = (int)(Const.selectedOnList.Info.BPMstart * speed / 100);
                Const.SpeedCon = speedfl;
            }

            txtSpeed.text = ((float)speed / 100).ToString("0.00") + "x";
            txtSpdAnother.text = "CON " + speedfl.ToString();
        }

        private void SpeedUpFluid()
        {
            if (speedfl < 2000) speedfl += 25;
            Const.SpeedCon = speedfl;

            // 폴더일 때는 변경 없이 처리
            if (Const.selectedOnList.Type == ItemType.BMS)
            {
                speed = (int)((double)speedfl / Const.selectedOnList.Info.BPMstart * 100);
                Const.SpeedStd = speed;
            }

            txtSpeed.text = speedfl.ToString();
            txtSpdAnother.text = "STD " + ((float)speed / 100).ToString("0.00") + "x";
        }

        private void SpeedDownFluid()
        {
            if (speedfl > 100) speedfl -= 25;
            Const.SpeedCon = speedfl;

            // 폴더일 때는 변경 없이 처리
            if (Const.selectedOnList.Type == ItemType.BMS)
            {
                speed = (int)((double)speedfl / Const.selectedOnList.Info.BPMstart * 100);
                Const.SpeedStd = speed;
            }

            txtSpeed.text = speedfl.ToString();
            txtSpdAnother.text = "STD " + ((float)speed / 100).ToString("0.00") + "x";
        }

        private void JudgeUIOnOff()
        {
            if(Const.DisplayJudge == 0)
            {
                Const.DisplayJudge = 1;
                txtJudgePanel.text = "ON";
                txtJudgePanel.color = new Color(1f, 120f / 255, 0f);
            }
            else
            {
                Const.DisplayJudge = 0;
                txtJudgePanel.text = "OFF";
                txtJudgePanel.color = Color.white;
            }
            sfxPlay.PlayOneShot(sfxSource);
        }

        private void AutoOnOff()
        {
            switch (Const.Auto)
            {
                case AutoPlayType.OFF:
                    txtAuto.text = "TURNTABLE";
                    txtAuto.color = Color.magenta;
                    Const.Auto = AutoPlayType.TURNTABLE;
                    break;
                case AutoPlayType.TURNTABLE:
                    txtAuto.text = "ALL";
                    txtAuto.color = Color.red;
                    Const.Auto = AutoPlayType.ALL;
                    break;
                case AutoPlayType.ALL:
                    txtAuto.text = "OFF";
                    txtAuto.color = Color.white;
                    Const.Auto = AutoPlayType.OFF;
                    break;
            }
            sfxPlay.PlayOneShot(sfxSource);
        }

        private void GaugeTypeChange()
        {
            switch (Const.GaugeType)
            {
                case GaugeType.ASSISTED:
                    txtGauge.text = "EASY";
                    txtGauge.color = new Color(159f / 255, 1f, 180f / 255);
                    Const.GaugeType = GaugeType.EASY;
                    break;
                case GaugeType.EASY:
                    txtGauge.text = "NORMAL";
                    txtGauge.color = new Color(159f / 255, 215f / 255, 1f);
                    Const.GaugeType = GaugeType.NORMAL;
                    break;
                case GaugeType.NORMAL:
                    txtGauge.text = "HARD";
                    txtGauge.color = new Color(1f, 159f / 255, 159f / 255);
                    Const.GaugeType = GaugeType.HARD;
                    break;
                case GaugeType.HARD:
                    txtGauge.text = "EX-HARD";
                    txtGauge.color = new Color(1f, 207f / 255, 79f / 255);
                    Const.GaugeType = GaugeType.EXHARD;
                    break;
                case GaugeType.EXHARD:
                    txtGauge.text = "ASSISTED";
                    txtGauge.color = new Color(206f / 255, 159f / 255, 1f);
                    Const.GaugeType = GaugeType.ASSISTED;
                    break;
            }
            sfxPlay.PlayOneShot(sfxSource);
        }

        private void SpeedTypeChange()
        {
            if (Const.SpdType == SpdType.STANDARD)
            {
                txtSpdType.text = "CONSTANT";
                txtSpdType.color = new Color(80f / 255, 201f / 255, 125f / 255);
                Const.SpdType = SpdType.CONSTANT;
                txtSpeed.text = speedfl.ToString();
                txtSpdAnother.text = "STD " + ((float)speed / 100).ToString("0.00") + "x";
            }
            else
            {
                txtSpdType.text = "STANDARD";
                txtSpdType.color = new Color(80f / 255, 201f / 255, 125f / 255);
                Const.SpdType = SpdType.STANDARD;
                txtSpeed.text = ((float)speed/100).ToString("0.00");
                txtSpdAnother.text = "CON " + speedfl.ToString();
            }
            sfxPlay.PlayOneShot(sfxSource);
        }

        private void GraphTargetChange()
        {
            switch (Const.GraphTarget)
            {
                case GraphTargetType.OFF:
                    txtTarget.text = "A";
                    txtTarget.color = new Color(196f / 255, 52f / 255, 174f / 255);
                    Const.GraphTarget = GraphTargetType.A;
                    break;
                case GraphTargetType.A:
                    txtTarget.text = "AA";
                    txtTarget.color = new Color(1f, 133f / 255, 45f / 255);
                    Const.GraphTarget = GraphTargetType.AA;
                    break;
                case GraphTargetType.AA:
                    txtTarget.text = "AAA";
                    txtTarget.color = new Color(1f, 64f / 255, 64f / 255);
                    Const.GraphTarget = GraphTargetType.AAA;
                    break;
                case GraphTargetType.AAA:
                    txtTarget.text = "MAX";
                    txtTarget.color = new Color(164f / 255, 71f / 255, 117f / 255);
                    Const.GraphTarget = GraphTargetType.MAX;
                    break;
                case GraphTargetType.MAX:
                default:
                    txtTarget.text = "OFF";
                    txtTarget.color = Color.white;
                    Const.GraphTarget = GraphTargetType.OFF;
                    break;
            }
            sfxPlay.PlayOneShot(sfxSource);
        }

        private void NoteLayoutChange()
        {
            switch (Const.NoteLayout)
            {
                case NoteLayout.NORMAL:
                    txtNoteLayout.text = "RANDOM";
                    txtNoteLayout.color = new Color(1f, 231f / 255, 142f / 255);
                    txtNoteDesc.text = Const.RandomDescRD[(int)Const.Language];
                    Const.NoteLayout = NoteLayout.RANDOM;
                    break;
                case NoteLayout.RANDOM:
                    txtNoteLayout.text = "MIRROR";
                    txtNoteDesc.text = Const.RandomDescMR[(int)Const.Language];
                    Const.NoteLayout = NoteLayout.MIRROR;
                    break;
                case NoteLayout.MIRROR:
                    txtNoteLayout.text = "S-RAN";
                    txtNoteDesc.text = Const.RandomDescSR[(int)Const.Language];
                    Const.NoteLayout = NoteLayout.SRAN;
                    break;
                case NoteLayout.SRAN:
                    txtNoteLayout.text = "C-RAN";
                    txtNoteDesc.text = Const.RandomDescCR[(int)Const.Language];
                    Const.NoteLayout = NoteLayout.CRAN;
                    break;
                case NoteLayout.CRAN:
                    txtNoteLayout.text = "CUSTOM";
                    txtNoteLayout.color = new Color(1f, 231f / 255, 142f / 255);
                    txtNoteDesc.text = Const.RandomDescCU[(int)Const.Language];
                    Const.NoteLayout = NoteLayout.CUSTOM;
                    break;
                case NoteLayout.CUSTOM:
                    txtNoteLayout.text = "NORMAL";
                    txtNoteLayout.color = Color.white;
                    txtNoteDesc.text = Const.RandomDescNR[(int)Const.Language];
                    Const.NoteLayout = NoteLayout.NORMAL;
                    break;
            }
            sfxPlay.PlayOneShot(sfxSource);
        }

        private void OpenCustomRandom()
        {
            MusicListUI.SetNotOnTop();
            customRandom.SetActive(true);
            GetComponent<CustomPattern>().EnableWindow();
        }

        private void JudgeTypeChange()
        {
            switch (Const.JudgeType)
            {
                case JudgeType.ARCADE:
                    txtJudgeType.text = "ORIGINAL";
                    Const.JudgeType = JudgeType.ORIGINAL;
                    break;
                case JudgeType.ORIGINAL:
                    txtJudgeType.text = "LR2";
                    Const.JudgeType = JudgeType.LR2;
                    break;
                case JudgeType.LR2:
                    txtJudgeType.text = "BEATORAJA";
                    Const.JudgeType = JudgeType.BEATORAJA;
                    break;
                case JudgeType.BEATORAJA:
                    txtJudgeType.text = "ARCADE";
                    Const.JudgeType = JudgeType.ARCADE;
                    break;
            }
            sfxPlay.PlayOneShot(sfxSource);
        }

        private void GearSkinChange()
        {
            switch (Const.GearSkin)
            {
                case SkinType.NORMAL:
                    Const.GearSkin = SkinType.DARK;
                    txtGearSkin.text = "DARK";
                    break;
                case SkinType.DARK:
                    Const.GearSkin = SkinType.NORMAL;
                    txtGearSkin.text = "NORMAL";
                    break;
            }
            sfxPlay.PlayOneShot(sfxSource);
        }

        private void GearSizeChange()
        {
            switch (Const.GearSize)
            {
                case SkinSize.STANDARD:
                    Const.GearSize = SkinSize.WIDE125;
                    txtGearSize.text = "WIDE(1.25)";
                    break;
                case SkinSize.WIDE125:
                    Const.GearSize = SkinSize.WIDE150;
                    txtGearSize.text = "WIDE(1.50)";
                    break;
                case SkinSize.WIDE150:
                    Const.GearSize = SkinSize.STANDARD;
                    txtGearSize.text = "STANDARD";
                    break;
            }
            sfxPlay.PlayOneShot(sfxSource);
        }

        private void NoteSkinChange()
        {
            // 노트 스킨
            switch (Const.NoteSkin)
            {
                case NoteSkin.NORMAL:
                    Const.NoteSkin = NoteSkin.PASTEL;
                    txtNoteSkin.text = "PASTEL";
                    break;
                case NoteSkin.PASTEL:
                    Const.NoteSkin = NoteSkin.NORMAL;
                    txtNoteSkin.text = "NORMAL";
                    break;
            }
            sfxPlay.PlayOneShot(sfxSource);
        }

        private void NoteSizeChange()
        {
            // 노트 사이즈
            switch (Const.NoteSize)
            {
                case NoteSize.NORMAL:
                    Const.NoteSize = NoteSize.SLIM;
                    txtNoteSize.text = "SLIM";
                    break;
                case NoteSize.SLIM:
                    Const.NoteSize = NoteSize.FAT;
                    txtNoteSize.text = "FAT";
                    break;
                case NoteSize.FAT:
                    Const.NoteSize = NoteSize.NORMAL;
                    txtNoteSize.text = "NORMAL";
                    break;
            }
            sfxPlay.PlayOneShot(sfxSource);
        }

        private void GraphSizeChange()
        {
            switch (Const.GraphType)
            {
                case GraphType.NORMAL:
                    Const.GraphType = GraphType.SMALL;
                    txtGraphType.text = "SMALL";
                    break;
                case GraphType.SMALL:
                    Const.GraphType = GraphType.MINI;
                    txtGraphType.text = "MINI";
                    break;
                case GraphType.MINI:
                    Const.GraphType = GraphType.OFFGEAR;
                    txtGraphType.text = "OFF(GEAR)";
                    break;
                case GraphType.OFFGEAR:
                    Const.GraphType = GraphType.OFFBGA;
                    txtGraphType.text = "OFF(BGA)";
                    break;
                case GraphType.OFFBGA:
                    Const.GraphType = GraphType.NORMAL;
                    txtGraphType.text = "NORMAL";
                    break;
            }
            sfxPlay.PlayOneShot(sfxSource);
        }

        private void LayoutChange()
        {
            switch(Const.LayoutType)
            {
                case UILayoutType.TYPEA:
                    Const.LayoutType = UILayoutType.TYPEB;
                    txtLayoutType.text = "TYPE B";
                    txtLayoutDesc.text = Const.LayoutDesc[1, (int)Const.Language];
                    break;
                case UILayoutType.TYPEB:
                    Const.LayoutType = UILayoutType.TYPEA;
                    txtLayoutType.text = "TYPE A";
                    txtLayoutDesc.text = Const.LayoutDesc[0, (int)Const.Language];
                    break;
            }
            sfxPlay.PlayOneShot(sfxSource);
        }

        private void BGAChange()
        {
            switch (Const.BGAOnOff)
            {
                case 0:
                    Const.BGAOnOff = 1;
                    txtBGAOnOff.text = "ON";
                    break;
                case 1:
                    Const.BGAOnOff = 0;
                    txtBGAOnOff.text = "OFF";
                    break;
            }
            sfxPlay.PlayOneShot(sfxSource);
        }

        private void PlaySideChange()
        {
            switch (Const.PlayerSide)
            {
                case 0:
                    Const.PlayerSide = 1;
                    txtPlaySide.text = "2P";
                    break;
                case 1:
                    Const.PlayerSide = 0;
                    txtPlaySide.text = "1P";
                    break;
            }
            sfxPlay.PlayOneShot(sfxSource);
        }

        private void FastSlowChange()
        {
            switch (Const.FastSlow)
            {
                case DisplayPosType.OFF:
                    txtFastSlow.text = "TYPE A";
                    Const.FastSlow = DisplayPosType.TYPEA;
                    break;
                case DisplayPosType.TYPEA:
                    txtFastSlow.text = "TYPE B";
                    Const.FastSlow = DisplayPosType.TYPEB;
                    break;
                case DisplayPosType.TYPEB:
                    txtFastSlow.text = "OFF";
                    Const.FastSlow = DisplayPosType.OFF;
                    break;
            }
            txtFastSlowDesc.text =
                Const.FSDiffDisplayDesc[(int)Const.FastSlow, (int)Const.Language];
            sfxPlay.PlayOneShot(sfxSource);
        }

        private void TargetDiffChange()
        {
            switch (Const.TargetDiff)
            {
                case DisplayPosType.OFF:
                    txtTargetDiff.text = "TYPE A";
                    Const.TargetDiff = DisplayPosType.TYPEA;
                    break;
                case DisplayPosType.TYPEA:
                    txtTargetDiff.text = "TYPE B";
                    Const.TargetDiff = DisplayPosType.TYPEB;
                    break;
                case DisplayPosType.TYPEB:
                    txtTargetDiff.text = "OFF";
                    Const.TargetDiff = DisplayPosType.OFF;
                    break;
            }
            txtTargetDiffDesc.text =
                Const.FSDiffDisplayDesc[(int)Const.TargetDiff, (int)Const.Language];
            sfxPlay.PlayOneShot(sfxSource);
        }

        private void RateChange()
        {
            switch (Const.RateDiff)
            {
                case DisplayPosType.OFF:
                    txtRateDiff.text = "TYPE A";
                    Const.RateDiff = DisplayPosType.TYPEA;
                    break;
                case DisplayPosType.TYPEA:
                    txtRateDiff.text = "TYPE B";
                    Const.RateDiff = DisplayPosType.TYPEB;
                    break;
                case DisplayPosType.TYPEB:
                    txtRateDiff.text = "OFF";
                    Const.RateDiff = DisplayPosType.OFF;
                    break;
            }
            txtRateDiffDesc.text =
                Const.FSDiffDisplayDesc[(int)Const.RateDiff, (int)Const.Language];
            sfxPlay.PlayOneShot(sfxSource);
        }
    }
}
