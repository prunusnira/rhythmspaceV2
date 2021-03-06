﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace BMSPlayer
{
    public class MappingChanger : MonoBehaviour
    {
        public GameObject self;
        public Text desc;
        public Text changeableKey;

        void Update()
        {
            if (KeyMapping.keyAssigned)
            {
                Const.KeyToChange = 0;
                self.SetActive(false);
            }
            else
            {
                SetKeyName(Const.KeyToChange);
            }

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Const.KeyToChange = 0;
                self.SetActive(false);
            }

            // 키 설정
            foreach (KeyCode code in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(code))
                {
                    string key = code.ToString();

                    Keys.SaveButtonSetting(Const.KeyToChange, key, false);
                }
            }

            /*if (Input.GetAxis("axis3") > 0.5)
            {
                // LT XB
                Keys.SaveButtonSetting(Const.KeyToChange, "Axis3p", true);
            }
            if (Input.GetAxis("axis3") < -0.5)
            {
                // RT XB
                Keys.SaveButtonSetting(Const.KeyToChange, "Axis3m", true);
            }*/

            if (Input.GetAxis("axisX") > 0.5)
            {
                // DPAD RIGHT XB
                Keys.SaveButtonSetting(Const.KeyToChange, "AxisXp", true);
            }
            if (Input.GetAxis("axisX") < -0.5)
            {
                // DPAD LEFT XB
                Keys.SaveButtonSetting(Const.KeyToChange, "AxisXm", true);
            }

            if (Input.GetAxis("axisY") > 0.5)
            {
                // DPAD RIGHT XB
                Keys.SaveButtonSetting(Const.KeyToChange, "AxisYp", true);
            }
            if (Input.GetAxis("axisY") < -0.5)
            {
                // DPAD LEFT XB
                Keys.SaveButtonSetting(Const.KeyToChange, "AxisYm", true);
            }

            if (Input.GetAxis("axis6") > 0.5)
            {
                // DPAD RIGHT XB
                Keys.SaveButtonSetting(Const.KeyToChange, "Axis6p", true);
            }
            if (Input.GetAxis("axis6") < -0.5)
            {
                // DPAD LEFT XB
                Keys.SaveButtonSetting(Const.KeyToChange, "Axis6m", true);
            }

            if (Input.GetAxis("axis7") > 0.5)
            {
                // DPAD RIGHT DS4
                // DPAD UP XB
                Keys.SaveButtonSetting(Const.KeyToChange, "Axis7p", true);
            }
            if (Input.GetAxis("axis7") < -0.5)
            {
                // DPAD LEFT DS4
                // DPAD DOWN XB
                Keys.SaveButtonSetting(Const.KeyToChange, "Axis7m", true);
            }

            if (Input.GetAxis("axis8") > 0.5)
            {
                // DPAD UP DS4
                Keys.SaveButtonSetting(Const.KeyToChange, "Axis8p", true);
            }
            if (Input.GetAxis("axis8") < -0.5)
            {
                // DPAD DOWN DS4
                Keys.SaveButtonSetting(Const.KeyToChange, "Axis8m", true);
            }

            if (Input.GetAxis("axis9") > 0.5)
            {
                // LT XB
                Keys.SaveButtonSetting(Const.KeyToChange, "Axis9p", true);
            }

            if (Input.GetAxis("axis9") < -0.5)
            {
                // LT XB
                Keys.SaveButtonSetting(Const.KeyToChange, "Axis9m", true);
            }

            if (Input.GetAxis("axis10") > 0.5)
            {
                // RT XB
                Keys.SaveButtonSetting(Const.KeyToChange, "Axis10p", true);
            }
            if (Input.GetAxis("axis10") < -0.5)
            {
                // RT XB
                Keys.SaveButtonSetting(Const.KeyToChange, "Axis10m", true);
            }
        }

        private void SetKeyName(int keypos)
        {
            desc.text = Const.KeyChangeDescription[(int)Const.Language];
            switch (keypos)
            {
                case 10:
                case 30:
                    changeableKey.text = "TurnTable Left(Up)";
                    break;
                case 11:
                case 31:
                    changeableKey.text = "Button 1";
                    break;
                case 12:
                case 32:
                    changeableKey.text = "Button 2";
                    break;
                case 13:
                case 33:
                    changeableKey.text = "Button 3";
                    break;
                case 14:
                case 34:
                    changeableKey.text = "Button 4";
                    break;
                case 15:
                case 35:
                    changeableKey.text = "Button 5";
                    break;
                case 16:
                case 36:
                    changeableKey.text = "Button 6";
                    break;
                case 17:
                case 37:
                    changeableKey.text = "Button 7";
                    break;
                case 18:
                case 38:
                    changeableKey.text = "TurnTable Right(Down)";
                    break;
                case 19:
                case 39:
                    changeableKey.text = "Function 1";
                    break;
                case 20:
                case 40:
                    changeableKey.text = "Function 2";
                    break;
            }
        }

        
    }
}
