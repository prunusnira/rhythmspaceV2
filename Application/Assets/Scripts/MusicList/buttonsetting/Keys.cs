using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace BMSPlayer
{
    public class Keys
    {
        // 컨트롤러 설정의 명칭에 대해
        // 1. 아날로그는 사용하지 않음
        // 2. DPAD_LEFT_XB / DPAD_UP_XB / DPAD_DOWN_XB / DPAD_RIGHT_XB
        //    DPAD_LEFT_P4 / DPAD_UP_P4 / DPAD_DOWN_P4 / DPAD_RIGHT_P4
        //    의 string으로 저장하며 이 문자가 인식되면 적절히 처리
        // 3. 유니티 설정과 마찬가지로 joystick button n 의 형식으로 버튼을 정의
        public static string[] DEFAULT_KB = {
            "LeftShift",
            "A", "S", "D", "Space", "J", "K", "L",
            "Semicolon",
            "BackQuote", "Tab"
        };

        public static string[] DEFAULT_XB = {
            "Axis9p",
            "Axis6m", "Axis7p", "Axis6p",
            "JoystickButton2",
            "JoystickButton3",
            "JoystickButton1",
            "JoystickButton5",
            "Axis10p",
            "JoystickButton6",
            "JoystickButton7"
        };

        public static string[] DEFAULT_DS4 =
        {
            "JoystickButton6",
            "JoystickButton4",
            "Axis8p", "Axis7p", "JoystickButton0",
            "JoystickButton3", "JoystickButton1", "JoystickButton5",
            "JoystickButton7",
            "JoystickButton8",
            "JoystickButton9"
        };

        public static string[] DEFAULT_PITCH =
        {
            "AxisYp",
            "JoystickButton7",
            "JoystickButton2", "JoystickButton1", "JoystickButton5",
            "JoystickButton0", "JoystickButton4", "AxisXm",
            "AxisYm",
            "JoystickButton9", "JoystickButton8"
        };

        public static string[] DEFAULT_LR2 =
        {
            "AxisXm",
            "JoystickButton0", "JoystickButton1", "JoystickButton2",
            "JoystickButton3", "JoystickButton4", "JoystickButton5",
            "JoystickButton6",
            "AxisXp",
            "JoystickButton8", "JoystickButton9"
        };

        public static bool[] DEFAULT_DPAD_DS4 =
        {
            false, false, true, true,
            false, false, false, false, false,
            false, false
        };

        public static bool[] DEFAULT_DPAD_XB =
        {
            true, true, true, true,
            false, false, false, false, true,
            false, false
        };

        public static bool[] DEFAULT_DPAD_PITCH =
        {
            true, false, false, false,
            false, false, false, true, true,
            false, false
        };

        public static bool[] DEFAULT_DPAD_LR2 =
        {
            true, false, false, false,
            false, false, false, false, true,
            false, false
        };

        public static string[] btnSet1 = new string[11];
        public static string[] btnSet2 = new string[11];
        public static bool[] btnAxisSet1 = new bool[11];
        public static bool[] btnAxisSet2 = new bool[11];

        public static void setKBDefault()
        {
            for(int i = 10; i < 21; i++)
            {
                SaveButtonSetting(i, DEFAULT_KB[i-10], false);
            }
        }

        public static void setXBDefault()
        {
            for (int i = 30; i < 41; i++)
            {
                SaveButtonSetting(i, DEFAULT_XB[i - 30], DEFAULT_DPAD_XB[i - 30]);
            }
        }

        public static void setDS4Default()
        {
            for (int i = 30; i < 41; i++)
            {
                SaveButtonSetting(i, DEFAULT_DS4[i - 30], DEFAULT_DPAD_DS4[i - 30]);
            }
        }

        public static void setPITCHDefault()
        {
            for (int i = 30; i < 41; i++)
            {
                SaveButtonSetting(i, DEFAULT_PITCH[i - 30], DEFAULT_DPAD_PITCH[i - 30]);
            }
        }

        public static void setLR2Default()
        {
            for (int i = 30; i < 41; i++)
            {
                SaveButtonSetting(i, DEFAULT_LR2[i - 30], DEFAULT_DPAD_LR2[i - 30]);
            }
        }

        // Key Input Wrapper
        public static bool GetKeyDown(string key)
        {
            if (key != null)
            {
                return Input.GetKeyDown((KeyCode)Enum.Parse(typeof(KeyCode), key));
            }
            return false;
        }
        public static bool GetKey(string key)
        {
            if (key != null)
            {
                return Input.GetKey((KeyCode)Enum.Parse(typeof(KeyCode), key));
            }
            return false;
        }

        public static float GetAxisValue(string key)
        {
            // 받은 키가 dpad이면 그 쪽을 처리
            if (key == "AxisXp") // axis 3
            {
                return Input.GetAxis("axisX");
            }
            else if (key == "AxisXm") // axis 3
            {
                return Input.GetAxis("axisX");
            }
            else if (key == "AxisYp") // axis 3
            {
                return Input.GetAxis("axisY");
            }
            else if (key == "AxisYm") // axis 3
            {
                return Input.GetAxis("axisY");
            }
            else if (key == "Axis6p") // axis 6
            {
                float val = Input.GetAxis("axis6");
                if (val > 0.5)
                    return val;
            }
            else if (key == "Axis6m") // axis 6
            {
                float val = Input.GetAxis("axis6");
                if (val < -0.5)
                    return val;
            }
            else if (key == "Axis7p") // axis 7
            {
                return Input.GetAxis("axis7");
            }
            else if (key == "Axis7m") // axis 7
            {
                return Input.GetAxis("axis7");
            }
            else if (key == "Axis8p") // axis 8
            {
                return Input.GetAxis("axis8");
            }
            else if (key == "Axis8m") // axis 8
            {
                return Input.GetAxis("axis8");
            }
            else if (key == "Axis9p") // axis 9
            {
                return Input.GetAxis("axis9");
            }
            else if (key == "Axis9m") // axis 9
            {
                return Input.GetAxis("axis9");
            }
            else if (key == "Axis10p") // axis 10
            {
                return Input.GetAxis("axis10");
            }
            else if (key == "Axis10m") // axis 10
            {
                return Input.GetAxis("axis10");
            }
            return 0;
        }

        public static bool GetKeyUp(string key)
        {
            if (key != null)
            {
                return Input.GetKeyUp((KeyCode)Enum.Parse(typeof(KeyCode), key));
            }
            return false;
        }

        public static void LoadButtonSetting()
        {
            btnSet1 = new string[11] {
                KeyMapping.Set1key1,
                KeyMapping.Set1key2,
                KeyMapping.Set1key3,
                KeyMapping.Set1key4,
                KeyMapping.Set1key5,
                KeyMapping.Set1key6,
                KeyMapping.Set1key7,
                KeyMapping.Set1key8,
                KeyMapping.Set1key9,
                KeyMapping.Set1key10,
                KeyMapping.Set1key11
            };

            btnSet2 = new string[11] {
                KeyMapping.Set2key1,
                KeyMapping.Set2key2,
                KeyMapping.Set2key3,
                KeyMapping.Set2key4,
                KeyMapping.Set2key5,
                KeyMapping.Set2key6,
                KeyMapping.Set2key7,
                KeyMapping.Set2key8,
                KeyMapping.Set2key9,
                KeyMapping.Set2key10,
                KeyMapping.Set2key11
            };

            btnAxisSet1 = new bool[11]
            {
                KeyMapping.Set1Axis1,
                KeyMapping.Set1Axis2,
                KeyMapping.Set1Axis3,
                KeyMapping.Set1Axis4,
                KeyMapping.Set1Axis5,
                KeyMapping.Set1Axis6,
                KeyMapping.Set1Axis7,
                KeyMapping.Set1Axis8,
                KeyMapping.Set1Axis9,
                KeyMapping.Set1Axis10,
                KeyMapping.Set1Axis11
            };

            btnAxisSet2 = new bool[11]
            {
                KeyMapping.Set2Axis1,
                KeyMapping.Set2Axis2,
                KeyMapping.Set2Axis3,
                KeyMapping.Set2Axis4,
                KeyMapping.Set2Axis5,
                KeyMapping.Set2Axis6,
                KeyMapping.Set2Axis7,
                KeyMapping.Set2Axis8,
                KeyMapping.Set2Axis9,
                KeyMapping.Set2Axis10,
                KeyMapping.Set2Axis11
            };

            if (btnSet1[0].Equals("")) setKBDefault();
            if (btnSet2[0].Equals("")) setLR2Default();
        }

        // Get Key Press
        #region Get Key Press from various situation
        public static bool GetTurnTableLeftDown()
        {
            return GetBtnDown(0);
        }

        public static bool GetTurnTableRightDown()
        {
            return GetBtnDown(8);
        }

        public static bool GetTurnTableLeftKeep()
        {
            return GetBtn(0);
        }

        public static bool GetTurnTableRightKeep()
        {
            return GetBtn(8);
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
                (btnAxisSet1[i] && btnSet1[i].EndsWith("p") &&
                    GetAxisValue(btnSet1[i]) > 0.5) ||
                (btnAxisSet1[i] && btnSet1[i].EndsWith("m") &&
                    GetAxisValue(btnSet1[i]) < -0.5) ||
                (btnAxisSet2[i] && btnSet2[i].EndsWith("p") &&
                    GetAxisValue(btnSet2[i]) > 0.5) ||
                (btnAxisSet2[i] && btnSet2[i].EndsWith("m") &&
                    GetAxisValue(btnSet2[i]) < -0.5) ||
                (!btnAxisSet1[i] && GetKeyDown(btnSet1[i])) ||
                (!btnAxisSet2[i] && GetKeyDown(btnSet2[i]));
        }

        public static bool GetBtn(int i)
        {
            return
                (btnAxisSet1[i] && btnSet1[i].EndsWith("p") &&
                    GetAxisValue(btnSet1[i]) > 0.5) ||
                (btnAxisSet1[i] && btnSet1[i].EndsWith("m") &&
                    GetAxisValue(btnSet1[i]) < -0.5) ||
                (btnAxisSet2[i] && btnSet2[i].EndsWith("p") &&
                    GetAxisValue(btnSet2[i]) > 0.5) ||
                (btnAxisSet2[i] && btnSet2[i].EndsWith("m") &&
                    GetAxisValue(btnSet2[i]) < -0.5) ||
                (!btnAxisSet1[i] && GetKey(btnSet1[i])) ||
                (!btnAxisSet2[i] && GetKey(btnSet2[i]));
        }
        #endregion

        public static void SaveButtonSetting(int keypos, string key, bool isAxis)
        {
            switch (keypos)
            {
                case 10:
                    btnSet1[keypos-10] = key;
                    KeyMapping.Set1key1 = key;
                    if (isAxis)
                    {
                        btnAxisSet1[keypos - 10] = true;
                        KeyMapping.Set1Axis1 = true;
                    }
                    else
                    {
                        btnAxisSet1[keypos - 10] = false;
                        KeyMapping.Set1Axis1 = false;
                    }
                    break;
                case 11:
                    btnSet1[keypos - 10] = key;
                    KeyMapping.Set1key2 = key;
                    if (isAxis)
                    {
                        btnAxisSet1[keypos - 10] = true;
                        KeyMapping.Set1Axis2 = true;
                    }
                    else
                    {
                        btnAxisSet1[keypos - 10] = false;
                        KeyMapping.Set1Axis2 = false;
                    }
                    break;
                case 12:
                    btnSet1[keypos - 10] = key;
                    KeyMapping.Set1key3 = key;
                    if (isAxis)
                    {
                        btnAxisSet1[keypos - 10] = true;
                        KeyMapping.Set1Axis3 = true;
                    }
                    else
                    {
                        btnAxisSet1[keypos - 10] = false;
                        KeyMapping.Set1Axis3 = false;
                    }
                    break;
                case 13:
                    btnSet1[keypos - 10] = key;
                    KeyMapping.Set1key4 = key;
                    if (isAxis)
                    {
                        btnAxisSet1[keypos - 10] = true;
                        KeyMapping.Set1Axis4 = true;
                    }
                    else
                    {
                        btnAxisSet1[keypos - 10] = false;
                        KeyMapping.Set1Axis4 = false;
                    }
                    break;
                case 14:
                    btnSet1[keypos - 10] = key;
                    KeyMapping.Set1key5 = key;
                    if (isAxis)
                    {
                        btnAxisSet1[keypos - 10] = true;
                        KeyMapping.Set1Axis5 = true;
                    }
                    else
                    {
                        btnAxisSet1[keypos - 10] = false;
                        KeyMapping.Set1Axis5 = false;
                    }
                    break;
                case 15:
                    btnSet1[keypos - 10] = key;
                    KeyMapping.Set1key6 = key;
                    if (isAxis)
                    {
                        btnAxisSet1[keypos - 10] = true;
                        KeyMapping.Set1Axis6 = true;
                    }
                    else
                    {
                        btnAxisSet1[keypos - 10] = false;
                        KeyMapping.Set1Axis6 = false;
                    }
                    break;
                case 16:
                    btnSet1[keypos - 10] = key;
                    KeyMapping.Set1key7 = key;
                    if (isAxis)
                    {
                        btnAxisSet1[keypos - 10] = true;
                        KeyMapping.Set1Axis7 = true;
                    }
                    else
                    {
                        btnAxisSet1[keypos - 10] = false;
                        KeyMapping.Set1Axis7 = false;
                    }
                    break;
                case 17:
                    btnSet1[keypos - 10] = key;
                    KeyMapping.Set1key8 = key;
                    if (isAxis)
                    {
                        btnAxisSet1[keypos - 10] = true;
                        KeyMapping.Set1Axis8 = true;
                    }
                    else
                    {
                        btnAxisSet1[keypos - 10] = false;
                        KeyMapping.Set1Axis8 = false;
                    }
                    break;
                case 18:
                    btnSet1[keypos - 10] = key;
                    KeyMapping.Set1key9 = key;
                    if (isAxis)
                    {
                        btnAxisSet1[keypos - 10] = true;
                        KeyMapping.Set1Axis9 = true;
                    }
                    else
                    {
                        btnAxisSet1[keypos - 10] = false;
                        KeyMapping.Set1Axis9 = false;
                    }
                    break;
                case 19:
                    btnSet1[keypos - 10] = key;
                    KeyMapping.Set1key10 = key;
                    if (isAxis)
                    {
                        btnAxisSet1[keypos - 10] = true;
                        KeyMapping.Set1Axis10 = true;
                    }
                    else
                    {
                        btnAxisSet1[keypos - 10] = false;
                        KeyMapping.Set1Axis10 = false;
                    }
                    break;
                case 20:
                    btnSet1[keypos - 10] = key;
                    KeyMapping.Set1key11 = key;
                    if (isAxis)
                    {
                        btnAxisSet1[keypos - 10] = true;
                        KeyMapping.Set1Axis11 = true;
                    }
                    else
                    {
                        btnAxisSet1[keypos - 10] = false;
                        KeyMapping.Set1Axis11 = false;
                    }
                    break;
                case 30:
                    btnSet2[keypos - 30] = key;
                    KeyMapping.Set2key1 = key;
                    if (isAxis)
                    {
                        btnAxisSet2[keypos - 30] = true;
                        KeyMapping.Set2Axis1 = true;
                    }
                    else
                    {
                        btnAxisSet2[keypos - 30] = false;
                        KeyMapping.Set2Axis1 = false;
                    }
                    break;
                case 31:
                    btnSet2[keypos - 30] = key;
                    KeyMapping.Set2key2 = key;
                    if (isAxis)
                    {
                        btnAxisSet2[keypos - 30] = true;
                        KeyMapping.Set2Axis2 = true;
                    }
                    else
                    {
                        btnAxisSet2[keypos - 30] = false;
                        KeyMapping.Set2Axis2 = false;
                    }
                    break;
                case 32:
                    btnSet2[keypos - 30] = key;
                    KeyMapping.Set2key3 = key;
                    if (isAxis)
                    {
                        btnAxisSet2[keypos - 30] = true;
                        KeyMapping.Set2Axis3 = true;
                    }
                    else
                    {
                        btnAxisSet2[keypos - 30] = false;
                        KeyMapping.Set2Axis3 = false;
                    }
                    break;
                case 33:
                    btnSet2[keypos - 30] = key;
                    KeyMapping.Set2key4 = key;
                    if (isAxis)
                    {
                        btnAxisSet2[keypos - 30] = true;
                        KeyMapping.Set2Axis4 = true;
                    }
                    else
                    {
                        btnAxisSet2[keypos - 30] = false;
                        KeyMapping.Set2Axis4 = false;
                    }
                    break;
                case 34:
                    btnSet2[keypos - 30] = key;
                    KeyMapping.Set2key5 = key;
                    if (isAxis)
                    {
                        btnAxisSet2[keypos - 30] = true;
                        KeyMapping.Set2Axis5 = true;
                    }
                    else
                    {
                        btnAxisSet2[keypos - 30] = false;
                        KeyMapping.Set2Axis5 = false;
                    }
                    break;
                case 35:
                    btnSet2[keypos - 30] = key;
                    KeyMapping.Set2key6 = key;
                    if (isAxis)
                    {
                        btnAxisSet2[keypos - 30] = true;
                        KeyMapping.Set2Axis6 = true;
                    }
                    else
                    {
                        btnAxisSet2[keypos - 30] = false;
                        KeyMapping.Set2Axis6 = false;
                    }
                    break;
                case 36:
                    btnSet2[keypos - 30] = key;
                    KeyMapping.Set2key7 = key;
                    if (isAxis)
                    {
                        btnAxisSet2[keypos - 30] = true;
                        KeyMapping.Set2Axis7 = true;
                    }
                    else
                    {
                        btnAxisSet2[keypos - 30] = false;
                        KeyMapping.Set2Axis7 = false;
                    }
                    break;
                case 37:
                    btnSet2[keypos - 30] = key;
                    KeyMapping.Set2key8 = key;
                    if (isAxis)
                    {
                        btnAxisSet2[keypos - 30] = true;
                        KeyMapping.Set2Axis8 = true;
                    }
                    else
                    {
                        btnAxisSet2[keypos - 30] = false;
                        KeyMapping.Set2Axis8 = false;
                    }
                    break;
                case 38:
                    btnSet2[keypos - 30] = key;
                    KeyMapping.Set2key9 = key;
                    if (isAxis)
                    {
                        btnAxisSet2[keypos - 30] = true;
                        KeyMapping.Set2Axis9 = true;
                    }
                    else
                    {
                        btnAxisSet2[keypos - 30] = false;
                        KeyMapping.Set2Axis9 = false;
                    }
                    break;
                case 39:
                    btnSet2[keypos - 30] = key;
                    KeyMapping.Set2key10 = key;
                    if (isAxis)
                    {
                        btnAxisSet2[keypos - 30] = true;
                        KeyMapping.Set2Axis10 = true;
                    }
                    else
                    {
                        btnAxisSet2[keypos - 30] = false;
                        KeyMapping.Set2Axis10 = false;
                    }
                    break;
                case 40:
                    btnSet2[keypos - 30] = key;
                    KeyMapping.Set2key11 = key;
                    if (isAxis)
                    {
                        btnAxisSet2[keypos - 30] = true;
                        KeyMapping.Set2Axis11 = true;
                    }
                    else
                    {
                        btnAxisSet2[keypos - 30] = false;
                        KeyMapping.Set2Axis11 = false;
                    }
                    break;
            }
            KeyMapping.keyAssigned = true;
            MusicListUI.isKeyChanged = true;
        }
    }
}
