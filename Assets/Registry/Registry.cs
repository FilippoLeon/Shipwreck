using LUA;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[MoonSharpUserData]
public class Registry {
    public PartRegistry partRegistry;

    public EntityRegistry entityRegistry;

    public NameRegistry namesRegistry;

    public Registry() {
        ScriptLoader.LoadScript("Entity", "Entity.lua");

        partRegistry = new PartRegistry();
        entityRegistry = new EntityRegistry();

        namesRegistry = new NameRegistry();
    }
}
