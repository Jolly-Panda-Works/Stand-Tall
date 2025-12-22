using JollyPanda.LastFlag.Handlers;
using UnityEngine;

namespace USound
{
    internal class SoundEventsListener : MonoBehaviour
    {
        private void OnEnable()
        {
            Informant.OnChangeUIPage += ChangeBackgroundSound;
        }

        private void OnDisable()
        {
            Informant.OnChangeUIPage -= ChangeBackgroundSound;
        }

        private void ChangeBackgroundSound(PageType type)
        {
            switch (type)
            {
                case PageType.HUD:
                    SoundManager.Instance.PlayGameBackground();
                    break;
                case PageType.Home:
                    SoundManager.Instance.PlayHomeBackground();
                    break;
            }
        }

    }
}
