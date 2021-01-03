using BMSCore;
using System.Collections.Generic;
using UnityEngine;

namespace BMSPlayer
{
    public class SoundControllerUnity : MonoBehaviour, ISoundController
    {
        //private List<AudioSource> audioSource;

        /*private static SoundControllerUnity instance;
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
                audioSource[i] = gameObject.AddComponent<AudioSource>();
            }
        }*/

        private void Awake()
        {
            //audioSource = new List<AudioSource>();
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
                AudioSource audio = gameObject.AddComponent<AudioSource>();
                audio.clip = clip;
                bms.WavFilesAC.Add(val, audio);
            }
        }

        public void PlayKeySound(string wavFile, BMS bms, int line)
        {
            try
            {
                //audioSource[line].PlayOneShot(bms.WavFilesAC[wavFile]);
                bms.WavFilesAC[wavFile].Play();
            }
            catch (System.Exception e)
            {
                Debug.Log("ERROR: " + wavFile);
            }
        }

        public bool CheckSoundPlaying(BMS bms = null)
        {
            bool isPlaying = false;
            /*for (int i = 0; i < Const.CHANNEL; i++)
            {
                isPlaying = audioSource[i].isPlaying;
                if (isPlaying) break;
            }*/
            foreach(AudioSource s in bms.WavFilesAC.Values)
            {
                if (s.isPlaying)
                {
                    isPlaying = true;
                    break;
                }
            }
            return isPlaying;
        }

        public void StopAll(BMS bms = null)
        {
            /*foreach (AudioSource source in audioSource)
            {
                if (source.isPlaying) source.Stop();
            }*/
            foreach (AudioSource s in bms.WavFilesAC.Values)
            {
                if (s.isPlaying) s.Stop();
            }
        }

        public void PauseAll(BMS bms = null)
        {
            /*foreach (AudioSource source in audioSource)
            {
                if (source.isPlaying) source.Pause();
            }*/

            foreach (AudioSource s in bms.WavFilesAC.Values)
            {
                if (s.isPlaying) s.Pause();
            }
        }

        public void ResumeAll(BMS bms = null)
        {
            /*foreach (AudioSource source in audioSource)
            {
                if (source.isPlaying) source.UnPause();
            }*/

            foreach (AudioSource s in bms.WavFilesAC.Values)
            {
                if (s.isPlaying) s.UnPause();
            }
        }

        public void FreeMemory(BMS bms)
        {
            /*foreach (AudioClip snd in bms.WavFilesAC.Values)
            {
                snd.UnloadAudioData();
            }*/

            foreach (AudioSource s in bms.WavFilesAC.Values)
            {
                s.clip.UnloadAudioData();
            }
            bms.WavFilesAC.Clear();
        }
    }
}