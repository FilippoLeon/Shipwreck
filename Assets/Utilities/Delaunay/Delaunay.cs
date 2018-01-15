using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delaunay {
    public List<Vertex> vertices = new List<Vertex>();

    public List<Triangle> triangles = new List<Triangle>();

    public void Create() {
        // Add points at "infinity"
        triangles.Add(new Triangle(new int[] { 0, 1, 2}));

        for (int P = 3; P < vertices.Count; ++P) {
            Vertex vP = vertices[P];

            Triangle ABC = FindContaining(P);

            Triangle[] newTriangles = new Triangle[] {
                 new Triangle(new int[] { P, ABC[0], ABC[1] }),
                 new Triangle(new int[] { P, ABC[1], ABC[2] }),
                 new Triangle(new int[] { P, ABC[2], ABC[0] }),
            };
            triangles.AddRange(newTriangles);
            ABC.ReplaceBy(newTriangles);

            int X = ABC[0];
            bool progress = false;
            while ( !progress || X != ABC[0] ) {
                progress = true;
                Triangle PXY = null;
                int Y = FindContainingEdge(new int[] { P, X }, out PXY, P);
                if ( (X != 0 && X != 1 && X != 2) || (Y != 0 && Y != 1 && Y != 2)) {
                    Triangle ZYX = null;
                    int Z = FindContainingEdge(new int[] { Y, X }, out ZYX, P);
                    if ( Inside(P, X, Y, Z) ) {
                        
                        Triangle[] newTrianglesFlip = new Triangle[] {
                            new Triangle(new int[] {P, X, Z}),
                            new Triangle(new int[] {P, Z, Y}),
                        };

                        triangles.AddRange(newTrianglesFlip);

                        PXY.ReplaceBy(newTrianglesFlip);
                        ZYX.ReplaceBy(newTrianglesFlip);
                        progress = false;
                    }
                }
                if(progress) {
                    X = Y;
                }
            }

        }
    }

    private bool Inside(int A, int B, int C, int D) {
        Vertex vAp = vertices[A];
        Vertex vBp = vertices[B];
        Vertex vCp = vertices[C];
        Vertex vDp = vertices[D];

        Vector3 vA, vB, vC, vD;

        //if ( vBp == null ) {
        //    if( vCp == null) {
        //        return true;
        //    } else if( vDp == null ) {
        //        return false;
        //    } else {
        //        vA = vertices[A].coordinate;
        //        vC = vertices[C].coordinate;
        //        vD = vertices[D].coordinate;
        //        return Vector3.Cross(vD - vA, vC - vA).z >= 0;
        //    }
        //} else if( vCp == null ) {
        //    if( vDp == null ) {
        //        return false;
        //    } else {
        //        vA = vertices[A].coordinate;
        //        vB = vertices[B].coordinate;
        //        vD = vertices[D].coordinate;
        //        return Vector3.Cross(vB - vA, vD - vA).z >= 0;
        //    }
        //} else if( vDp == null ) {
        //    return false;
        //}
        vA = vertices[A].coordinate;
        vB = vertices[B].coordinate;
        vC = vertices[C].coordinate;
        vD = vertices[D].coordinate;

        Matrix4x4 m = new Matrix4x4();
        Vector4 x = new Vector4(vA.x, vB.x, vC.x, vD.x);
        Vector4 y = new Vector4(vA.y, vB.y, vC.y, vD.y);
        
        m.SetColumn(0, x);
        m.SetColumn(1, y);
        m.SetColumn(2, Vector4.Scale(x,x) + Vector4.Scale(y,y));
        m.SetColumn(3, Vector4.one);

        return (m.determinant >= 0);
    }

    private int FindContainingEdge(int[] edge, out Triangle ZYX, int different) {
        foreach (Triangle t in triangles) {
            if( t.GetReplacements() != null ) {
                continue;
            }
            for (int i = 0; i < 3; ++i) {
                if (t[i] == edge[0] && t[(i+1)%3] == edge[1]) {
                    ZYX = t;
                    if (t[(i + 2) % 3] != different) {
                        return t[(i + 2) % 3];
                    }
                }
            }
        }
        throw new Exception("Cannot find tirangle containing edge.");
    }

    private Triangle FindContaining(int P) {
        Triangle next = triangles[0];
        while (true) {
            Triangle[] childs = next.GetReplacements();
            if( childs == null ) {
                return next;
            }
            foreach ( Triangle c in childs ) {
                if( TriangleContainsPoint(c, P)  ) {
                    next = c;
                    break;
                }
            }
        }


    }

    private bool TriangleContainsPoint(Triangle c, int P) {
        //int count = 0;
        //for (int i = 0; i < 3; ++i) {
        //    if (c[i] == 0 || c[i] == 1 || c[i] == 2) {
        //        count++;
        //    }
        //}
        //if(count >= 1) {
        //    return true;
        //}
        Vertex vP = vertices[P];
        for(int i = 0; i  < 3; ++i) {
            Vector3 edge = vertices[c[(i + 1) % 3]].coordinate - vertices[c[i]].coordinate;
            Vector3 vec = vP.coordinate - vertices[c[i]].coordinate;
            float crossZ = Vector3.Cross(edge, vec).z;
            if( crossZ < 0) {
                return false;
            }
        }
        return true;
    }
}
