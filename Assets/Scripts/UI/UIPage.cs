using System.Collections.Generic;
using UnityEngine;
using System;

namespace MJUtilities.UI
{
    public abstract class UIPage : MonoBehaviour
    {
        public PageType pageType;
        
        public abstract void OnSetValues();
        public abstract void OnAwake();
    }
}

[Serializable]
public enum PageType
{
    HUD,
    Home
}