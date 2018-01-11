using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[MoonSharpUserData]
public class Registry {
    public PartRegistry partRegistry;

    public Registry() {
        partRegistry = new PartRegistry();
    }
}
