using MoonSharp.Interpreter;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Entities.World {
    [MoonSharpUserData]
    public class Inventory : IEnumerable {
        public class Item {
            public Part part;
            public int quantity = 1;
        }

        public int Funds {
            get; set;
        }

        private Dictionary<Part, Item> _inventory = new Dictionary<Part, Item>();

        public IEnumerator GetEnumerator() {
            return (IEnumerator) _inventory.GetEnumerator();
        }

        public Dictionary<Part, Item> Get() {
            return _inventory;
        }

        public void Add(Item item) {
            _inventory.Add(item.part, item);
        }

        public Item Get(Part part) {
            if (_inventory.ContainsKey(part)) {
                return _inventory[part];
            }
            return null;
        }

        public bool Contains(Part p) {
            return _inventory.ContainsKey(p);
        }

        /// <summary>
        /// Remove a part from the inventory. Returns the part: if all parts where returned
        /// then the part itself will be returned, otherwise a clone will be returned.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="quantity"></param>
        /// <returns>A new part if all parts are remove, the same part otherwise. Null if you cannot remove the item.</returns>
        internal Part Remove(Part part, int quantity) {
            if( _inventory.ContainsKey(part) && _inventory[part].quantity >= quantity ) {
                _inventory[part].quantity -= quantity;
                if(_inventory[part].quantity == 0) {
                    _inventory.Remove(part);
                    return part;
                } else {
                    return part.Clone();
                }
            }
            return null;
        }

        internal void Add(Part part, int quantity) {
            if( _inventory.ContainsKey(part) ) {
                _inventory[part].quantity += quantity;
            } else {
                _inventory[part] = new Item() { part = part, quantity = quantity };
            }
        }
    }
}