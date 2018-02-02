using MoonSharp.Interpreter;
using UnityEngine;

[MoonSharpUserData]
public struct Coordinate {
    public int x, y;

    public Coordinate(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public Coordinate(Vector3 coord) {
        x = Mathf.CeilToInt(coord.x - 0.5f);
        y = Mathf.CeilToInt(coord.y - 0.5f);
    }

    public Vector2 ToVector() {
        return new Vector2(x, y);
    }

    public static Coordinate Up = new Coordinate(0, 1);
    public static Coordinate Down = new Coordinate(0, -1);
    public static Coordinate Right = new Coordinate(1, 0);
    public static Coordinate Left = new Coordinate(-1, 0);
    
    public static Coordinate operator +(Coordinate a, Coordinate b) {
        return new Coordinate(a.x + b.x, a.y + b.y);
    }

    public static bool operator ==(Coordinate a, Coordinate b) {
        return (a.x == b.x && a.y == b.y);
    }
    public static bool operator !=(Coordinate a, Coordinate b) {
        return (a.x != b.x || a.y != b.y);
    }
    public override string ToString() {
        return "(" + x + "," + y + ")";
    }

    public override bool Equals(object obj) {
        if (!(obj is Coordinate)) {
            return false;
        }

        var coordinate = (Coordinate) obj;
        return x == coordinate.x &&
               y == coordinate.y;
    }

    public override int GetHashCode() {
        var hashCode = 1502939027;
        hashCode = hashCode * -1521134295 + base.GetHashCode();
        hashCode = hashCode * -1521134295 + x.GetHashCode();
        hashCode = hashCode * -1521134295 + y.GetHashCode();
        return hashCode;
    }
}