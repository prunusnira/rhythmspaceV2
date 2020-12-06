using UnityEngine;
using UnityEngine.UI;

namespace BMSPlayer
{
    public class PlayOptionSetting: Setting
    {
        public static bool langChange = false;

        public Text desc;

        /*
         * Game Play Settings
         */
        // Speed
        public Text titleSpeed;
        public Button btnSpdDown;
        public Button btnSpdUp;
        public Text txtSpeed;
        private int speed;
        private int speedfl;

        // Speed Type
        public Text titleSpdType;
        public Button btnSpdStd;
        public Button btnSpdCon;

        // Gauge Type
        public Text titleGaugeType;
        public Button btnGaugeLeft;
        public Button btnGaugeRight;
        public Text txtGauge;

        // Note Layout
        public Text titleNoteLayout;
        public Button btnLayoutLeft;
        public Button btnLayoutRight;
        public Text txtNoteLayout;
        public Button btnCustomRandom;
        public GameObject customRandom;

        // Judgement Type
        public Text titleJudgeType;
        public Button btnJudgeTypeLeft;
        public Button btnJudgeTypeRight;
        public Text txtJudgeType;

        // Auto
        public Text titleAuto;
        public Button btnAutoLeft;
        public Button btnAutoRight;
        public Text txtAuto;

        // Graph Target
        public Text titleGraphTarget;
        public Button btnTargetLeft;
        public Button btnTargetRight;
        public Text txtTarget;

        // PlaySide
        public Text titlePlaySide;
        public Button btnPlaySideLeft;
        public Button btnPlaySideRight;
        public Text txtPlaySide;

        /*
         * In-Game UI Settings
         */
        // GearSkin
        public Text titleGearSkin;
        public Button btnGearSkinLeft;
        public Button btnGearSkinRight;
        public Text txtGearSkin;

        // GearSize
        public Text titleGearSize;
        public Button btnGearSizeLeft;
        public Button btnGearSizeRight;
        public Text txtGearSize;

        // NoteSkin
        public Text titleNoteSkin;
        public Button btnNoteSkinLeft;
        public Button btnNoteSkinRight;
        public Text txtNoteSkin;

        // NoteSize
        public Text titleNoteSize;
        public Button btnNoteSizeLeft;
        public Button btnNoteSizeRight;
        public Text txtNoteSize;

        // GraphSize
        public Text titleGraphType;
        public Button btnGraphTypeLeft;
        public Button btnGraphTypeRight;
        public Text txtGraphType;

        // UI Layout
        public Text titleUILayout;
        public Button btnUILayoutTypeLeft;
        public Button btnUILayoutTypeRight;
        public Text txtUILayoutType;
        public Text txtUILayoutDesc;

        // BGA ON/OFF
        public Text titleBGA;
        public Button btnBGALeft;
        public Button btnBGARight;
        public Text txtBGAOnOff;

        // Judge Panel
        public Text titleJudgePanel;
        public Button btnJudgePanelLeft;
        public Button btnJudgePanelRight;
        public Text txtJudgePanel;

        /*
         * In-Game UI Settings
         */
        // Fast/Slow
        public Text titleFastSlow;
        public Button btnFastSlowLeft;
        public Button btnFastSlowRight;
        public Text txtFastSlow;

        // Target Difference
        public Text titleTargetDiff;
        public Button btnTargetDiffLeft;
        public Button btnTargetDiffRight;
        public Text txtTargetDiff;

        // Rate diff
        public Text titleRate;
        public Button btnRateLeft;
        public Button btnRateRight;
        public Text txtRateDiff;

        // SFX Play
        public AudioSource sfxPlay;
        public AudioClip sfxSource;

        // Initial Setting
        public static bool gearSizeInit = false;
        public static bool judgeInit = false;

        public override void Awake()
        {
            base.Awake();

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

            btnSpdStd.onClick.AddListener(delegate {
                SpeedTypeChange(false);
            });
            btnSpdCon.onClick.AddListener(delegate {
                SpeedTypeChange(true);
            });

            btnGaugeLeft.onClick.AddListener(delegate {
                GaugeTypeChange(false);
            });
            btnGaugeRight.onClick.AddListener(delegate {
                GaugeTypeChange(true);
            });

            btnLayoutLeft.onClick.AddListener(delegate {
                NoteLayoutChange(false);
            });
            btnLayoutRight.onClick.AddListener(delegate {
                NoteLayoutChange(true);
            });
            btnCustomRandom.onClick.AddListener(OpenCustomRandom);

            btnJudgeTypeLeft.onClick.AddListener(delegate {
                JudgeTypeChange(false);
            });
            btnJudgeTypeRight.onClick.AddListener(delegate {
                JudgeTypeChange(true);
            });

            btnAutoLeft.onClick.AddListener(AutoOnOff);
            btnAutoRight.onClick.AddListener(AutoOnOff);

            btnTargetLeft.onClick.AddListener(delegate {
                GraphTargetChange(false);
            });
            btnTargetRight.onClick.AddListener(delegate {
                GraphTargetChange(true);
            });

            btnPlaySideLeft.onClick.AddListener(PlaySideChange);
            btnPlaySideRight.onClick.AddListener(PlaySideChange);

            // Page 2
            btnGearSkinLeft.onClick.AddListener(GearSkinChange);
            btnGearSkinRight.onClick.AddListener(GearSkinChange);

            btnGearSizeLeft.onClick.AddListener(delegate {
                GearSizeChange(false);
            });
            btnGearSizeRight.onClick.AddListener(delegate {
                GearSizeChange(true);
            });

            btnNoteSkinLeft.onClick.AddListener(NoteSkinChange);
            btnNoteSkinRight.onClick.AddListener(NoteSkinChange);

            btnNoteSizeLeft.onClick.AddListener(delegate {
                NoteSizeChange(false);
            });
            btnNoteSizeRight.onClick.AddListener(delegate {
                NoteSizeChange(true);
            });

            btnGraphTypeLeft.onClick.AddListener(delegate {
                GraphSizeChange(false);
            });
            btnGraphTypeRight.onClick.AddListener(delegate {
                GraphSizeChange(true);
            });

            btnUILayoutTypeLeft.onClick.AddListener(LayoutChange);
            btnUILayoutTypeRight.onClick.AddListener(LayoutChange);

            btnBGALeft.onClick.AddListener(BGAChange);
            btnBGARight.onClick.AddListener(BGAChange);

            // Page 3
            btnJudgePanelLeft.onClick.AddListener(JudgeUIOnOff);
            btnJudgePanelRight.onClick.AddListener(JudgeUIOnOff);

            btnFastSlowLeft.onClick.AddListener(FastSlowChange);
            btnFastSlowRight.onClick.AddListener(FastSlowChange);

            btnTargetDiffLeft.onClick.AddListener(TargetDiffChange);
            btnTargetDiffRight.onClick.AddListener(TargetDiffChange);

            btnRateLeft.onClick.AddListener(RateChange);
            btnRateRight.onClick.AddListener(RateChange);

            // 오토 설정
            switch (Const.Auto)
            {
                case AutoPlayType.OFF:
                    txtAuto.text = "OFF";
                    break;
                case AutoPlayType.TURNTABLE:
                    txtAuto.text = "TURNTABLE";
                    break;
                case AutoPlayType.ALL:
                    txtAuto.text = "ALL";
                    break;
            }

            // 판정 표시 설정
            if(Const.DisplayJudge == 0)
            {
                txtJudgePanel.text = "OFF";
            }
            else
            {
                txtJudgePanel.text = "ON";
            }

            // 속도
            if(Const.SpdType == SpdType.STANDARD)
            {
                speed = Const.SpeedStd;
                txtSpeed.text = ((float)speed / 100).ToString("0.00") + "x";
            }
            else
            {
                speedfl = Const.SpeedCon;
                txtSpeed.text = speedfl.ToString();
            }

            // 게이지 타입
            switch(Const.GaugeType)
            {
                case GaugeType.ASSISTED:
                    txtGauge.text = "ASSISTED";
                    break;
                case GaugeType.EASY:
                    txtGauge.text = "EASY";
                    break;
                case GaugeType.NORMAL:
                    txtGauge.text = "NORMAL";
                    break;
                case GaugeType.HARD:
                    txtGauge.text = "HARD";
                    break;
                case GaugeType.EXHARD:
                    txtGauge.text = "EX-HARD";
                    break;
            }

            // 그래프
            switch(Const.GraphTarget)
            {
                case GraphTargetType.RECORD:
                    txtTarget.text = "MY RECORD";
                    break;
                case GraphTargetType.A:
                    txtTarget.text = "A";
                    break;
                case GraphTargetType.AA:
                    txtTarget.text = "AA";
                    break;
                case GraphTargetType.AAA:
                    txtTarget.text = "AAA";
                    break;
                case GraphTargetType.MAX:
                    txtTarget.text = "MAX";
                    break;
                case GraphTargetType.OFF:
                default:
                    txtTarget.text = "OFF";
                    break;
            }

            // 판정
            InitJudge();

            // 배치
            switch(Const.NoteLayout)
            {
                case NoteLayout.RANDOM:
                    txtNoteLayout.text = "RANDOM";
                    break;
                case NoteLayout.MIRROR:
                    txtNoteLayout.text = "MIRROR";
                    break;
                case NoteLayout.SRAN:
                    txtNoteLayout.text = "S-RAN";
                    break;
                case NoteLayout.CRAN:
                    txtNoteLayout.text = "C-RAN";
                    break;
                case NoteLayout.CUSTOM:
                    txtNoteLayout.text = "CUSTOM";
                    break;
                case NoteLayout.NORMAL:
                default:
                    txtNoteLayout.text = "NORMAL";
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
                    txtUILayoutType.text = "TYPE A";
                    txtUILayoutDesc.text = Const.LayoutDesc[0, (int)Const.Language];
                    break;
                case UILayoutType.TYPEB:
                    txtUILayoutType.text = "TYPE B";
                    txtUILayoutDesc.text = Const.LayoutDesc[1, (int)Const.Language];
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

            UpdateOption();
        }

        public void UpdateOption()
        {
            desc.text = Const.playopDesc[(int)Const.Language];
            titleSpeed.text = Const.playopSpeed[(int)Const.Language];
            titleSpdType.text = Const.playopSpdType[(int)Const.Language];
            titleGaugeType.text = Const.playopGaugeType[(int)Const.Language];
            titleNoteLayout.text = Const.playopNoteLayout[(int)Const.Language];
            titleJudgeType.text = Const.playopJudgeType[(int)Const.Language];
            titleAuto.text = Const.playopAuto[(int)Const.Language];
            titleGraphTarget.text = Const.playopGraphTarget[(int)Const.Language];
            titlePlaySide.text = Const.playopPlaySide[(int)Const.Language];
            titleGearSkin.text = Const.playopGearSkin[(int)Const.Language];
            titleGearSize.text = Const.playopGearSize[(int)Const.Language];
            titleNoteSkin.text = Const.playopNoteSkin[(int)Const.Language];
            titleNoteSize.text = Const.playopNoteSize[(int)Const.Language];
            titleGraphType.text = Const.playopGraphType[(int)Const.Language];
            titleUILayout.text = Const.playopUILayout[(int)Const.Language];
            titleBGA.text = Const.playopBGA[(int)Const.Language];
            titleJudgePanel.text = Const.playopJudgePanel[(int)Const.Language];
            titleFastSlow.text = Const.playopFastSlow[(int)Const.Language];
            titleTargetDiff.text = Const.playopTargetDiff[(int)Const.Language];
            titleRate.text = Const.playopRate[(int)Const.Language];
        }

        public override void Update()
        {
            base.Update();

            if(MusicListUI.isCustomRandom)
            {
                MusicListUI.isCustomRandom = false;
                Const.NoteLayout = NoteLayout.CRAN;
                txtNoteLayout.text = "CUSTOM";
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

            if(langChange)
            {
                langChange = false;
                UpdateOption();
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
        }

        private void JudgeUIOnOff()
        {
            if(Const.DisplayJudge == 0)
            {
                Const.DisplayJudge = 1;
                txtJudgePanel.text = "ON";
            }
            else
            {
                Const.DisplayJudge = 0;
                txtJudgePanel.text = "OFF";
            }
            sfxPlay.PlayOneShot(sfxSource);
        }

        private void AutoOnOff()
        {
            switch (Const.Auto)
            {
                case AutoPlayType.OFF:
                    txtAuto.text = "TURNTABLE";
                    Const.Auto = AutoPlayType.TURNTABLE;
                    break;
                case AutoPlayType.TURNTABLE:
                    txtAuto.text = "ALL";
                    Const.Auto = AutoPlayType.ALL;
                    break;
                case AutoPlayType.ALL:
                    txtAuto.text = "OFF";
                    Const.Auto = AutoPlayType.OFF;
                    break;
            }
            sfxPlay.PlayOneShot(sfxSource);
        }

        private void GaugeTypeChange(bool next)
        {
            if(next)
            {
                switch (Const.GaugeType)
                {
                    case GaugeType.ASSISTED:
                        txtGauge.text = "EASY";
                        Const.GaugeType = GaugeType.EASY;
                        break;
                    case GaugeType.EASY:
                        txtGauge.text = "NORMAL";
                        Const.GaugeType = GaugeType.NORMAL;
                        break;
                    case GaugeType.NORMAL:
                        txtGauge.text = "HARD";
                        Const.GaugeType = GaugeType.HARD;
                        break;
                    case GaugeType.HARD:
                        txtGauge.text = "EX-HARD";
                        Const.GaugeType = GaugeType.EXHARD;
                        break;
                    case GaugeType.EXHARD:
                        txtGauge.text = "ASSISTED";
                        Const.GaugeType = GaugeType.ASSISTED;
                        break;
                }
            }
            else
            {
                switch (Const.GaugeType)
                {
                    case GaugeType.ASSISTED:
                        txtGauge.text = "EX-HARD";
                        Const.GaugeType = GaugeType.EXHARD;
                        break;
                    case GaugeType.EASY:
                        txtGauge.text = "ASSISTED";
                        Const.GaugeType = GaugeType.ASSISTED;
                        break;
                    case GaugeType.NORMAL:
                        txtGauge.text = "EASY";
                        Const.GaugeType = GaugeType.EASY;
                        break;
                    case GaugeType.HARD:
                        txtGauge.text = "NORMAL";
                        Const.GaugeType = GaugeType.NORMAL;
                        break;
                    case GaugeType.EXHARD:
                        txtGauge.text = "HARD";
                        Const.GaugeType = GaugeType.HARD;
                        break;
                }
            }
            sfxPlay.PlayOneShot(sfxSource);
        }

        private void SpeedTypeChange(bool con)
        {
            if(con)
            {
                Const.SpdType = SpdType.CONSTANT;
                txtSpeed.text = speedfl.ToString();
            }
            else
            {
                Const.SpdType = SpdType.STANDARD;
                txtSpeed.text = ((float)speed / 100).ToString("0.00");
            }
            sfxPlay.PlayOneShot(sfxSource);
        }

        private void GraphTargetChange(bool next)
        {
            if(next)
            {
                switch (Const.GraphTarget)
                {
                    case GraphTargetType.OFF:
                        txtTarget.text = "MY RECORD";
                        Const.GraphTarget = GraphTargetType.RECORD;
                        break;
                    case GraphTargetType.RECORD:
                        txtTarget.text = "A";
                        Const.GraphTarget = GraphTargetType.A;
                        break;
                    case GraphTargetType.A:
                        txtTarget.text = "AA";
                        Const.GraphTarget = GraphTargetType.AA;
                        break;
                    case GraphTargetType.AA:
                        txtTarget.text = "AAA";
                        Const.GraphTarget = GraphTargetType.AAA;
                        break;
                    case GraphTargetType.AAA:
                        txtTarget.text = "MAX";
                        Const.GraphTarget = GraphTargetType.MAX;
                        break;
                    case GraphTargetType.MAX:
                    default:
                        txtTarget.text = "OFF";
                        Const.GraphTarget = GraphTargetType.OFF;
                        break;
                }
            }
            else
            {
                switch (Const.GraphTarget)
                {
                    case GraphTargetType.OFF:
                        txtTarget.text = "MAX";
                        Const.GraphTarget = GraphTargetType.MAX;
                        break;
                    case GraphTargetType.RECORD:
                        txtTarget.text = "OFF";
                        Const.GraphTarget = GraphTargetType.OFF;
                        break;
                    case GraphTargetType.A:
                        txtTarget.text = "MY RECORD";
                        Const.GraphTarget = GraphTargetType.RECORD;
                        break;
                    case GraphTargetType.AA:
                        txtTarget.text = "A";
                        Const.GraphTarget = GraphTargetType.A;
                        break;
                    case GraphTargetType.AAA:
                        txtTarget.text = "AA";
                        Const.GraphTarget = GraphTargetType.AA;
                        break;
                    case GraphTargetType.MAX:
                    default:
                        txtTarget.text = "AAA";
                        Const.GraphTarget = GraphTargetType.AAA;
                        break;
                }
            }
            sfxPlay.PlayOneShot(sfxSource);
        }

        private void NoteLayoutChange(bool next)
        {
            if(next)
            {
                switch (Const.NoteLayout)
                {
                    case NoteLayout.NORMAL:
                        txtNoteLayout.text = "RANDOM";
                        Const.NoteLayout = NoteLayout.RANDOM;
                        break;
                    case NoteLayout.RANDOM:
                        txtNoteLayout.text = "MIRROR";
                        Const.NoteLayout = NoteLayout.MIRROR;
                        break;
                    case NoteLayout.MIRROR:
                        txtNoteLayout.text = "S-RAN";
                        Const.NoteLayout = NoteLayout.SRAN;
                        break;
                    case NoteLayout.SRAN:
                        txtNoteLayout.text = "C-RAN";
                        Const.NoteLayout = NoteLayout.CRAN;
                        break;
                    case NoteLayout.CRAN:
                        txtNoteLayout.text = "CUSTOM";
                        Const.NoteLayout = NoteLayout.CUSTOM;
                        break;
                    case NoteLayout.CUSTOM:
                        txtNoteLayout.text = "NORMAL";
                        Const.NoteLayout = NoteLayout.NORMAL;
                        break;
                }
            }
            else
            {
                switch (Const.NoteLayout)
                {
                    case NoteLayout.NORMAL:
                        txtNoteLayout.text = "CUSTOM";
                        Const.NoteLayout = NoteLayout.CUSTOM;
                        break;
                    case NoteLayout.RANDOM:
                        txtNoteLayout.text = "NORMAL";
                        Const.NoteLayout = NoteLayout.NORMAL;
                        break;
                    case NoteLayout.MIRROR:
                        txtNoteLayout.text = "RANDOM";
                        Const.NoteLayout = NoteLayout.RANDOM;
                        break;
                    case NoteLayout.SRAN:
                        txtNoteLayout.text = "MIRROR";
                        Const.NoteLayout = NoteLayout.MIRROR;
                        break;
                    case NoteLayout.CRAN:
                        txtNoteLayout.text = "S-RAN";
                        Const.NoteLayout = NoteLayout.SRAN;
                        break;
                    case NoteLayout.CUSTOM:
                        txtNoteLayout.text = "C-RAN";
                        Const.NoteLayout = NoteLayout.CRAN;
                        break;
                }
            }
            sfxPlay.PlayOneShot(sfxSource);
        }

        private void OpenCustomRandom()
        {
            MusicListUI.SetNotOnTop();
            customRandom.SetActive(true);
            GetComponent<CustomPattern>().EnableWindow();
        }

        private void JudgeTypeChange(bool next)
        {
            if(next)
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
            }
            else
            {
                switch (Const.JudgeType)
                {
                    case JudgeType.ARCADE:
                        txtJudgeType.text = "BEATORAJA";
                        Const.JudgeType = JudgeType.BEATORAJA;
                        break;
                    case JudgeType.ORIGINAL:
                        txtJudgeType.text = "ARCADE";
                        Const.JudgeType = JudgeType.ARCADE;
                        break;
                    case JudgeType.LR2:
                        txtJudgeType.text = "ORIGINAL";
                        Const.JudgeType = JudgeType.ORIGINAL;
                        break;
                    case JudgeType.BEATORAJA:
                        txtJudgeType.text = "LR2";
                        Const.JudgeType = JudgeType.LR2;
                        break;
                }
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

        private void GearSizeChange(bool next)
        {
            if(next)
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
            }
            else
            {
                switch (Const.GearSize)
                {
                    case SkinSize.STANDARD:
                        Const.GearSize = SkinSize.WIDE150;
                        txtGearSize.text = "WIDE(1.50)";
                        break;
                    case SkinSize.WIDE125:
                        Const.GearSize = SkinSize.STANDARD;
                        txtGearSize.text = "STANDARD";
                        break;
                    case SkinSize.WIDE150:
                        Const.GearSize = SkinSize.WIDE125;
                        txtGearSize.text = "WIDE(1.25)";
                        break;
                }
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

        private void NoteSizeChange(bool next)
        {
            if(next)
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
            }
            else
            {
                // 노트 사이즈
                switch (Const.NoteSize)
                {
                    case NoteSize.NORMAL:
                        Const.NoteSize = NoteSize.FAT;
                        txtNoteSize.text = "FAT";
                        break;
                    case NoteSize.SLIM:
                        Const.NoteSize = NoteSize.NORMAL;
                        txtNoteSize.text = "NORMAL";
                        break;
                    case NoteSize.FAT:
                        Const.NoteSize = NoteSize.SLIM;
                        txtNoteSize.text = "SLIM";
                        break;
                }
            }
            sfxPlay.PlayOneShot(sfxSource);
        }

        private void GraphSizeChange(bool next)
        {
            if(next)
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
            }
            else
            {
                switch (Const.GraphType)
                {
                    case GraphType.NORMAL:
                        Const.GraphType = GraphType.OFFBGA;
                        txtGraphType.text = "OFF(BGA)";
                        break;
                    case GraphType.SMALL:
                        Const.GraphType = GraphType.NORMAL;
                        txtGraphType.text = "NORMAL";
                        break;
                    case GraphType.MINI:
                        Const.GraphType = GraphType.SMALL;
                        txtGraphType.text = "SMALL";
                        break;
                    case GraphType.OFFGEAR:
                        Const.GraphType = GraphType.MINI;
                        txtGraphType.text = "MINI";
                        break;
                    case GraphType.OFFBGA:
                        Const.GraphType = GraphType.OFFGEAR;
                        txtGraphType.text = "OFF(GEAR)";
                        break;
                }
            }
            sfxPlay.PlayOneShot(sfxSource);
        }

        private void LayoutChange()
        {
            switch(Const.LayoutType)
            {
                case UILayoutType.TYPEA:
                    Const.LayoutType = UILayoutType.TYPEB;
                    txtUILayoutType.text = "TYPE B";
                    txtUILayoutDesc.text = Const.LayoutDesc[1, (int)Const.Language];
                    break;
                case UILayoutType.TYPEB:
                    Const.LayoutType = UILayoutType.TYPEA;
                    txtUILayoutType.text = "TYPE A";
                    txtUILayoutDesc.text = Const.LayoutDesc[0, (int)Const.Language];
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
            sfxPlay.PlayOneShot(sfxSource);
        }
    }
}
