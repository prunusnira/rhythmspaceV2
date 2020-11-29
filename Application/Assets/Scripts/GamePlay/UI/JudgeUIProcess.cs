using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BMSPlayer
{
    public class JudgeUIProcess : MonoBehaviour
    {
        private TimingType currentJudge;

        // 패슬, 레이트, 타겟
        public TextMesh[] txtInfo1CNr;
        public TextMesh[] txtInfo2CNr;
        public TextMesh[] txtInfo3CNr;
        public TextMesh[] txtInfo1SNr;
        public TextMesh[] txtInfo2SNr;
        public TextMesh[] txtInfo3SNr;
        public TextMesh[] txtInfo1CW125;
        public TextMesh[] txtInfo2CW125;
        public TextMesh[] txtInfo3CW125;
        public TextMesh[] txtInfo1SW125;
        public TextMesh[] txtInfo2SW125;
        public TextMesh[] txtInfo3SW125;
        public TextMesh[] txtInfo1CW150;
        public TextMesh[] txtInfo2CW150;
        public TextMesh[] txtInfo3CW150;
        public TextMesh[] txtInfo1SW150;
        public TextMesh[] txtInfo2SW150;
        public TextMesh[] txtInfo3SW150;
        private TextMesh txtInfoFS;
        private TextMesh txtInfoTarget;
        private TextMesh txtInfoRate;

        // 판정 콤보
        public TextMeshPro[] txtJudgeBMNr;
        public TextMeshPro[] txtJudgeBMW125;
        public TextMeshPro[] txtJudgeBMW150;
        private TextMeshPro txtJudgeBM;
        private int infoNumA = 0;

        // 풀콤 퍼펙
        public SpriteRenderer[] spFCNr;
        public SpriteRenderer[] spFCW125;
        public SpriteRenderer[] spFCW150;
        private SpriteRenderer spFCMark;
        public Sprite spPF;

        private double timeLastComboPopup;
        private double timeLastTimingPopup;

        private void Awake()
        {
            switch (Const.GearSize)
            {
                case SkinSize.STANDARD:
                    txtJudgeBM = txtJudgeBMNr[Const.PlayerSide];
                    spFCMark = spFCNr[Const.PlayerSide];
                    break;
                case SkinSize.WIDE125:
                    txtJudgeBM = txtJudgeBMW125[Const.PlayerSide];
                    spFCMark = spFCW125[Const.PlayerSide];
                    break;
                case SkinSize.WIDE150:
                    txtJudgeBM = txtJudgeBMW150[Const.PlayerSide];
                    spFCMark = spFCW150[Const.PlayerSide];
                    break;
            }
            SideInfoDisplayPosition();
        }

        void Update()
        {
            double timing = (double)DateTime.Now.Ticks / 1000000 - timeLastComboPopup;
            if (timing > 10)
            {
                txtJudgeBM.gameObject.SetActive(false);
                if (txtInfoFS != null)
                    txtInfoFS.gameObject.SetActive(false);
                if (txtInfoRate != null)
                    txtInfoRate.gameObject.SetActive(false);
                if (txtInfoTarget != null)
                    txtInfoTarget.gameObject.SetActive(false);
            }

            if (txtJudgeBM.gameObject.activeSelf && timing % 1.5 < 0.1)
            {
                // 판정별 색상 변경 처리
                StartCoroutine(ComboChangeBM(currentJudge));
            }
        }

        public void UpdateJudge(TimingType judgetype, int combo, string accuracy, int ms, int score)
        {
            string judgeStr = "";
            currentJudge = judgetype;
            // Judge update
            switch (judgetype)
            {
                case TimingType.PERFECT:
                case TimingType.GREAT:
                    judgeStr = "GREAT";
                    break;
                case TimingType.GOOD:
                    judgeStr = "GOOD";
                    break;
                case TimingType.BAD:
                    judgeStr = "BAD";
                    break;
                case TimingType.POOR:
                case TimingType.EPOOR:
                    judgeStr = "POOR";
                    break;
            }

            // Combo update
            if (judgetype == TimingType.BAD || judgetype == TimingType.POOR)
            {
                txtJudgeBM.text = judgeStr;
            }
            else
            {
                txtJudgeBM.text = judgeStr + " " + combo;
            }
            txtJudgeBM.gameObject.SetActive(true);

            if (txtInfoRate != null)
            {
                txtInfoRate.text = accuracy;
                txtInfoRate.gameObject.SetActive(true);
            }

            if (txtInfoFS != null)
            {
                if (ms > 0)
                {
                    txtInfoFS.color = new Color(135f / 255, 206f / 255, 235f / 255);
                    txtInfoFS.text = "FAST " + ms.ToString() + "ms";
                }
                else if (ms < 0)
                {
                    txtInfoFS.color = Color.red;
                    txtInfoFS.text = "SLOW " + Math.Abs(ms).ToString() + "ms";
                }
                else
                {
                    txtInfoFS.text = "";
                }
                txtInfoFS.gameObject.SetActive(true);
            }

            if (txtInfoTarget != null)
            {
                int targetDiff = score - Const.ResultTarget;

                if (targetDiff < 0)
                {
                    txtInfoTarget.color = Color.red;
                    txtInfoTarget.text = targetDiff.ToString();
                }
                else
                {
                    txtInfoTarget.color = Color.white;
                    txtInfoTarget.text = "+" + targetDiff.ToString();
                }
                txtInfoTarget.gameObject.SetActive(true);
            }

            timeLastComboPopup = (double)DateTime.Now.Ticks / 1000000;
            timeLastTimingPopup = (double)DateTime.Now.Ticks / 1000000;
        }

        public void ShowFCPFMark(int type)
        {
            switch(type)
            {
                case 0:
                    spFCMark.sprite = spPF;
                    break;
            }

            spFCMark.gameObject.SetActive(true);
        }

        IEnumerator ComboChangeBM(TimingType type)
        {
            if (type == TimingType.PERFECT)
            {
                txtJudgeBM.colorGradient = new VertexGradient(
                    new Color(56f / 255, 122f / 255, 208f / 255),
                    new Color(56f / 255, 122f / 255, 208f / 255),
                    new Color(183f / 255, 196f / 255, 255f / 255),
                    new Color(183f / 255, 196f / 255, 255f / 255)
                    );
                yield return new WaitForSeconds(0.05f);
                txtJudgeBM.colorGradient = new VertexGradient(
                    new Color(232f / 255, 86f / 255, 155f / 255),
                    new Color(232f / 255, 86f / 255, 155f / 255),
                    new Color(234f / 255, 164f / 255, 179f / 255),
                    new Color(234f / 255, 164f / 255, 179f / 255)
                    );
                yield return new WaitForSeconds(0.05f);
                txtJudgeBM.colorGradient = new VertexGradient(
                    new Color(175f / 255, 232f / 255, 197f / 255)
                    );
                yield return new WaitForSeconds(0.05f);
            }
            else
            {
                txtJudgeBM.colorGradient = new VertexGradient(
                    new Color(217f / 255, 150f / 255, 0f),
                    new Color(217f / 255, 150f / 255, 0f),
                    new Color(255f / 255, 225f / 255, 196f / 255),
                    new Color(255f / 255, 225f / 255, 196f / 255)
                    );
                yield return new WaitForSeconds(0.1f);
                txtJudgeBM.colorGradient = new VertexGradient(
                    new Color(0f, 0f, 0f, 0f)
                    );
                yield return new WaitForSeconds(0.05f);
            }
        }

        private void SideInfoDisplayPosition()
        {
            // 각 포지션 개수 확인
            switch (Const.FastSlow)
            {
                case DisplayPosType.OFF:
                    txtInfoFS = null;
                    break;
                case DisplayPosType.TYPEA:
                    infoNumA++;
                    break;
                case DisplayPosType.TYPEB:
                    switch(Const.GearSize)
                    {
                        case SkinSize.STANDARD:
                            txtInfoFS = txtInfo3SNr[Const.PlayerSide];
                            break;
                        case SkinSize.WIDE125:
                            txtInfoFS = txtInfo3SW125[Const.PlayerSide];
                            break;
                        case SkinSize.WIDE150:
                            txtInfoFS = txtInfo3SW150[Const.PlayerSide];
                            break;
                    }
                    break;
            }
            switch (Const.TargetDiff)
            {
                case DisplayPosType.OFF:
                    txtInfoTarget = null;
                    break;
                case DisplayPosType.TYPEA:
                    infoNumA++;
                    break;
                case DisplayPosType.TYPEB:
                    switch (Const.GearSize)
                    {
                        case SkinSize.STANDARD:
                            txtInfoTarget = txtInfo2SNr[Const.PlayerSide];
                            break;
                        case SkinSize.WIDE125:
                            txtInfoTarget = txtInfo2SW125[Const.PlayerSide];
                            break;
                        case SkinSize.WIDE150:
                            txtInfoTarget = txtInfo2SW150[Const.PlayerSide];
                            break;
                    }
                    break;
            }
            switch (Const.RateDiff)
            {
                case DisplayPosType.OFF:
                    txtInfoRate = null;
                    break;
                case DisplayPosType.TYPEA:
                    infoNumA++;
                    break;
                case DisplayPosType.TYPEB:
                    switch (Const.GearSize)
                    {
                        case SkinSize.STANDARD:
                            txtInfoRate = txtInfo1SNr[Const.PlayerSide];
                            break;
                        case SkinSize.WIDE125:
                            txtInfoRate = txtInfo1SW125[Const.PlayerSide];
                            break;
                        case SkinSize.WIDE150:
                            txtInfoRate = txtInfo1SW150[Const.PlayerSide];
                            break;
                    }
                    break;
            }

            // 개수 별 스타일 처리
            switch (infoNumA)
            {
                case 1:
                    if (Const.FastSlow == DisplayPosType.TYPEA)
                    {
                        switch (Const.GearSize)
                        {
                            case SkinSize.STANDARD:
                                txtInfoFS = txtInfo2CNr[Const.PlayerSide];
                                break;
                            case SkinSize.WIDE125:
                                txtInfoFS = txtInfo2CW125[Const.PlayerSide];
                                break;
                            case SkinSize.WIDE150:
                                txtInfoFS = txtInfo2CW150[Const.PlayerSide];
                                break;
                        }
                    }
                    if (Const.RateDiff == DisplayPosType.TYPEA)
                    {
                        switch (Const.GearSize)
                        {
                            case SkinSize.STANDARD:
                                txtInfoRate = txtInfo2CNr[Const.PlayerSide];
                                break;
                            case SkinSize.WIDE125:
                                txtInfoRate = txtInfo2CW125[Const.PlayerSide];
                                break;
                            case SkinSize.WIDE150:
                                txtInfoRate = txtInfo2CW150[Const.PlayerSide];
                                break;
                        }
                    }
                    if (Const.TargetDiff == DisplayPosType.TYPEA)
                    {
                        switch (Const.GearSize)
                        {
                            case SkinSize.STANDARD:
                                txtInfoTarget = txtInfo2CNr[Const.PlayerSide];
                                break;
                            case SkinSize.WIDE125:
                                txtInfoTarget = txtInfo2CW125[Const.PlayerSide];
                                break;
                            case SkinSize.WIDE150:
                                txtInfoTarget = txtInfo2CW150[Const.PlayerSide];
                                break;
                        }
                    }
                    break;
                case 2:
                    if (Const.FastSlow == DisplayPosType.TYPEA)
                    {
                        if (Const.RateDiff == DisplayPosType.TYPEA)
                        {
                            switch (Const.GearSize)
                            {
                                case SkinSize.STANDARD:
                                    txtInfoRate = txtInfo1CNr[Const.PlayerSide];
                                    txtInfoFS = txtInfo3CNr[Const.PlayerSide];
                                    break;
                                case SkinSize.WIDE125:
                                    txtInfoRate = txtInfo1CW125[Const.PlayerSide];
                                    txtInfoFS = txtInfo3CW125[Const.PlayerSide];
                                    break;
                                case SkinSize.WIDE150:
                                    txtInfoRate = txtInfo1CW150[Const.PlayerSide];
                                    txtInfoFS = txtInfo3CW150[Const.PlayerSide];
                                    break;
                            }
                        }
                        else if (Const.TargetDiff == DisplayPosType.TYPEA)
                        {
                            switch (Const.GearSize)
                            {
                                case SkinSize.STANDARD:
                                    txtInfoTarget = txtInfo1CNr[Const.PlayerSide];
                                    txtInfoFS = txtInfo3CNr[Const.PlayerSide];
                                    break;
                                case SkinSize.WIDE125:
                                    txtInfoTarget = txtInfo1CW125[Const.PlayerSide];
                                    txtInfoFS = txtInfo3CW125[Const.PlayerSide];
                                    break;
                                case SkinSize.WIDE150:
                                    txtInfoTarget = txtInfo1CW150[Const.PlayerSide];
                                    txtInfoFS = txtInfo3CW150[Const.PlayerSide];
                                    break;
                            }
                        }
                    }
                    if (Const.RateDiff == DisplayPosType.TYPEA)
                    {
                        if (Const.TargetDiff == DisplayPosType.TYPEA)
                        {
                            switch (Const.GearSize)
                            {
                                case SkinSize.STANDARD:
                                    txtInfoRate = txtInfo1CNr[Const.PlayerSide];
                                    txtInfoTarget = txtInfo3CNr[Const.PlayerSide];
                                    break;
                                case SkinSize.WIDE125:
                                    txtInfoRate = txtInfo1CW125[Const.PlayerSide];
                                    txtInfoTarget = txtInfo3CW125[Const.PlayerSide];
                                    break;
                                case SkinSize.WIDE150:
                                    txtInfoRate = txtInfo1CW150[Const.PlayerSide];
                                    txtInfoTarget = txtInfo3CW150[Const.PlayerSide];
                                    break;
                            }
                        }
                    }
                    break;
                case 3:
                    switch (Const.GearSize)
                    {
                        case SkinSize.STANDARD:
                            txtInfoTarget = txtInfo1CNr[Const.PlayerSide];
                            txtInfoRate = txtInfo2CNr[Const.PlayerSide];
                            txtInfoFS = txtInfo3CNr[Const.PlayerSide];
                            break;
                        case SkinSize.WIDE125:
                            txtInfoTarget = txtInfo1CW125[Const.PlayerSide];
                            txtInfoRate = txtInfo2CW125[Const.PlayerSide];
                            txtInfoFS = txtInfo3CW125[Const.PlayerSide];
                            break;
                        case SkinSize.WIDE150:
                            txtInfoTarget = txtInfo1CW150[Const.PlayerSide];
                            txtInfoRate = txtInfo2CW150[Const.PlayerSide];
                            txtInfoFS = txtInfo3CW150[Const.PlayerSide];
                            break;
                    }
                    break;
            }
        }
    }
}
