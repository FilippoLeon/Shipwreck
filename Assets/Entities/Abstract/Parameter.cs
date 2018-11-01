

using MoonSharp.Interpreter;
using System.Collections.Generic;

[MoonSharpUserData]
public class ParameterTable {

    void Insert(string key, Table obj) {
        //values.Add(key, obj.);
    }

    void Insert(string key, object obj) {
        values.Add(key, obj);
    }

    void Remove(string key) {
        values.Remove(key);
    }

    void At(string key) {

    }

    Dictionary<string, object> values;
}