using UnityEngine;
using UnityEngine.UI;

namespace MJUtilities.UI
{
    public class ConfirmPopup : UIPopup
    {
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;

        public override void OnAwake()
        {
            confirmButton.onClick.AddListener(OnConfirmClicked);
            cancelButton.onClick.AddListener(OnCancelClicked);
        }

        private void OnDestroy()
        {
            confirmButton.onClick.RemoveListener(OnConfirmClicked);
            cancelButton.onClick.RemoveListener(OnCancelClicked);
        }

        public override void OnSetValues()
        {
            
        }

        // ConfirmPopup is not used by the current flow. It only closes itself;
        // subscribe to Confirmed to run a custom action when it is needed.
        public event System.Action Confirmed;

        private void OnConfirmClicked()
        {
            Confirmed?.Invoke();
            CloseThisPopup();
        }
        
        private void OnCancelClicked()
        {
            CloseThisPopup();
        }

    }
}