using BMSCore;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace BMSPlayer
{
    class SkinResult: MonoBehaviour
    {
        [Header("Background")]
        public VideoPlayer BGVideo;
        public Image BGImage;

        [Header("Title")]
        public Image ResultTitle;

        [Header("Select Loop")]
        public AudioSource bgmLoop;

        private void Awake()
        {
            if (SkinLoader.SkinData["IsResultBGVideo"] == "true")
            {
                BGVideo.url = "file://" + SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["ResultBGFilePath"];
                BGImage.gameObject.SetActive(false);
            }
            else
            {
                BGImage.sprite = Tools.createSpriteFromFile(SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["ResultBGFilePath"]);
                BGVideo.gameObject.SetActive(false);
            }

            ResultTitle.sprite = Tools.createSpriteFromFile(SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["ResultTitle"]);
            
            bgmLoop.loop = true;
            bgmLoop.volume = 0.5f;
            WWW www = new WWW("file://" + SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["ResultLoop"]);
            bgmLoop.clip = www.GetAudioClip(false, true);
            bgmLoop.Play();
        }
    }
}
