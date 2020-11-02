using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

namespace BMSPlayer
{
    public class BGAControl : MonoBehaviour
    {
        public SpriteRenderer bgaImage;
        public RectTransform bgaRect;
        public SpriteRenderer layerImage;
        public RectTransform layerRect;
        public VideoPlayer bgaVideo;
        public GameObject bgaVideoLayer;
        public GameObject bgaErrorLayer;

        public void BGAVideoActivate()
        {
            bgaVideoLayer.SetActive(true);
        }

        public void BGAVideoPreload(string file)
        {
            bgaVideo.url = "file://" + file;
            bgaVideo.errorReceived += BGAErrorLayer;
            bgaVideo.Prepare();
        }

        public void BGAImageActivate()
        {
            bgaImage.gameObject.SetActive(true);
        }

        public void LayerImageActivate()
        {
            layerImage.gameObject.SetActive(true);
        }

        public void BGAVideoPlay()
        {
            bgaVideo.Play();
        }

        public bool isBGAPlaying()
        {
            return bgaVideo.isPlaying;
        }

        public void BGAErrorLayer(VideoPlayer source, string msg)
        {
            // 메모리 릭 방지
            bgaVideo.errorReceived -= BGAErrorLayer;

            // 레이어 띄우기
            bgaErrorLayer.SetActive(true);
            bgaErrorLayer.GetComponentsInChildren<TextMeshPro>()[0].text =
                Const.videoCodecMsg[(int)Const.Language];
        }

        public void BGAImageSetting(Sprite img)
        {
            if (img != null)
            {
                bgaImage.sprite = img;

                float width = bgaImage.sprite.bounds.size.x;
                float height = bgaImage.sprite.bounds.size.y;

                float rectWidth = bgaRect.sizeDelta.x;
                float rectHeight = bgaRect.sizeDelta.y;

                bgaImage.gameObject.transform.localScale =
                    new Vector3(
                        rectWidth / width,
                        rectHeight / height
                    );
            }
        }

        public void LayerImageSetting(Sprite img)
        {
            if (img != null)
            {
                layerImage.sprite = img;

                float width = layerImage.sprite.bounds.size.x;
                float height = layerImage.sprite.bounds.size.y;

                float rectWidth = layerRect.sizeDelta.x;
                float rectHeight = layerRect.sizeDelta.y;

                layerImage.gameObject.transform.localScale =
                    new Vector3(
                        rectWidth / width,
                        rectHeight / height
                    );
            }
        }

        public void PauseBGAVideo()
        {
            bgaVideo.Pause();
        }

        public void ResumeBGAVideo()
        {
            bgaVideo.Play();
        }
    }
}
