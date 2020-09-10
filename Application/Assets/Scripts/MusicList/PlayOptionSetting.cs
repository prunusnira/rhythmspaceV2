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
        private int speed;

        // Auto
        public Image spAutoOnOff;
        public Sprite AutoOn;
        public Sprite AutoOff;

        // Judge
        public Image spJudgeOnOff;
        public Sprite JudgeOn;
        public Sprite JudgeOff;

        // Guage Type
        public Image spGuage;
        public Sprite GAssisted;
        public Sprite GEasy;
        public Sprite GNormal;
        public Sprite GHard;
        public Sprite GExHard;
        private JudgeType currentJudge;

        public void Awake()
        {
            // 현재 값 가져오기
            speed = Const.GetSpeed();
            txtSpeed.text = ((float)speed / 100).ToString("0.00") + "x";

            if(Const.GetAuto() == 0)
            {
                spAutoOnOff.sprite = AutoOff;
            }
            else
            {
                spAutoOnOff.sprite = AutoOn;
            }

            if(Const.GetPJudge() == 0)
            {
                spJudgeOnOff.sprite = JudgeOff;
            }
            else
            {
                spJudgeOnOff.sprite = JudgeOn;
            }

            currentJudge = Const.GetJudgeType();
            switch(currentJudge)
            {
                case JudgeType.ASSISTED:
                    spGuage.sprite = GAssisted;
                    break;
                case JudgeType.EASY:
                    spGuage.sprite = GEasy;
                    break;
                case JudgeType.NORMAL:
                    spGuage.sprite = GNormal;
                    break;
                case JudgeType.HARD:
                    spGuage.sprite = GHard;
                    break;
                case JudgeType.EXHARD:
                    spGuage.sprite = GExHard;
                    break;
            }
        }

        public void Update()
        {
            if(Input.GetKeyDown(KeyCode.F1))
            {
                SpeedDown();
            }
            else if(Input.GetKeyDown(KeyCode.F2))
            {
                SpeedUp();
            }
            else if(Input.GetKeyDown(KeyCode.F3))
            {
                AutoOnOff();
            }
            else if (Input.GetKeyDown(KeyCode.F4))
            {
                JudgeOnOff();
            }
            else if (Input.GetKeyDown(KeyCode.F5))
            {
                JudgeTypeChange();
            }
        }

        public void SpeedUp()
        {
            if(speed < 1000) speed += 25;
            Const.SetSpeed(speed);
            txtSpeed.text = ((float)speed/100).ToString("0.00") + "x";
        }

        public void SpeedDown()
        {
            if (speed > 50) speed -= 25;
            Const.SetSpeed(speed);
            txtSpeed.text = ((float)speed / 100).ToString("0.00") + "x";
        }

        public void JudgeOnOff()
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

        public void AutoOnOff()
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

        public void JudgeTypeChange()
        {
            switch (currentJudge)
            {
                case JudgeType.ASSISTED:
                    spGuage.sprite = GEasy;
                    currentJudge = JudgeType.EASY;
                    break;
                case JudgeType.EASY:
                    spGuage.sprite = GNormal;
                    currentJudge = JudgeType.NORMAL;
                    break;
                case JudgeType.NORMAL:
                    spGuage.sprite = GHard;
                    currentJudge = JudgeType.HARD;
                    break;
                case JudgeType.HARD:
                    spGuage.sprite = GExHard;
                    currentJudge = JudgeType.EXHARD;
                    break;
                case JudgeType.EXHARD:
                    spGuage.sprite = GAssisted;
                    currentJudge = JudgeType.ASSISTED;
                    break;
            }
            Const.SetJudgeType(currentJudge);
        }
    }
}
