using BMSCore;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BMSPlayer
{
    public class SoundControllerNAudio : ISoundController
    {
        private static SoundControllerNAudio instance;
        public static SoundControllerNAudio Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new SoundControllerNAudio();
                }
                return instance;
            }
        }

        public SoundControllerNAudio()
        {
        }

        // Execute after BMSAnalyzer.FullAnalyzer worked
        public void PreloadSound(BMS bms)
        {
            foreach (string val in bms.WavList.Keys)
            {
                string filepath = bms.FolderPath + bms.WavList[val];
                FMOD.Sound snd;
                FMODUnity.RuntimeManager.CoreSystem.createSound(filepath, FMOD.MODE.CREATESAMPLE, out snd);
                bms.WavFilesFM.Add(val, snd);
            }
        }

        public void PlayKeySound(string wavFile, BMS bms, int line)
        {
            if(bms.WavFilesFM.ContainsKey(wavFile))
            {
                
            }
        }

        public bool CheckSoundPlaying()
        {
            bool isPlaying = false;
            /*foreach(FMOD.Channel c in channels)
            {
                c.isPlaying(out isPlaying);
                if(isPlaying)
                {
                    break;
                }
            }*/
            return isPlaying;
        }

        public void StopAll()
        {
            /*foreach (FMOD.Channel c in channels)
            {
                c.stop();
            }*/
        }

        public void PauseAll()
        {
            /*foreach (FMOD.Channel c in channels)
            {
                c.setPaused(true);
            }*/
        }

        public void ResumeAll()
        {
            /*foreach (FMOD.Channel c in channels)
            {
                c.setPaused(false);
            }*/
        }

        public void FreeMemory(BMS bms)
        {
            foreach (FMOD.Sound snd in bms.WavFilesFM.Values)
            {
                snd.release();
            }
        }
    }
}