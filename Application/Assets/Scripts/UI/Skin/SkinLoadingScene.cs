using BMSCore;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace BMSPlayer
{
    class SkinLoadingScene: MonoBehaviour
    {
        [Header("Background")]
        public VideoPlayer BGVideo;
        public Image BGImage;

        [Header("BGM")]
        public AudioSource bgm;

        private void Awake()
        {
            if (SkinLoader.SkinData["IsLoadingBGVideo"] == "true")
            {
                BGVideo.url = "file://" + SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["LoadingBGFilePath"];
                BGImage.gameObject.SetActive(false);
            }
            else
            {
                BGImage.sprite = Tools.createSpriteFromFile(SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["LoadingBGFilePath"]);
                BGVideo.gameObject.SetActive(false);
            }

            bgm.loop = false;
            bgm.volume = 0.5f;
            WWW www = new WWW("file://" + SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["LoadingBGM"]);
            bgm.clip = www.GetAudioClip(false, true);
            bgm.Play();
        }
    }
}
