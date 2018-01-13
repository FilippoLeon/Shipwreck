using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

    class Player : Entity<Player> {
        int funds;

        List<Part> inventory = new List<Part>();

        public override Player Clone() {
            throw new NotImplementedException();
        }
}
