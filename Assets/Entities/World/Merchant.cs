using MoonSharp.Interpreter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[MoonSharpUserData]
public class Merchant : Entity<Merchant>, INpc {

    public class Item {
        public Part part;
        public int quantity = 5;
        public int price = 100;
    }

    Dictionary<string, List<Item>> inventory = new Dictionary<string, List<Item>>();

    List<Item> basket = new List<Item>();

    public Merchant(string name) {
        Name = name;
        inventory["addons"] = new List<Item> {
            new Item{part=Verse.Instance.registry.partRegistry.Get("turret")},
            new Item{part=Verse.Instance.registry.partRegistry.Get("heater")},
            new Item{part=Verse.Instance.registry.partRegistry.Get("pressurizer")},
            new Item{part=Verse.Instance.registry.partRegistry.Get("heater") },

        };
        inventory["hulls"] = new List<Item> {
            new Item{part=Verse.Instance.registry.partRegistry.Get("basic_hull") },
        };
    }

    public override Merchant Clone() {
        throw new System.NotImplementedException();
    }

    public override void Update() {
        throw new System.NotImplementedException();
    }
}
