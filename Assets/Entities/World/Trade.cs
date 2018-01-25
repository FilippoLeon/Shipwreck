using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;

namespace Assets.Entities.World {
    [MoonSharpUserData]
    public class Trade {
        private Inventory _i1, _i2;

        public Dictionary<Part, Inventory.Item> basket = new Dictionary<Part, Inventory.Item>();

        public Inventory.Item Basket(Part p) {
            return basket[p];
        }

        public bool InBasket(Part p) {
            return basket.ContainsKey(p);
        }

        public int AmountInBasket(Part p) {
            if(InBasket(p)) {
                return basket[p].quantity;
            }
            return 0;
        }

        public Trade(Inventory inventory, Inventory other) {
            _i2 = inventory; // destination (negative means add here)
            _i1 = other; // source (negative means remove from here)
        }

        public Inventory GetBuyInventory() {
            return _i1;
        }
        public Inventory GetSellInventory() {
            return _i2;
        }

        public void AddToBasket(Part part, int quantity) {
            if(quantity == 0) {
                return;
            }

            int inBasket = 0;
            if( basket.ContainsKey((part)) ) {
                inBasket = basket[part].quantity;
            }
            Inventory.Item i1p = _i1.Get(part);
            int sourceInventoryQuantity = 0;
            if ( i1p != null ) {
                sourceInventoryQuantity = -i1p.quantity;
            }
            Inventory.Item i2p = _i2.Get(part);
            int destinationInventoryQuantity = 0;
            if (i2p != null) {
                destinationInventoryQuantity = i2p.quantity;
            }

            int newQuantity = Math.Min(Math.Max(inBasket + quantity, sourceInventoryQuantity), destinationInventoryQuantity);
            if (inBasket == 0) {
                basket[part] = new Inventory.Item() { part = part, quantity = newQuantity };
            } else {
                basket[part].quantity = newQuantity;
            }
        }

        public int GetCost() {
            int totalCost = 0;
            foreach (Inventory.Item item in basket.Values) {
                totalCost += item.quantity * item.part.Price;
            }
            return totalCost;
        }

        public void Reset() {
            basket.Clear();
        }

        public bool FinalizeTransaction() {
            int totalCost = GetCost();
            if( (totalCost > 0 && _i1.Funds < totalCost) || (totalCost < 0 && -_i2.Funds > totalCost) ) {
                return false;
            } else {
                _i2.Funds += totalCost;
                _i1.Funds -= totalCost;
            }

            foreach (Inventory.Item item in basket.Values) {
                if(item.quantity > 0) {
                    Part p = _i2.Remove(item.part, item.quantity);
                    _i1.Add(p, item.quantity);
                } else if(item.quantity < 0) {
                    Part p = _i1.Remove(item.part, -item.quantity);
                    _i2.Add(p, -item.quantity);
                }
            }

            Reset();

            return true;
        }
    }
}