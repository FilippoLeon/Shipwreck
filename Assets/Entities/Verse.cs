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

    List<Ship> ships = new List<Ship>();
    List<Player> players = new List<Player>();

    public class Selection {
        public Coordinate coordinate { set; get; }
        public int index { set; get; }
        public IEmitter selection { set; get; }
    }

    public Selection selection;

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

    public void SpawnEntity(ConcreteEntity entity, Coordinate position) {
        // new and return new object (from prototype? nope, must clone and blablas
        // Emit 
        throw new NotImplementedException();
    }

    public Verse() {
        if(Instance == null ) {
            Instance = this;
        } else {
            Debug.LogError("More than one Verse whaat?");
        }

        registry = new Registry();
    }

    public void AddPlayer(Player player) {
        players.Add(player);
    }

    public void AddShip(Ship ship) {
        ships.Add(ship);
    }

    public void Select(Coordinate where) {
        List<Part> listOfParts = ships[0].PartAt(where);
        if( listOfParts == null || listOfParts.Count == 0 ) {
            return;
        }
        if( selection == null || selection.coordinate != where ) {
            selection = new Selection { coordinate = where, index = 0, selection = listOfParts[0] };
            Debug.Log(String.Format("Selecting new coordinate {0}.", where));
        } else if( selection.coordinate == where ) {
            selection.index = (selection.index + 1) % listOfParts.Count;
            selection.selection = listOfParts[selection.index];
            Debug.Log(String.Format("Selecting same coordinate {0}, indesx = {1}", where, selection.index));
        }
        if( selection.selection != null ) {
            GUIController.childs["part_view"].SetParameters(new object[] { selection.selection });
        }
    }

    public override Verse Clone() {
        throw new NotImplementedException();
    }
    public override void Update() {
        foreach(Player p in players) {
            p.Update();
        }
        foreach(Ship s in ships) {
            s.Update();
        }
    }
}
