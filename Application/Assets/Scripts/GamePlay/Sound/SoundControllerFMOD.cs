using BMSCore;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BMSPlayer
{
    public class SoundControllerFMOD : ISoundController
    {
        private FMOD.ChannelGroup channelGroup;
        private List<FMOD.Channel> channels;

        private static SoundControllerFMOD instance;
        public static SoundControllerFMOD Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new SoundControllerFMOD();
                }
                return instance;
            }
        }

        public SoundControllerFMOD()
        {
            channels = new List<FMOD.Channel>();
            channelGroup = new FMOD.ChannelGroup();
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
            FMOD.Channel channel;
            FMODUnity.RuntimeManager.CoreSystem.playSound(
                bms.WavFilesFM[wavFile],
                channelGroup,
                false,
                out channel
            );
            channel.setVolume(0.5f);
            channel.setLoopCount(0);
            channels.Add(channel);
        }

        public bool CheckSoundPlaying()
        {
            bool isPlaying = false;
            foreach(FMOD.Channel c in channels)
            {
                c.isPlaying(out isPlaying);
                if(isPlaying)
                {
                    break;
                }
            }
            return isPlaying;
        }

        public void StopAll()
        {
            foreach (FMOD.Channel c in channels)
            {
                c.stop();
            }
        }

        public void PauseAll()
        {
            foreach (FMOD.Channel c in channels)
            {
                c.setPaused(true);
            }
        }

        public void ResumeAll()
        {
            foreach (FMOD.Channel c in channels)
            {
                c.setPaused(false);
            }
        }

        public void FreeMemory(BMS bms)
        {
            foreach (FMOD.Sound snd in bms.WavFilesFM.Values)
            {
                snd.release();
            }
        }

        public void FMODErrorCheck(FMOD.RESULT result)
        {
            if(result != FMOD.RESULT.OK)
            {
                Debug.LogError(FMOD.Error.String(result));
            }
        }
    }
}