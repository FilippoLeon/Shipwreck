﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Starfield : MonoBehaviour {

    public GameObject[] stars;

    public Color[] colors;

    public Material material;
    private Material[] materials;

    public float scale = 0.1f;

    public int density = 250;

    public int quadrantSizeX = 50;
    public int quadrantSizeY = 50;

    [Range(0f, 10f)]
    public float speed = 1f;
    public Vector3 velocity = new Vector3(-0.11f, 0.2f);

    public float sizeMin = 0.6f;
    public float sizeMax = 1.5f;


    public int[] weight;
    private int[] sums;

    private GameObject[] quadrants;

    public Material trMat;

    void Start() {
        quadrants = new GameObject[4];
       
        Debug.Assert(weight.Length == colors.Length);

        materials = new Material[colors.Length];
        for(int i = 0; i < colors.Length; ++i) {
            materials[i] = new Material(material);
            materials[i].color = colors[i];
            materials[i].SetColor("_EmissionColor", colors[i]);
            materials[i].name = "star-" + i.ToString();
        }

        sums = new int[weight.Length];
        int sum = 0;
        for(int i = 0; i < weight.Length; ++i) {
            sum += weight[i];
            sums[i] = sum;
        }
      
        // Create 4 quadrants that will swap position
        for (int j = 0; j < quadrants.Length; ++j) {
            quadrants[j] = new GameObject("quadrant-" + j.ToString());
            quadrants[j].transform.SetParent(this.transform);

            // Populate quadrants with stars
            for (int i = 0; i < density; ++i) {
                int si = Random.Range(0, stars.Length - 1);
                GameObject copy = Instantiate(stars[si]);
                copy.SetActive(true);

                //GameObject go = new GameObject();
                //TrailRenderer tr = go.AddComponent<TrailRenderer>();
                //tr.time = 0.3f;
                //tr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                //tr.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
                //tr.startWidth = 0.1f;
                //tr.endWidth = 0.01f;
                //Gradient gr = new Gradient();
                //gr.colorKeys = new GradientColorKey[] {
                //    new GradientColorKey(Color.red, 0.5f),
                //    new GradientColorKey(Color.white, 0.5f),
                //};
                //tr.colorGradient = gr;
                //tr.receiveShadows = false;
                //tr.material = trMat;
                //go.transform.SetParent(copy.transform);

                copy.transform.SetParent(this.quadrants[j].transform);
                copy.transform.position = new Vector3(
                    Random.Range(-quadrantSizeX / 2, quadrantSizeX / 2),
                    Random.Range(-quadrantSizeY / 2, quadrantSizeY / 2),
                    0
                    );
                float r = scale * Random.Range(sizeMin, sizeMax);
                copy.transform.localScale = new Vector3(r, 1, r);

                //Debug.Log(sum);
                int ri = Random.Range(0, sum);
                //Debug.Log(ri);
                int ix = System.Array.BinarySearch(sums, ri);

                if (ix < 0) {
                    ix = ~ix;
                }
                //Debug.Log(ix);
                copy.transform.GetComponent<MeshRenderer>().material = materials[ix];
            }
        }

        StartField();
    }
	
    /// <summary>
    /// Move quadrants to the correct position
    /// </summary>
    void StartField() {
        quadrants[1].transform.position -= Vector3.left * quadrantSizeX;
        quadrants[2].transform.position -= Vector3.down * quadrantSizeY;
        quadrants[3].transform.position -= new Vector3( quadrantSizeX, quadrantSizeY );
    }
    
    /// <summary>
    /// Shift quadrants.
    /// </summary>
    void Update() {
        Translate(velocity * speed);
    }
    
    /// <summary>
    /// Shift quadrants by delta amount, if boundaries are reached, move quadrants around.
    /// </summary>
    /// <param name="delta"></param>
    void Translate(Vector2 delta) {
        for (int i = 0; i < quadrants.Length; ++i) {
            quadrants[i].transform.Translate(delta);

            //bool translated = false;

            Vector3 pos = quadrants[i].transform.localPosition;
            if( pos.x < -quadrantSizeX ) {
                pos.x += 2 * quadrantSizeX;
                //translated = true;
            } else if( pos.x > quadrantSizeX ) {
                pos.x -= 2 * quadrantSizeX;
                //translated = true;
            }
            if ( pos.y < -quadrantSizeY ) {
                pos.y += 2 * quadrantSizeY;
                //translated = true;
            } else if( pos.y > quadrantSizeY ) {
                pos.y -= 2 * quadrantSizeY;
                //translated = true;
            }
            quadrants[i].transform.localPosition = pos;
            //if ( translated ) {
            //    quadrants[i].SetActive(false);
            //} else {
            //    quadrants[i].SetActive(true);
            //}
        }
    }
}

