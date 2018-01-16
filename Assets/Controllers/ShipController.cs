using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        Ship.Name = "Serenity 2.0";

        AddShip(Ship);

        GUIController.Find("player_panel").SetParameters(new object[] { Player });
        GUIController.Find("ship_view").SetParameters(new object[] { Ship });
    }

    void AddShip(Ship ship) {

        GameObject o = new GameObject();
        o.name = "Ship_" + ship.Name;
        ShipComponent shipComponent = o.AddComponent<ShipComponent>();


        GameObject tv = new GameObject();
        tv.transform.SetParent(tacticalViewObject.transform);
        ShipTacticalViewComponent shipTacticalViewComponent = tv.AddComponent<ShipTacticalViewComponent>();
        tv.name = "Ship_" + ship.Name + "_tv";

        Ship.register(shipComponent);
        Ship.register(shipTacticalViewComponent);
    }
}
