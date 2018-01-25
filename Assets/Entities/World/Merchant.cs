using System.Collections.Generic;
using MoonSharp.Interpreter;

namespace Assets.Entities.World {
    [MoonSharpUserData]
    public class Merchant : Entity<Merchant>, INpc {
        
        /// <summary>
        /// Merchant's inventory with amounts and prices.
        /// </summary>
        private Inventory inventory = new Inventory();

        public Inventory Inventory { get { return inventory; } }

        public Merchant(string name) {
            Name = name;
            inventory = new Inventory {
                new Inventory.Item{part=Verse.Instance.registry.partRegistry.Get("turret")},
                new Inventory.Item{part=Verse.Instance.registry.partRegistry.Get("heater"), quantity = 20},
                new Inventory.Item{part=Verse.Instance.registry.partRegistry.Get("pressurizer"), quantity = 10},
                new Inventory.Item{part=Verse.Instance.registry.partRegistry.Get("heater") },
                new Inventory.Item{part=Verse.Instance.registry.partRegistry.Get("basic_hull") },
            };
        }

        public Trade GetTrade(Inventory other) {
            return new Trade(inventory, other);
        }

        public override Merchant Clone() {
            throw new System.NotImplementedException();
        }

        public override void Update() {
            throw new System.NotImplementedException();
        }
        
    }
}
