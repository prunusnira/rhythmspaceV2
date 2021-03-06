﻿using BMSCore;
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
        public VideoPlayer bgaVideoPlayer;
        public SpriteRenderer bgaVideoMPG;
        public SpriteRenderer bgaVideoNormal;
        public GameObject bgaErrorLayer;

        private bool isVLCNeeded = false;

        // VLC Player
        private LibVLC libVLC;
        private MediaPlayer mediaPlayer;
        private Texture2D tex = null;
        private bool play = false;

        public static int playerWidth = 0;
        public static int playerHeight = 0;

        private void Awake()
        {
            Core.Initialize(Application.dataPath);
            libVLC = new LibVLC(/*"--no-osd", "--verbose=2"*/);
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
                        bgaVideoMPG.sprite =
                            Tools.TextureToSprite(
                                tex,
                                (int)width,
                                (int)height,
                                bgaVideoMPG.GetComponent<RectTransform>().pivot);
                        bgaVideoMPG.material.mainTexture = tex;
                        bgaVideoMPG.drawMode = SpriteDrawMode.Sliced;
                        bgaVideoMPG.size = new Vector2(playerWidth, playerHeight);
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
        }

        public void BGAVideoActivateVLC()
        {
            bgaVideoMPG.gameObject.SetActive(true);
        }

        public void BGAVideoActivateNM()
        {
            bgaVideoNormal.gameObject.SetActive(true);
        }

        public void BGAVideoPreloadVLC(string file)
        {
            mediaPlayer.Media = new Media(
                libVLC,
                new Uri("file://" + file)
            );
            mediaPlayer.SetLogoInt(VideoLogoOption.Opacity, 0);

            play = true;
        }

        public void BGAVideoPreloadNM(string file)
        {
            bgaVideoPlayer.url = "file://" + file;
            bgaVideoPlayer.errorReceived += BGAErrorLayer;
            bgaVideoPlayer.Prepare();
        }

        public void BGAImageActivate()
        {
            bgaImage.gameObject.SetActive(true);
        }

        public void LayerImageActivate()
        {
            layerImage.gameObject.SetActive(true);
        }

        public void BGAVideoPlayVLC()
        {
            mediaPlayer.Play();
        }

        public void BGAVideoPlayNM()
        {
            bgaVideoPlayer.Play();
        }

        public bool isBGAPlayingVLC()
        {
            return mediaPlayer.IsPlaying;
        }

        public bool isBGAPlayingNM()
        {
            return bgaVideoPlayer.isPlaying;
        }

        public void BGAErrorLayer(VideoPlayer source, string msg)
        {
            // 메모리 릭 방지
            bgaVideoPlayer.errorReceived -= BGAErrorLayer;

            // 레이어 띄우기
            bgaErrorLayer.SetActive(true);
            bgaErrorLayer.GetComponentsInChildren<TextMeshProUGUI>()[0].text =
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

        public void PauseBGAVideoVLC()
        {
            mediaPlayer.Pause();
        }

        public void PauseBGAVideoNM()
        {
            bgaVideoPlayer.Pause();
        }

        public void ResumeBGAVideoVLC()
        {
            mediaPlayer.Play();
        }

        public void ResumeBGAVideoNM()
        {
            bgaVideoPlayer.Play();
        }

        public void SetVLCNeed(bool need)
        {
            isVLCNeeded = need;
        }

        public bool IsVLCNeeded()
        {
            return isVLCNeeded;
        }
    }
}
