using UnityEngine;

namespace BMSPlayer
{
    class StageFailed: MonoBehaviour
    {
        public GameObject panelFailed;
        public AudioSource sfx;

        public void ShowGameOver()
        {
            panelFailed.SetActive(true);
            sfx.Play();
        }
    }
}
