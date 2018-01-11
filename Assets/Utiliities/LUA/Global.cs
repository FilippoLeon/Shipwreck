using System;

namespace LUA {
    internal class Global {
        public string typeName;
        public Type type;

        public Global(string typeName, Type type) {
            this.typeName = typeName;
            this.type = type;
        }
    }
}