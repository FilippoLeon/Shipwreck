using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;

namespace UI {

    [MoonSharpUserData]
    public class Input {
        public static bool LShift {
            get { return UnityEngine.Input.GetKey(KeyCode.LeftShift); }
        }

    }

}
