using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class OverlayComponent : MeshComponent {
    Func<int, int, int> Heatmap;
    Texture2D overlayTexture;

    new void Awake() {
        SetResolution(1, 1);

        Heatmap = (i, j) => {
            int id = -1;
            if ((Emitter as Verse).ships != null && (Emitter as Verse).ships.Count != 0) {
                Ship ship = (Emitter as Verse).ships[0];
                if (ship != null) {
                    List<Part> parts = ship.PartAt(new Coordinate(i, j));
                    if (parts != null && parts.Count != 0) {
                        id = parts[0].Health;
                    }
                }
            }
            return id;
        };

        palette = File.ReadAllLines(StreamingAssets.ReadFile("Colormaps", "rainbow_bgyrm_35-85_c71_n256.csv")).Select(
            line => {
                string[] lineS = line.Split(',');
                return new Color(float.Parse(lineS[0]), float.Parse(lineS[1]), float.Parse(lineS[2]));
            }
        ).ToArray();

        base.Awake();

        name = "Overlay";

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
        }
    }

    private void Update() {
        int paletteSize = palette.Length;
        for (int i = 0; i < sizeX - 1; ++i) {
            for (int j = 0; j < sizeY - 1; ++j) {
                int val = Heatmap(i, j);
                overlayTexture.SetPixels(i * tileSizeX, j * tileSizeY, tileSizeX, tileSizeY,
                    GetPalettePixels(val < 0 ? paletteSize : val % paletteSize)
                    );
            }
        }

        overlayTexture.Apply(false);
    }

}

