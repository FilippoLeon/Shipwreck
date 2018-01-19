using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Galaxy : Entity<Galaxy>, IView {
    public Dictionary<Coordinate, SolarSystem> systems = new Dictionary<Coordinate, SolarSystem>();

    Verse Verse {set; get;}

    Delaunay del;

    public Galaxy(Verse verse) {
        Verse = verse;
    }

    public override Galaxy Clone() {
        throw new System.NotImplementedException();
    }

    public override void Update() {
        throw new System.NotImplementedException();
    }

    internal void Initialize() {
        del = new Delaunay();

        Vector3[] starts = new Vector3[] {
             new Vector3(-100, 100, 0),
             new Vector3(0, -200, 0),
             new Vector3(100, 100, 0),
        };

        foreach (Vector3 start in starts) {
            del.vertices.Add(new Vertex { coordinate = start });
        }

        Emit("OnInitialize", new object[] { });
    }

    internal void Create() {
        int size = 44;

        for(int i = 0; i < size; ++i) {
            AddSystem();
        }

        Generate();
    }

    void Generate() {

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

            // Add link
            CreateLink(systems[new Coordinate(del.vertices[t[0]].coordinate)], systems[new Coordinate(del.vertices[t[1]].coordinate)]);
            CreateLink(systems[new Coordinate(del.vertices[t[1]].coordinate)], systems[new Coordinate(del.vertices[t[2]].coordinate)]);
            CreateLink(systems[new Coordinate(del.vertices[t[2]].coordinate)], systems[new Coordinate(del.vertices[t[0]].coordinate)]);
        }

        Emit("OnGenerate", new object[] { });
    }

    private void CreateLink(SolarSystem solarSystem1, SolarSystem solarSystem2) {
        solarSystem1.links.Add(solarSystem2);
        solarSystem2.links.Add(solarSystem1);
    }
    
    internal void AddSystem() {
        string name = Verse.registry.namesRegistry.GetRandom("systems");
        Coordinate coordinate = new Coordinate((int) UnityEngine.Random.Range(-50f,50f), (int) UnityEngine.Random.Range(-50f,50f));
        if( systems.ContainsKey(coordinate) ) {
            return;
        }
        
        systems[coordinate] = new SolarSystem(name, this, coordinate);

        del.vertices.Add(new Vertex { coordinate = coordinate.ToVector() });

        minVec.x = Mathf.Min(minVec.x, coordinate.x);
        minVec.y = Mathf.Min(minVec.y, coordinate.y);
        maxVec.x = Mathf.Max(maxVec.x, coordinate.x);
        maxVec.y = Mathf.Max(maxVec.y, coordinate.y);

        // Use min and max
        del.vertices[0].coordinate.x = Mathf.Min(del.vertices[0].coordinate.x, -4 * Mathf.Abs(coordinate.x));
        del.vertices[0].coordinate.y = Mathf.Max(del.vertices[0].coordinate.y, 4 * Mathf.Abs(coordinate.y));

        del.vertices[2].coordinate.x = Mathf.Max(del.vertices[2].coordinate.x, 4 * Mathf.Abs(coordinate.x));
        del.vertices[2].coordinate.y = Mathf.Max(del.vertices[2].coordinate.y, 4 * Mathf.Abs(coordinate.y));

        del.vertices[1].coordinate.y = Mathf.Min(del.vertices[1].coordinate.y, -4 * Mathf.Abs(coordinate.y));

        Emit("OnAddSystem", new object[] { systems[coordinate], coordinate });
    }

    Coordinate minVec, maxVec;

    public Coordinate GetMin() {
        return minVec;
    }

    public Coordinate GetMax() {
        return maxVec;
    }
}
