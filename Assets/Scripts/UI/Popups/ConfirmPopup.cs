using UnityEngine;
using UnityEngine.UI;

namespace MJUtilities.UI
{
    public class ConfirmPopup : UIPopup
    {
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;

        public bool loadScene;

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

        private void OnConfirmClicked()
        {
            if(loadScene){
                // GameManager.instance.LoadScene();
                }
            //else
                //GameManager.instance.LoadScenario();
        } 
        
        private void OnCancelClicked()
        {
            CloseThisPopup();
        }

    }
}