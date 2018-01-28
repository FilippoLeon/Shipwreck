using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystemComponent : ObserverBehaviour<SolarSystem> {

    public GameObject SolarSystemObject;

    public void DisplaySystem() {
        SolarSystemObject = new GameObject("SolarSystem");
        foreach(Planet planet in Emitter.planets) { 
            GameObject planetComponent = GetComponentInParent<GalaxyComponent>().mainController.GetPlanet(planet);
            planetComponent.transform.SetParent(SolarSystemObject.transform);
            
            float angle = UnityEngine.Random.Range(0f, 2 * Mathf.PI);

            planetComponent.transform.position = EllipsePoint(angle, planet.Orbit.radius, planet.Orbit.eccentricity);

            DrawOrbit(planet.Orbit.radius, planet.Orbit.eccentricity);
        }
    }

    private Vector3 EllipsePoint(float angle, float r, float eps) {
        float r_ = r * Mathf.Sqrt(1-eps*eps);
        return new Vector3(r * Mathf.Cos(angle), r_ * Mathf.Sin(angle));
    }


    private void DrawOrbit(float r, float eps) {
        GameObject o = new GameObject();
        LineRenderer lr = o.AddComponent<LineRenderer>();
        o.transform.SetParent(SolarSystemObject.transform);

        o.layer = (int) AppInfo.Layer.StarMap;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;

        int npoints = 100;
        Vector3[] points = new Vector3[npoints];
        for(int i = 0; i < npoints; ++i) {
            float angle = (float) i / (npoints - 1) * 2 * Mathf.PI;
            points[i] = EllipsePoint(angle, r, eps);
        }
        lr.positionCount = npoints;
        lr.SetPositions(points);

        lr.startColor = Color.white;
        lr.endColor = Color.black;
        lr.material = GetComponentInParent<GalaxyComponent>().starPathMaterial;
    }

    public override void HandleEvent(string signal, object[] args) {
        throw new NotImplementedException();
    }

    public override void HandleEvent(string signal) {
        base.HandleEvent(signal);
    }
}
