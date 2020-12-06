using BMSCore;

namespace BMSPlayer
{
    public interface ISoundController
    {
        void PreloadSound(BMS bms);

        void PlayKeySound(string wavFile, BMS bms, int line);

        bool CheckSoundPlaying(BMS bms = null);

        void StopAll(BMS bms = null);

        void PauseAll(BMS bms = null);

        void ResumeAll(BMS bms = null);

        void FreeMemory(BMS bms);
    }
}