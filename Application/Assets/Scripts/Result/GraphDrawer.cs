using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphDrawer : MonoBehaviour
{
    public RectTransform graph;
    public Sprite point;

    private List<GameObject> verticies;

    public void AddVertexes(List<int> data)
    {
        verticies = new List<GameObject>();
        float width = graph.sizeDelta.x;
        float height = graph.sizeDelta.y;
        int size = data.Count;

        for (int i = 0; i < size; i++)
        {
            float xpos = width * i / size;
            float ypos = height * data[i] / 10000;

            GameObject obj = new GameObject("point", typeof(SpriteRenderer));
            obj.transform.SetParent(graph, false);
            obj.GetComponent<SpriteRenderer>().sprite = point;
            obj.transform.localPosition = new Vector3(xpos, ypos, 0);
            verticies.Add(obj);
        }

        for(int i = 0; i < size - 2; i++)
        {
            ConnectVertexes(verticies[i], verticies[i + 1]);
        }
    }

    public void ConnectVertexes(GameObject o1, GameObject o2)
    {
        float o1x = o1.transform.localPosition.x;
        float o1y = o1.transform.localPosition.y;
        float o2x = o2.transform.localPosition.x;
        float o2y = o2.transform.localPosition.y;

        float dist = Vector3.Distance(o1.transform.localPosition, o2.transform.localPosition);

        GameObject obj = new GameObject("line", typeof(SpriteRenderer));
        obj.transform.SetParent(graph, false);
        obj.GetComponent<SpriteRenderer>().sprite = point;
        obj.transform.localPosition = new Vector3((o1x+o2x)/2, (o1y+o2y)/2, 0);
        obj.transform.localScale = new Vector3(dist, 1, 1);
        obj.transform.localRotation = Quaternion.FromToRotation(
            o2.transform.localPosition, o1.transform.localPosition);
        verticies.Add(obj);
    }
}
