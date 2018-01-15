using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalaxyComponent : ObserverBehaviour<Galaxy> {

    public GameObject vert;

    public Material starPathMaterial;
    
    void Start() {
        Delaunay del = new Delaunay();

        Vector3[] starts = new Vector3[] {
             new Vector3(-10, 5, 0),
             new Vector3(0, -15, 0),
             new Vector3(10, 5, 0),
        };

        foreach (Vector3 start in starts) {
            del.vertices.Add(new Vertex { coordinate = start });
        }

        int Npoints = 25;
        for (int i = 0; i < Npoints; ++i) {
            float x = Random.Range(-5.0f, 5.0f);
            float y = Random.Range(-5.0f, 5.0f);
            
            Vector3 pos = new Vector3(x, y, 0);

            GameObject v = Instantiate(vert);
            v.transform.SetParent(transform);
            v.transform.localPosition = pos;
            v.layer = (int) AppInfo.Layer.StarMap;

            del.vertices.Add(new Vertex { coordinate = pos });
        }
        vert.SetActive(false);

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
                lr.startWidth = 0.05f;
                lr.endWidth = 0.05f;
                lr.startColor = Color.red;
                lr.endColor = Color.red;
                lr.gameObject.layer = (int)AppInfo.Layer.StarMap;
                lr.material = starPathMaterial;
            }
        }
    }

    public override void HandleEvent(string signal, object[] args) {
        switch(signal) {
            case "OnCreate":
                break;
        }
    }

    public override void HandleEvent(string signal) {
        throw new System.NotImplementedException();
    }
}
