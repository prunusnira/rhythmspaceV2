using BMSCore;

namespace BMSPlayer
{
    public interface ISoundController
    {
        void PreloadSound(BMS bms);

        void PlayNormalSound(string file);

        void PlayKeySound(string wavFile, BMS bms, int line);

        bool CheckSoundPlaying();

        void StopAll();

        void PauseAll();

        void ResumeAll();

        void FreeMemory(BMS bms);
    }
}