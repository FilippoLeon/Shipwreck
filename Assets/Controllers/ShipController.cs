using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;

public class ShipController : MonoBehaviour {

    public Ship Ship { set; get; }
    public Player Player { set; get; }

    public GameObject tacticalViewObject;

    void Start () {
        tacticalViewObject = new GameObject("Tactical view");

        Player = new Player(GetComponent<MainController>().Verse);
        Player.Funds = 1000;

        Ship = new Ship(GetComponent<MainController>().Verse);
        Ship.AddPart(GetComponent<MainController>().Verse.registry.partRegistry.Get("root"), new Coordinate(0, 0));
        Ship.Name = "Serenity 2.0";

        AddShip(Ship);

        GetComponent<MainController>().Verse.NextShip();

        Ship Ship2 = new Ship(GetComponent<MainController>().Verse);
        Ship2.Name = "Serenity 1.0";

        AddShip(Ship2);
        Ship2.Position = new Vector2(10, 10);
        Ship2.AddPart(GetComponent<MainController>().Verse.registry.partRegistry.Get("root"), new Coordinate(0,0));
        Ship2.AddPart(GetComponent<MainController>().Verse.registry.partRegistry.Get("basic_hull"), new Coordinate(0, 1));

        GUIController.Find("player_panel").SetParameters(new object[] { Player });
        GUIController.Find("ship_view").SetParameters(new object[] { Ship });
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
