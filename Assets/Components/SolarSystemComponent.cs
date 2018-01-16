using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystemComponent : ObserverBehaviour<SolarSystem> {

    public void DisplaySystem() {
        int planets = 5;
        for(int p = 0; p < planets; ++p) {
            GameObject o = GetComponentInParent<GalaxyComponent>().mainController.GetPlanet();

            float r = UnityEngine.Random.Range(1f, 10f);
            float eps = UnityEngine.Random.Range(0f, 0.001f);
            float angle = UnityEngine.Random.Range(0f, 2 * Mathf.PI);

            o.transform.position = EllipsePoint(angle, r, eps);

            DrawOrbit(r, eps);
        }
    }

    private Vector3 EllipsePoint(float angle, float r, float eps) {
        float r_ = r * Mathf.Sqrt(1-eps*eps);
        return new Vector3(r * Mathf.Cos(angle), r_ * Mathf.Sin(angle));
    }


    private void DrawOrbit(float r, float eps) {
        GameObject o = new GameObject();
        LineRenderer lr = o.AddComponent<LineRenderer>();

        o.layer = (int) AppInfo.Layer.StarMap;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;

        int npoints = 30;
        Vector3[] points = new Vector3[npoints];
        for(int i = 0; i < npoints; ++i) {
            float angle = (float) i / (npoints - 1) * 2 * Mathf.PI;
            points[i] = EllipsePoint(angle, r, eps);
        }
        lr.numPositions = npoints;
        lr.SetPositions(points);

        lr.startColor = Color.white;
        lr.endColor = Color.black;
        lr.material = GetComponentInParent<GalaxyComponent>().starPathMaterial;
    }

    public override void HandleEvent(string signal, object[] args) {
        throw new NotImplementedException();
    }

    public override void HandleEvent(string signal) {
        throw new NotImplementedException();
    }
}
