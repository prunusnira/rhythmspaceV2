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
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string FolderPath { get; set; }
        public int Player { get; set; }
        public string Gerne { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Artist { get; set; }
        public string SubArtist { get; set; }
        public double BPMStart { get; set; }
        public double BPMMin { get; set; }
        public double BPMMax { get; set; }
        public int Level { get; set; }
        public int Rank { get; set; }
        public int TotalNotes { get; set; }
        public int Difficulty { get; set; }
        public string StageFile { get; set; }
        public LNType LNType { get; set; } // default = LNType1
        public string LNObj { get; set; } // Only used for LNOBJ
        public int LastBar { get; set; } // last number of bar
        public string BGAVideoFile { get; set; }

        // <Bar#, <Ch#, note>> : map structure - int = Ch#
        public Dictionary<string, string> WavList = new Dictionary<string, string>();
        public Dictionary<string, AudioClip> WavFilesAC = new Dictionary<string, AudioClip>();
        public Dictionary<string, FMOD.Sound> WavFilesFM = new Dictionary<string, FMOD.Sound>();
        public Dictionary<int, List<string>> Music = new Dictionary<int, List<string>>();
        public Dictionary<int, string> BPMNote = new Dictionary<int, string>();
        public Dictionary<int, string> BPMNoteType2 = new Dictionary<int, string>();
        public Dictionary<string, double> BPMNum = new Dictionary<string, double>();
        public Dictionary<int, Dictionary<string, string>> Note = new Dictionary<int, Dictionary<string, string>>();
        public Dictionary<int, double> BarLength = new Dictionary<int, double>();
        public Dictionary<string, Sprite> BGAImages = new Dictionary<string, Sprite>();
        public Dictionary<int, string> BGANote = new Dictionary<int, string>();
        public Dictionary<string, int> StopList = new Dictionary<string, int>();
        public Dictionary<int, string> StopNote = new Dictionary<int, string>();
        
        // Constructor
        public BMS(string path)
        {
            FilePath = path;
            FolderPath = SetFolderPath(FilePath);
            FileName = SetFileName(FilePath);
            Player = 1;
            Gerne = "";
            Title = "";
            SubTitle = "";
            Artist = "";
            SubArtist = "";
            BPMStart = 0;
            BPMMin = 0;
            BPMMax = 0;
            Level = 0;
            Rank = 0;
            TotalNotes = 0;
            Difficulty = 0;
            StageFile = "";
            LNType = LNType.Type1;
            LNObj = "";
            LastBar = 0;
            BGAVideoFile = "";
        }

        private string SetFolderPath(string path)
        {
            int count = 0;
            FolderPath = "";
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
                if (i != 0) FolderPath += "/";
                FolderPath += vc[i];
            }
            return FolderPath + "/";
        }

        private string SetFileName(string path)
        {
            char[] seperator = { '/', '\\' };
            string[] pathDiv = path.Split(seperator);
            return pathDiv[pathDiv.Length-1];
        }
    }
}
