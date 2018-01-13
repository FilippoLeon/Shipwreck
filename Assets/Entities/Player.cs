using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Player : Entity<Player> {
    Verse verse;

    int funds;
    public int Funds {
        set {
            Emit("OnFundsChanged");
            funds = value;
        }
        get {
            return funds;
        }
    }

    public Player(Verse Verse) {
        verse = Verse;
        verse.AddPlayer(this);
    }

    List<Part> inventory = new List<Part>();

    public override Player Clone() {
        throw new NotImplementedException();
    }

    public override void Update() {

    }
}
