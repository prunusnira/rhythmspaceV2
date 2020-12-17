using BMSPlayer;
using UnityEngine;

namespace BMSPlayer
{
    public class HPController
    {
        private static HPController instance;
        public static HPController Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new HPController();
                }
                return instance;
            }
        }

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
        private int HPPerfect { get; set; }
        private int HPGreat { get; set; }
        private int HPGood { get; set; }
        private int HPBad { get; set; }
        private int HPMiss { get; set; }
        private int HPEPoor { get; set; }

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
            GaugeType type = Const.GaugeType;
            if (type == GaugeType.HARD && CurrentHP <= HPMax * 0.3)
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
            GaugeType type = Const.GaugeType;
            if (type == GaugeType.HARD && CurrentHP <= HPMax * 0.3)
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
            GaugeType type = Const.GaugeType;
            if (type == GaugeType.HARD && CurrentHP <= HPMax * 0.3)
            {
                CurrentHP += HPEPoor / 2;
            }
            else
            {
                CurrentHP += HPEPoor;
            }
            if (CurrentHP < HPMin) { CurrentHP = HPMin; }
        }

        public void SetHPJudgeType(JudgeType judge, int rank, GaugeType type, int totalval, int notecount)
        {
            int arcade = CalculateHPChange(notecount);
            switch (type)
            {
                case GaugeType.ASSISTED:
                case GaugeType.EASY:
                    CurrentHP = (int)(HPMax * 0.2);

                    if (judge == JudgeType.LR2 && totalval != 0)
                    {
                        double alpha = (double)totalval / notecount;
                        HPPerfect = (int)(alpha * 120);
                        HPGreat = (int)(alpha * 120);
                        HPGood = (int)(alpha * 60);
                        HPBad = -320;
                        HPMiss = -480;
                        HPEPoor = -160;
                    }
                    else if(judge == JudgeType.BEATORAJA && totalval != 0)
                    {
                        double alpha = (double)totalval / notecount;
                        HPPerfect = (int)(alpha * 100);
                        HPGreat = (int)(alpha * 100);
                        HPGood = (int)(alpha * 50);
                        HPBad = -150;
                        HPMiss = -450;
                        HPEPoor = -100;
                    }
                    else
                    {
                        HPPerfect = arcade;
                        HPGreat = arcade;
                        HPGood = arcade / 2;
                        HPBad = -160;
                        HPMiss = -480;
                        HPEPoor = -160;
                    }
                    break;
                case GaugeType.NORMAL:
                    CurrentHP = (int)(HPMax * 0.2);

                    if (judge == JudgeType.LR2 && totalval != 0)
                    {
                        double alpha = (double)totalval / notecount;
                        HPPerfect = (int)(alpha * 100);
                        HPGreat = (int)(alpha * 100);
                        HPGood = (int)(alpha * 50);
                        HPBad = -400;
                        HPMiss = -600;
                        HPEPoor = -200;
                    }
                    else if (judge == JudgeType.BEATORAJA && totalval != 0)
                    {
                        double alpha = (double)totalval / notecount;
                        HPPerfect = (int)(alpha * 100);
                        HPGreat = (int)(alpha * 100);
                        HPGood = (int)(alpha * 50);
                        HPBad = -300;
                        HPMiss = -600;
                        HPEPoor = -200;
                    }
                    else
                    {
                        HPPerfect = arcade;
                        HPGreat = arcade;
                        HPGood = arcade / 2;
                        HPBad = -160;
                        HPMiss = -600;
                        HPEPoor = -200;
                    }
                    break;
                case GaugeType.HARD:
                    CurrentHP = HPMax;

                    if(judge == JudgeType.LR2 && totalval != 0)
                    {
                        double alpha = 10;
                        if(notecount > 1000)
                        {
                            if (totalval >= 240) alpha = 1;
                            else if (totalval >= 230) alpha = 1.11;
                            else if (totalval >= 210) alpha = 1.25;
                            else if (totalval >= 200) alpha = 1.5;
                            else if (totalval >= 180) alpha = 1.666;
                            else if (totalval >= 160) alpha = 2;
                            else if (totalval >= 150) alpha = 2.5;
                            else if (totalval >= 130) alpha = 3.333;
                            else if (totalval >= 120) alpha = 5;
                            else totalval = 10;
                        }
                        else
                        {
                            // 노트수 1000 이하에서는 감소량 확인 못함
                            alpha = 0.833;
                        }

                        HPPerfect = 10;
                        HPGreat = 10;
                        HPGood = 5;
                        HPBad = (int)(alpha * -600);
                        HPMiss = (int)(alpha * -1000);
                        HPEPoor = (int)(alpha * -200);
                    }
                    else if(judge == JudgeType.BEATORAJA && totalval != 0)
                    {
                        double alpha = (double)(totalval - 320) * 2 / notecount;
                        if (alpha > 0.15) alpha = 0.15;
                        if (alpha < 0) alpha = 0;

                        HPPerfect = (int)(alpha / 0.15 * 20);
                        HPGreat = (int)(alpha / 0.15 * 20);
                        HPGood = (int)(alpha / 0.15 * 10);
                        HPBad = -400;
                        HPMiss = -800;
                        HPEPoor = -400;
                    }
                    else
                    {
                        HPPerfect = 16;
                        HPGreat = 16;
                        HPGood = 0;
                        HPBad = -500;
                        HPMiss = -960;
                        HPEPoor = -500;
                    }
                    break;
                case GaugeType.EXHARD:
                    CurrentHP = HPMax;

                    if (judge == JudgeType.LR2 && totalval != 0)
                    {
                        // LR2에는 EXH가 없으므로 적당히 보정한다
                        double alpha = 10;
                        if (notecount > 1000)
                        {
                            if (totalval >= 240) alpha = 1;
                            else if (totalval >= 230) alpha = 1.11;
                            else if (totalval >= 210) alpha = 1.25;
                            else if (totalval >= 200) alpha = 1.5;
                            else if (totalval >= 180) alpha = 1.666;
                            else if (totalval >= 160) alpha = 2;
                            else if (totalval >= 150) alpha = 2.5;
                            else if (totalval >= 130) alpha = 3.333;
                            else if (totalval >= 120) alpha = 5;
                            else totalval = 10;
                        }
                        else
                        {
                            // 노트수 1000 이하에서는 감소량 확인 못함
                            alpha = 0.833;
                        }

                        HPPerfect = 10;
                        HPGreat = 10;
                        HPGood = 0;
                        HPBad = (int)(alpha * -1200);
                        HPMiss = (int)(alpha * -2000);
                        HPEPoor = (int)(alpha * -400);
                    }
                    else if (judge == JudgeType.BEATORAJA && totalval != 0)
                    {
                        double alpha = (double)(totalval - 320) * 2 / notecount;
                        if (alpha > 0.15) alpha = 0.15;
                        if (alpha < 0) alpha = 0;

                        HPPerfect = (int)(alpha / 0.15 * 20);
                        HPGreat = (int)(alpha / 0.06 * 20);
                        HPGood = 0;
                        HPBad = -800;
                        HPMiss = -1600;
                        HPEPoor = -800;
                    }
                    else
                    {
                        HPPerfect = 16;
                        HPGreat = 16;
                        HPGood = 0;
                        HPBad = -1000;
                        HPMiss = -1920;
                        HPEPoor = -1000;
                    }
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