using UnityEngine;

namespace MJUtilities.UI
{
    public abstract class UIElements : MonoBehaviour
    {
        protected abstract void Reset();
        public abstract void OnAwake();
        public abstract void OnSetValues();
    }
}

