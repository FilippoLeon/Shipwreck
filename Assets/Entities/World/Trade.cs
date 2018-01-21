using System;
using System.Collections.Generic;
using static Assets.Entities.World.Inventory;

namespace Assets.Entities.World {
    public class Trade {
        private Inventory _i1, _i2;

        public Dictionary<Part, Inventory.Item> basket = new Dictionary<Part, Inventory.Item>();

        public Trade(Inventory inventory, Inventory other) {
            _i1 = inventory;
            _i2 = other;
        }

        public void AddToBasket(Part part, int quantity) {
            if(quantity == 0) {
                return;
            }

            int inBasket = 0;
            if( basket.ContainsKey((part)) ) {
                inBasket = basket[part].quantity;
            }
            if (quantity > 0) {
                int sourceInventoryQuantity = -_i1.Get(part).quantity;
                int destinationInventoryQuantity = _i2.Get(part).quantity;

                int newQuantity = Math.Min(Math.Max(inBasket + quantity, sourceInventoryQuantity), destinationInventoryQuantity);
                if (inBasket == 0) {
                    basket[part] = new Inventory.Item() { part = part, quantity = newQuantity };
                } else {
                    basket[part].quantity = newQuantity;
                }
            }
        }

        public void FinalizeTransaction() {
            int totalCost = 0;
            foreach (Item item in basket.Values) {
                totalCost += item.quantity * item.part.Price;
            }
            if( totalCost > 0 && _i1.Funds < totalCost || totalCost < 0 && _i2.Funds > totalCost) {
                return;
            } else {
                _i1.Funds += totalCost;
                _i2.Funds -= totalCost;
            }

            foreach (Item item in basket.Values) {
                if(item.quantity > 0) {
                    Part p = _i1.Remove(item.part, item.quantity);
                    _i2.Add(p, item.quantity);
                } else if(item.quantity < 0) {
                    Part p = _i2.Remove(item.part, -item.quantity);
                    _i1.Add(p, -item.quantity);
                }
            }
        }
    }
}