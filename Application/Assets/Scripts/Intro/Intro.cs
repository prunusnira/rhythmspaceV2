using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BMSPlayer
{
    public class Intro : MonoBehaviour
    {
        public GameObject fadeCube;
        private bool isFading = false;

        private void Awake()
        {
            fadeCube.GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        }

        private void OnGUI()
        {
            if (isFading)
            {
                StartCoroutine("FadeOut");
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return) && !isFading)
            {
                isFading = true;
                fadeCube.SetActive(true);
            }

            if (fadeCube.GetComponent<Renderer>().material.color.a >= 1.0f)
            {
                SceneManager.LoadScene("MusicSelect");
            }
        }

        IEnumerator FadeOut()
        {
            if (fadeCube.GetComponent<Renderer>().material.color.a < 1.0f)
            {
                Color c = fadeCube.GetComponent<Renderer>().material.color;
                c.a += 0.05f;
                fadeCube.GetComponent<Renderer>().material.color = c;
                yield return new WaitForSeconds(0.2f);
            }
            else
            {
                isFading = false;
                yield return null;
            }
        }
    }
}
