﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace GUI {
    public interface IWidget : IDisposable {
        void SetParent(IWidget parent);
        string Id { set; get; }
        GameObject GameObject { set; get; }
        IWidget Root { set; get; }
        void Update(object[] args);
        void SetParameters(object[] args);

        Dictionary<string, int> GetArgDict();
        object[] GetArgs();

        IEmitter GetArgument(string name);

        System.Action ChangeArguments { set; get; }

        GameObject GetContentGameObject();
        string GetToolTipText();

        void Show();

        void Hide();

        void Toggle();
    }
}