using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class ShipController : MonoBehaviour {

    public Ship Ship { set; get; }
    public Player Player { set; get; }

    void Start () {
        Player = new Player(GetComponent<MainController>().Verse);
        Player.Funds = 1000;

        Ship = new Ship(GetComponent<MainController>().Verse);
        Ship.Name = "Serenity 2.0";

        GameObject o = new GameObject();
        o.name = "Ship";
        ShipComponent shipComponent = o.AddComponent<ShipComponent>();

        Ship.register(shipComponent);

        GUIController.Find("player_panel").SetParameters(new object[] { Player });
        GUIController.Find("ship_view").SetParameters(new object[] { Ship });
    }
}
