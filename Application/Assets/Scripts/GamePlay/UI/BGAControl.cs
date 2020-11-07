using BMSCore;
using LibVLCSharp;
using System;
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

        // VLC Player
        public Renderer bgaVideoRenderer;
        /*private LibVLC libVLC;
        private MediaPlayer mediaPlayer;
        private Texture2D tex = null;
        private bool play = false;*/

        public static int playerWidth = 0;
        public static int playerHeight = 0;

        /*private void Awake()
        {
            Core.Initialize(Application.dataPath);
            libVLC = new LibVLC("--no-osd", "--verbose=2");
            mediaPlayer = new MediaPlayer(libVLC);
        }

        private void Update()
        {
            if(play)
            {
                if(tex == null)
                {
                    uint width = 0;
                    uint height = 0;
                    mediaPlayer.Size(0, ref width, ref height);
                    IntPtr texptr = mediaPlayer.GetTexture(out bool updated);
                    if (width != 0 && height != 0 && updated && texptr != IntPtr.Zero)
                    {
                        tex = Texture2D.CreateExternalTexture(
                            (int)width, (int)height,
                            TextureFormat.RGBA32,
                            false,
                            true,
                            texptr
                        );
                        ((SpriteRenderer)bgaVideoRenderer).sprite =
                            Tools.TextureToSprite(
                                tex,
                                (int)width,
                                (int)height,
                                bgaVideoRenderer.GetComponent<RectTransform>().pivot);
                        bgaVideoRenderer.material.mainTexture = tex;
                        ((SpriteRenderer)bgaVideoRenderer).drawMode = SpriteDrawMode.Sliced;
                        ((SpriteRenderer)bgaVideoRenderer).size = new Vector2(playerWidth, playerHeight);
                    }
                }
                else
                {
                    IntPtr texptr = mediaPlayer.GetTexture(out bool updated);
                    if (updated)
                    {
                        tex.UpdateExternalTexture(texptr);
                    }
                }
            }
        }

        private void OnDisable()
        {
            mediaPlayer.Stop();
            mediaPlayer.Dispose();
            mediaPlayer = null;

            libVLC.Dispose();
            libVLC = null;
        }*/

        public void BGAVideoActivate()
        {
            bgaVideoLayer.SetActive(true);
        }

        public void BGAVideoPreload(string file)
        {
            bgaVideo.url = "file://" + file;
            bgaVideo.errorReceived += BGAErrorLayer;
            bgaVideo.Prepare();

            /*mediaPlayer.Media = new Media(
                libVLC,
                new Uri("file://" + file)
            );
            mediaPlayer.SetLogoInt(VideoLogoOption.Opacity, 0);
            mediaPlayer.Play();

            play = true;*/
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
            //mediaPlayer.Play();
        }

        public bool isBGAPlaying()
        {
            return bgaVideo.isPlaying;
            //return mediaPlayer.IsPlaying;
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
            //mediaPlayer.Pause();
        }

        public void ResumeBGAVideo()
        {
            bgaVideo.Play();
            //mediaPlayer.Play();
        }
    }
}
