using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MJUtilities.UI
{
    public abstract class UIPopup : MonoBehaviour
    {
        [Header("Type")]
        public PopupType popupType;
        
        [Header("Can Be Null")]
        [SerializeField] private Button backButton;

        public abstract void OnAwake();
        public abstract void OnSetValues();

        protected virtual void OnEnable()
        {

        }

        public void CloseThisPopup()
        {
            //gameObject.SetActive(false);
            UIManager.instance.ClosePopup(this);
        }
        

        public void BackButtonConfig()
        {
            if (backButton == null)
                return; // the header says "Can Be Null"

            // remove only our own handler, so other listeners (click sound) stay
            backButton.onClick.RemoveListener(BackButtonClicked);
            backButton.onClick.AddListener(BackButtonClicked);
        }
        
        public virtual void BackButtonClicked()
        {
            CloseThisPopup();
        }
    }
    public enum PopupType
    {
        Win,
        Lose,
        Pause,
        Countdown,
        Setting,
        Upgrade,
        Confirm,
        Language,
        // appended at the end on purpose: existing popups keep their serialized values
        Shop,
        Customize
    }
}