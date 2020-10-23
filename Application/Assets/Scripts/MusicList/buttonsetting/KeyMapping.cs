using UnityEngine;

namespace BMSPlayer
{
    public class KeyMapping
    {
        // Options
        public static bool keyAssigned = false;

        // Key Setting
        #region Key Set 1
        public static string Set1key1
        {
            get
            {
                return PlayerPrefs.GetString("btnSet1Key1", "");
            }
            set
            {
                PlayerPrefs.SetString("btnSet1Key1", value);
            }
        }

        public static string Set1key2
        {
            get
            {
                return PlayerPrefs.GetString("btnSet1Key2", "");
            }
            set
            {
                PlayerPrefs.SetString("btnSet1Key2", value);
            }
        }

        public static string Set1key3
        {
            get
            {
                return PlayerPrefs.GetString("btnSet1Key3", "");
            }
            set
            {
                PlayerPrefs.SetString("btnSet1Key3", value);
            }
        }

        public static string Set1key4
        {
            get
            {
                return PlayerPrefs.GetString("btnSet1Key4", "");
            }
            set
            {
                PlayerPrefs.SetString("btnSet1Key4", value);
            }
        }

        public static string Set1key5
        {
            get
            {
                return PlayerPrefs.GetString("btnSet1Key5", "");
            }
            set
            {
                PlayerPrefs.SetString("btnSet1Key5", value);
            }
        }

        public static string Set1key6
        {
            get
            {
                return PlayerPrefs.GetString("btnSet1Key6", "");
            }
            set
            {
                PlayerPrefs.SetString("btnSet1Key6", value);
            }
        }

        public static string Set1key7
        {
            get
            {
                return PlayerPrefs.GetString("btnSet1Key7", "");
            }
            set
            {
                PlayerPrefs.SetString("btnSet1Key7", value);
            }
        }

        public static string Set1key8
        {
            get
            {
                return PlayerPrefs.GetString("btnSet1Key8", "");
            }
            set
            {
                PlayerPrefs.SetString("btnSet1Key8", value);
            }
        }

        public static string Set1key9
        {
            get
            {
                return PlayerPrefs.GetString("btnSet1Key9", "");
            }
            set
            {
                PlayerPrefs.SetString("btnSet1Key9", value);
            }
        }
        #endregion

        #region Key Set 2
        public static string Set2key1
        {
            get
            {
                return PlayerPrefs.GetString("btnSet2Key1", "");
            }
            set
            {
                PlayerPrefs.SetString("btnSet2Key1", value);
            }
        }

        public static string Set2key2
        {
            get
            {
                return PlayerPrefs.GetString("btnSet2Key2", "");
            }
            set
            {
                PlayerPrefs.SetString("btnSet2Key2", value);
            }
        }

        public static string Set2key3
        {
            get
            {
                return PlayerPrefs.GetString("btnSet2Key3", "");
            }
            set
            {
                PlayerPrefs.SetString("btnSet2Key3", value);
            }
        }

        public static string Set2key4
        {
            get
            {
                return PlayerPrefs.GetString("btnSet2Key4", "");
            }
            set
            {
                PlayerPrefs.SetString("btnSet2Key4", value);
            }
        }

        public static string Set2key5
        {
            get
            {
                return PlayerPrefs.GetString("btnSet2Key5", "");
            }
            set
            {
                PlayerPrefs.SetString("btnSet2Key5", value);
            }
        }

        public static string Set2key6
        {
            get
            {
                return PlayerPrefs.GetString("btnSet2Key6", "");
            }
            set
            {
                PlayerPrefs.SetString("btnSet2Key6", value);
            }
        }

        public static string Set2key7
        {
            get
            {
                return PlayerPrefs.GetString("btnSet2Key7", "");
            }
            set
            {
                PlayerPrefs.SetString("btnSet2Key7", value);
            }
        }

        public static string Set2key8
        {
            get
            {
                return PlayerPrefs.GetString("btnSet2Key8", "");
            }
            set
            {
                PlayerPrefs.SetString("btnSet2Key8", value);
            }
        }

        public static string Set2key9
        {
            get
            {
                return PlayerPrefs.GetString("btnSet2Key9", "");
            }
            set
            {
                PlayerPrefs.SetString("btnSet2Key9", value);
            }
        }
        #endregion

        #region Axis Set 1
        public static bool Set1Axis1
        {
            get
            {
                return PlayerPrefs.GetInt("btnSet1Axis1", 0) == 1 ? true : false;
            }
            set
            {
                PlayerPrefs.SetInt("btnSet1Axis1", value ? 1 : 0);
            }
        }

        public static bool Set1Axis2
        {
            get
            {
                return PlayerPrefs.GetInt("btnSet1Axis2", 0) == 1 ? true : false;
            }
            set
            {
                PlayerPrefs.SetInt("btnSet1Axis2", value ? 1 : 0);
            }
        }

        public static bool Set1Axis3
        {
            get
            {
                return PlayerPrefs.GetInt("btnSet1Axis3", 0) == 1 ? true : false;
            }
            set
            {
                PlayerPrefs.SetInt("btnSet1Axis3", value ? 1 : 0);
            }
        }

        public static bool Set1Axis4
        {
            get
            {
                return PlayerPrefs.GetInt("btnSet1Axis4", 0) == 1 ? true : false;
            }
            set
            {
                PlayerPrefs.SetInt("btnSet1Axis4", value ? 1 : 0);
            }
        }

        public static bool Set1Axis5
        {
            get
            {
                return PlayerPrefs.GetInt("btnSet1Axis5", 0) == 1 ? true : false;
            }
            set
            {
                PlayerPrefs.SetInt("btnSet1Axis5", value ? 1 : 0);
            }
        }

        public static bool Set1Axis6
        {
            get
            {
                return PlayerPrefs.GetInt("btnSet1Axis6", 0) == 1 ? true : false;
            }
            set
            {
                PlayerPrefs.SetInt("btnSet1Axis6", value ? 1 : 0);
            }
        }

        public static bool Set1Axis7
        {
            get
            {
                return PlayerPrefs.GetInt("btnSet1Axis7", 0) == 1 ? true : false;
            }
            set
            {
                PlayerPrefs.SetInt("btnSet1Axis7", value ? 1 : 0);
            }
        }

        public static bool Set1Axis8
        {
            get
            {
                return PlayerPrefs.GetInt("btnSet1Axis8", 0) == 1 ? true : false;
            }
            set
            {
                PlayerPrefs.SetInt("btnSet1Axis8", value ? 1 : 0);
            }
        }

        public static bool Set1Axis9
        {
            get
            {
                return PlayerPrefs.GetInt("btnSet1Axis9", 0) == 1 ? true : false;
            }
            set
            {
                PlayerPrefs.SetInt("btnSet1Axis9", value ? 1 : 0);
            }
        }
        #endregion

        #region Axis Set 2
        public static bool Set2Axis1
        {
            get
            {
                return PlayerPrefs.GetInt("btnSet2Axis1", 0) == 1 ? true : false;
            }
            set
            {
                PlayerPrefs.SetInt("btnSet2Axis1", value ? 1 : 0);
            }
        }

        public static bool Set2Axis2
        {
            get
            {
                return PlayerPrefs.GetInt("btnSet2Axis2", 0) == 1 ? true : false;
            }
            set
            {
                PlayerPrefs.SetInt("btnSet2Axis2", value ? 1 : 0);
            }
        }

        public static bool Set2Axis3
        {
            get
            {
                return PlayerPrefs.GetInt("btnSet2Axis3", 0) == 1 ? true : false;
            }
            set
            {
                PlayerPrefs.SetInt("btnSet2Axis3", value ? 1 : 0);
            }
        }

        public static bool Set2Axis4
        {
            get
            {
                return PlayerPrefs.GetInt("btnSet2Axis4", 0) == 1 ? true : false;
            }
            set
            {
                PlayerPrefs.SetInt("btnSet2Axis4", value ? 1 : 0);
            }
        }

        public static bool Set2Axis5
        {
            get
            {
                return PlayerPrefs.GetInt("btnSet2Axis5", 0) == 1 ? true : false;
            }
            set
            {
                PlayerPrefs.SetInt("btnSet2Axis5", value ? 1 : 0);
            }
        }

        public static bool Set2Axis6
        {
            get
            {
                return PlayerPrefs.GetInt("btnSet2Axis6", 0) == 1 ? true : false;
            }
            set
            {
                PlayerPrefs.SetInt("btnSet2Axis6", value ? 1 : 0);
            }
        }

        public static bool Set2Axis7
        {
            get
            {
                return PlayerPrefs.GetInt("btnSet2Axis7", 0) == 1 ? true : false;
            }
            set
            {
                PlayerPrefs.SetInt("btnSet2Axis7", value ? 1 : 0);
            }
        }

        public static bool Set2Axis8
        {
            get
            {
                return PlayerPrefs.GetInt("btnSet2Axis8", 0) == 1 ? true : false;
            }
            set
            {
                PlayerPrefs.SetInt("btnSet2Axis8", value ? 1 : 0);
            }
        }

        public static bool Set2Axis9
        {
            get
            {
                return PlayerPrefs.GetInt("btnSet2Axis9", 0) == 1 ? true : false;
            }
            set
            {
                PlayerPrefs.SetInt("btnSet2Axis9", value ? 1 : 0);
            }
        }
        #endregion
    }
}
