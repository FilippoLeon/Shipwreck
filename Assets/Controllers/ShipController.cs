using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Assets.Entities.World;
using UnityEngine;

public class ShipController : MonoBehaviour {

    public Ship Ship { set; get; }
    public Player Player { set; get; }

    public GameObject tacticalViewObject;

    void Start () {
        Verse verse = GetComponent<MainController>().Verse;

        tacticalViewObject = new GameObject("Tactical view");

        Player = new Player(GetComponent<MainController>().Verse) {Funds = 1000};
        Player.Inventory.Add(new Inventory.Item() { part = verse.registry.partRegistry.Get("turret"), quantity = 1 });

        Ship = new Ship(verse) {Name = "Serenity 2.0"};

        AddShip(Ship);
        Ship.AddPart(verse.registry.partRegistry.Get("root"), new Coordinate(0, 0));

        verse.NextShip();

        Ship Ship2 = new Ship(verse);
        Ship2.Name = "Serenity 1.0";

        AddShip(Ship2);
        Ship2.Position = new Vector2(10, 10);
        Ship2.AddPart(verse.registry.partRegistry.Get("root"), new Coordinate(0,0));
        Ship2.AddPart(verse.registry.partRegistry.Get("basic_hull"), new Coordinate(0, 1));

        GUIController.Find("player_panel").SetParameters(new object[] { Player });
        GUIController.Find("ship_view").SetParameters(new object[] { Ship });

        // Test warp and merchant stuff
        Ship.WarpTo(Enumerable.ToList(verse.Galaxy.systems.Values)[0].planets[0]);
        Ship.Location.AddNpc(new Merchant("Jim"));
        (Ship.Location.GetNpc(0) as Merchant).Inventory.Funds = 2000;
        GUIController.Find("merchant_view").SetParameters(new object[] { Ship.Location.GetNpc(0) });
    }
    
    public Dictionary<Ship, ShipComponent> shipComponents = new Dictionary<Ship, ShipComponent>();
    
    void AddShip(Ship ship) {

        GameObject o = new GameObject();
        o.name = "Ship_" + ship.Name;
        ShipComponent shipComponent = o.AddComponent<ShipComponent>();
        shipComponents.Add(ship, shipComponent);

        GameObject tv = new GameObject();
        tv.transform.SetParent(tacticalViewObject.transform);
        ShipTacticalViewComponent shipTacticalViewComponent = tv.AddComponent<ShipTacticalViewComponent>();
        tv.name = "Ship_" + ship.Name + "_tv";

        ship.register(shipComponent);
        ship.register(shipTacticalViewComponent);
    }

    internal ShipComponent GetShipComponent(Ship ship) {
        return shipComponents[ship];
    }
}
