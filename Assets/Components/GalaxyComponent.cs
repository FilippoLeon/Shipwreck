using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalaxyComponent : ObserverBehaviour<Galaxy> {

    public GameObject vert;

    public Material starPathMaterial;

    Delaunay del;
   

    void Initialize() {
        del = new Delaunay();

        Vector3[] starts = new Vector3[] {
             new Vector3(-100, 100, 0),
             new Vector3(0, -200, 0),
             new Vector3(100, 100, 0),
        };

        foreach (Vector3 start in starts) {
            del.vertices.Add(new Vertex { coordinate = start });
        }

        vert.SetActive(false);
    }

    void AddSystem(Coordinate coord) {
        Vector3 pos = new Vector3(coord.x, coord.y, 0);

        GameObject v = Instantiate(vert);
        v.transform.SetParent(transform);
        v.transform.localPosition = pos;
        v.layer = (int)AppInfo.Layer.StarMap;
        v.SetActive(true);

        del.vertices.Add(new Vertex { coordinate = pos });

        del.vertices[0].coordinate.x = Mathf.Min(del.vertices[0].coordinate.x, -4 * Mathf.Abs(pos.x));
        del.vertices[0].coordinate.y = Mathf.Max(del.vertices[0].coordinate.y, 4 * Mathf.Abs(pos.y));

        del.vertices[2].coordinate.x = Mathf.Max(del.vertices[2].coordinate.x, 4 * Mathf.Abs(pos.x));
        del.vertices[2].coordinate.y = Mathf.Max(del.vertices[2].coordinate.y, 4 * Mathf.Abs(pos.y));

        del.vertices[1].coordinate.y = Mathf.Min(del.vertices[1].coordinate.y, -4 * Mathf.Abs(pos.y));
    }

    public void Generate() {
        del.Create();
        foreach (Triangle t in del.triangles) {
            if (t.GetReplacements() != null) {
                continue;
            }
            if ((t[0] == 0 || t[0] == 1 || t[0] == 2)
                || (t[(0 + 1) % 3] == 0 || t[(0 + 1) % 3] == 1 || t[(0 + 1) % 3] == 2)
                || (t[(0 + 2) % 3] == 0 || t[(0 + 2) % 3] == 1 || t[(0 + 2) % 3] == 2)) {
                continue;
            }
            for (int i = 0; i < 3; ++i) {
                GameObject edge = new GameObject("Edge");
                edge.transform.SetParent(transform);
                LineRenderer lr = edge.AddComponent<LineRenderer>();
                lr.SetPositions(new Vector3[] { del.vertices[t[i]].coordinate, del.vertices[t[(i + 1) % 3]].coordinate });
                lr.startWidth = 0.2f;
                lr.endWidth = 0.2f;
                lr.startColor = Color.red;
                lr.endColor = Color.red;
                lr.gameObject.layer = (int)AppInfo.Layer.StarMap;
                lr.material = starPathMaterial;
            }
        }
    }

    public override void HandleEvent(string signal, object[] args) {
        switch(signal) {
            case "OnInitialize":
                Initialize();
                break;
            case "OnGenerate":
                Generate();
                break;
            case "OnAddSystem":
                AddSystem((Coordinate) args[0]);
                break;
        }
    }

    public override void HandleEvent(string signal) {
        throw new System.NotImplementedException();
    }
}
