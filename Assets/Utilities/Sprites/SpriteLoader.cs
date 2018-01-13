using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class SpriteLoader {
    static public SpriteLoader Instance = null;

    string spritesFolder = "Sprites";

    public Vector2 defaultSize = new Vector2(64, 64);
    public Vector2 defaultPivot = new Vector2(0.5f, 0.5f);

    static public int multiplierX, multiplierY;

    public Texture2D placeholderTexture;
    public Sprite placeHolder;

    Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
    Dictionary<string, Dictionary<string, Sprite>> categories = new Dictionary<string, Dictionary<string, Sprite>>();

    public class SpriteData {
        public string name;
        public Rect rect;
        public Vector2 pivot;

        public SpriteData() { }

        public SpriteData(Rect rect, Vector2 pivot) {
            this.rect = rect;
            this.pivot = pivot;
        }

    }

    public SpriteLoader(string spritesFolder) {
        if( Instance == null ) {
            Instance = this;
        } else {
            Debug.LogError("Only one instance of 'SpriteUtilities' is allowed.");
        }
        this.spritesFolder = spritesFolder;
        
        try {
            DirectoryInfo dir = new DirectoryInfo(Path.Combine(Application.streamingAssetsPath, spritesFolder));

            FileInfo[] fileInfo = dir.GetFiles("*.png");
            foreach (FileInfo file in fileInfo) {
                loadSpriteSheet(file);
            }
        } catch(DirectoryNotFoundException) {
            Debug.LogError("Invalid sprites directory, no Sprite loaded.");
        }

        buildPlaceholderSprite();
    }

    /// <summary>
    /// Look up for sprite categories, return wanted sprite if available.
    /// </summary>
    /// <param name="category"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public Sprite tryLoadSprite(string category, string id) {
        if (categories.ContainsKey(category) && categories[category].ContainsKey(id))
            return categories[category][id];
        return placeHolder;
    }
    public Sprite GetSprite(string name) {
        if (name != null && sprites.ContainsKey(name) && sprites[name] != null) {
            return sprites[name];
        } else {
            return placeHolder;
        }
    }

    internal Sprite Load(SpriteInfo spriteInfo, IEmitter obj = null) {
        if( spriteInfo == null ) {
            Debug.LogError("null SpriteInfo.");
            return placeHolder;
        }
        if (spriteInfo.category == null) { return GetSprite(spriteInfo.GetId(obj)); }
        return tryLoadSprite(spriteInfo.category, spriteInfo.GetId(obj));
    }

    void loadSpriteSheet(FileInfo path) {
        string[] split = path.FullName.Split('.');
        string pathXml = string.Join(".", split, 0, split.Length - 1) + ".xml";
        string fileName = path.Name.Split('.')[0];

        Debug.Log(string.Format("Loading \"{0}\" ({1})...", path, fileName));

        byte[] imageData = File.ReadAllBytes(path.FullName);

        Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);

        if (tex.LoadImage(imageData)) {
            tex.filterMode = FilterMode.Point;
        } else {
            Debug.LogError("Cannot load texture!");
        }

        Debug.Assert(tex != null, "Texture can't be null, invalid sheet map?");
        
        if (File.Exists(pathXml)) {
            XmlReaderSettings settings = new XmlReaderSettings();
            XmlReader reader = XmlReader.Create(pathXml, settings);

            loadSprites(reader, tex);
        } else {
            SpriteData data = new SpriteData(new Rect(Vector2.zero, defaultSize), defaultPivot);
            data.name = fileName;
            sprites[data.name] = Sprite.Create(tex, data.rect, data.pivot);
            return;
        }
    }

    private void loadSprites(XmlReader reader, Texture2D tex) {
        reader.MoveToContent();

        Vector2 dP = defaultPivot;
        Vector2 dS = defaultSize;
        if (reader.GetAttribute("defaultSize") != null) {
            string[] size = reader.GetAttribute("defaultSize").Split(',');
            dS = new Vector2(Convert.ToSingle(size[0]), Convert.ToSingle(size[1]));
        }
        multiplierX = (int)dS.x;
        multiplierY = (int)dS.y;

        if (reader.GetAttribute("multiplier") != null) {
            string[] size = reader.GetAttribute("multiplier").Split(',');
            multiplierX = (int)Convert.ToSingle(size[0]);
            multiplierY = (int)Convert.ToSingle(size[1]);
        }
        if (reader.GetAttribute("defaultPivot") != null) {
            string[] pivot = reader.GetAttribute("defaultPivot").Split(',');
            dP = new Vector2(Convert.ToSingle(pivot[0]), Convert.ToSingle(pivot[1]));
        }

        int defaultPpu = 32;
        if (reader.GetAttribute("defaultPpu") != null) {
            defaultPpu = Convert.ToInt32(reader.GetAttribute("defaultPpu"));
        }

        int count = 0;
        while (reader.Read()) {
            XmlNodeType nodeType = reader.NodeType;
            switch (nodeType) {
                case XmlNodeType.Element:
                    Debug.Assert(reader.Name == "Sprite");
                    Vector2 pos;
                    if (reader.GetAttribute("start") != null) {
                        string[] start = reader.GetAttribute("start").Split(',');
                        pos = new Vector2(Convert.ToSingle(start[0]) * multiplierX, Convert.ToSingle(start[1]) * multiplierY);
                    } else {
                        pos = new Vector2(0, 0);
                    }
                    Vector2 dS1 = dS;
                    if (reader.GetAttribute("size") != null) {
                        string[] size = reader.GetAttribute("size").Split(',');
                        dS1 = new Vector2(Convert.ToSingle(size[0]), Convert.ToSingle(size[1]));
                    }
                    Vector2 dP1 = dP;
                    if (reader.GetAttribute("pivot") != null) {
                        string[] pivot = reader.GetAttribute("pivot").Split(',');
                        dP1 = new Vector2(Convert.ToSingle(pivot[0]), Convert.ToSingle(pivot[1]));
                    }
                    int ppu = defaultPpu;
                    if (reader.GetAttribute("ppu") != null) {
                        ppu = Convert.ToInt32(reader.GetAttribute("ppu"));
                    }
                    Vector4 border = new Vector4(0, 0, 0, 0);
                    if (reader.GetAttribute("border") != null) {
                        string[] borderStr = reader.GetAttribute("border").Split(',');
                        border = new Vector4(
                            Convert.ToSingle(borderStr[0]), 
                            Convert.ToSingle(borderStr[1]),
                            Convert.ToSingle(borderStr[2]), 
                            Convert.ToSingle(borderStr[3])
                            );
                    }
                    string cat = reader.GetAttribute("category");
                    string name = reader.ReadInnerXml();

                    Debug.Assert(tex != null);
                    sprites[name] = Sprite.Create(tex, new Rect(pos, dS1), dP1, ppu, 0, SpriteMeshType.FullRect, border);
                    count++;
                    if (cat != null) {
                        if (!categories.ContainsKey(cat)) {
                            categories[cat] = new Dictionary<string, Sprite>();
                        }
                        categories[cat][name] = sprites[name];
                    }
                    Debug.Assert(sprites[name] != null);
                    break;
                default:
                    break;
            }
        }
        Debug.Log(string.Format("Loaded {0} sprites...", count));

    }

    private void buildPlaceholderSprite() {

        if (placeHolder != null) {
            return;
        }
        int sizeX = 32, sizeY = 32;
        placeholderTexture = new Texture2D(sizeX, sizeY, TextureFormat.RGBA32, false);
        placeholderTexture.filterMode = FilterMode.Point;

        Color[] colors = new Color[] {
            new Color(247f/255f, 152f/255f, 19f/255f),
            new Color(247f/255f, 19f/255f, 98f/255f)
        };

        for (int i = 0; i < sizeX; i++) {
            for (int j = 0; j < sizeY; j++) {
                int color = 0;
                if ((i / 16 + j / 16) % 2 == 0) color = 1;
                placeholderTexture.SetPixel(i, j, colors[color]);
            }

        }
        int ppu = 32;

        placeholderTexture.Apply();

        placeHolder = Sprite.Create(placeholderTexture,
            new Rect(0f, 0f, sizeX, sizeY),
            new Vector2(0.5f, 0.5f), ppu
        );
    }
}
