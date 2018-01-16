using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalaxyComponent : ObserverBehaviour<Galaxy> {

    public GameObject vert;

    public Material starPathMaterial;

    public Camera Camera;

    public MainController mainController;

    void Initialize() {
        mainController = FindObjectOfType<MainController>();

        vert.SetActive(false);
    }

    void AddSystem(SolarSystem system, Coordinate coord) {
        Vector3 pos = new Vector3(coord.x, coord.y, 0);

        GameObject v = Instantiate(vert);
        v.transform.SetParent(transform);
        v.transform.localPosition = pos;
        v.layer = (int)AppInfo.Layer.StarMap;
        v.SetActive(true);

        v.AddComponent<SolarSystemComponent>();
        system.register(v.GetComponent<SolarSystemComponent>());
        

        GameObject g = new GameObject("text");
        g.transform.SetParent(v.transform);
        g.AddComponent<TextMesh>().text = system.Name;
        g.transform.localPosition = new Vector3(0.4f,0.4f,0);
        g.layer = (int) AppInfo.Layer.StarMap;
    }

    public void Generate() {
        foreach (SolarSystem system in Emitter.systems.Values) {
            foreach(SolarSystem other in system.links) { 
                GameObject edge = new GameObject("Edge");
                edge.transform.SetParent(transform);
                LineRenderer lr = edge.AddComponent<LineRenderer>();
                lr.SetPositions(
                    new Vector3[] {
                        new Vector3(system.coordinate.x, system.coordinate.y, 0.1f),
                        new Vector3(other.coordinate.x, other.coordinate.y, 0.1f),
                        }
                    );
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
                AddSystem((SolarSystem) args[0], (Coordinate) args[1]);
                break;
        }
    }

    public override void HandleEvent(string signal) {
        throw new System.NotImplementedException();
    }
}
