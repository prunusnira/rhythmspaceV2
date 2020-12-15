using TMPro;
using UnityEngine;

namespace BMSPlayer
{
    class StageCleared: MonoBehaviour
    {
        public TextMeshPro[] resultNr;
        public TextMeshPro[] resultW125;
        public TextMeshPro[] resultW150;
        private TextMeshPro textResult;

        private void Awake()
        {
            switch(Const.GearSize)
            {
                case SkinSize.STANDARD:
                    textResult = resultNr[Const.PlayerSide];
                    break;
                case SkinSize.WIDE125:
                    textResult = resultW125[Const.PlayerSide];
                    break;
                case SkinSize.WIDE150:
                    textResult = resultW150[Const.PlayerSide];
                    break;
            }
        }

        public void ShowClearScoreStat(int ex, int total)
        {
            string resultStr = "STAGE FINISHED\n";
            if(ex >= 2 * total * 8.5f / 9)
            {
                // MAX-
                int val = total * 2 - ex;
                resultStr += "MAX-" + val;
            }
            else if(ex >= 2 * total * 8f / 9)
            {
                // AAA+
                int val = ex - total * 2 * 8 / 9;
                resultStr += "AAA+" + val;
            }
            else if (ex >= 2 * total * 7.5f / 9)
            {
                // AAA-
                int val = total * 2 * 8 / 9 - ex;
                resultStr += "AAA-" + val;
            }
            else if (ex >= 2 * total * 7f / 9)
            {
                // AA+
                int val = ex - total * 2 * 7 / 9;
                resultStr += "AA+" + val;
            }
            else if (ex >= 2 * total * 6.5f / 9)
            {
                // AA-
                int val = total * 2 * 7 / 9 - ex;
                resultStr += "AA-" + val;
            }
            else if (ex >= 2 * total * 6f / 9)
            {
                // A+
                int val = ex - total * 2 * 6 / 9;
                resultStr += "A+" + val;
            }
            else if (ex >= 2 * total * 5.5f / 9)
            {
                // A-
                int val = total * 2 * 6 / 9 - ex;
                resultStr += "A-" + val;
            }
            else if (ex >= 2 * total * 5f / 9)
            {
                // B+
                int val = ex - total * 2 * 5 / 9;
                resultStr += "B+" + val;
            }
            else if (ex >= 2 * total * 4.5f / 9)
            {
                // B-
                int val = total * 2 * 5 / 9 - ex;
                resultStr += "B-" + val;
            }
            else if (ex >= 2 * total * 4f / 9)
            {
                // C+
                int val = ex - total * 2 * 4 / 9;
                resultStr += "C+" + val;
            }
            else
            {
                // C-
                int val = total * 2 * 4 / 9 - ex;
                resultStr += "C-" + val;
            }
            textResult.text = resultStr;
            textResult.gameObject.SetActive(true);
        }
    }
}
