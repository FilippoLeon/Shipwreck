using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class Generator {

    Dictionary<string, Dictionary<string, EntityGenerator>> generators = new Dictionary<string, Dictionary<string, EntityGenerator>>();

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
        pathXml = Path.Combine(Application.streamingAssetsPath, "Data/Prototypes/Ships.xml");
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
                    case "ShipGenerator":
                        count++;
                        EntityGenerator pg = new EntityGenerator(reader);
                        generators[reader.Name][pg.Id] = pg;
                        break;
                }
            }

            Debug.LogFormat("Loaded {0} Generators prototypes.", count);
        } else {
            Debug.LogErrorFormat("File '{0}' not found.", pathXml);
        }
    }

    public void Generate<T>(T p, int points = 0) where T : Entity<T> {
        int totScore = 0;
        SortedList<int, EntityGenerator> avail_generators = new SortedList<int, EntityGenerator>();
        foreach(EntityGenerator gen in generators[typeof(T).ToString() + "Generator"].Values) {
            DynValue genScore = (gen.Call("GetGenerationScore", new object[] { Verse.Instance, p, points }) as DynValue);
            int score = 1;
            if (genScore != null) {
                score = (int)genScore.Number;
            }
            if(score > 0) {
                avail_generators[totScore] = gen;
                totScore += score;
            }
        }

        int rnd = UnityEngine.Random.Range(0, totScore);

        int k = lowerBound(avail_generators, rnd);
        avail_generators[k].Call("OnGenerate", new object[] { Verse.Instance, p, points });
        p.Icon = avail_generators[k].Icon;
    }

}