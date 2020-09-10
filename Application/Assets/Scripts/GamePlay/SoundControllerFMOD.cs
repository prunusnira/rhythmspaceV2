using BMSCore;
using BMSPlayer;
using UnityEngine;

namespace BMSPlayer
{
    public class SoundControllerFMOD : MonoBehaviour, ISoundController
    {
        private FMOD.ChannelGroup channelGroup;
        private FMOD.Channel[] channel;

        public void Initialize()
        {
            channel = new FMOD.Channel[Const.CHANNEL];
            channelGroup = new FMOD.ChannelGroup();
        }

        public void InitSoundChannels()
        {
            for (int i = 0; i < Const.CHANNEL; i++)
            {
                channel[i] = new FMOD.Channel();
                channel[i].setLoopCount(0);
                channel[i].setChannelGroup(channelGroup);
            }
        }

        // Execute after BMSAnalyzer.FullAnalyzer worked
        public void PreloadSound(BMS bms)
        {

            foreach (string val in bms.mWavList.Keys)
            {
                string filepath = bms.getFolderPath() + bms.mWavList[val];
                FMOD.Sound snd;
                FMODUnity.RuntimeManager.CoreSystem.createSound(filepath, FMOD.MODE.DEFAULT/*CREATESAMPLE*/, out snd);
                bms.mWavFilesFM.Add(val, snd);
            }
        }

        public void PlayKeySound(int lane, string wavFile, ref BMS bms)
        {
            try
            {
                FMODUnity.RuntimeManager.CoreSystem.playSound(
                    bms.mWavFilesFM[wavFile],
                    channelGroup,
                    false,
                    out channel[lane]
                );
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
                channel[i].isPlaying(out isPlaying);
                if (isPlaying) break;
            }
            return isPlaying;
        }

        public void StopAll()
        {
            channelGroup.stop();
        }
    }
}