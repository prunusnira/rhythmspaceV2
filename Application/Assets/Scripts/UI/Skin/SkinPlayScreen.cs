using BMSCore;
using UnityEngine;

namespace BMSPlayer
{
    class SkinPlayScreen: MonoBehaviour
    {
        // 설정할 것
        // 1. 각 플레이어의 기어, 프레임
        // 2. 노트 생성시마다 노트 스킨 바꿔줘야함
        public SpriteRenderer P1Std;
        public SpriteRenderer P1W125;
        public SpriteRenderer P1W150;

        public SpriteRenderer P2Std;
        public SpriteRenderer P2W125;
        public SpriteRenderer P2W150;

        public SpriteRenderer CoverSud1PStd;
        public SpriteRenderer CoverSud1PW125;
        public SpriteRenderer CoverSud1PW150;

        public SpriteRenderer CoverSud2PStd;
        public SpriteRenderer CoverSud2PW125;
        public SpriteRenderer CoverSud2PW150;

        public SpriteRenderer CoverHid1PStd;
        public SpriteRenderer CoverHid1PW125;
        public SpriteRenderer CoverHid1PW150;

        public SpriteRenderer CoverHid2PStd;
        public SpriteRenderer CoverHid2PW125;
        public SpriteRenderer CoverHid2PW150;

        public SpriteRenderer CoverLift1PStd;
        public SpriteRenderer CoverLift1PW125;
        public SpriteRenderer CoverLift1PW150;

        public SpriteRenderer CoverLift2PStd;
        public SpriteRenderer CoverLift2PW125;
        public SpriteRenderer CoverLift2PW150;

        public SpriteRenderer FrameUpper;
        public SpriteRenderer FrameLower;

        private void Awake()
        {
            P1Std.sprite = Tools.createSpriteFromFile(SkinSetting.GearSetMainPath + "/" + SkinLoader.GearData["P1Standard"], 1f, 0.5f, 0.5f);
            P1W125.sprite = Tools.createSpriteFromFile(SkinSetting.GearSetMainPath + "/" + SkinLoader.GearData["P1Wide125"], 1f, 0.5f, 0.5f);
            P1W150.sprite = Tools.createSpriteFromFile(SkinSetting.GearSetMainPath + "/" + SkinLoader.GearData["P1Wide150"], 1f, 0.5f, 0.5f);

            P2Std.sprite = Tools.createSpriteFromFile(SkinSetting.GearSetMainPath + "/" + SkinLoader.GearData["P2Standard"], 1f, 0.5f, 0.5f);
            P2W125.sprite = Tools.createSpriteFromFile(SkinSetting.GearSetMainPath + "/" + SkinLoader.GearData["P2Wide125"], 1f, 0.5f, 0.5f);
            P2W150.sprite = Tools.createSpriteFromFile(SkinSetting.GearSetMainPath + "/" + SkinLoader.GearData["P2Wide150"], 1f, 0.5f, 0.5f);

            CoverSud1PStd.sprite = Tools.createSpriteFromFile(SkinSetting.GearSetMainPath + "/" + SkinLoader.GearData["CoverSuddenStd"], 1f, 0.5f, 0.5f);
            CoverSud1PW125.sprite = Tools.createSpriteFromFile(SkinSetting.GearSetMainPath + "/" + SkinLoader.GearData["CoverSuddenW125"], 1f, 0.5f, 0.5f);
            CoverSud1PW150.sprite = Tools.createSpriteFromFile(SkinSetting.GearSetMainPath + "/" + SkinLoader.GearData["CoverSuddenW150"], 1f, 0.5f, 0.5f);
            CoverSud2PStd.sprite = Tools.createSpriteFromFile(SkinSetting.GearSetMainPath + "/" + SkinLoader.GearData["CoverSuddenStd"], 1f, 0.5f, 0.5f);
            CoverSud2PW125.sprite = Tools.createSpriteFromFile(SkinSetting.GearSetMainPath + "/" + SkinLoader.GearData["CoverSuddenW125"], 1f, 0.5f, 0.5f);
            CoverSud2PW150.sprite = Tools.createSpriteFromFile(SkinSetting.GearSetMainPath + "/" + SkinLoader.GearData["CoverSuddenW150"], 1f, 0.5f, 0.5f);

            CoverHid1PStd.sprite = Tools.createSpriteFromFile(SkinSetting.GearSetMainPath + "/" + SkinLoader.GearData["CoverHiddenStd"], 1f, 0.5f, 0.5f);
            CoverHid1PW125.sprite = Tools.createSpriteFromFile(SkinSetting.GearSetMainPath + "/" + SkinLoader.GearData["CoverHiddenW125"], 1f, 0.5f, 0.5f);
            CoverHid1PW150.sprite = Tools.createSpriteFromFile(SkinSetting.GearSetMainPath + "/" + SkinLoader.GearData["CoverHiddenW150"], 1f, 0.5f, 0.5f);
            CoverHid2PStd.sprite = Tools.createSpriteFromFile(SkinSetting.GearSetMainPath + "/" + SkinLoader.GearData["CoverHiddenStd"], 1f, 0.5f, 0.5f);
            CoverHid2PW125.sprite = Tools.createSpriteFromFile(SkinSetting.GearSetMainPath + "/" + SkinLoader.GearData["CoverHiddenW125"], 1f, 0.5f, 0.5f);
            CoverHid2PW150.sprite = Tools.createSpriteFromFile(SkinSetting.GearSetMainPath + "/" + SkinLoader.GearData["CoverHiddenW150"], 1f, 0.5f, 0.5f);

            CoverLift1PStd.sprite = Tools.createSpriteFromFile(SkinSetting.GearSetMainPath + "/" + SkinLoader.GearData["CoverLiftStd"], 1f, 0.5f, 0.5f);
            CoverLift1PW125.sprite = Tools.createSpriteFromFile(SkinSetting.GearSetMainPath + "/" + SkinLoader.GearData["CoverLiftW125"], 1f, 0.5f, 0.5f);
            CoverLift1PW150.sprite = Tools.createSpriteFromFile(SkinSetting.GearSetMainPath + "/" + SkinLoader.GearData["CoverLiftW150"], 1f, 0.5f, 0.5f);
            CoverLift2PStd.sprite = Tools.createSpriteFromFile(SkinSetting.GearSetMainPath + "/" + SkinLoader.GearData["CoverLiftStd"], 1f, 0.5f, 0.5f);
            CoverLift2PW125.sprite = Tools.createSpriteFromFile(SkinSetting.GearSetMainPath + "/" + SkinLoader.GearData["CoverLiftW125"], 1f, 0.5f, 0.5f);
            CoverLift2PW150.sprite = Tools.createSpriteFromFile(SkinSetting.GearSetMainPath + "/" + SkinLoader.GearData["CoverLiftW150"], 1f, 0.5f, 0.5f);

            FrameUpper.sprite = Tools.createSpriteFromFile(SkinSetting.GearSetMainPath + "/" + SkinLoader.GearData["FrameUpper"], 1f, 0.5f, 0.5f);
            FrameLower.sprite = Tools.createSpriteFromFile(SkinSetting.GearSetMainPath + "/" + SkinLoader.GearData["FrameLower"], 1f, 0.5f, 0.5f);

            SkinSetting.GaugeAssisted = Tools.createSpriteFromFile(SkinSetting.GearSetMainPath + "/" + SkinLoader.GearData["GaugeAssisted"], 0.3f, 0.5f, 0.5f);
            SkinSetting.GaugeEasy = Tools.createSpriteFromFile(SkinSetting.GearSetMainPath + "/" + SkinLoader.GearData["GaugeEasy"], 0.3f, 0.5f, 0.5f);
            SkinSetting.GaugeNormal = Tools.createSpriteFromFile(SkinSetting.GearSetMainPath + "/" + SkinLoader.GearData["GaugeNormal"], 0.3f, 0.5f, 0.5f);
            SkinSetting.GaugeHard = Tools.createSpriteFromFile(SkinSetting.GearSetMainPath + "/" + SkinLoader.GearData["GaugeHard"], 0.3f, 0.5f, 0.5f);
            SkinSetting.GaugeEXHard = Tools.createSpriteFromFile(SkinSetting.GearSetMainPath + "/" + SkinLoader.GearData["GaugeEXHard"], 0.3f, 0.5f, 0.5f);

            // 그래프
            SkinSetting.GraphBGBig = Tools.createSpriteFromFile(SkinSetting.GearSetMainPath + "/" + SkinLoader.GearData["GraphBGBig"], 1f, 0.5f, 0.5f);
            SkinSetting.GraphBGSmall = Tools.createSpriteFromFile(SkinSetting.GearSetMainPath + "/" + SkinLoader.GearData["GraphBGSmall"], 1f, 0.5f, 0.5f);
            SkinSetting.GraphBGMini = Tools.createSpriteFromFile(SkinSetting.GearSetMainPath + "/" + SkinLoader.GearData["GraphBGMini"], 1f, 0.5f, 0.5f);

            // 각 노트 불러오기
            SkinSetting.NoteScr = Tools.createSpriteFromFile(SkinSetting.NoteSetMainPath + "/" + SkinLoader.NoteData["NoteScr"], 1f, 0.5f, 0.5f);
            SkinSetting.NoteScrLn = Tools.createSpriteFromFile(SkinSetting.NoteSetMainPath + "/" + SkinLoader.NoteData["NoteScrLn"], 1f, 0.5f, 0.5f);
            SkinSetting.NoteScrAuto = Tools.createSpriteFromFile(SkinSetting.NoteSetMainPath + "/" + SkinLoader.NoteData["NoteScrAuto"], 1f, 0.5f, 0.5f);
            SkinSetting.NoteBtn1 = Tools.createSpriteFromFile(SkinSetting.NoteSetMainPath + "/" + SkinLoader.NoteData["NoteBtn1"], 1f, 0.5f, 0.5f);
            SkinSetting.NoteBtn1Ln = Tools.createSpriteFromFile(SkinSetting.NoteSetMainPath + "/" + SkinLoader.NoteData["NoteBtn1Ln"], 1f, 0.5f, 0.5f);
            SkinSetting.NoteBtn2 = Tools.createSpriteFromFile(SkinSetting.NoteSetMainPath + "/" + SkinLoader.NoteData["NoteBtn2"], 1f, 0.5f, 0.5f);
            SkinSetting.NoteBtn2Ln = Tools.createSpriteFromFile(SkinSetting.NoteSetMainPath + "/" + SkinLoader.NoteData["NoteBtn2Ln"], 1f, 0.5f, 0.5f);
            SkinSetting.NoteBtn3 = Tools.createSpriteFromFile(SkinSetting.NoteSetMainPath + "/" + SkinLoader.NoteData["NoteBtn3"], 1f, 0.5f, 0.5f);
            SkinSetting.NoteBtn3Ln = Tools.createSpriteFromFile(SkinSetting.NoteSetMainPath + "/" + SkinLoader.NoteData["NoteBtn3Ln"], 1f, 0.5f, 0.5f);
            SkinSetting.NoteBtn4 = Tools.createSpriteFromFile(SkinSetting.NoteSetMainPath + "/" + SkinLoader.NoteData["NoteBtn4"], 1f, 0.5f, 0.5f);
            SkinSetting.NoteBtn4Ln = Tools.createSpriteFromFile(SkinSetting.NoteSetMainPath + "/" + SkinLoader.NoteData["NoteBtn4Ln"], 1f, 0.5f, 0.5f);
            SkinSetting.NoteBtn5 = Tools.createSpriteFromFile(SkinSetting.NoteSetMainPath + "/" + SkinLoader.NoteData["NoteBtn5"], 1f, 0.5f, 0.5f);
            SkinSetting.NoteBtn5Ln = Tools.createSpriteFromFile(SkinSetting.NoteSetMainPath + "/" + SkinLoader.NoteData["NoteBtn5Ln"], 1f, 0.5f, 0.5f);
            SkinSetting.NoteBtn6 = Tools.createSpriteFromFile(SkinSetting.NoteSetMainPath + "/" + SkinLoader.NoteData["NoteBtn6"], 1f, 0.5f, 0.5f);
            SkinSetting.NoteBtn6Ln = Tools.createSpriteFromFile(SkinSetting.NoteSetMainPath + "/" + SkinLoader.NoteData["NoteBtn6Ln"], 1f, 0.5f, 0.5f);
            SkinSetting.NoteBtn7 = Tools.createSpriteFromFile(SkinSetting.NoteSetMainPath + "/" + SkinLoader.NoteData["NoteBtn7"], 1f, 0.5f, 0.5f);
            SkinSetting.NoteBtn7Ln = Tools.createSpriteFromFile(SkinSetting.NoteSetMainPath + "/" + SkinLoader.NoteData["NoteBtn7Ln"], 1f, 0.5f, 0.5f);
            SkinSetting.NoteMine = Tools.createSpriteFromFile(SkinSetting.NoteSetMainPath + "/" + SkinLoader.NoteData["NoteMine"], 1f, 0.5f, 0.5f);

            SkinSetting.Note2PScr = Tools.createSpriteFromFile(SkinSetting.NoteSetMainPath + "/" + SkinLoader.NoteData["Note2PScr"]);
            SkinSetting.Note2PScrLn = Tools.createSpriteFromFile(SkinSetting.NoteSetMainPath + "/" + SkinLoader.NoteData["Note2PScrLn"]);
            SkinSetting.Note2PScrAuto = Tools.createSpriteFromFile(SkinSetting.NoteSetMainPath + "/" + SkinLoader.NoteData["Note2PScrAuto"]);
            SkinSetting.Note2PBtn1 = Tools.createSpriteFromFile(SkinSetting.NoteSetMainPath + "/" + SkinLoader.NoteData["Note2PBtn1"]);
            SkinSetting.Note2PBtn1Ln = Tools.createSpriteFromFile(SkinSetting.NoteSetMainPath + "/" + SkinLoader.NoteData["Note2PBtn1Ln"]);
            SkinSetting.Note2PBtn2 = Tools.createSpriteFromFile(SkinSetting.NoteSetMainPath + "/" + SkinLoader.NoteData["Note2PBtn2"]);
            SkinSetting.Note2PBtn2Ln = Tools.createSpriteFromFile(SkinSetting.NoteSetMainPath + "/" + SkinLoader.NoteData["Note2PBtn2Ln"]);
            SkinSetting.Note2PBtn3 = Tools.createSpriteFromFile(SkinSetting.NoteSetMainPath + "/" + SkinLoader.NoteData["Note2PBtn3"]);
            SkinSetting.Note2PBtn3Ln = Tools.createSpriteFromFile(SkinSetting.NoteSetMainPath + "/" + SkinLoader.NoteData["Note2PBtn3Ln"]);
            SkinSetting.Note2PBtn4 = Tools.createSpriteFromFile(SkinSetting.NoteSetMainPath + "/" + SkinLoader.NoteData["Note2PBtn4"]);
            SkinSetting.Note2PBtn4Ln = Tools.createSpriteFromFile(SkinSetting.NoteSetMainPath + "/" + SkinLoader.NoteData["Note2PBtn4Ln"]);
            SkinSetting.Note2PBtn5 = Tools.createSpriteFromFile(SkinSetting.NoteSetMainPath + "/" + SkinLoader.NoteData["Note2PBtn5"]);
            SkinSetting.Note2PBtn5Ln = Tools.createSpriteFromFile(SkinSetting.NoteSetMainPath + "/" + SkinLoader.NoteData["Note2PBtn5Ln"]);
            SkinSetting.Note2PBtn6 = Tools.createSpriteFromFile(SkinSetting.NoteSetMainPath + "/" + SkinLoader.NoteData["Note2PBtn6"]);
            SkinSetting.Note2PBtn6Ln = Tools.createSpriteFromFile(SkinSetting.NoteSetMainPath + "/" + SkinLoader.NoteData["Note2PBtn6Ln"]);
            SkinSetting.Note2PBtn7 = Tools.createSpriteFromFile(SkinSetting.NoteSetMainPath + "/" + SkinLoader.NoteData["Note2PBtn7"]);
            SkinSetting.Note2PBtn7Ln = Tools.createSpriteFromFile(SkinSetting.NoteSetMainPath + "/" + SkinLoader.NoteData["Note2PBtn7Ln"]);
        }
    }
}
