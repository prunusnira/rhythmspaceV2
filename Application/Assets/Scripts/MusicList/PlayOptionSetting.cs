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

        // Skin
        public Text txtSkin;

        // Custom Random
        public GameObject customRandom;

        public void Awake()
        {
            // 현재 값 가져오기

            // 오토 설정
            if(Const.Auto == AutoPlayType.OFF)
            {
                txtAuto.text = "OFF";
                txtAuto.color = Color.white;
            }
            else
            {
                txtAuto.text = "ON";
                txtAuto.color = Color.red;
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
            if (Const.Skin == "") Const.Skin = "black";
            switch (Const.Skin)
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
        }

        public void Update()
        {
            if(Input.GetKey(KeyCode.F1))
            {
                if(Const.SpdType == SpdType.FIXED)
                {
                    SpeedDownFixed();
                }
                else if (Const.SpdType == SpdType.FLUID)
                {
                    SpeedDownFluid();
                }
            }
            else if(Input.GetKey(KeyCode.F2))
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
            else if(Input.GetKeyDown(KeyCode.F4))
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
            else if(Input.GetKeyDown(KeyCode.F8))
            {
                NoteLayoutChange();
            }
            else if(Input.GetKeyDown(KeyCode.F9))
            {
                JudgeTypeChange();
            }
            else if (Input.GetKeyDown(KeyCode.F10))
            {
                SkinChange();
            }
            else if(Input.GetKeyDown(KeyCode.F11))
            {
                MusicListUI.SetNotOnTop();
                customRandom.SetActive(true);
                GetComponent<CustomPattern>().EnableWindow();
            }
        }

        private void SpeedUpFixed()
        {
            if(speed < 2000) speed++;
            Const.SpeedFixed = speed;
            speedfl = (int)(Const.selectedOnList.Info.BPMstart * speed / 100);
            Const.SpeedFluid = speedfl;

            txtSpeed.text = ((float)speed/100).ToString("0.00") + "x";
            txtSpdAnother.text = "FLUID " + speedfl.ToString();
        }

        private void SpeedDownFixed()
        {
            if (speed > 50) speed--;
            Const.SpeedFixed = speed;
            speedfl = (int)(Const.selectedOnList.Info.BPMstart * speed / 100);
            Const.SpeedFluid = speedfl;

            txtSpeed.text = ((float)speed / 100).ToString("0.00") + "x";
            txtSpdAnother.text = "FLUID " + speedfl.ToString();
        }

        private void SpeedUpFluid()
        {
            if (speedfl < 2000) speedfl++;
            Const.SpeedFluid = speedfl;
            speed = (int)((double)speedfl / Const.selectedOnList.Info.BPMstart * 100);
            Const.SpeedFixed = speed;

            txtSpeed.text = speedfl.ToString();
            txtSpdAnother.text = "FIXED " + ((float)speed / 100).ToString("0.00") + "x";
        }

        private void SpeedDownFluid()
        {
            if (speedfl > 100) speedfl--;
            Const.SpeedFluid = speedfl;
            speed = (int)((double)speedfl / Const.selectedOnList.Info.BPMstart * 100);
            Const.SpeedFixed = speed;

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
            if(Const.Auto == AutoPlayType.OFF)
            {
                Const.Auto = AutoPlayType.ON;
                txtAuto.text = "ON";
                txtAuto.color = Color.red;
            }
            else
            {
                Const.Auto = AutoPlayType.OFF;
                txtAuto.text = "OFF";
                txtAuto.color = Color.white;
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

            switch (Const.Skin)
            {
                case "black":
                    txtSkin.text = "WHITE";
                    Const.Skin = "white";
                    break;
                case "white":
                    txtSkin.text = "DARK";
                    Const.Skin = "dark";
                    break;
                case "dark":
                    txtSkin.text = "BLACK";
                    Const.Skin = "black";
                    break;
            }
        }
    }
}
