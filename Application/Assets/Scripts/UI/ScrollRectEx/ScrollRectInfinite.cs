using BMSCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Note: Vertical Infinite
public class ScrollRectInfinite : ScrollRect {
    public GameObject prefab;
    public delegate GameObject ObjectSetup(MusicListData data);

    private Dictionary<int, MusicListData> itemList;
    private RectTransform objRect;

    private int idxTop = 0;
    private int idxBot = 0;
    private float height = 0;
    private int currentIdx;

    public void Init(List<MusicListData> list, ObjectSetup f)
    {
        objRect = GetComponent<RectTransform>();

        itemList = new Dictionary<int, MusicListData>();
        // 초기 아이템 정의
        foreach(MusicListData o in list)
        {
            itemList.Add(itemList.Count, o);
        }

        int size = 0;
        currentIdx = 0;
        // 리스트에 아이템 채우기
        while(height < objRect.rect.height)
        {
            AddItemInit(currentIdx, f);
            currentIdx++;

            if (currentIdx == itemList.Count) currentIdx = 0;
            size++;
        }

        idxTop = 0;

        int pos;
        if(size%2 == 0)
        {
            pos = size / 2;
        }
        else
        {
            pos = size / 2 + 1;
        }

        if(itemList.Count >= pos)
        {
            currentIdx = pos - 1;
        }
        else
        {
            currentIdx = itemList.Count % pos - 1;
        }
    }

    // 초기 아이템 추가
    public void AddItemInit(int idx, ObjectSetup f)
    {
        GameObject obj = f(itemList[idx]);
        obj.transform.SetParent(content.transform, false);
        obj.transform.SetAsLastSibling();
        height += obj.GetComponent<LayoutElement>().minHeight;
        idxBot = idx;
    }

    // 가장 아래에 있는 아이템을 지우고 맨 위에 index에 맞는 아이템 추가
    public void AddItemTop(ObjectSetup f)
    {
        // 추가될 아이템의 idx 가져오기
        if (idxTop == 0) idxTop = itemList.Count - 1;
        else idxTop--;

        GameObject obj = f(itemList[idxTop]);
        obj.transform.SetParent(content.transform, false);
        obj.transform.SetAsFirstSibling();
        height += obj.GetComponent<LayoutElement>().minHeight;

        Destroy(content.GetChild(content.childCount - 1).gameObject);

        if (idxBot == 0) idxBot = itemList.Count - 1;
        else idxBot--;

        if (currentIdx == 0) currentIdx = itemList.Count - 1;
        else currentIdx--;

        Debug.Log(idxTop + " " + currentIdx + " " + idxBot);
    }

    // 가장 위에 있는 아이템을 지우고 맨 아래에 index에 맞는 아이템 추가
    public void AddItemBottom(ObjectSetup f)
    {
        // 추가될 아이템의 idx 가져오기
        if (idxBot == itemList.Count - 1) idxBot = 0;
        else idxBot++;

        GameObject obj = f(itemList[idxBot]);
        obj.transform.SetParent(content.transform, false);
        obj.transform.SetAsLastSibling();
        height += obj.GetComponent<LayoutElement>().minHeight;

        Destroy(content.GetChild(0).gameObject);

        if (idxTop == itemList.Count - 1) idxTop = 0;
        else idxTop++;

        if (currentIdx == itemList.Count - 1) currentIdx = 0;
        else currentIdx++;

        Debug.Log(idxTop + " " + currentIdx + " " + idxBot);
    }

    public void Clear()
    {
        itemList.Clear();
        content.transform.DetachChildren();
    }

    // Getter/Setter
    public int GetCurrent()
    {
        return currentIdx;
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
