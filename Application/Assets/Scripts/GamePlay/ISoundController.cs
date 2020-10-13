using BMSCore;

namespace BMSPlayer
{
    public interface ISoundController
    {
        void Initialize();

        void InitSoundChannels();

        // Execute after BMSAnalyzer.FullAnalyzer worked
        void PreloadSound(BMS bms);

        void PlayKeySound(string wavFile, BMS bms, int line);

        bool CheckSoundPlaying();

        void StopAll();

        void FreeMemory(BMS bms);
    }
}