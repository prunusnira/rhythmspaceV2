using BMSPlayer;
using UnityEngine;

namespace BMSPlayer
{
    public class HPController : MonoBehaviour
    {
        public int HPMin
        {
            get
            {
                return 0;
            }
        }

        public int HPMax
        {
            get
            {
                return 10000;
            }
        }

        public int CurrentHP { get; set; }
        public int HPPerfect { get; set; }
        public int HPGreat { get; set; }
        public int HPGood { get; set; }
        public int HPBad { get; set; }
        public int HPMiss { get; set; }
        public int HPEPoor { get; set; }

        public void hpChangePerfect()
        {
            CurrentHP += HPPerfect;
            if (CurrentHP > HPMax) { CurrentHP = HPMax; }
        }

        public void hpChangeGreat()
        {
            CurrentHP += HPGreat;
            if (CurrentHP > HPMax) { CurrentHP = HPMax; }
        }

        public void hpChangeGood()
        {
            CurrentHP += HPGood;
            if (CurrentHP > HPMax) { CurrentHP = HPMax; }
        }

        public void hpChangeBad()
        {
            // 하드는 30퍼 이하에서 감소량 반값
            JudgeType type = Const.JudgeType;
            if (type == JudgeType.HARD && CurrentHP <= HPMax * 0.3)
            {
                CurrentHP += HPBad / 2;
            }
            else
            {
                CurrentHP += HPBad;
            }
            if (CurrentHP < HPMin) { CurrentHP = HPMin; }
        }

        public void hpChangeMiss()
        {
            // 하드는 30퍼 이하에서 감소량 반값
            JudgeType type = Const.JudgeType;
            if (type == JudgeType.HARD && CurrentHP <= HPMax * 0.3)
            {
                CurrentHP += HPMiss / 2;
            }
            else
            {
                CurrentHP += HPMiss;
            }
            if (CurrentHP < HPMin) { CurrentHP = HPMin; }
        }

        public void hpChangeEPoor()
        {
            // 하드는 30퍼 이하에서 감소량 반값
            JudgeType type = Const.JudgeType;
            if (type == JudgeType.HARD && CurrentHP <= HPMax * 0.3)
            {
                CurrentHP += HPEPoor / 2;
            }
            else
            {
                CurrentHP += HPEPoor;
            }
            if (CurrentHP < HPMin) { CurrentHP = HPMin; }
        }

        public void SetHPJudgeType(JudgeType type, int notecount)
        {
            int changeEN = CalculateHPChange(notecount);
            switch (type)
            {
                case JudgeType.ASSISTED:
                case JudgeType.EASY:
                    CurrentHP = (int)(HPMax * 0.2);
                    HPPerfect = changeEN;
                    HPGreat = changeEN;
                    HPGood = changeEN / 2;
                    HPBad = -160;
                    HPMiss = -480;
                    HPEPoor = -160;
                    break;
                case JudgeType.NORMAL:
                    CurrentHP = (int)(HPMax * 0.2);
                    HPPerfect = changeEN;
                    HPGreat = changeEN;
                    HPGood = changeEN / 2;
                    HPBad = -160;
                    HPMiss = -600;
                    HPEPoor = -200;
                    break;
                case JudgeType.HARD:
                    CurrentHP = HPMax;
                    HPPerfect = 16;
                    HPGreat = 16;
                    HPGood = 0;
                    HPBad = -500;
                    HPMiss = -960;
                    HPEPoor = -500;
                    break;
                case JudgeType.EXHARD:
                    CurrentHP = HPMax;
                    HPPerfect = 16;
                    HPGreat = 16;
                    HPGood = 0;
                    HPBad = -1000;
                    HPMiss = -1920;
                    HPEPoor = -1000;
                    break;
                default:
                    CurrentHP = (int)(HPMax * 0.2);
                    HPPerfect = 1;
                    HPGreat = 1;
                    HPGood = 1;
                    HPBad = -160;
                    HPMiss = -500;
                    HPEPoor = -160;
                    break;
            }
        }

        // 이지/노멀 게이지는 노트 개수와 판정 타입에 따라
        // HP 회복량이 달라짐
        // Perfect, Great은 똑같고 Good은 반값
        private int CalculateHPChange(int notecount)
        {
            return (int)Mathf.Round(((0.01f * notecount + 6.5f) / 7.605f / notecount) * 10000);
        }

        public bool IsHpMin()
        {
            return CurrentHP <= HPMin ? true : false;
        }
    }
}