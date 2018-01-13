using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Starfield : MonoBehaviour {

    public GameObject[] stars;

    public Color[] colors;

    public Material material;
    private Material[] materials;

    public float scale = 0.1f;

    public int density = 1000;

    public int quadrantSizeX = 100;
    public int quadrantSizeY = 100;

    [Range(0f, 10f)]
    public float speed = 1f;
    public Vector3 velocity = new Vector3(-0.11f, 0.2f);

    public float sizeMin = 0.6f;
    public float sizeMax = 1.5f;


    public int[] weight;
    private int[] sums;

    private GameObject[] quadrants;

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

        for (int j = 0; j < quadrants.Length; ++j) {
            quadrants[j] = new GameObject("quadrant-" + j.ToString());
            quadrants[j].transform.SetParent(this.transform);

            for (int i = 0; i < density; ++i) {
                int si = Random.Range(0, stars.Length - 1);
                GameObject copy = Instantiate(stars[si]);
                copy.SetActive(true);

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
	
    
    void StartField() {
        quadrants[1].transform.position -= Vector3.left * quadrantSizeX;
        quadrants[2].transform.position -= Vector3.down * quadrantSizeY;
        quadrants[3].transform.position -= new Vector3( quadrantSizeX, quadrantSizeY );
    }

    // Update is called once per frame
    void Update() {
        Translate(velocity * speed);
    }
    

    void Translate(Vector2 delta) {
        for (int i = 0; i < quadrants.Length; ++i) {
            quadrants[i].transform.Translate(delta);

            Vector3 pos = quadrants[i].transform.position;
            if( pos.x < -quadrantSizeX ) {
                pos.x += 2 * quadrantSizeX;
            } else if( pos.x > quadrantSizeX ) {
                pos.x -= 2 * quadrantSizeX;
            }
            if ( pos.y < -quadrantSizeY ) {
                pos.y += 2 * quadrantSizeY;
            } else if( pos.y > quadrantSizeY ) {
                pos.y -= 2 * quadrantSizeY;
            }
            quadrants[i].transform.position = pos;
        }
    }
}

