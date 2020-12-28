using BMSCore;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace BMSPlayer
{
    class SkinMusicList: MonoBehaviour
    {
        [Header("Background")]
        public VideoPlayer BGVideo;
        public Image BGImage;

        [Header("Title")]
        public Image ListTitle;

        [Header("Difficulty")]
        public Image diffBeg;
        public Image diffNor;
        public Image diffHyp;
        public Image diffAno;
        public Image diffIns;

        [Header("Select Loop")]
        public AudioSource bgmLoop;

        public static bool IsSkinChanged = false;

        // MusicList 한정으로 Start에서 스킨을 적용함
        private void Start()
        {
            ChangeSkin();
        }

        private void Update()
        {
            if(IsSkinChanged)
            {
                IsSkinChanged = false;
                ChangeSkin();
            }

            if(!bgmLoop.isPlaying)
            {
                bgmLoop.Play();
            }
        }

        public void ChangeSkin()
        {
            if (SkinLoader.SkinData["IsListBGVideo"] == "true")
            {
                BGVideo.url = "file://" + SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["ListBGFilePath"];
                BGImage.gameObject.SetActive(false);
                BGVideo.gameObject.SetActive(true);
            }
            else
            {
                BGImage.sprite = Tools.createSpriteFromFile(SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["ListBGFilePath"]);
                BGVideo.gameObject.SetActive(false);
                BGImage.gameObject.SetActive(true);
            }

            ListTitle.sprite = Tools.createSpriteFromFile(SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["ListTitle"]);

            // Difficulty 표기 부분 Sprite
            SkinSetting.DiffBegOff = Tools.createSpriteFromFile(SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["DiffBegOff"]);
            SkinSetting.DiffBegOn = Tools.createSpriteFromFile(SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["DiffBegOn"]);
            SkinSetting.DiffNorOff = Tools.createSpriteFromFile(SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["DiffNorOff"]);
            SkinSetting.DiffNorOn = Tools.createSpriteFromFile(SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["DiffNorOn"]);
            SkinSetting.DiffHypOff = Tools.createSpriteFromFile(SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["DiffHypOff"]);
            SkinSetting.DiffHypOn = Tools.createSpriteFromFile(SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["DiffHypOn"]);
            SkinSetting.DiffAnoOff = Tools.createSpriteFromFile(SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["DiffAnoOff"]);
            SkinSetting.DiffAnoOn = Tools.createSpriteFromFile(SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["DiffAnoOn"]);
            SkinSetting.DiffInsOff = Tools.createSpriteFromFile(SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["DiffInsOff"]);
            SkinSetting.DiffInsOn = Tools.createSpriteFromFile(SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["DiffInsOn"]);
            SkinSetting.DiffUnkOff = Tools.createSpriteFromFile(SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["DiffUnkOff"]);
            SkinSetting.DiffUnkOn = Tools.createSpriteFromFile(SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["DiffUnkOn"]);

            diffBeg.sprite = SkinSetting.DiffBegOff;
            diffNor.sprite = SkinSetting.DiffNorOff;
            diffHyp.sprite = SkinSetting.DiffHypOff;
            diffAno.sprite = SkinSetting.DiffAnoOff;
            diffIns.sprite = SkinSetting.DiffInsOff;

            // 클리어 램프
            SkinSetting.RampNoPlay = Tools.createSpriteFromFile(SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["RampNoPlay"]);
            SkinSetting.RampAssist = Tools.createSpriteFromFile(SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["RampAssist"]);
            SkinSetting.RampEasy = Tools.createSpriteFromFile(SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["RampEasy"]);
            SkinSetting.RampNormal = Tools.createSpriteFromFile(SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["RampNormal"]);
            SkinSetting.RampHard = Tools.createSpriteFromFile(SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["RampHard"]);
            SkinSetting.RampEXHard = Tools.createSpriteFromFile(SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["RampEXHard"]);
            SkinSetting.RampFullCombo = Tools.createSpriteFromFile(SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["RampFullCombo"]);
            SkinSetting.RampPerfect = Tools.createSpriteFromFile(SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["RampPerfect"]);
            SkinSetting.RampFailed = Tools.createSpriteFromFile(SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["RampFailed"]);

            // 리스트 이미지
            SkinSetting.FolderImg = Tools.createSpriteFromFile(SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["FolderImg"]);
            SkinSetting.TableImg = Tools.createSpriteFromFile(SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["TableImg"]);
            SkinSetting.NameImg = Tools.createSpriteFromFile(SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["NameImg"]);
            SkinSetting.RankImg = Tools.createSpriteFromFile(SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["RankImg"]);
            SkinSetting.PatternImg = Tools.createSpriteFromFile(SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["PatternImg"]);
            SkinSetting.PatternNotExistImg = Tools.createSpriteFromFile(SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["PatternNotExistImg"]);

            // 랭크
            SkinSetting.RankAAA = Tools.createSpriteFromFile(SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["RankAAA"], 1f, 0.5f, 0.5f);
            SkinSetting.RankAA = Tools.createSpriteFromFile(SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["RankAA"], 1f, 0.5f, 0.5f);
            SkinSetting.RankA = Tools.createSpriteFromFile(SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["RankA"], 1f, 0.5f, 0.5f);
            SkinSetting.RankB = Tools.createSpriteFromFile(SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["RankB"], 1f, 0.5f, 0.5f);
            SkinSetting.RankC = Tools.createSpriteFromFile(SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["RankC"], 1f, 0.5f, 0.5f);
            SkinSetting.RankD = Tools.createSpriteFromFile(SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["RankD"], 1f, 0.5f, 0.5f);
            SkinSetting.RankE = Tools.createSpriteFromFile(SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["RankE"], 1f, 0.5f, 0.5f);
            SkinSetting.RankF = Tools.createSpriteFromFile(SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["RankF"], 1f, 0.5f, 0.5f);

            bgmLoop.loop = true;
            bgmLoop.volume = 0.5f;
            WWW www = new WWW("file://" + SkinSetting.SkinSetMainPath + "/" + SkinLoader.SkinData["SelectLoop"]);
            bgmLoop.clip = www.GetAudioClip(false, true);
        }
    }
}
