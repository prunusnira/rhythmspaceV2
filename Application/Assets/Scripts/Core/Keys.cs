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
            "LeftShift", "A", "S", "D", "Space", "J", "K", "L"
        };

        public static string[] DEFAULT_XB = {
            "DPAD_LEFT_XB", "DPAD_UP_XB",
            "DPAD_RIGHT_XB", "JoystickButton2",
            "JoystickButton3", "JoystickButton1",
            "JoystickButton4", "JoystickButton5"
        };

        public static string[] DEFAULT_DS4 =
        {
            "DPAD_LEFT_P4", "DPAD_UP_P4",
            "DPAD_RIGHT_P4", "JoystickButton0",
            "JoystickButton3", "JoystickButton2",
            "JoystickButton4", "JoystickButton5"
        };

        public static bool[] DEFAULT_DPAD =
        {
            true, true, true, false,
            false, false, false, false
        };

        public static string[] btnkb = new string[8];
        public static string[] btnct = new string[8];
        public static bool[] dpad = new bool[8];

        public static void setKBDefault()
        {
            btnkb = DEFAULT_KB;
            SaveKBSetting();
        }

        public static void setXBDefault()
        {
            btnct = DEFAULT_XB;
            dpad = DEFAULT_DPAD;
            SaveCOSetting();
        }

        public static void setDS4Default()
        {
            btnct = DEFAULT_DS4;
            dpad = DEFAULT_DPAD;
            SaveCOSetting();
        }

        public static bool checkDefaultFileExist()
        {
            string path = Application.dataPath + "/KeySetting/Preset/defaultp4.conf";
            if (File.Exists(path))
                return true;
            else
                return false;
        }

        public static void loadPreset(string name)
        {
            string path = Application.dataPath + "/KeySetting/Preset/" + name + ".conf";
            string[] data = File.ReadAllLines(path);

            foreach (string s in data)
            {
                string[] val = s.Split('=');
                switch (val[0])
                {
                    case "btn1k": btnkb[0] = val[1]; break;
                    case "btn2k": btnkb[1] = val[1]; break;
                    case "btn3k": btnkb[2] = val[1]; break;
                    case "btn4k": btnkb[3] = val[1]; break;
                    case "btn5k": btnkb[4] = val[1]; break;
                    case "btn6k": btnkb[5] = val[1]; break;
                    case "btn7k": btnkb[6] = val[1]; break;
                    case "btn8k": btnkb[7] = val[1]; break;
                    case "btn1c": btnct[0] = val[1]; break;
                    case "btn2c": btnct[1] = val[1]; break;
                    case "btn3c": btnct[2] = val[1]; break;
                    case "btn4c": btnct[3] = val[1]; break;
                    case "btn5c": btnct[4] = val[1]; break;
                    case "btn6c": btnct[5] = val[1]; break;
                    case "btn7c": btnct[6] = val[1]; break;
                    case "btn8c": btnct[7] = val[1]; break;
                    case "dpad1": if (val[1] == "True") dpad[0] = true; else dpad[0] = false; break;
                    case "dpad2": if (val[1] == "True") dpad[1] = true; else dpad[1] = false; break;
                    case "dpad3": if (val[1] == "True") dpad[2] = true; else dpad[2] = false; break;
                    case "dpad4": if (val[1] == "True") dpad[3] = true; else dpad[3] = false; break;
                    case "dpad5": if (val[1] == "True") dpad[4] = true; else dpad[4] = false; break;
                    case "dpad6": if (val[1] == "True") dpad[5] = true; else dpad[5] = false; break;
                    case "dpad7": if (val[1] == "True") dpad[6] = true; else dpad[6] = false; break;
                    case "dpad8": if (val[1] == "True") dpad[7] = true; else dpad[7] = false; break;
                }
            }
        }

        public static void savePreset(string name)
        {
            string path = Application.dataPath + "/KeySetting/Preset/" + name + ".conf";
            Directory.CreateDirectory(Application.dataPath + "/KeySetting/Preset/");

            string data = "";
            for(int i = 0; i < btnkb.Length; i++)
            {
                data += "btn" + (i + 1) + "k=" + btnkb[0] + "\n";
            }
            for (int i = 0; i < btnct.Length; i++)
            {
                data += "btn" + (i + 1) + "c=" + btnct[0] + "\n";
            }
            for (int i = 0; i < dpad.Length; i++)
            {
                data += "dpad" + (i + 1) + "=" + dpad[0] + "\n";
            }
            File.WriteAllText(path, data);
        }

        // Key Input Wrapper
        public static bool GetKeyAxisDown(string key)
        {
            // 받은 키가 dpad이면 그 쪽을 처리
            if (key == "DPAD_LEFT_XB") // axis 6
            {
                return Input.GetAxis("axis6m") < 0;
            }
            else if (key == "DPAD_UP_XB") // axis 7
            {
                return Input.GetAxis("axis7m") < 0;
            }
            else if (key == "DPAD_RIGHT_XB") // axis 6
            {
                return Input.GetAxis("axis6p") > 0;
            }
            else if (key == "DPAD_DOWN_XB") // axis 7
            {
                return Input.GetAxis("axis7p") > 0;
            }
            else if (key == "LT_XB") // axis 3
            {
                return Input.GetAxis("axis3m") < 0;
            }
            else if (key == "RT_XB") // axis 3
            {
                return Input.GetAxis("axis3p") > 0;
            }
            else if (key == "DPAD_LEFT_P4") // axis 7
            {
                return Input.GetAxis("axis7m") < 0;
            }
            else if (key == "DPAD_UP_P4") // axis 8
            {
                return Input.GetAxis("axis8p") > 0;
            }
            else if (key == "DPAD_RIGHT_P4") // axis 7
            {
                return Input.GetAxis("axis7p") > 0;
            }
            else if (key == "DPAD_DOWN_P4") // axis 8
            {
                return Input.GetAxis("axis8m") < 0;
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
                return Input.GetAxis("axis6m") == 0;
            }
            else if (key == "DPAD_UP_XB") // axis 7
            {
                return Input.GetAxis("axis7m") == 0;
            }
            else if (key == "DPAD_RIGHT_XB") // axis 6
            {
                return Input.GetAxis("axis6p") == 0;
            }
            else if (key == "DPAD_DOWN_XB") // axis 7
            {
                return Input.GetAxis("axis7p") == 0;
            }
            else if (key == "LT_XB") // axis 3
            {
                return Input.GetAxis("axis3m") == 0;
            }
            else if (key == "RT_XB") // axis 3
            {
                return Input.GetAxis("axis3p") == 0;
            }
            else if (key == "DPAD_LEFT_P4") // axis 7
            {
                return Input.GetAxis("axis7m") == 0;
            }
            else if (key == "DPAD_UP_P4") // axis 8
            {
                return Input.GetAxis("axis8p") == 0;
            }
            else if (key == "DPAD_RIGHT_P4") // axis 7
            {
                return Input.GetAxis("axis7p") == 0;
            }
            else if (key == "DPAD_DOWN_P4") // axis 8
            {
                return Input.GetAxis("axis8m") == 0;
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
                PlayerPrefs.SetString("btn"+(i+1).ToString()+"k", btnkb[i]);
            }
        }

        public static void SaveCOSetting()
        {
            for (int i = 0; i < btnct.Length; i++)
            {
                PlayerPrefs.SetString("btn" + (i + 1).ToString() + "c", btnct[i]);
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
                btnkb[0] = PlayerPrefs.GetString(
                    "btn"+(i+1).ToString() +"k", DEFAULT_KB[i]
                );
            }

            if (btnkb[0].Equals(null)) setKBDefault();
        }

        public static void LoadSOSetting()
        {
            for (int i = 0; i < btnct.Length; i++)
            {
                btnct[0] = PlayerPrefs.GetString(
                    "btn" + (i + 1).ToString() + "c", DEFAULT_DS4[i]
                );
            }

            for (int i = 0; i < dpad.Length; i++)
            {
                dpad[0] = bool.Parse(PlayerPrefs.GetString(
                    "dpad" + (i + 1).ToString(), DEFAULT_DPAD[i].ToString())
                );
            }

            if (btnct.Equals(null)) setDS4Default();
        }
    }
}
