using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class XmlUtilities {
    public static Vector2 ToVector2(string vectorString) {
        if (vectorString == null) {
            return new Vector2(0, 0);
        }
        string[] split = vectorString.Split(',');
        return new Vector2(Convert.ToSingle(split[0]), Convert.ToSingle(split[1]));
    }

    internal static int[] ToIntArray(string vectorString) {
        if (vectorString == null) {
            return new int[] {0, 0, 0, 0 };
        }
        return vectorString.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
    }
}
