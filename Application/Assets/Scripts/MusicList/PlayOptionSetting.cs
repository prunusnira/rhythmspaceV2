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
        public Image spSpdType;
        public Sprite spdFixed;
        public Sprite spdFluid;

        // Auto
        public Image spAutoOnOff;
        public Sprite AutoOn;
        public Sprite AutoOff;

        // Judge
        public Image spJudgeOnOff;
        public Sprite JudgeOn;
        public Sprite JudgeOff;

        // Gauge Type
        public Image spGauge;
        public Sprite GAssisted;
        public Sprite GEasy;
        public Sprite GNormal;
        public Sprite GHard;
        public Sprite GExHard;

        // Target Graph Type
        public Image spTarget;
        public Sprite TOff;
        public Sprite TA;
        public Sprite TAA;
        public Sprite TAAA;
        public Sprite TMax;

        public void Awake()
        {
            // 현재 값 가져오기

            // 오토 설정
            if(Const.GetAuto() == 0)
            {
                spAutoOnOff.sprite = AutoOff;
            }
            else
            {
                spAutoOnOff.sprite = AutoOn;
            }

            // 판정 표시 설정
            if(Const.GetPJudge() == 0)
            {
                spJudgeOnOff.sprite = JudgeOff;
            }
            else
            {
                spJudgeOnOff.sprite = JudgeOn;
            }

            // 속도
            if(Const.GetSpdType() == SpdType.FIXED)
            {
                speed = Const.GetSpeedFixed();
                spSpdType.sprite = spdFixed;
                txtSpeed.text = ((float)speed / 100).ToString("0.00") + "x";
            }
            else
            {
                speedfl = Const.GetSpeedFluid();
                spSpdType.sprite = spdFluid;
                txtSpeed.text = speedfl.ToString();
            }

            // 게이지 타입
            switch(Const.GetJudgeType())
            {
                case JudgeType.ASSISTED:
                    spGauge.sprite = GAssisted;
                    break;
                case JudgeType.EASY:
                    spGauge.sprite = GEasy;
                    break;
                case JudgeType.NORMAL:
                    spGauge.sprite = GNormal;
                    break;
                case JudgeType.HARD:
                    spGauge.sprite = GHard;
                    break;
                case JudgeType.EXHARD:
                    spGauge.sprite = GExHard;
                    break;
            }

            // 그래프
            switch(Const.GetGraphTarget())
            {
                case GraphTargetType.OFF:
                    spTarget.sprite = TOff;
                    break;
                case GraphTargetType.A:
                    spTarget.sprite = TA;
                    break;
                case GraphTargetType.AA:
                    spTarget.sprite = TAA;
                    break;
                case GraphTargetType.AAA:
                    spTarget.sprite = TAAA;
                    break;
                case GraphTargetType.MAX:
                    spTarget.sprite = TMax;
                    break;
                default:
                    spTarget.sprite = TOff;
                    break;
            }
        }

        public void Update()
        {
            if(Input.GetKeyDown(KeyCode.F1))
            {
                if(Const.GetSpdType() == SpdType.FIXED)
                {
                    SpeedDownFixed();
                }
            }
            else if(Input.GetKeyDown(KeyCode.F2))
            {
                if (Const.GetSpdType() == SpdType.FIXED)
                {
                    SpeedUpFixed();
                }
            }
            else if(Input.GetKey(KeyCode.F1))
            {
                if (Const.GetSpdType() == SpdType.FLUID)
                {
                    SpeedDownFluid();
                }
            }
            else if (Input.GetKey(KeyCode.F2))
            {
                if (Const.GetSpdType() == SpdType.FLUID)
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
                JudgeOnOff();
            }
            else if (Input.GetKeyDown(KeyCode.F6))
            {
                JudgeTypeChange();
            }
            else if (Input.GetKeyDown(KeyCode.F7))
            {
                GraphTargetChange();
            }
        }

        private void SpeedUpFixed()
        {
            if(speed < 1000) speed += 25;
            Const.SetSpeedFixed(speed);
            speedfl = (int)(Const.selectedMusic.BPMstart * speed / 100);
            Const.SetSpeedFluid(speedfl);

            txtSpeed.text = ((float)speed/100).ToString("0.00") + "x";
            txtSpdAnother.text = "FLUID " + speedfl.ToString();
        }

        private void SpeedDownFixed()
        {
            if (speed > 50) speed -= 25;
            Const.SetSpeedFixed(speed);
            speedfl = (int)(Const.selectedMusic.BPMstart * speed / 100);
            Const.SetSpeedFluid(speedfl);

            txtSpeed.text = ((float)speed / 100).ToString("0.00") + "x";
            txtSpdAnother.text = "FLUID " + speedfl.ToString();
        }

        private void SpeedUpFluid()
        {
            if (speedfl < 1000) speedfl++;
            Const.SetSpeedFluid(speedfl);
            speed = (int)((double)speedfl / Const.selectedMusic.BPMstart * 100);
            Const.SetSpeedFixed(speed);

            txtSpeed.text = speedfl.ToString();
            txtSpdAnother.text = "FIXED " + ((float)speed / 100).ToString("0.00") + "x";
        }

        private void SpeedDownFluid()
        {
            if (speedfl > 100) speedfl--;
            Const.SetSpeedFluid(speedfl);
            speed = (int)((double)speedfl / Const.selectedMusic.BPMstart * 100);
            Const.SetSpeedFixed(speed);

            txtSpeed.text = speedfl.ToString();
            txtSpdAnother.text = "FIXED " + ((float)speed / 100).ToString("0.00") + "x";
        }

        private void JudgeOnOff()
        {
            if(Const.GetPJudge() == 0)
            {
                Const.SetPJudge(1);
                spJudgeOnOff.sprite = JudgeOn;
            }
            else
            {
                Const.SetPJudge(0);
                spJudgeOnOff.sprite = JudgeOff;
            }
        }

        private void AutoOnOff()
        {
            if(Const.GetAuto() == 0)
            {
                Const.SetAuto(1);
                spAutoOnOff.sprite = AutoOn;
            }
            else
            {
                Const.SetAuto(0);
                spAutoOnOff.sprite = AutoOff;
            }
        }

        private void JudgeTypeChange()
        {
            switch (Const.GetJudgeType())
            {
                case JudgeType.ASSISTED:
                    spGauge.sprite = GEasy;
                    Const.SetJudgeType(JudgeType.EASY);
                    break;
                case JudgeType.EASY:
                    spGauge.sprite = GNormal;
                    Const.SetJudgeType(JudgeType.NORMAL);
                    break;
                case JudgeType.NORMAL:
                    spGauge.sprite = GHard;
                    Const.SetJudgeType(JudgeType.HARD);
                    break;
                case JudgeType.HARD:
                    spGauge.sprite = GExHard;
                    Const.SetJudgeType(JudgeType.EXHARD);
                    break;
                case JudgeType.EXHARD:
                    spGauge.sprite = GAssisted;
                    Const.SetJudgeType(JudgeType.ASSISTED);
                    break;
            }
        }

        private void SpeedTypeChange()
        {
            if (Const.GetSpdType() == SpdType.FIXED)
            {
                spSpdType.sprite = spdFluid;
                Const.SetSpdType(SpdType.FLUID);
                txtSpeed.text = speedfl.ToString();
                txtSpdAnother.text = "FIXED " + ((float)speed / 100).ToString("0.00") + "x";
            }
            else
            {
                spSpdType.sprite = spdFixed;
                Const.SetSpdType(SpdType.FIXED);
                txtSpeed.text = ((float)speed/100).ToString("0.00");
                txtSpdAnother.text = "FLUID " + speedfl.ToString();
            }
        }

        private void GraphTargetChange()
        {
            switch (Const.GetGraphTarget())
            {
                case GraphTargetType.OFF:
                    spTarget.sprite = TA;
                    Const.SetGraphTarget(GraphTargetType.A);
                    break;
                case GraphTargetType.A:
                    spTarget.sprite = TAA;
                    Const.SetGraphTarget(GraphTargetType.AA);
                    break;
                case GraphTargetType.AA:
                    spTarget.sprite = TAAA;
                    Const.SetGraphTarget(GraphTargetType.AAA);
                    break;
                case GraphTargetType.AAA:
                    spTarget.sprite = TMax;
                    Const.SetGraphTarget(GraphTargetType.MAX);
                    break;
                case GraphTargetType.MAX:
                    spTarget.sprite = TOff;
                    Const.SetGraphTarget(GraphTargetType.OFF);
                    break;
                default:
                    spTarget.sprite = TOff;
                    break;
            }
        }
    }
}
