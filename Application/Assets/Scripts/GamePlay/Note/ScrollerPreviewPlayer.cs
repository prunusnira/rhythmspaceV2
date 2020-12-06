using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using BMSCore;

namespace BMSPlayer
{
    // Scroller를 간소화하여 사운드 재생에만 사용되는 클래스
    public class ScrollerPreviewPlayer
    {
        // user play info
        private int noteCount = 0;
        private int processedNotes = 0;
        private double addStopTiming = 0;

        // 노트 스크롤
        private double noteTiming;

        // 소리 재생
        private ISoundController soundController = SoundControllerFMOD.Instance;

        public void PlaySetup(int noteCount, int rank)
        {
            this.noteCount = noteCount;
        }

        public void PlayBGM(List<BGMNote> noteBGM, BMS bms, double time)
        {
            List<BGMNote> removeCandidate = new List<BGMNote>();

            for (int i = 0; i < noteBGM.Count; i++)
            {
                BGMNote current = noteBGM[i];

                if (current.Timing <= time && !current.Used)
                {
                    soundController.PlayKeySound(current.Wav, bms, 0);
                    current.Used = true;
                    removeCandidate.Add(current);
                }
            }

            foreach (BGMNote n in removeCandidate)
            {
                noteBGM.Remove(n);
            }
        }

        public void PlayPreviewSound(List<PlayNote>[] notePlay, List<LongNote> lnlist,
            BMS bms, double timePassed)
        {
            // 각 라인에 대해 검사
            for (int i = 0; i < Const.LINE; i++)
            {
                List<PlayNote> removeCandidate = new List<PlayNote>();

                // 1. 현재 라인을 가져옴
                List<PlayNote> currentLine = notePlay[i];

                // 2. 현재 라인의 첫 노트를 가져옴
                if (currentLine.Count == 0) continue;

                PlayNote cnote = currentLine[0];

                // 3. 해당 노트의 위치가 판정선 아래인지 확인
                if (cnote.Timing <= timePassed && !cnote.Used)
                {
                    if (cnote.PlayNoteType == NoteType.SINGLE)
                    {
                        soundController.PlayKeySound(cnote.Wav, bms, cnote.Line);
                        removeCandidate.Add(cnote);

                        processedNotes++;
                    }
                    // 롱노트 시작 처리
                    else if (cnote.PlayNoteType == NoteType.LNSTART)
                    {
                        soundController.PlayKeySound(cnote.Wav, bms, cnote.Line);

                        removeCandidate.Add(lnlist[cnote.LNNum].End);
                        removeCandidate.Add(lnlist[cnote.LNNum].Mid);
                        removeCandidate.Add(cnote);

                        processedNotes++;
                    }
                }

                foreach (PlayNote n in removeCandidate)
                {
                    if(notePlay[i].Contains(n)) notePlay[i].Remove(n);
                    n.Used = true;
                }
            }
        }

        public double GetJudgeTiming(double noteTime, double playTime)
        {
            return noteTime - playTime;
        }

        public int GetProcessedNotes()
        {
            return processedNotes;
        }
    }
}
