/**
 * BMS 파일의 구조
 * 크게 Header Field와 Main Data Field로 나뉜다
 * 
 * 1. Header Field
 * #PLAYER		: 플레이어
 * #GENRE		: 장르
 * #TITLE		: 이름
 * #SUBTITLE    : 부제목
 * #ARTIST		: 작곡가
 * #SUBARTIST   : BGA 등 다른 작업
 * #BPM 		: BPM
 * #PLAYLEVEL	: 보면 레벨
 * #RANK		: 판정 난이도
 *                (0 = Very Hard, 1 = Hard, 2 = Normal, 3 = Easy)
 * #DIFFUCULTY  : 패턴 난이도 (1~5 기반, beginner, normal, hyper 등)
 * #TOTAL		: 토탈 치
 * #STAGEFILE	: 타이틀 이미지
 * #WAVXX		: 노래 및 키음 데이터
 * #BMPXX		: BGA (04 Channel)
 * #BPMXX       : BPM Type2 (08 Channel)
 * #STOPXX      : STOP (09 Channel)
 * #BACKBMP     : BGA 미지원시 백그라운드 이미지
 * #LNTYPE      : (있다면) LNType 지정 (1 혹은 2)
 * #LNOBJ       : LNOBJ 형식일 때 사용하는 롱노트 끝 노트
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
 * 
 * Player 1
 * 16 = 스크래치
 * 11 = 1, 12 = 2, 13 = 3, 14 = 4, 15 = 5, 18 = 6, 19 = 7
 * 51~56, 58, 59 = 각 라인 별 롱노트 (LNType 1)
 * D1, D2, D3, D4, D5, D6, D8, D9 = 라인별 지뢰노트
 * 
 * Player 2
 * 26 = 스크래치
 * 21 = 1, 22 = 2, 23 = 3, 24 = 4, 25 = 5, 28 = 6, 29 = 7
 * 61~66, 68, 69 = 각 라인 별 롱노트 (LNType 1)
 * E1, E2, E3, E4, E5, E6, E8, E9 = 라인별 지뢰노트
 * 
 * 03 = BPM, 값은 Hex로 들어간다
 * 04 = BGA, 07 = LAYER, 06 = POOR
 * 01 = B01~B32 (백그라운드로 나오는 사운드, B1~B32까지의 모든 구간이 01 하나로 되어있음에 주의해야 함)
 * 02 = 마디 길이, 뒤에 소수점이 오며 이 값에 맞게 마디의 크기가 달라진다
 *      (ex - 0.75 = 3/4박, 1.25 = 5/4박)
 * (11~16, 18, 19에서 10자리만 5로 바꾼 것, 롱노트는 2개씩 짝이 되어야 한다)
 * 09 = STOP
 * (스크롤을 잠시 멈추는 STOP은 별도로 헤더에 #STOPxx 가 발생한다)
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
        public int Difficulty { get; set; }
        public string StageFile { get; set; }
        public LNType LNType { get; set; } // default = LNType1
        public string LNObj { get; set; } // Only used for LNOBJ
        public int LastBar { get; set; } // last number of bar
        public string BGAVideoFile { get; set; }
        public int TotalValue { get; set; }

        // <WAV##, WavFileName>
        public Dictionary<string, string> WavList = new Dictionary<string, string>();

        // <WAV##, FMOD Sound>
        public Dictionary<string, FMOD.Sound> WavFilesFM = new Dictionary<string, FMOD.Sound>();

        // <BAR#, Channel 03>
        public Dictionary<int, string> BPMNote = new Dictionary<int, string>();

        // <BAR#, Channel 08>
        public Dictionary<int, string> BPMNoteType2 = new Dictionary<int, string>();

        // <BPM##, double로 표현된 bpm> - Channel 08에서 사용하는 BPM 데이터
        public Dictionary<string, double> BPMNum = new Dictionary<string, double>();

        // <BAR#, Channel 01> - Channel 01은 한 bar에 여러개가 들어갈 수 있음
        public Dictionary<int, List<string>> Music = new Dictionary<int, List<string>>();

        // <BAR#, <Channel #, Note String>> - 각 채널별 노트 등록 (플레이노트)
        public Dictionary<int, Dictionary<string, string>> PlayNote = new Dictionary<int, Dictionary<string, string>>();

        // <BAR#, <Channel #, Note String>> - 각 채널별 노트 등록 (지뢰노트)
        public Dictionary<int, Dictionary<string, string>> MineNote = new Dictionary<int, Dictionary<string, string>>();

        // <BAR#, Channel 02> - 4/4박자를 1로 하여 double로 bar의 길이를 결정
        public Dictionary<int, double> BarLength = new Dictionary<int, double>();

        // <BMP##, FileName>
        public Dictionary<string, string> BGAPaths = new Dictionary<string, string>();
        public Dictionary<string, Sprite> BGAImages = new Dictionary<string, Sprite>();

        // <BAR#, Channel 04>
        public Dictionary<int, string> BGANote = new Dictionary<int, string>();

        // <BAR#, Channel 07>
        public Dictionary<int, string> LayerNote = new Dictionary<int, string>();

        // <STOP##, Time> - 192 노트를 1박 기준으로 보고 멈추는 시간을 설정
        public Dictionary<string, int> StopList = new Dictionary<string, int>();

        // <BAR#, Channel 09>
        public Dictionary<int, string> StopNote = new Dictionary<int, string>();

        // 호환성을 위해 임시로 남겨둠
        public Dictionary<string, AudioClip> WavFilesAC = new Dictionary<string, AudioClip>();
        
        // 랜덤 처리를 위해 데이터 보존
        public List<string> Random = new List<string>();

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
            Rank = 2;
            Difficulty = 0;
            StageFile = "";
            LNType = LNType.Type1;
            LNObj = "";
            LastBar = 0;
            BGAVideoFile = "";
            TotalValue = 0;
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
