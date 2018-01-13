using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using MoonSharp.Interpreter;
using System.IO;

namespace LUA {
    public class ScriptLoader {
        protected static Dictionary<string, Script> script = new Dictionary<string, Script>();

        static private string directory = "Scripts";

        static private List<Global> globals = new List<Global>();

        public ScriptLoader() {
            UserData.RegisterAssembly();

            UserData.RegisterType<Color>();

            LoadScript("test", "test.lua");
            
            DynValue ret = Call("test", "test");
            Debug.Assert(ret.CastToBool() == true);
        }

        public static void LoadScript(string category, string filename) {
            if (!script.ContainsKey(category)) {
                script[category] = new Script();

                script[category].Options.DebugPrint += (string str) => {
                    Debug.Log("LUA: " + str);
                };
                foreach (Global global in globals) {
                    script[category].Globals[global.typeName] = global.type;
                }
            }

            FileInfo info = new FileInfo(
                Path.Combine(Application.streamingAssetsPath,
                    Path.Combine(directory, filename)
                )
            );
            Load(category, info);
        }

        private static void Load(string category, FileInfo file) {
            StreamReader reader = new StreamReader(file.OpenRead());
            string textScript = reader.ReadToEnd();

            DoString(category, textScript);
        }

        public static void DoString(string category, string content) {
            if( !script.ContainsKey(category) ) {
                Debug.LogError(String.Format("Invalid script category '{0}'!", category));
                return;
            }
            try {
                script[category].DoString(content);
            } catch (SyntaxErrorException e) {
                Debug.LogError("LUA error: " + e.DecoratedMessage);
            }
        }

        public static DynValue Call(string category, string function, params object[] args) {
            object func = script[category].Globals[function];
            try {
                return script[category].Call(func, args);
            } catch (ScriptRuntimeException e) {
                Debug.LogError("Script exception: " + e.DecoratedMessage);
                return null;
            } catch (ArgumentException e) {
                Debug.LogError("Script exception while running '" + function + "': " + e.Message);
                return null;
            }
        }

        public static void RegisterPlaceolder(string category, Type type) {
            string typeName = type.ToString().Split('.').Last().Split('+').Last();
            Debug.Log(String.Format("Registering placeholder for type {0} in category {1} as {2}",
                type, category, typeName));

            if (category == null) {
                foreach (Script s in script.Values) {
                    s.Globals[typeName] = type;
                    globals.Add(new Global(typeName, type));
                }
            } else {
                script[category].Globals[typeName] = type;
            }
        }
    }
}
