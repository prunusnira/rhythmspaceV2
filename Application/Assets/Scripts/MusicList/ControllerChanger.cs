using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BMSPlayer
{
    public class ControllerChanger : MonoBehaviour
    {
        public GameObject self;

        void Update()
        {
            if (name != "" && Input.GetMouseButtonDown(0))
            {
                Vector3 touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                RaycastHit hit;

                string name = Const.KeyToChange;
                string key = "";
                bool dp = false;

                if (Physics.Raycast(touchPos, Vector3.forward, out hit))
                {
                    switch (hit.collider.name)
                    {
                        case "dpad_left_xb":
                            key = "DPAD_LEFT_XB";
                            dp = true;
                            break;
                        case "dpad_up_xb":
                            key = "DPAD_UP_XB";
                            dp = true;
                            break;
                        case "dpad_right_xb":
                            key = "DPAD_RIGHT_XB";
                            dp = true;
                            break;
                        case "dpad_down_xb":
                            key = "DPAD_DOWN_XB";
                            dp = true;
                            break;
                        case "a_xb":
                            key = "JoystickButton0";
                            break;
                        case "b_xb":
                            key = "JoystickButton1";
                            break;
                        case "x_xb":
                            key = "JoystickButton2";
                            break;
                        case "y_xb":
                            key = "JoystickButton3";
                            break;
                        case "lb_xb":
                            key = "JoystickButton4";
                            break;
                        case "lt_xb":
                            key = "LT_XB";
                            dp = true;
                            break;
                        case "rb_xb":
                            key = "JoystickButton5";
                            break;
                        case "rt_xb":
                            key = "RT_XB";
                            dp = true;
                            break;
                        case "dpad_left_p4":
                            key = "DPAD_LEFT_P4";
                            dp = true;
                            break;
                        case "dpad_up_p4":
                            key = "DPAD_UP_P4";
                            dp = true;
                            break;
                        case "dpad_right_p4":
                            key = "DPAD_RIGHT_P4";
                            dp = true;
                            break;
                        case "dpad_down_p4":
                            key = "DPAD_DOWN_P4";
                            dp = true;
                            break;
                        case "x_p4":
                            key = "JoystickButton1";
                            break;
                        case "ci_p4":
                            key = "JoystickButton2";
                            break;
                        case "sq_p4":
                            key = "JoystickButton0";
                            break;
                        case "tr_p4":
                            key = "JoystickButton3";
                            break;
                        case "l1_p4":
                            key = "JoystickButton4";
                            break;
                        case "l2_p4":
                            key = "JoystickButton6";
                            break;
                        case "r1_p4":
                            key = "JoystickButton5";
                            break;
                        case "r2_p4":
                            key = "JoystickButton7";
                            break;
                        case "closeCtrlSetting":
                            self.SetActive(false);
                            break;
                    }

                    switch (name)
                    {
                        case "c51":
                            Keys.btnct[0] = key;
                            Keys.dpad[0] = dp;
                            break;
                        case "c52":
                            Keys.btnct[1] = key;
                            Keys.dpad[1] = dp;
                            break;
                        case "c53":
                            Keys.btnct[2] = key;
                            Keys.dpad[2] = dp;
                            break;
                        case "c54":
                            Keys.btnct[3] = key;
                            Keys.dpad[3] = dp;
                            break;
                        case "c55":
                            Keys.btnct[4] = key;
                            Keys.dpad[4] = dp;
                            break;
                        case "c56":
                            Keys.btnct[5] = key;
                            Keys.dpad[5] = dp;
                            break;
                        case "c87":
                            Keys.btnct[6] = key;
                            Keys.dpad[6] = dp;
                            break;
                        case "c88":
                            Keys.btnct[7] = key;
                            Keys.dpad[7] = dp;
                            break;
                    }

                    Const.isKeyChanged = true;
                }
            }

            if (Const.isKeyChanged)
            {
                Const.KeyToChange = "";
                self.SetActive(false);
            }
        }
    }
}
