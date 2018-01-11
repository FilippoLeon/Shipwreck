using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Starfield : MonoBehaviour {

    public GameObject[] stars;

    public Color[] colors;

    public Material material;
    private Material[] materials;

    public float scale = 0.1f;

    public int[] weight;
    private int[] sums;

    void Start() {
        int density = 1000;
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

        for (int i = 0; i < density; ++i) {
            int si = Random.Range(0, stars.Length - 1);
            GameObject copy = Instantiate(stars[si]);

            copy.transform.SetParent(this.transform);
            copy.transform.position = new Vector3(Random.Range(-50f, 50f), Random.Range(-50f, 50f), 0);
            float r = scale * Random.Range(0.6f, 1.5f);
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
	
	// Update is called once per frame
	void Update () {
		
	}
}
