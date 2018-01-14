using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class OverlayComponent : MeshComponent {
    Func<Coordinate, int> Heatmap;

    Texture2D overlayTexture;

    public GameObject colorMap;

    new void Awake() {
        SetResolution(1, 1);
        name = "Overlay";


        base.Awake();

        GeneratePalette("rainbow_bgyrm_35-85_c71_n256.csv");


        paletteData = Directory.GetFiles(StreamingAssets.GetPath("Colormaps"), "*.csv");
    }

    string[] paletteData = null;
    int index = 0;

    public void NextPalette() {
        index = (index + 1) % paletteData.Length;
        GeneratePalette(paletteData[index]);
    }
    public void PreviousPalette() {
        index = (index + paletteData.Length - 1) % paletteData.Length;
        GeneratePalette(paletteData[index]);
    }

    public void GeneratePalette(string name) {

        palette = File.ReadAllLines(StreamingAssets.ReadFile("Colormaps", name)).Select(
            line => {
                string[] lineS = line.Split(',');
                return new Color(float.Parse(lineS[0]), float.Parse(lineS[1]), float.Parse(lineS[2]));
            }
        ).ToArray();

        int paletteSize = palette.Length;
        paletteTexture = new Texture2D(tileSizeX, tileSizeY * (paletteSize + 1));

        Color[] colors = paletteTexture.GetPixels();
        for (int s = 0; s < paletteSize; ++s) {
            Color baseColor = palette[s];
            for (int i = 0; i < tileSizeX; ++i) {
                for (int j = 0; j < tileSizeY; ++j) {
                    colors[(s * tileSizeX + i) * tileSizeY + j] = baseColor;
                }
            }
        }
        Color transparent = new Color(0f, 0f, 0f, 1.0f);
        for (int i = 0; i < tileSizeX; ++i) {
            for (int j = 0; j < tileSizeY; ++j) {
                colors[(paletteSize * tileSizeX + i) * tileSizeY + j] = transparent;
            }
        }
        paletteTexture.SetPixels(colors);
        paletteTexture.Apply(false);
    }

    public void SetMap(Func<Coordinate, int> hm) {
        Heatmap = hm;
    }

    public override void CreateMesh() {
        zAxis = -0.01f;

        base.CreateMesh();

        meshRenderer.material = new Material(Shader.Find("Sprites/Diffuse"));
        Color col = meshRenderer.material.color;
        col.a = 0.5f;
        meshRenderer.material.color = col;

        // Set main texture (world)
        overlayTexture = new Texture2D(tileSizeX * (sizeX - 1), tileSizeY * (sizeY - 1), TextureFormat.ARGB32, false);
        meshRenderer.material.mainTexture = overlayTexture;
        meshRenderer.material.mainTexture.filterMode = FilterMode.Point;

        colorMap = new GameObject("colormap");
        Mesh colorMapMesh = new Mesh();
        colorMapMesh.vertices = new Vector3[] {
            new Vector3(0, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(1, 1, 0),
            new Vector3(1, 0, 0),
        };
        colorMapMesh.uv = new Vector2[] {
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(1, 0),
        };
        colorMapMesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
        //colorMap.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Sprites/Default"));
        //colorMap.GetComponent<MeshRenderer>().material.mainTexture = paletteTexture;
        //colorMap.AddComponent<MeshFilter>().mesh = colorMapMesh;
        //colorMap.transform.localScale = new Vector3(0.5f, 5, 1);

        colorMap.AddComponent<Image>().sprite = Sprite.Create(paletteTexture, new Rect(0, 0, 1, 257), new Vector2(0f, 0f));
        colorMap.GetComponent<Image>().material = new Material(Shader.Find("Sprites/Default"));
        colorMap.transform.SetParent(GameObject.Find("Canvas").transform);
        colorMap.GetComponent<Image>().rectTransform.pivot = new Vector2(0, 0);
        colorMap.GetComponent<Image>().rectTransform.anchorMin = new Vector2(0, 0);
        colorMap.GetComponent<Image>().rectTransform.anchorMax = new Vector2(0, 0);
        colorMap.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(32, 100);
        colorMap.GetComponent<Image>().rectTransform.position = new Vector2(0, 0);

        Update();
    }
    
    public override void HandleEvent(string signal) {
        throw new NotImplementedException();
    }

    public override void HandleEvent(string signal, object[] args) {
        switch( signal ) {
            case "VerseCreated":
                CreateMesh();
                break;
            case "SetMap":
                SetMap(args[0] as Func<Coordinate, int>);
                break;
        }
    }

    private void Update() {
        if( Input.GetKeyDown(KeyCode.Tab)) {
            if (Input.GetKey(KeyCode.LeftShift)) {
                NextPalette();
            } else {
                PreviousPalette();
            }
            colorMap.GetComponent<Image>().sprite = Sprite.Create(paletteTexture, new Rect(0, 0, 1, 257), new Vector2(0f, 0f));
        }

            if ( Heatmap == null) {
            overlay.SetActive(false);
            return;
        } else {
            overlay.SetActive(true);
        }

        int paletteSize = palette.Length;
        for (int i = 0; i < sizeX - 1; ++i) {
            for (int j = 0; j < sizeY - 1; ++j) {
                Coordinate c = new Coordinate(i - offsetX, j - offsetY);
                int val = Heatmap(c);
                overlayTexture.SetPixels(i * tileSizeX, j * tileSizeY, tileSizeX, tileSizeY,
                    GetPalettePixels(val < 0 ? paletteSize : val % paletteSize)
                    );
            }
        }

        overlayTexture.Apply(false);
    }

}

