using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NameRegistry {
    public Dictionary<string, List<string>> names = new Dictionary<string, List<string>>();

    public NameRegistry() {
        DirectoryInfo dir = new DirectoryInfo(StreamingAssets.GetPath("Data/Names"));

        FileInfo[] fileInfo = dir.GetFiles("*.txt");

        foreach (FileInfo file in fileInfo) {
            Read(file.FullName);
        }
    }

    public List<string> this[string s] {
        get { return names[s]; }
    }

    public void Read(string name) {
        string[] lines = File.ReadAllLines(name);
        if( lines[0][0] == '@' ) {
            string cat = lines[0].TrimStart('@');
            if ( !names.ContainsKey( cat ) ) {
                names[cat] = new List<string>();
            }
            for(int i = 1; i < lines.Length; ++i) {
                names[cat].Add(lines[i]);
            }
            Debug.Log(String.Format("Loaded '{0}' files into category '{1}' from '{2}'", lines.Length, cat, name));
        } else {
            Debug.LogError("No category specified (first line must begin with '@'.");
        }
    }

    public string GetRandom(string v) {
        if( !names.ContainsKey(v) || names[v] == null) {
            Debug.LogError("Category not found!");
            return null;
        } else {
            return names[v][UnityEngine.Random.Range(0, names[v].Count)];
        }
    }
}