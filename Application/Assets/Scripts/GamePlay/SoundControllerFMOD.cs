using BMSCore;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BMSPlayer
{
    public class SoundControllerFMOD : MonoBehaviour, ISoundController
    {
        private FMOD.ChannelGroup channelGroup;
        private List<FMOD.Channel> channels;

        public void Initialize()
        {
            channels = new List<FMOD.Channel>();
            channelGroup = new FMOD.ChannelGroup();
        }

        public void InitSoundChannels()
        {
            /*for (int i = 0; i < Const.CHANNEL; i++)
            {
                channel[i] = new FMOD.Channel();
                channel[i].setLoopCount(0);
                channel[i].setChannelGroup(channelGroup);
            }*/
        }

        // Execute after BMSAnalyzer.FullAnalyzer worked
        public void PreloadSound(BMS bms)
        {

            foreach (string val in bms.mWavList.Keys)
            {
                string filepath = bms.getFolderPath() + bms.mWavList[val];
                FMOD.Sound snd;
                FMODUnity.RuntimeManager.CoreSystem.createSound(filepath, FMOD.MODE.CREATESAMPLE, out snd);
                bms.mWavFilesFM.Add(val, snd);
            }
        }

        public void PlayKeySound(int lane, string wavFile, ref BMS bms)
        {
            FMOD.RESULT result = FMOD.RESULT.OK;
            try
            {
                FMOD.Channel channel;
                result = FMODUnity.RuntimeManager.CoreSystem.playSound(
                    bms.mWavFilesFM[wavFile],
                    channelGroup,
                    false,
                    out channel
                );
                channel.setLoopCount(0);
                channels.Add(channel);
            }
            catch(Exception e)
            {
                FMODErrorCheck(result);
            }

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
            channelGroup.stop();
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