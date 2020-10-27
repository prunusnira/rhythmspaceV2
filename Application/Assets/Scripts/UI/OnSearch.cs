using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BMSPlayer
{
    public class OnSearch : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        public void OnSelect(BaseEventData eventData)
        {
            MusicListUI.isTop = false;
        }

        public void OnDeselect(BaseEventData eventData)
        {
            MusicListUI.isTop = true;
        }
    }
}
