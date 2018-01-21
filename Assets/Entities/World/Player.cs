using Assets.Entities.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Player : Entity<Player> {
    Verse verse;

    private Inventory _inventory = new Inventory();
    
    public int Funds {
        set {
            Emit("OnFundsChanged");
            _inventory.Funds = value;
        }
        get {
            return _inventory.Funds;
        }
    }

    public Player(Verse Verse) {
        verse = Verse;
        verse.AddPlayer(this);
    }
    
    public override Player Clone() {
        throw new NotImplementedException();
    }

    public override void Update() {

    }
}
