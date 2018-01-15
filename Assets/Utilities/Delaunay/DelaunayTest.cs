using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelaunayTest : MonoBehaviour {

    public GameObject vert;

    // Use this for initialization
    void Start() {
        Delaunay del = new Delaunay();

        Vector3[] starts = new Vector3[] {
             new Vector3(-10, 5, 0),
             new Vector3(0, -15, 0),
             new Vector3(10, 5, 0),
        };

        foreach (Vector3 start in starts) {
            //GameObject v = Instantiate(vert);
            //v.transform.position = start/*;*/

            del.vertices.Add(new Vertex { coordinate = start });
        }
        //del.vertices.Add(null);
        //del.vertices.Add(null);
        //del.vertices.Add(null);

        int Npoints = 25;
        for(int i = 0; i < Npoints; ++i) {
            float x = Random.Range(-5.0f, 5.0f);
            float y = Random.Range(-5.0f, 5.0f);
            //if(y > 5 - x) {
            //    continue;
            //}


            Vector3 pos = new Vector3(x, y, 0);

            GameObject v = Instantiate(vert);
            v.transform.position = pos;

            del.vertices.Add(new Vertex { coordinate = pos });
        }
        vert.SetActive(false);

        del.Create();
        foreach(Triangle t in del.triangles) {
            if(t.GetReplacements() != null) {
                continue;
            }
            if ((t[0] == 0 || t[0] == 1 || t[0] == 2)
                || (t[(0 + 1) % 3] == 0 || t[(0 + 1) % 3] == 1 || t[(0 + 1) % 3] == 2) 
                || (t[(0 + 2) % 3] == 0 || t[(0 + 2) % 3] == 1 || t[(0 + 2) % 3] == 2)) {
                continue;
            }
            for(int i = 0; i < 3; ++i) {
                    Debug.DrawLine(
                    del.vertices[t[i]].coordinate,
                    del.vertices[t[(i + 1) % 3]].coordinate,
                    Color.yellow, 100
                    );
                GameObject edge = new GameObject("edge");
                LineRenderer lr = edge.AddComponent<LineRenderer>();
                lr.SetPositions(new Vector3[] { del.vertices[t[i]].coordinate, del.vertices[t[(i + 1) % 3]].coordinate });
                lr.startWidth = 0.05f;
                lr.endWidth = 0.05f;
                lr.startColor = Color.red;
                lr.endColor = Color.red;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
