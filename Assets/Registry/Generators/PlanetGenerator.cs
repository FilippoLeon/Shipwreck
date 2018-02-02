using System;
using System.Xml;

internal class EntityGenerator : Entity<EntityGenerator> {
    public override string Category {
        get {
            return "Generator";
        }
    }

    public EntityGenerator(XmlReader reader) {
        ReadXml(reader);
    }

    override public void ReadXml(XmlReader reader) {
        base.ReadCurrentElement(reader);

        XmlReader subreader = reader.ReadSubtree();
        while (subreader.Read()) {
            if (subreader.NodeType == XmlNodeType.Element) {
                ReadElement(subreader);
            }
        }
        subreader.Close();
    }

    private string id;
    public override string Id { get { return id; } set { id = value; } }

    public override object GetParameter(string name) {
        throw new System.NotImplementedException();
    }

    public override object SetParameter(string name, object v) {
        throw new System.NotImplementedException();
    }

    public override EntityGenerator Clone() {
        throw new NotImplementedException();
    }

    public override void Update() {
        throw new NotImplementedException();
    }
}