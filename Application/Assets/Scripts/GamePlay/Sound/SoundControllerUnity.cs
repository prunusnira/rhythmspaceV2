using BMSCore;
using UnityEngine;

namespace BMSPlayer
{
    public class SoundControllerUnity : ISoundController
    {
        private AudioSource[] audioSource;

        private static SoundControllerUnity instance;
        public static SoundControllerUnity Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SoundControllerUnity();
                }
                return instance;
            }
        }

        public SoundControllerUnity()
        {
            audioSource = new AudioSource[Const.CHANNEL];
            for (int i = 0; i < Const.CHANNEL; i++)
            {
                //audioSource[i] = gameObject.GetComponent<AudioSource>();
            }
        }

        // Execute after BMSAnalyzer.FullAnalyzer worked
        public void PreloadSound(BMS bms)
        {
            foreach (string val in bms.WavList.Keys)
            {
                string filepath = bms.FolderPath + bms.WavList[val];
                string encoded = filepath.Replace("#", "%23");
                WWW path = new WWW("file://" + encoded);
                AudioClip clip = path.GetAudioClip();
                clip.name = val;
                clip.LoadAudioData();
                while (clip.loadState != AudioDataLoadState.Loaded) { }
                bms.WavFilesAC.Add(val, clip);
            }
        }

        public void PlayKeySound(string wavFile, BMS bms, int line)
        {
            try
            {
                audioSource[line].PlayOneShot(bms.WavFilesAC[wavFile]);
            }
            catch (System.Exception e)
            {
                Debug.Log("ERROR: " + wavFile);
            }
        }

        public bool CheckSoundPlaying(BMS bms = null)
        {
            bool isPlaying = false;
            for (int i = 0; i < Const.CHANNEL; i++)
            {
                isPlaying = audioSource[i].isPlaying;
                if (isPlaying) break;
            }
            return isPlaying;
        }

        public void StopAll(BMS bms = null)
        {
            foreach (AudioSource source in audioSource)
            {
                if (source.isPlaying) source.Stop();
            }
        }

        public void PauseAll(BMS bms = null)
        {
            foreach (AudioSource source in audioSource)
            {
                if (source.isPlaying) source.Pause();
            }
        }

        public void ResumeAll(BMS bms = null)
        {
            foreach (AudioSource source in audioSource)
            {
                if (source.isPlaying) source.UnPause();
            }
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