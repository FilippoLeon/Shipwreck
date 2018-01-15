using System;
using System.Collections;
using System.Collections.Generic;

public class Triangle {
    int[] indices;
    Triangle[] replacements;
    
    public Triangle(int[] indices) {
        this.indices = indices;
    }
    public int this[int i] {
        get { return indices[i]; }
        set { indices[i] = value; }
    }

    internal void ReplaceBy(Triangle[] newTriangles) {
        replacements = newTriangles;
    }

    internal Triangle[] GetReplacements() {
        return replacements;
    }
}
