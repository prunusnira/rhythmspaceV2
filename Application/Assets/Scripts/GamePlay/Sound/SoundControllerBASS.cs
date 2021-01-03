using BMSCore;
using System;
using Un4seen.Bass;

namespace BMSPlayer
{
    public class SoundControllerBASS : ISoundController
    {
        private static SoundControllerBASS instance;
        public static SoundControllerBASS Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new SoundControllerBASS();
                }
                return instance;
            }
        }

        public SoundControllerBASS()
        {
        }

        // Execute after BMSAnalyzer.FullAnalyzer worked
        public void PreloadSound(BMS bms)
        {
            Bass.BASS_Free();
            Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);

            foreach (string val in bms.WavList.Keys)
            {
                string filepath = bms.FolderPath + bms.WavList[val];
                int stream = Bass.BASS_StreamCreateFile(filepath, 0, 0, BASSFlag.BASS_DEFAULT);
                bms.WavFilesBA.Add(val, stream);
            }
        }

        public void PlayKeySound(string wavFile, BMS bms, int line)
        {
            if(bms.WavFilesBA.ContainsKey(wavFile))
            {
                Bass.BASS_ChannelPlay(bms.WavFilesBA[wavFile], false);
            }
        }

        public bool CheckSoundPlaying(BMS bms = null)
        {
            bool isPlaying = false;
            foreach(int stream in bms.WavFilesBA.Values)
            {
                BASSActive active = Bass.BASS_ChannelIsActive(stream);
                if(active == BASSActive.BASS_ACTIVE_PLAYING)
                {
                    isPlaying = true;
                    break;
                }
            }
            return isPlaying;
        }

        public void StopAll(BMS bms = null)
        {
            Bass.BASS_Stop();
        }

        public void PauseAll(BMS bms = null)
        {
            Bass.BASS_Pause();
        }

        public void ResumeAll(BMS bms = null)
        {
            Bass.BASS_Start();
        }

        public void FreeMemory(BMS bms)
        {
            Bass.BASS_Free();
        }
    }
}