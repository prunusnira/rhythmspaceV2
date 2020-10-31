using BMSCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Note: Vertical Infinite
namespace BMSPlayer
{
    public class ScrollRectInfinite : ScrollRect
    {
        public GameObject prefab;
        public delegate GameObject ObjectSetup(ListItemNode data, int i);

        private Dictionary<int, ListItemNode> itemList;
        private RectTransform objRect;

        private int currentIdx = 0;
        private int upperSize = 8;
        private int bottomSize = 10;

        private int[] itemidx = new int[18];

        public void Init(List<ListItemNode> list, int listpos, ObjectSetup f)
        {
            objRect = GetComponent<RectTransform>();
            itemList = new Dictionary<int, ListItemNode>();

            foreach (ListItemNode n in list)
            {
                itemList.Add(itemList.Count, n);
            }

            if(itemList.Count > 0)
            {
                if (itemList.Count / (upperSize + bottomSize) < 1)
                {
                    // Top 설정
                    int upperCount = upperSize / itemList.Count;
                    int upperLeft = upperSize % itemList.Count;

                    for (int i = 0; i < upperLeft; i++)
                    {
                        itemidx[i] = i + itemList.Count - upperLeft;
                    }
                    for (int i = 0; i < upperCount; i++)
                    {
                        for (int j = 0; j < itemList.Count; j++)
                        {
                            itemidx[upperLeft + itemList.Count * i + j] = j;
                        }
                    }

                    // Bottom 설정
                    int bottomCount = bottomSize / itemList.Count;
                    int bottomLeft = bottomSize % itemList.Count;

                    for (int i = 0; i < bottomCount; i++)
                    {
                        for (int j = 0; j < itemList.Count; j++)
                        {
                            itemidx[upperSize + itemList.Count * i + j] = j;
                        }
                    }
                    for (int i = 0; i < bottomLeft; i++)
                    {
                        itemidx[upperSize + i + itemList.Count * bottomCount] = i;
                    }
                }
                else
                {
                    for (int i = itemList.Count - 8; i < itemList.Count; i++)
                    {
                        itemidx[i - (itemList.Count - 8)] = i;
                    }
                    for (int i = 0; i < 10; i++)
                    {
                        itemidx[i + 8] = i;
                    }
                }

                // 리스트에 아이템 채우기
                AddItem(f);
            }
        }

        // 초기 아이템 추가
        public void AddItem(ObjectSetup f)
        {
            foreach(int i in itemidx)
            {
                GameObject obj = f(itemList[i], i);
                obj.transform.SetParent(content.transform, false);
            }
        }

        // 가장 아래에 있는 아이템을 지우고 맨 위에 index에 맞는 아이템 추가
        public void AddItemTop(ObjectSetup f)
        {
            for (int i = 0; i < itemidx.Length - 1; i++)
            {
                if(itemidx[i] == 0)
                {
                    itemidx[i] = itemList.Count - 1;
                }
                else
                {
                    itemidx[i] = itemidx[i] - 1;
                }
            }

            Clear();
            AddItem(f);

            if (currentIdx == 0) currentIdx = itemList.Count - 1;
            else currentIdx--;
            Const.ListPos = currentIdx;
        }

        // 가장 위에 있는 아이템을 지우고 맨 아래에 index에 맞는 아이템 추가
        public void AddItemBottom(ObjectSetup f)
        {

            for (int i = 0; i < itemidx.Length - 1; i++)
            {
                if (itemidx[i] == itemList.Count - 1)
                {
                    itemidx[i] = 0;
                }
                else
                {
                    itemidx[i] = itemidx[i] + 1;
                }
            }

            Clear();
            AddItem(f);

            if (currentIdx == itemList.Count - 1) currentIdx = 0;
            else currentIdx++;
            Const.ListPos = currentIdx;
        }

        public void Clear()
        {
            //content.transform.DetachChildren();
            for(int i = content.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(content.transform.GetChild(i).gameObject);
            }
        }

        // Getter/Setter
        public ListItemNode GetCurrent()
        {
            return itemList[currentIdx];
        }

        public int GetCurrentIdx()
        {
            return currentIdx;
        }

        public void ChangeCurrentIdx(int idx)
        {
            currentIdx = idx;
        }

        public void ResetIndex()
        {
            currentIdx = 0;
        }

        public int GetItemCount()
        {
            return itemList.Count;
        }

        // 마우스로 움직이지 않도록 변경
        #region Override
        public override void OnBeginDrag(PointerEventData eventData)
        {

        }

        public override void OnDrag(PointerEventData eventData)
        {

        }

        public override void OnEndDrag(PointerEventData eventData)
        {

        }
        #endregion
    }
}