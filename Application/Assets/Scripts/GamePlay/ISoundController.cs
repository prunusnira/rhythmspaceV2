using BMSCore;

namespace BMSPlayer
{
    public interface ISoundController
    {
        void Initialize();

        void InitSoundChannels();

        // Execute after BMSAnalyzer.FullAnalyzer worked
        void PreloadSound(BMS bms);

        void PlayKeySound(int lane, string wavFile, ref BMS bms);

        bool CheckSoundPlaying();

        void StopAll();
    }
}