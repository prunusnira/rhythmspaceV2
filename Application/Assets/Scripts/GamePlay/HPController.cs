using BMSPlayer;
using UnityEngine;

namespace BMSPlayer
{
    public class HPController : MonoBehaviour
    {
        private int currentHP;
        private int hpMin = 0;
        private int hpMax = 10000;
        private int hpPerfect;
        private int hpGreat;
        private int hpGood;
        private int hpBad;
        private int hpMiss;

        public void hpChangePerfect()
        {
            currentHP += hpPerfect;
            if (currentHP > hpMax) { currentHP = hpMax; }
        }

        public void hpChangeGreat()
        {
            currentHP += hpGreat;
            if (currentHP > hpMax) { currentHP = hpMax; }
        }

        public void hpChangeGood()
        {
            currentHP += hpGood;
            if (currentHP > hpMax) { currentHP = hpMax; }
        }

        public void hpChangeBad()
        {
            // 하드는 30퍼 이하에서 감소량 반값
            JudgeType type = Const.GetJudgeType();
            if (type == JudgeType.HARD && currentHP <= hpMax * 0.3)
            {
                currentHP += hpBad / 2;
            }
            else
            {
                currentHP += hpBad;
            }
            if (currentHP < hpMin) { currentHP = hpMin; }
        }

        public void hpChangeMiss()
        {
            // 하드는 30퍼 이하에서 감소량 반값
            JudgeType type = Const.GetJudgeType();
            if (type == JudgeType.HARD && currentHP <= hpMax * 0.3)
            {
                currentHP += hpMiss / 2;
            }
            else
            {
                currentHP += hpMiss;
            }
            if (currentHP < hpMin) { currentHP = hpMin; }
        }

        public void SetHPJudgeType(JudgeType type, int notecount)
        {
            int changeEN = CalculateHPChange(notecount);
            switch (type)
            {
                case JudgeType.ASSISTED:
                case JudgeType.EASY:
                    currentHP = (int)(hpMax * 0.2);
                    hpPerfect = changeEN;
                    hpGreat = changeEN;
                    hpGood = changeEN / 2;
                    hpBad = -160;
                    hpMiss = -480;
                    break;
                case JudgeType.NORMAL:
                    currentHP = (int)(hpMax * 0.2);
                    hpPerfect = changeEN;
                    hpGreat = changeEN;
                    hpGood = changeEN / 2;
                    hpBad = -160;
                    hpMiss = -600;
                    break;
                case JudgeType.HARD:
                    currentHP = hpMax;
                    hpPerfect = 16;
                    hpGreat = 16;
                    hpGood = 0;
                    hpBad = -500;
                    hpMiss = -960;
                    break;
                case JudgeType.EXHARD:
                    currentHP = hpMax;
                    hpPerfect = 16;
                    hpGreat = 16;
                    hpGood = 0;
                    hpBad = -1000;
                    hpMiss = -1920;
                    break;
                default:
                    currentHP = (int)(hpMax * 0.2);
                    hpPerfect = 1;
                    hpGreat = 1;
                    hpGood = 1;
                    hpBad = -160;
                    hpMiss = -500;
                    break;
            }
        }

        // 이지/노멀 게이지는 노트 개수와 판정 타입에 따라
        // HP 회복량이 달라짐
        // Perfect, Great은 똑같고 Good은 반값
        private int CalculateHPChange(int notecount)
        {
            return (int)Mathf.Round(760.5f / (0.01f * notecount + 6.5f));
        }

        // Getter
        public int GetHPMax()
        {
            return hpMax;
        }

        public int GetHPMin()
        {
            return hpMin;
        }

        public int GetHP()
        {
            return currentHP;
        }

        public bool isHpMin()
        {
            return currentHP <= hpMin ? true : false;
        }
    }
}