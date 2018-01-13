using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class ShipController : MonoBehaviour {

    public Ship Ship { set; get; }

    void Start () {
        Ship = new Ship(GetComponent<MainController>().Verse);

        GameObject o = new GameObject();
        o.name = "Ship";
        ShipComponent shipComponent = o.AddComponent<ShipComponent>();

        Ship.register(shipComponent);

        GUIController.Find("ship_view").SetParameters(new object[] { Ship });
        //GUIController.Find("ship_view").Update(null);

        Ship.Name = "NewName";
        Ship.Health = 55;
    }

    void Update() {
        Ship.Update();
    }
}
