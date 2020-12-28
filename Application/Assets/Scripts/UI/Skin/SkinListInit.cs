using UnityEngine;

namespace BMSPlayer
{
    class SkinListInit: MonoBehaviour
    {
        private void Awake()
        {
            SkinLoader.CollectSkinList();

            SkinLoader.LoadSkinPath();
        }

        private void Update()
        {
            if(!SkinSetting.IsSkinLoaded)
            {
                SkinLoader.LoadSkinPath();
                SkinMusicList.IsSkinChanged = true;
                SkinSetting.IsSkinLoaded = true;
            }
        }
    }
}
