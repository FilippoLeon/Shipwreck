using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystemComponent : ObserverBehaviour<SolarSystem> {

    public GameObject SolarSystemObject;

    public GameObject DisplaySystem() {
        SolarSystemObject = new GameObject("SolarSystem");


        GameObject sunComponent = GetComponentInParent<GalaxyComponent>().mainController.GetPlanet();
        sunComponent.transform.SetParent(SolarSystemObject.transform);
        if ( true ) {
            sunComponent.GetComponentInChildren<SpriteRenderer>().sprite
                = SpriteLoader.Instance.Load(new SpriteInfo("Planet", "giant_star"), null).sprite;
        }
        sunComponent.transform.position = new Vector2(0f,0f);

        foreach (Planet planet in Emitter.planets) { 
            GameObject planetComponent = GetComponentInParent<GalaxyComponent>().mainController.GetPlanet(planet);
            planetComponent.transform.SetParent(SolarSystemObject.transform);

            if (planet.Icon != null) {
                planetComponent.GetComponentInChildren<SpriteRenderer>().sprite 
                    = SpriteLoader.Instance.Load(planet.Icon.Get(0), planet).sprite;
            }

            float angle = UnityEngine.Random.Range(0f, 2 * Mathf.PI);

            planetComponent.transform.position = EllipsePoint(angle, planet.Orbit.radius, planet.Orbit.eccentricity);

            DrawOrbit(angle, planet.Orbit.radius, planet.Orbit.eccentricity);
        }
        return SolarSystemObject;
    }

    private Vector3 EllipsePoint(float angle, float r, float eps) {
        float r_ = r * Mathf.Sqrt(1-eps*eps);
        return new Vector3(r * Mathf.Cos(angle), r_ * Mathf.Sin(angle));
    }


    private void DrawOrbit(float angle, float r, float eps) {
        GameObject o = new GameObject();
        LineRenderer lr = o.AddComponent<LineRenderer>();
        o.transform.SetParent(SolarSystemObject.transform);

        o.layer = (int) AppInfo.Layer.StarMap;
        lr.startWidth = 0.08f;
        lr.endWidth = 0.03f;

        int npoints = 100;
        Vector3[] points = new Vector3[npoints];
        for(int i = 0; i < npoints; ++i) {
            float c = (float) i / (npoints - 1) * 2 * Mathf.PI;
            points[i] = EllipsePoint(angle + c, r, eps);
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
