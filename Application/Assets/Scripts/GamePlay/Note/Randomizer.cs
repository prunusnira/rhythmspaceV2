using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BMSPlayer
{
    // Fisher-Yates Shuffle 참고
    // https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
    // 노멀/랜덤/미러/CRAN/커스텀에만 적용
    public class Randomizer
    {
        private System.Random rand;

        public Randomizer()
        {
            rand = new System.Random();
        }

        public int[] GetNoteLayout()
        {
            int[] layout = { 1, 2, 3, 4, 5, 6, 7 };

            if(Const.ChangeLayout)
            {
                switch (Const.NoteLayout)
                {
                    case NoteLayout.RANDOM:
                        layout = CreateRandomSP(layout);
                        break;
                    case NoteLayout.MIRROR:
                        layout = CreateMirror(layout);
                        break;
                    case NoteLayout.CRAN:
                        layout = CreateCRan(layout);
                        break;
                    case NoteLayout.CUSTOM:
                        layout = Const.CustomRandom;
                        break;
                    case NoteLayout.SRAN:
                        layout = null;
                        break;
                }
                Const.CurrentLayout = layout;
            }
            else
            {
                layout = Const.CurrentLayout;
            }
            return layout;
        }

        private int[] CreateRandomSP(int[] layout)
        {
            int[] random = layout;
            FisherYates(random);
            return random;
        }

        private int[] CreateMirror(int[] layout)
        {
            int[] mirror = new int[layout.Length];

            for(int i = 0; i < layout.Length; i++)
            {
                mirror[i] = layout[layout.Length - i - 1];
            }
            return mirror;
        }

        private int[] CreateCRan(int[] layout)
        {
            int[] blue = { 2, 4, 6 };
            int[] white = { 1, 3, 5, 7 };

            FisherYates(blue);
            FisherYates(white);

            int[] result =
            {
                white[0], blue[0], white[1], blue[1],
                white[2], blue[2], white[3]
            };

            return result;
        }

        private void FisherYates(int[] data)
        {
            // 0~n-2 루프
            for (int i = 0; i < data.Length - 2; i++)
            {
                // i~n-2에서 랜덤 값을 선택
                // [i, n-1)
                int linepick = rand.Next(i, data.Length - 1);

                // i와 선택된 라인을 스왑
                swap(ref data[i], ref data[linepick]);
            }
        }

        private void swap(ref int a, ref int b)
        {
            int temp = a;
            a = b;
            b = temp;
        }
    }
}
