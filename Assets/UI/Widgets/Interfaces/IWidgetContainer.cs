using System.Collections.Generic;
using UnityEngine.UI;

namespace GUI {
    public interface IWidgetContainer : IWidget {
        IEnumerable<IWidget> Childs { get; }
    }
}