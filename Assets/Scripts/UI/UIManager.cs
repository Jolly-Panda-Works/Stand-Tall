using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using UnityEngine.Serialization;
using JollyPanda.LastFlag.Handlers;

namespace MJUtilities.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager instance;
        public bool _DontDestroyOnLoad = true;

        [Header("Panels & Popups")] 
        public List<UIPage> pageList = new List<UIPage>();
        public List<UIPopup> popupList = new List<UIPopup>();

        private PageType currentPageType;
        private bool hasActivePopup;
        

        private void Awake()
        {
            if (_DontDestroyOnLoad)
            {
                if (!instance)
                {
                    instance = this;
                    DontDestroyOnLoad(this);
                }
                else
                    Destroy(gameObject);
            }
            else
            {
                if (!instance)
                    instance = this;
            }

            foreach (var page in pageList)
            {
                page.OnAwake();
            }

            foreach (var popup in popupList)
            {
                popup.OnAwake();
            }

            CloseEverything();
        }

        private void Start()
        {
            //OpenPage(PageType.HUD);
            OpenPage(PageType.Home);
        }

        public void CloseEverything()
        {
            foreach (var popup in popupList)
            {
                popup.gameObject.SetActive(false);
            }

            foreach (var page in pageList)
            {
                page.gameObject.SetActive(false);
            }
            
            hasActivePopup = false;
        }

        public void ClosePopups()
        {
            foreach (var popup in popupList)
            {
                popup.gameObject.SetActive(false);
            }
        }

        public void UpdateUI()
        {
            foreach (var page in pageList)
            {
                page.OnSetValues();
            }
        }

        public UIPage GetPage(PageType pageType)
        {
            foreach (var page in pageList)
            {
                if (page.pageType == pageType)
                    return page;
            }

            return null;
        }

        public UIPopup GetPopup(PopupType popupType)
        {
            foreach (var popup in popupList)
            {
                if (popup.popupType == popupType)
                    return popup;
            }

            return null;
        }

        public void OpenPage(PageType pageType)
        {
            //Debug.Log("OpenPage: " + pageType);

            foreach (var page in pageList)
            {
                if (page.pageType != pageType)
                    continue;

                currentPageType = pageType;

                page.gameObject.SetActive(true);
                page.OnSetValues();
            }

            Informant.OnChangePage(pageType);
        }
        public void OpenPage(PageType pageType, bool closeLastPage)
        {
            ClosePage(currentPageType);
            OpenPage(pageType);
        }

        public void ClosePage(PageType pageType)
        {
            foreach (var page in pageList)
            {
                if (page.pageType != pageType)
                    continue;

                page.gameObject.SetActive(false);
                page.OnSetValues();
            }
        }

        public UIPage GetCurrentPage()
        {
            return GetPage(currentPageType);
        }

        public bool HasActivePopup() => hasActivePopup;

        public void OpenPopup(PopupType popupType)
        {
            foreach (var popup in popupList)
            {
                if (popup.popupType != popupType)
                    continue;

                hasActivePopup = true;
                popup.gameObject.SetActive(true);
                popup.OnSetValues();
            }
        }

        public void ClosePopup(PopupType popupType)
        {
            foreach (var popup in popupList)
            {
                if (popup.popupType != popupType)
                    continue;

                hasActivePopup = false;
                popup.gameObject.SetActive(false);
            }
        }
        
        public void ClosePopup(UIPopup popup)
        {
            hasActivePopup = false;
            popup.gameObject.SetActive(false);
        }
    }
}