/**
 * BMS 파일의 구조
 * 크게 Header Field와 Main Data Field로 나뉜다
 * 
 * 1. Header Field
 * #PLAYER		: 플레이어
 * #GENRE		: 장르
 * #TITLE		: 이름
 * #ARTIST		: 작곡가
 * #BPM		: BPM
 * #PLAYLEVEL	: 보면 레벨
 * #RANK		: 난이도
 * #TOTAL		: 노트 수
 * #STAGEFILE	: 타이틀 이미지
 * #WAVXX		: 노래 및 키음 데이터, XX는 0~9와 A~Z를 모두 사용함
 * #BMPXX		: BGA
 * #BACKBMP	: BGA 미지원시 백그라운드 이미지
 * 
 * 2. Main Data Field
 * #00000
 * 5자리의 숫자로 구성되며, 보면의 위치를 나타냄
 * 앞의 3자리는 마디(Bar), 뒤의 2자리는 마디 내에서의 위치(Channel)
 * 
 * 중요: Bar는 10진수, Channel은 16진수이지만 여기서는 10진수로 처리한다
 * DP를 포함하더라도 11~19, 21~29, 51~59, 61~69만 사용하기 때문
 * 
 * Bar는 한 박자, Channel == 노트 라인을 의미함
 * 노트 배치 기준에 따라 Channel의 각 숫자의 의미가 달라진다.
 * 16 = 스크래치
 * 11 = 1, 12 = 2, 13 = 3, 14 = 4, 15 = 5, 18 = 6, 19 = 7
 * 03 = BPM, 값은 Hex로 들어간다
 * 04 = BGA, 07 = LAYER, 06 = POOR
 * 01 = B01~B32 (백그라운드로 나오는 사운드, B1~B32까지의 모든 구간이 01 하나로 되어있음에 주의해야 함)
 * 02 = 마디 단축, 뒤에 소수점이 오며 이 값에 맞게 마디의 크기가 달라진다
 * 51~56, 58, 59 = 각 라인 별 롱노트
 * (11~16, 18, 19에서 10자리만 5로 바꾼 것, 롱노트는 2개씩 짝이 되어야 한다)
 * 09 = STOP
 * (스크롤을 잠시 멈추는 STOP은 별도로 헤더에 #STOPxx 가 발생한다, 아마 순서일듯?)
 * 
 * 노트 데이터
 * 노트 데이터는 그리드 방식으로 찍는다.
 * 예를 들어, 01 이면 맨 처음에 노트가 하나 찍히고 Bar 1개 동안 그 채널에는 노트가 없다
 * 0101 이면 Bar의 시작과 1/2지점에 노트가 각각 하나씩 찍힌다.
 * 010101 이면 Bar를 3등분하여 0, 1/3, 2/3 지점에 노트가 하나씩 찍힌다
 * 즉, 문자의 수 / 2 = 박자 개수이고, WAV Number가 있으면 키가 있는 것이다. 
 * (00은 WAV 번호 할당이 없다)
 */
using System.Collections.Generic;
using UnityEngine;

namespace BMSCore
{
    public class BMS
    {
        private string mFilePath = "";
        private string mFileName = "";
        private string mFolderPath = "";
        private int mPlayer = 0;
        private string mGerne = "";
        private string mTitle = "";
        private string mArtist = "";
        private double mBPMStart = 0f;
        private double mBPMMin = 0f;
        private double mBPMMax = 0f;
        private int mLevel = 0;
        private int mRank = 0;
        private int mTotalNotes = 0;
        private string mStageFile = "";
        private LNType mLNType = LNType.Type1; // default = LNType1
        private string mLNObj = ""; // Only used for LNOBJ

        // <Bar#, <Ch#, note>> : map structure - int = Ch#
        public Dictionary<string, string> mWavList = new Dictionary<string, string>();
        public Dictionary<string, AudioClip> mWavFilesAC = new Dictionary<string, AudioClip>();
        public Dictionary<string, FMOD.Sound> mWavFilesFM = new Dictionary<string, FMOD.Sound>();
        public Dictionary<int, List<string>> mMusic = new Dictionary<int, List<string>>();
        public Dictionary<int, string> mBPMNote = new Dictionary<int, string>();
        public Dictionary<int, string> mBPMNoteType2 = new Dictionary<int, string>();
        public Dictionary<string, double> mBPMNum = new Dictionary<string, double>();
        public Dictionary<int, Dictionary<int, string>> mNote = new Dictionary<int, Dictionary<int, string>>();
        public Dictionary<int, double> mBarLength = new Dictionary<int, double>();
        public Dictionary<string, Sprite> mBGAImages = new Dictionary<string, Sprite>();
        public Dictionary<int, string> mBGANote = new Dictionary<int, string>();
        public int lastBar = 0; // last number of bar
        public string bgaVideoFile = null;

        /// Methods

        // Constructor
        public BMS(string path)
        {
            setFilePath(path);
        }

        // Setter
        public void setFilePath(string path) { mFilePath = path; mFolderPath = setFolderPath(mFilePath); mFileName = setFileName(mFilePath); }
        public void setPlayer(int player) { mPlayer = player; }
        public void setGerne(string gerne) { mGerne = gerne; }
        public void setTitle(string title) { mTitle = title; }
        public void setArtist(string artist) { mArtist = artist; }
        public void setBPMStart(double bpm) { mBPMStart = bpm; }
        public void setBPMMin(double bpm) { mBPMMin = bpm; }
        public void setBPMMax(double bpm) { mBPMMax = bpm; }
        public void setLevel(int level) { mLevel = level; }
        public void setRank(int rank) { mRank = rank; }
        public void setTotalNotes(int notes) { mTotalNotes = notes; }
        public void setStageFile(string path) { mStageFile = path; }
        public void setLNType(LNType type) { mLNType = type; }
        public void setLNObj(string obj) { mLNObj = obj; }

        public string setFolderPath(string path)
        {
            int count = 0;
            mFolderPath = "";
            List<string> vc = new List<string>();

            char[] seperator = { '/', '\\' };
            string[] pathDiv = path.Split(seperator);

            for (int i = 0; i < pathDiv.Length; i++)
            {
                count++;
                vc.Add(pathDiv[i]);
            }

            for (int i = 0; i < count - 1; i++)
            {
                if (i != 0) mFolderPath += "/";
                mFolderPath += vc[i];
            }
            return mFolderPath + "/";
        }

        public string setFileName(string path)
        {
            char[] seperator = { '/', '\\' };
            string[] pathDiv = path.Split(seperator);
            return pathDiv[pathDiv.Length-1];
        }

        // Getter
        public string getFilePath() { return mFilePath; }
        public string getFileName() { return mFileName; }
        public string getFolderPath() { return mFolderPath; }
        public int getPlayer() { return mPlayer; }
        public string getGerne() { return mGerne; }
        public string getTitle() { return mTitle; }
        public string getArtist() { return mArtist; }
        public double getBPMStart() { return mBPMStart; }
        public double getBPMMin() { return mBPMMin; }
        public double getBPMMax() { return mBPMMax; }
        public int getLevel() { return mLevel; }
        public int getRank() { return mRank; }
        public int getTotalNotes() { return mTotalNotes; }
        public string getStageFile() { return mStageFile; }
        public LNType getLNType() { return mLNType; }
        public string getLNObj() { return mLNObj; }
    }
}
