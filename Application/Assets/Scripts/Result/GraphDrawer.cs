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

        for (int i = 0; i <= size; i++)
        {
            float xpos = width * i / size;
            float ypos = 0;
            if(i < size)
            {
                ypos = height * data[i] / 10000;
            }
            else
            {
                ypos = height * data[i-1] / 10000;
            }

            GameObject obj = new GameObject("point", typeof(SpriteRenderer));
            obj.transform.SetParent(graph, false);
            obj.GetComponent<SpriteRenderer>().sprite = point;
            obj.transform.localPosition = new Vector3(xpos, ypos, -2);
            verticies.Add(obj);
        }

        for (int i = 0; i < size; i++)
        {
            ConnectVertexes(verticies[i], verticies[i + 1]);
        }
    }

    public void ConnectVertexes(GameObject o1, GameObject o2)
    {
        Vector3 o1v = o1.transform.localPosition;
        Vector3 o2v = o2.transform.localPosition;
        float o1x = o1v.x;
        float o1y = o1v.y;
        float o2x = o2v.x;
        float o2y = o2v.y;

        float dx = o2x - o1x;
        float dy = o2y - o1y;

        float angle = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;

        float dist = Vector3.Distance(o1v, o2v);

        GameObject obj = new GameObject("line", typeof(SpriteRenderer));
        obj.transform.SetParent(graph, false);
        obj.GetComponent<SpriteRenderer>().sprite = point;
        obj.transform.localPosition = (o1v + o2v) / 2;
        obj.transform.localScale = new Vector3(dist / 2, 1, 1);

        obj.transform.localRotation =
            Quaternion.Euler(
                new Vector3(0f, 0f, angle)
            );
        verticies.Add(obj);
    }
}
