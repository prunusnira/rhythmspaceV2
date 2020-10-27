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
        public GameObject PageSet0;
        public GameObject PageSet1;

        // Speed
        public Text txtSpeed;
        public Text txtSpdAnother;
        private int speed;
        private int speedfl;

        // Speed Type
        public Text txtSpdType;

        // Auto
        public Text txtAuto;

        // Judge
        public Text txtJudgePanel;

        // Gauge Type
        public Text txtGauge;

        // Target Graph Type
        public Text txtTarget;

        // Note Layout
        public Text txtNoteLayout;
        public Text txtNoteDesc;

        // Judgement Type
        public Text txtJudgeType;

        // Custom Random
        public GameObject customRandom;

        // Skin
        public Text txtSkin;

        // GraphSize
        public Text txtGraphType;

        // Layout
        public Text txtLayoutType;
        public Text txtLayoutDesc;

        // BGA ON/OFF
        public Text txtBGAOnOff;

        // PlaySide
        public Text txtPlaySide;

        // Fast/Slow
        public Text txtFastSlow;
        public Text txtFastSlowDesc;

        // Target Difference
        public Text txtTargetDiff;
        public Text txtTargetDiffDesc;

        // Rate diff
        public Text txtRateDiff;
        public Text txtRateDiffDesc;

        public void Awake()
        {
            // 현재 값 가져오기

            // 오토 설정
            switch(Const.Auto)
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
            if(Const.SpdType == SpdType.FIXED)
            {
                speed = Const.SpeedFixed;
                txtSpdType.text = "FIXED";
                txtSpdType.color = new Color(80f / 255, 201f / 255, 125f / 255);
                txtSpeed.text = ((float)speed / 100).ToString("0.00") + "x";
            }
            else
            {
                speedfl = Const.SpeedFluid;
                txtSpdType.text = "FLUID";
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
            switch(Const.JudgeType)
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

            // 스킨
            if (Const.GearSkin == "") Const.GearSkin = "black";
            switch (Const.GearSkin)
            {
                case "black":
                    txtSkin.text = "BLACK";
                    break;
                case "white":
                    txtSkin.text = "WHITE";
                    break;
                case "dark":
                    txtSkin.text = "DARK";
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
            if(Input.GetKeyDown(KeyCode.Tab))
            {
                PageChange();
            }

            if(Page == 0)
            {

                if (Input.GetKey(KeyCode.F1))
                {
                    if (Const.SpdType == SpdType.FIXED)
                    {
                        SpeedDownFixed();
                    }
                    else if (Const.SpdType == SpdType.FLUID)
                    {
                        SpeedDownFluid();
                    }
                }
                else if (Input.GetKey(KeyCode.F2))
                {
                    if (Const.SpdType == SpdType.FIXED)
                    {
                        SpeedUpFixed();
                    }
                    else if (Const.SpdType == SpdType.FLUID)
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
                    JudgeTypeChange();
                }
                else if (Input.GetKeyDown(KeyCode.F11))
                {
                    MusicListUI.SetNotOnTop();
                    customRandom.SetActive(true);
                    GetComponent<CustomPattern>().EnableWindow();
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.F1))
                {
                    SkinChange();
                }
                else if (Input.GetKeyDown(KeyCode.F2))
                {
                    GraphSizeChange();
                }
                else if (Input.GetKeyDown(KeyCode.F3))
                {
                    LayoutChange();
                }
                else if (Input.GetKeyDown(KeyCode.F4))
                {
                    BGAChange();
                }
                else if (Input.GetKeyDown(KeyCode.F5))
                {
                    PlaySideChange();
                }
                else if (Input.GetKeyDown(KeyCode.F6))
                {
                    FastSlowChange();
                }
                else if (Input.GetKeyDown(KeyCode.F7))
                {
                    TargetDiffChange();
                }
                else if (Input.GetKeyDown(KeyCode.F8))
                {
                    RateDiffChange();
                }
            }
        }

        private void PageChange()
        {
            if(Page == 0)
            {
                Page = 1;
                PageSet0.SetActive(false);
                PageSet1.SetActive(true);
            }
            else
            {
                Page = 0;
                PageSet0.SetActive(true);
                PageSet1.SetActive(false);
            }
        }

        private void SpeedUpFixed()
        {
            if(speed < 2000) speed++;
            Const.SpeedFixed = speed;

            // 폴더일 때는 변경 없이 처리
            if (Const.selectedOnList.Type == ItemType.BMS)
            {
                speedfl = (int)(Const.selectedOnList.Info.BPMstart * speed / 100);
                Const.SpeedFluid = speedfl;
            }

            txtSpeed.text = ((float)speed/100).ToString("0.00") + "x";
            txtSpdAnother.text = "FLUID " + speedfl.ToString();
        }

        private void SpeedDownFixed()
        {
            if (speed > 50) speed--;
            Const.SpeedFixed = speed;

            // 폴더일 때는 변경 없이 처리
            if (Const.selectedOnList.Type == ItemType.BMS)
            {
                speedfl = (int)(Const.selectedOnList.Info.BPMstart * speed / 100);
                Const.SpeedFluid = speedfl;
            }

            txtSpeed.text = ((float)speed / 100).ToString("0.00") + "x";
            txtSpdAnother.text = "FLUID " + speedfl.ToString();
        }

        private void SpeedUpFluid()
        {
            if (speedfl < 2000) speedfl++;
            Const.SpeedFluid = speedfl;

            // 폴더일 때는 변경 없이 처리
            if (Const.selectedOnList.Type == ItemType.BMS)
            {
                speed = (int)((double)speedfl / Const.selectedOnList.Info.BPMstart * 100);
                Const.SpeedFixed = speed;
            }

            txtSpeed.text = speedfl.ToString();
            txtSpdAnother.text = "FIXED " + ((float)speed / 100).ToString("0.00") + "x";
        }

        private void SpeedDownFluid()
        {
            if (speedfl > 100) speedfl--;
            Const.SpeedFluid = speedfl;

            // 폴더일 때는 변경 없이 처리
            if (Const.selectedOnList.Type == ItemType.BMS)
            {
                speed = (int)((double)speedfl / Const.selectedOnList.Info.BPMstart * 100);
                Const.SpeedFixed = speed;
            }

            txtSpeed.text = speedfl.ToString();
            txtSpdAnother.text = "FIXED " + ((float)speed / 100).ToString("0.00") + "x";
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
        }

        private void SpeedTypeChange()
        {
            if (Const.SpdType == SpdType.FIXED)
            {
                txtSpdType.text = "FLUID";
                txtSpdType.color = new Color(80f / 255, 201f / 255, 125f / 255);
                Const.SpdType = SpdType.FLUID;
                txtSpeed.text = speedfl.ToString();
                txtSpdAnother.text = "FIXED " + ((float)speed / 100).ToString("0.00") + "x";
            }
            else
            {
                txtSpdType.text = "FIXED";
                txtSpdType.color = new Color(80f / 255, 201f / 255, 125f / 255);
                Const.SpdType = SpdType.FIXED;
                txtSpeed.text = ((float)speed/100).ToString("0.00");
                txtSpdAnother.text = "FLUID " + speedfl.ToString();
            }
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
        }

        private void SkinChange()
        {
            switch (Const.GearSkin)
            {
                case "black":
                    txtSkin.text = "WHITE";
                    Const.GearSkin = "white";
                    break;
                case "white":
                    txtSkin.text = "DARK";
                    Const.GearSkin = "dark";
                    break;
                case "dark":
                    txtSkin.text = "BLACK";
                    Const.GearSkin = "black";
                    break;
            }
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
        }

        private void RateDiffChange()
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
        }
    }
}
