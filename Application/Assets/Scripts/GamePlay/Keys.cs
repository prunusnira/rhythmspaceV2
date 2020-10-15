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
            "Semicolon"
        };

        public static string[] DEFAULT_XB = {
            "LT_XB",
            "JoystickButton4",
            "DPAD_UP_XB", "DPAD_RIGHT_XB", "JoystickButton2",
            "JoystickButton3", "JoystickButton1", "JoystickButton5",
            "RT_XB"
        };

        public static string[] DEFAULT_DS4 =
        {
            "JoystickButton6",
            "JoystickButton4",
            "DPAD_UP_DS4", "DPAD_RIGHT_DS4", "JoystickButton2",
            "JoystickButton3", "JoystickButton1", "JoystickButton5",
            "JoystickButton7"
        };

        public static bool[] DEFAULT_DPAD_DS4 =
        {
            false, false, true, true,
            false, false, false, false, false
        };

        public static bool[] DEFAULT_DPAD_XB =
        {
            true, false, true, true,
            false, false, false, false, true
        };

        public static string[] btnkb = new string[9];
        public static string[] btnct = new string[9];
        public static bool[] dpad = new bool[9];

        public static void setKBDefault()
        {
            btnkb = DEFAULT_KB;
            SaveKBSetting();
        }

        public static void setXBDefault()
        {
            btnct = DEFAULT_XB;
            dpad = DEFAULT_DPAD_XB;
            SaveCOSetting();
        }

        public static void setDS4Default()
        {
            btnct = DEFAULT_DS4;
            dpad = DEFAULT_DPAD_DS4;
            SaveCOSetting();
        }

        // Key Input Wrapper
        public static bool GetKeyAxisDown(string key)
        {
            // 받은 키가 dpad이면 그 쪽을 처리
            if (key == "DPAD_LEFT_XB") // axis 6
            {
                return Input.GetAxis("axis6") < 0;
            }
            else if (key == "DPAD_UP_XB") // axis 7
            {
                return Input.GetAxis("axis7") < 0;
            }
            else if (key == "DPAD_RIGHT_XB") // axis 6
            {
                return Input.GetAxis("axis6") > 0;
            }
            else if (key == "DPAD_DOWN_XB") // axis 7
            {
                return Input.GetAxis("axis7") > 0;
            }
            else if (key == "LT_XB") // axis 9
            {
                return Input.GetAxis("axis9") < 0;
            }
            else if (key == "RT_XB") // axis 10
            {
                return Input.GetAxis("axis10") > 0;
            }
            else if (key == "DPAD_LEFT_DS4") // axis 7
            {
                return Input.GetAxis("axis7") < 0;
            }
            else if (key == "DPAD_UP_DS4") // axis 8
            {
                return Input.GetAxis("axis8") > 0;
            }
            else if (key == "DPAD_RIGHT_DS4") // axis 7
            {
                return Input.GetAxis("axis7") > 0;
            }
            else if (key == "DPAD_DOWN_DS4") // axis 8
            {
                return Input.GetAxis("axis8") < 0;
            }
            else if (key != null)
            {
                return Input.GetKeyDown((KeyCode)Enum.Parse(typeof(KeyCode), key));
            }
            else
            {
                return false;
            }
        }

        // Key Input Wrapper
        public static bool GetKeyAxisUp(string key)
        {
            // 받은 키가 dpad이면 그 쪽을 처리
            if (key == "DPAD_LEFT_XB") // axis 6
            {
                return Input.GetAxis("axis6") == 0;
            }
            else if (key == "DPAD_UP_XB") // axis 7
            {
                return Input.GetAxis("axis7") == 0;
            }
            else if (key == "DPAD_RIGHT_XB") // axis 6
            {
                return Input.GetAxis("axis6") == 0;
            }
            else if (key == "DPAD_DOWN_XB") // axis 7
            {
                return Input.GetAxis("axis7") == 0;
            }
            else if (key == "LT_XB") // axis 9
            {
                return Input.GetAxis("axis9") == 0;
            }
            else if (key == "RT_XB") // axis 10
            {
                return Input.GetAxis("axis10") == 0;
            }
            else if (key == "DPAD_LEFT_DS4") // axis 7
            {
                return Input.GetAxis("axis7") == 0;
            }
            else if (key == "DPAD_UP_DS4") // axis 8
            {
                return Input.GetAxis("axis8") == 0;
            }
            else if (key == "DPAD_RIGHT_DS4") // axis 7
            {
                return Input.GetAxis("axis7") == 0;
            }
            else if (key == "DPAD_DOWN_DS4") // axis 8
            {
                return Input.GetAxis("axis8") == 0;
            }
            else if (key != null)
            {
                return Input.GetKeyUp((KeyCode)Enum.Parse(typeof(KeyCode), key));
            }
            else
            {
                return false;
            }
        }

        public static void SaveKBSetting()
        {
            for(int i = 0; i < btnkb.Length; i++)
            {
                PlayerPrefs.SetString("k"+(i+1).ToString(), btnkb[i]);
            }
        }

        public static void SaveCOSetting()
        {
            for (int i = 0; i < btnct.Length; i++)
            {
                PlayerPrefs.SetString("c" + (i + 1).ToString(), btnct[i]);
            }

            for (int i = 0; i < dpad.Length; i++)
            {
                PlayerPrefs.SetString("dpad" + (i + 1).ToString(), dpad[i].ToString());
            }
        }

        public static void LoadKBSetting()
        {
            for(int i = 0; i < btnkb.Length; i++)
            {
                btnkb[i] = PlayerPrefs.GetString(
                    "k"+(i+1).ToString(), DEFAULT_KB[i]
                );
            }

            if (btnkb[0].Equals(null)) setKBDefault();
        }

        public static void LoadSOSetting()
        {
            for (int i = 0; i < btnct.Length; i++)
            {
                btnct[i] = PlayerPrefs.GetString(
                    "c" + (i + 1).ToString(), DEFAULT_DS4[i]
                );
            }

            for (int i = 0; i < dpad.Length; i++)
            {
                dpad[i] = bool.Parse(PlayerPrefs.GetString(
                    "dpad" + (i + 1).ToString(), DEFAULT_DPAD_DS4[i].ToString())
                );
            }

            if (btnct[0].Equals(null)) setDS4Default();
        }

        public static string GetKey(string name, int i)
        {
            if(name.StartsWith("k"))
            {
                return PlayerPrefs.GetString(name, DEFAULT_KB[i]);
            }
            else
            {
                return PlayerPrefs.GetString(name, DEFAULT_DS4[i]);
            }
        }
    }
}
