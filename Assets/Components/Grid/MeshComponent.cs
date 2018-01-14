using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MeshComponent : ObserverBehaviour<Verse> {
    protected int tileSizeX = 32;
    protected int tileSizeY = 32;
    protected Texture2D paletteTexture;

    protected int sizeX = 30;
    protected int sizeY = 30;
    protected int offsetX = 15;
    protected int offsetY = 15;

    protected Color[] palette;
    protected MeshRenderer meshRenderer;
    protected Mesh mesh;

    protected float zAxis = 0f;

    protected GameObject overlay;

    public void Awake() {
        overlay = new GameObject("mesh");
        overlay.AddComponent<MeshFilter>();
        meshRenderer = overlay.AddComponent<MeshRenderer>();

        mesh = new Mesh();
        overlay.GetComponent<MeshFilter>().mesh = mesh;
    }

    public void SetResolution(int resX, int resY) {
        tileSizeX = resX;
        tileSizeY = resY;
    }

    protected Color[] GetPalettePixels(int index) {
        return paletteTexture.GetPixels(0, index * tileSizeY, tileSizeX, tileSizeY);
    }
   

    public virtual void CreateMesh() {
        float deltaX = offsetX + 0.5f;
        float deltaY = offsetY + 0.5f;

        Vector3[] vertices = new Vector3[sizeX * sizeY];
        int I = 0;
        for (int i = 0; i < sizeX; ++i) {
            for (int j = 0; j < sizeY; ++j) {
                // Move mesh to have centers at integer coordinates
                vertices[I++] = new Vector3(i - deltaX, j - deltaY, zAxis);
            }
        }

        I = 0;
        Vector2[] uv = new Vector2[sizeX * sizeY];
        for (int i = 0; i < sizeX; ++i) {
            for (int j = 0; j < sizeY; ++j) {
                uv[I++] = new Vector2(i / (float)(sizeX - 1), j / (float)(sizeY - 1));
            }
        }

        int[] triangles = new int[((sizeX - 1) * (sizeY - 1)) * 6];

        I = 0;
        for (int i = 0; i < sizeX - 1; ++i) {
            for (int j = 0; j < sizeY - 1; ++j) {
                int J = (sizeY * j) + i;
                triangles[I++] = J;
                triangles[I++] = J + 1;
                triangles[I++] = J + sizeY + 1;
                triangles[I++] = J;
                triangles[I++] = J + sizeY + 1;
                triangles[I++] = J + sizeY;
            }
        }
        
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }
    
}
