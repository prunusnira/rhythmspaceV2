﻿using BMSCore;
using UnityEngine;

namespace BMSPlayer
{
    public class SoundControllerUnity : MonoBehaviour, ISoundController
    {
        private AudioSource[] audioSource;

        public void Initialize()
        {
            audioSource = new AudioSource[Const.CHANNEL];
        }

        public void InitSoundChannels()
        {
            for (int i = 0; i < Const.CHANNEL; i++)
            {
                audioSource[i] = gameObject.GetComponent<AudioSource>();
            }
        }

        // Execute after BMSAnalyzer.FullAnalyzer worked
        public void PreloadSound(BMS bms)
        {
            foreach (string val in bms.mWavList.Keys)
            {
                string filepath = bms.getFolderPath() + bms.mWavList[val];
                string encoded = filepath.Replace("#", "%23");
                WWW path = new WWW("file://" + encoded);
                AudioClip clip = path.GetAudioClip();
                clip.name = val;
                clip.LoadAudioData();
                while (clip.loadState != AudioDataLoadState.Loaded) { }
                bms.mWavFilesAC.Add(val, clip);
            }
        }

        public void PlayKeySound(int lane, string wavFile, ref BMS bms)
        {
            try
            {
                audioSource[lane].PlayOneShot(bms.mWavFilesAC[wavFile]);

            }
            catch (System.Exception e)
            {
                Debug.Log("ERROR: " + wavFile);
            }
        }

        public bool CheckSoundPlaying()
        {
            bool isPlaying = false;
            for (int i = 0; i < Const.CHANNEL; i++)
            {
                isPlaying = audioSource[i].isPlaying;
                if (isPlaying) break;
            }
            return isPlaying;
        }

        public void StopAll()
        {
            foreach (AudioSource source in audioSource)
            {
                if (source.isPlaying) source.Stop();
            }
        }
    }
}