using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BMSPlayer
{
    public class KeyboardChanger : MonoBehaviour
    {
        public GameObject self;
        public TextMesh desc;
        public TextMesh changeableKey;

        void Update()
        {
            SetKeyName(Const.KeyToChange);
            StartCoroutine(KeyInputChecker(Const.KeyToChange));
            if (Const.isKeyChanged)
            {
                Const.KeyToChange = "";
                self.SetActive(false);
            }

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Const.KeyToChange = "";
                self.SetActive(false);
            }
        }

        private void SetKeyName(string name)
        {
            desc.text = Const.KeyChangeDescription[(int)Const.Language];
            switch (name)
            {
                case "k1":
                    changeableKey.text = "TurnTable 1 (Keyboard)";
                    break;
                case "k2":
                    changeableKey.text = "Button 1 (Keyboard)";
                    break;
                case "k3":
                    changeableKey.text = "Button 2 (Keyboard)";
                    break;
                case "k4":
                    changeableKey.text = "Button 3 (Keyboard)";
                    break;
                case "k5":
                    changeableKey.text = "Button 4 (Keyboard)";
                    break;
                case "k6":
                    changeableKey.text = "Button 5 (Keyboard)";
                    break;
                case "k7":
                    changeableKey.text = "Button 6 (Keyboard)";
                    break;
                case "k8":
                    changeableKey.text = "Button 7 (Keyboard)";
                    break;
                case "k9":
                    changeableKey.text = "TurnTable 2 (Keyboard)";
                    break;
            }
        }

        IEnumerator KeyInputChecker(string name)
        {
            string key = null;

            foreach (KeyCode code in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(code))
                {
                    key = code.ToString();

                    switch (name)
                    {
                        case "k1":
                            Keys.btnkb[0] = key;
                            break;
                        case "k2":
                            Keys.btnkb[1] = key;
                            break;
                        case "k3":
                            Keys.btnkb[2] = key;
                            break;
                        case "k4":
                            Keys.btnkb[3] = key;
                            break;
                        case "k5":
                            Keys.btnkb[4] = key;
                            break;
                        case "k6":
                            Keys.btnkb[5] = key;
                            break;
                        case "k7":
                            Keys.btnkb[6] = key;
                            break;
                        case "k8":
                            Keys.btnkb[7] = key;
                            break;
                        case "k9":
                            Keys.btnkb[8] = key;
                            break;
                    }
                    Const.isKeyChanged = true;
                }
            }

            yield return null;
        }
    }
}
