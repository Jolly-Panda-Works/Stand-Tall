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
            backButton.onClick.RemoveAllListeners();
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
        Language
    }
}