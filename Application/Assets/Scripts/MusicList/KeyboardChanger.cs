using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BMSPlayer
{
    public class KeyboardChanger : MonoBehaviour
    {
        public GameObject self;
        public TextMesh changeableKey;

        void Update()
        {
            string name = Const.GetKeyChange();
            changeKeyName(name);
            StartCoroutine(KeyInputChecker(name));
            if (Const.isKeyChanged)
            {
                Const.SetKeyChange("");
                self.SetActive(false);
            }

            if (name != "" && Input.GetMouseButtonDown(0))
            {
                Vector3 touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                RaycastHit hit;

                if (Physics.Raycast(touchPos, Vector3.forward, out hit))
                {
                    switch (hit.collider.name)
                    {
                        case "closeKeySetting":
                            self.SetActive(false);
                            break;
                    }
                }
            }
        }

        private void changeKeyName(string name)
        {
            switch (name)
            {
                case "k51":
                    changeableKey.text = "5/6/8 Key Button 1";
                    break;
                case "k52":
                    changeableKey.text = "5/6/8 Key Button 2";
                    break;
                case "k53":
                    changeableKey.text = "5/6/8 Key Button 3\n(In 5 buttons, Button 3 and 4 is mapped to middle button)";
                    break;
                case "k54":
                    changeableKey.text = "5/6/8 Key Button 4\n(In 5 buttons, Button 3 and 4 is mapped to middle button)";
                    break;
                case "k55":
                    changeableKey.text = "5/6/8 Key Button 5";
                    break;
                case "k56":
                    changeableKey.text = "5/6/8 Key Button 6";
                    break;
                case "k87":
                    changeableKey.text = "8 Key Button 7";
                    break;
                case "k88":
                    changeableKey.text = "8 Key Button 8";
                    break;
            }
        }

        IEnumerator KeyInputChecker(string name)
        {
            if (Input.anyKeyDown)
            {
                string key = Input.inputString;
                key = key.ToUpper();

                switch (name)
                {
                    case "k51":
                        Keys.btnkb[0] = key;
                        break;
                    case "k52":
                        Keys.btnkb[1] = key;
                        break;
                    case "k53":
                        Keys.btnkb[2] = key;
                        break;
                    case "k54":
                        Keys.btnkb[3] = key;
                        break;
                    case "k55":
                        Keys.btnkb[4] = key;
                        break;
                    case "k56":
                        Keys.btnkb[5] = key;
                        break;
                    case "k87":
                        Keys.btnkb[6] = key;
                        break;
                    case "k88":
                        Keys.btnkb[7] = key;
                        break;
                }

                Const.isKeyChanged = true;
            }

            yield return null;
        }
    }
}
