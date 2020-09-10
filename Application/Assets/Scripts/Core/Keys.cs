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
        public static string btn1k;
        public static string btn2k;
        public static string btn3k;
        public static string btn4k;
        public static string btn5k;
        public static string btn6k;
        public static string btn7k;
        public static string btn8k;

        public static string btn1c;
        public static string btn2c;
        public static string btn3c;
        public static string btn4c;
        public static string btn5c;
        public static string btn6c;
        public static string btn7c;
        public static string btn8c;

        public static bool dpad1;
        public static bool dpad2;
        public static bool dpad3;
        public static bool dpad4;
        public static bool dpad5;
        public static bool dpad6;
        public static bool dpad7;
        public static bool dpad8;

        public static void setKBDefault()
        {
            btn1k = "LeftShift";
            btn2k = "A";
            btn3k = "S";
            btn4k = "D";
            btn5k = "Space";
            btn6k = "J";
            btn7k = "K";
            btn8k = "L";

            SaveKBSetting();
        }

        public static void setXBDefault()
        {
            btn1c = "DPAD_LEFT_XB";
            btn2c = "DPAD_UP_XB";
            btn3c = "DPAD_RIGHT_XB";
            btn4c = "JoystickButton2";
            btn5c = "JoystickButton3";
            btn6c = "JoystickButton1";
            btn7c = "JoystickButton4";
            btn8c = "JoystickButton5";

            dpad1 = true;
            dpad2 = true;
            dpad3 = true;
            dpad4 = false;
            dpad5 = false;
            dpad6 = false;
            dpad7 = false;
            dpad8 = false;

            SaveCOSetting();
        }

        public static void setP4Default()
        {
            btn1c = "DPAD_LEFT_P4";
            btn2c = "DPAD_UP_P4";
            btn3c = "DPAD_RIGHT_P4";
            btn4c = "JoystickButton0";
            btn5c = "JoystickButton3";
            btn6c = "JoystickButton2";
            btn7c = "JoystickButton4";
            btn8c = "JoystickButton5";

            dpad1 = true;
            dpad2 = true;
            dpad3 = true;
            dpad4 = false;
            dpad5 = false;
            dpad6 = false;
            dpad7 = false;
            dpad8 = false;

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
                    case "btn1k": btn1k = val[1]; break;
                    case "btn2k": btn2k = val[1]; break;
                    case "btn3k": btn3k = val[1]; break;
                    case "btn4k": btn4k = val[1]; break;
                    case "btn5k": btn5k = val[1]; break;
                    case "btn6k": btn6k = val[1]; break;
                    case "btn7k": btn7k = val[1]; break;
                    case "btn8k": btn8k = val[1]; break;
                    case "btn1c": btn1c = val[1]; break;
                    case "btn2c": btn2c = val[1]; break;
                    case "btn3c": btn3c = val[1]; break;
                    case "btn4c": btn4c = val[1]; break;
                    case "btn5c": btn5c = val[1]; break;
                    case "btn6c": btn6c = val[1]; break;
                    case "btn7c": btn7c = val[1]; break;
                    case "btn8c": btn8c = val[1]; break;
                    case "dpad1": if (val[1] == "True") dpad1 = true; else dpad1 = false; break;
                    case "dpad2": if (val[1] == "True") dpad2 = true; else dpad2 = false; break;
                    case "dpad3": if (val[1] == "True") dpad3 = true; else dpad3 = false; break;
                    case "dpad4": if (val[1] == "True") dpad4 = true; else dpad4 = false; break;
                    case "dpad5": if (val[1] == "True") dpad5 = true; else dpad5 = false; break;
                    case "dpad6": if (val[1] == "True") dpad6 = true; else dpad6 = false; break;
                    case "dpad7": if (val[1] == "True") dpad7 = true; else dpad7 = false; break;
                    case "dpad8": if (val[1] == "True") dpad8 = true; else dpad8 = false; break;
                }
            }
        }

        public static void savePreset(string name)
        {
            string path = Application.dataPath + "/KeySetting/Preset/" + name + ".conf";
            Directory.CreateDirectory(Application.dataPath + "/KeySetting/Preset/");

            string data =
                "btn1k=" + btn1k + "\n" +
                "btn2k=" + btn2k + "\n" +
                "btn3k=" + btn3k + "\n" +
                "btn4k=" + btn4k + "\n" +
                "btn5k=" + btn5k + "\n" +
                "btn6k=" + btn6k + "\n" +
                "btn7k=" + btn7k + "\n" +
                "btn8k=" + btn8k + "\n" +
                "btn1c=" + btn1c + "\n" +
                "btn2c=" + btn2c + "\n" +
                "btn3c=" + btn3c + "\n" +
                "btn4c=" + btn4c + "\n" +
                "btn5c=" + btn5c + "\n" +
                "btn6c=" + btn6c + "\n" +
                "btn7c=" + btn7c + "\n" +
                "btn8c=" + btn8c + "\n" +
                "dpad1=" + dpad1 + "\n" +
                "dpad2=" + dpad2 + "\n" +
                "dpad3=" + dpad3 + "\n" +
                "dpad4=" + dpad4 + "\n" +
                "dpad5=" + dpad5 + "\n" +
                "dpad6=" + dpad6 + "\n" +
                "dpad7=" + dpad7 + "\n" +
                "dpad8=" + dpad8 + "\n";
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
            PlayerPrefs.SetString("btn1k", btn1k);
            PlayerPrefs.SetString("btn2k", btn2k);
            PlayerPrefs.SetString("btn3k", btn3k);
            PlayerPrefs.SetString("btn4k", btn4k);
            PlayerPrefs.SetString("btn5k", btn5k);
            PlayerPrefs.SetString("btn6k", btn6k);
            PlayerPrefs.SetString("btn7k", btn7k);
            PlayerPrefs.SetString("btn8k", btn8k);
        }

        public static void SaveCOSetting()
        {

            PlayerPrefs.SetString("btn1c", btn1c);
            PlayerPrefs.SetString("btn2c", btn2c);
            PlayerPrefs.SetString("btn3c", btn3c);
            PlayerPrefs.SetString("btn4c", btn4c);
            PlayerPrefs.SetString("btn5c", btn5c);
            PlayerPrefs.SetString("btn6c", btn6c);
            PlayerPrefs.SetString("btn7c", btn7c);
            PlayerPrefs.SetString("btn8c", btn8c);

            PlayerPrefs.SetString("dpad1", dpad1.ToString());
            PlayerPrefs.SetString("dpad2", dpad2.ToString());
            PlayerPrefs.SetString("dpad3", dpad3.ToString());
            PlayerPrefs.SetString("dpad4", dpad4.ToString());
            PlayerPrefs.SetString("dpad5", dpad5.ToString());
            PlayerPrefs.SetString("dpad6", dpad6.ToString());
            PlayerPrefs.SetString("dpad7", dpad7.ToString());
            PlayerPrefs.SetString("dpad8", dpad8.ToString());
        }

        public static void LoadKBSetting()
        {
            btn1k = PlayerPrefs.GetString("btn1k", "S");
            btn2k = PlayerPrefs.GetString("btn2k", "D");
            btn3k = PlayerPrefs.GetString("btn3k", "F");
            btn4k = PlayerPrefs.GetString("btn4k", "J");
            btn5k = PlayerPrefs.GetString("btn5k", "K");
            btn6k = PlayerPrefs.GetString("btn6k", "L");
            btn7k = PlayerPrefs.GetString("btn7k", "E");
            btn8k = PlayerPrefs.GetString("btn8k", "I");

            if (btn1k.Equals(null)) setKBDefault();
        }

        public static void LoadSOSetting()
        {

            btn1c = PlayerPrefs.GetString("btn1c", "DPAD_LEFT_P4");
            btn2c = PlayerPrefs.GetString("btn2c", "DPAD_UP_P4");
            btn3c = PlayerPrefs.GetString("btn3c", "DPAD_RIGHT_P4");
            btn4c = PlayerPrefs.GetString("btn4c", "JoystickButton0");
            btn5c = PlayerPrefs.GetString("btn5c", "JoystickButton3");
            btn6c = PlayerPrefs.GetString("btn6c", "JoystickButton2");
            btn7c = PlayerPrefs.GetString("btn7c", "JoystickButton4");
            btn8c = PlayerPrefs.GetString("btn8c", "JoystickButton5");

            dpad1 = bool.Parse(PlayerPrefs.GetString("dpad1", "true"));
            dpad2 = bool.Parse(PlayerPrefs.GetString("dpad2", "true"));
            dpad3 = bool.Parse(PlayerPrefs.GetString("dpad3", "true"));
            dpad4 = bool.Parse(PlayerPrefs.GetString("dpad4", "false"));
            dpad5 = bool.Parse(PlayerPrefs.GetString("dpad5", "false"));
            dpad6 = bool.Parse(PlayerPrefs.GetString("dpad6", "false"));
            dpad7 = bool.Parse(PlayerPrefs.GetString("dpad7", "false"));
            dpad8 = bool.Parse(PlayerPrefs.GetString("dpad8", "false"));

            if (btn1c.Equals(null)) setKBDefault();
        }
    }
}
