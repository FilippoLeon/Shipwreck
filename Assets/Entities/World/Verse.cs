using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[MoonSharpUserData]
public class Verse : Entity<Verse>, IView {
    public Registry registry;
    public static Verse Instance = null;

    public List<Ship> ships = new List<Ship>();
    List<Player> players = new List<Player>();

    public List<IEmitter> entities = new List<IEmitter>();

    public List<IEmitter> flaggedForRemovalEntities = new List<IEmitter>();

    public Dictionary<string, Func<Coordinate, int>> maps = new Dictionary<string, Func<Coordinate, int>>();
    
    /// <summary>
    /// Index of active ship.
    /// </summary>
    int index = 0;

    public Selection selection;

    public new string Name { get { return "TheVerse"; } }

    public enum VerseMode {
        None,
        Build
    };

    public VerseMode verseMode = VerseMode.None;
    public object[] modeArgs;

    ConcreteEntity selectionEntity;

    public ConcreteEntity buildIndicatorEntity;

    public SpriteLoader SpriteLoader {
        get {
            return SpriteController.spriteLoader;
        }
    }

    public Player Player {
        get {
            return players[0];
        }
    }

    public Galaxy Galaxy {set; get; }

    public class Selection {
        public Coordinate coordinate { set; get; }
        public int index { set; get; }
        public IEmitter selection { set; get; }
    }

    public void AddEntity<T>(ConcreteEntity<T> e) where T: class {
        entities.Add(e);
    }

    public void RemoveEntity(IEmitter e) {
        entities.Remove(e);
    }

    internal void RemoveEntity<T>(ConcreteEntity<T> e) where T: class {
        entities.Remove(e);
    }

    public void MarkForRemoval(ConcreteEntity e) {
        flaggedForRemovalEntities.Add(e);
    }

    public Ship ActiveShip() {
        return ships[index];
    }

    public void NextShip() {
        index = (index + 1) % ships.Count;

        Emit("OnActiveShipChanged", new object[] { });
    }
   

    public void SetMode(VerseMode mode, object[] args) {
        verseMode = mode;
        modeArgs = args;
    }

    public void SpawnEntity<T>(ConcreteEntity<T> entity, Vector2 position)  where T: class {
        Emit("SpawnEntity", new object[] { entity });
        entity.Position = position;
        entity.Spawn(position);
        entity.Verse =  this;
        AddEntity<T>(entity);
    }
    public void SpawnSelectionEntity<T>(ConcreteEntity<T> entity, Vector2 position) where T : class {
        Emit("SpawnSelectionEntity", new object[] { entity });
        entity.Position = position;
        entity.Spawn(position);
        entity.Verse = this;
        AddEntity<T>(entity);
    }

    public Verse() {
        if(Instance == null ) {
            Instance = this;
        } else {
            Debug.LogError("More than one Verse whaat?");
        }

        registry = new Registry();

        Galaxy = new Galaxy(this);
    }

    public void OnWarpTo(ILocation location) {
        Emit("OnWarpTo", new object[] { });
    }
    

    public void Create() {
        Galaxy.Initialize();

        maps["Health"] = (Coordinate c) => {
            int id = -1;
            if (this.ships != null &&this.ships.Count != 0) {
                Ship ship = this.ActiveShip();
                if (ship != null) {
                    Part part = ship.HullAt(c);
                    if (part != null) {
                        id = part.Health;
                    }
                }
            }
            return id;
        };
        maps["Pressure"] = (Coordinate c) => {
            int id = -1;
            if (this.ships != null && this.ships.Count != 0) {
                Ship ship = this.ActiveShip();
                if (ship != null) {
                    Part part = ship.HullAt(c);
                    if (part != null) {
                        id = Mathf.FloorToInt(
                            Convert.ToSingle(part.GetParameter("pressure")) / Convert.ToSingle(part.GetParameter("max_pressure")) * 255
                            );
                    }
                }
            }
            return id;
        };

        Galaxy.Create();

        selectionEntity = new ConcreteEntity();
        selectionEntity.Name = "Selector";
        selectionEntity.spriteInfo = new Icon(new SpriteInfo() { id = "selector_1", category = "UI" });

        SpawnSelectionEntity(selectionEntity, new Vector2(0, 0));
        selectionEntity.Active = false;

        buildIndicatorEntity = new ConcreteEntity();
        buildIndicatorEntity.Name = "BuildIndicator";
        SpawnEntity(buildIndicatorEntity, new Vector2(0, 0));
        buildIndicatorEntity.Active = false;

        Emit("VerseCreated", new object[] { });
    }

    public void AddPlayer(Player player) {
        players.Add(player);
    }

    public void AddShip(Ship ship) {
        ships.Add(ship);
    }

    public Ship GetShip(int index) {
        return ships[index];
    }

    public void Select(Coordinate where) {
        List<Part> listOfParts = ActiveShip().PartAt(where);
        if( listOfParts == null || listOfParts.Count == 0 ) {
            return;
        }
        if( selection == null || selection.coordinate != where ) {
            selection = new Selection { coordinate = where, index = 0, selection = listOfParts[0] };
            //Debug.Log(String.Format("Selecting new coordinate {0}.", where));
        } else if( selection.coordinate == where ) {
            selection.index = (selection.index + 1) % listOfParts.Count;
            selection.selection = listOfParts[selection.index];
            //Debug.Log(String.Format("Selecting same coordinate {0}, indesx = {1}", where, selection.index));
        }
        if( selection.selection != null ) {
            GUIController.childs["part_view"].SetParameters(new object[] { selection.selection });

            selectionEntity.Active = true;
            selectionEntity.Position = selection.coordinate.ToVector();
        } else {
            selectionEntity.Active = false;
        }
    }

    public override Verse Clone() {
        throw new NotImplementedException();
    }

    public void SetMap(string name) {
        Emit("SetMap", new object[] { name == null ? null : maps[name] });
    }

    public override void Update() {
        if (verseMode == VerseMode.Build) {

            if (modeArgs.Length >= 1 && modeArgs[0] is string) {
                Part p = registry.partRegistry.Get(modeArgs[0] as string, false);
                buildIndicatorEntity.SpriteInfo = new Icon(new SpriteInfo(p.SpriteInfo.Get(0)) { tint = Color.green, layer = 3 });
                buildIndicatorEntity.Active = true;
            }
        } else {
            buildIndicatorEntity.Active = false;
        }

        foreach(Player p in players) {
            p.Update();
        }
        foreach(Ship s in ships) {
            s.Update();
        }
        foreach(IEmitter e in entities) {
            if (e is IUpdateable) {
                (e as IUpdateable).Update();
            }
        }
        //if (Input.GetMouseButtonDown(2)) {
        //    selectionEntity.Active = !selectionEntity.Active;
        //}

        foreach(IEmitter e in flaggedForRemovalEntities) {
            RemoveEntity(e);
            if (e is ISelfDestructible) {
                (e as ISelfDestructible).SelfDestroy();
            }
        }
        flaggedForRemovalEntities.Clear();
    }

    public Coordinate GetMin() {
        return new Coordinate(-10, -10);
    }

    public Coordinate GetMax() {
        return new Coordinate(10, 10);
    }
}
