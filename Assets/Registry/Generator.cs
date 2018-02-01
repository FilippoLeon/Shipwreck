using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class Generator {

    Dictionary<string, PlanetGenerator> planetBiomes = new Dictionary<string, PlanetGenerator>();

    public static TKey lowerBound<TKey, TValue>(SortedList<TKey, TValue> dictionary, TKey key) where TKey : System.IComparable {
        if (dictionary == null || dictionary.Count <= 0) {
            throw new System.Exception("Empty of null list.");
        }
        if (dictionary.Count == 1) { return dictionary.GetEnumerator().Current.Key; }
        
        int lower = 0;
        int upper = dictionary.Count - 1;
        int testKey = (lower + upper) / 2;

        if (testKey >= dictionary.Count - 1) {
            return dictionary.Keys[dictionary.Count - 1];
        } else if (testKey < 0) {
            return dictionary.Keys[0];
        }
        while ( lower <= upper ) {
            int comparisonResult = key.CompareTo(dictionary.Keys[testKey]);
            if (comparisonResult == 0) {
                return key;
            } if (comparisonResult < 0) {
                upper = testKey - 1;
            } else {
                lower = testKey + 1;
            }
            testKey = (lower + upper) / 2;
        }
        
        return dictionary.Keys[testKey];
    }

    public Generator() {
        string pathXml = Path.Combine(Application.streamingAssetsPath, "Data/Prototypes/Biomes.xml");
        ReadPrototypes(pathXml);
    }

    public void ReadPrototypes(string pathXml) {
        if (File.Exists(pathXml)) {
            XmlReaderSettings settings = new XmlReaderSettings();
            XmlReader reader = XmlReader.Create(pathXml, settings);

            reader.ReadToFollowing("Generators");

            int count = 0;
            while (reader.Read()) {
                switch (reader.Name) {
                    case "PlanetGenerator":
                        count++;
                        PlanetGenerator pg = new PlanetGenerator(reader);
                        planetBiomes[pg.Id] = pg;
                        break;
                }
            }

            Debug.LogFormat("Loaded {0} Generators prototypes.", count);
        } else {
            Debug.LogErrorFormat("File '{0}' not found.", pathXml);
        }
    }

    public void Generate(Planet p) {
        int totScore = 0;
        SortedList<int, PlanetGenerator> generators = new SortedList<int, PlanetGenerator>();
        foreach(PlanetGenerator gen in planetBiomes.Values) {
            int score = (int) (gen.Call("OnScoreGenerator", new object[] { p }) as DynValue).Number;
            if(score > 0) {
                generators[totScore] = gen;
                totScore += score;
            }
        }

        int rnd = UnityEngine.Random.Range(0, totScore);

        int k = lowerBound(generators, rnd);
        generators[k].Call("OnGenerate", new object[] { p });
        p.SpriteInfo = generators[k].SpriteInfo;
    }

}