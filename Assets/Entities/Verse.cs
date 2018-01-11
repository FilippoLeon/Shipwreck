using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[MoonSharpUserData]
public class Verse : Entity<Verse> {
    public Registry registry;
    public static Verse Instance = null;

    public string Name { get { return "TheVerse"; } }
    
    public enum VerseMode {
        None,
        Build
    };

    public VerseMode verseMode = VerseMode.None;
    public object[] modeArgs;

    public void SetMode(VerseMode mode, object[] args) {
        verseMode = mode;
        modeArgs = args;
    }

    public Verse() {
        if(Instance == null ) {
            Instance = this;
        } else {
            Debug.LogError("More than one Verse whaat?");
        }

        registry = new Registry();
    }

    public override Verse Clone() {
        throw new NotImplementedException();
    }
}
