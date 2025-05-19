using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Utils;

namespace UIFramework.Examples
{
    public class BackPagGameController : MonoBehaviour
    {
        [SerializeField] private UISettings defaultUISettings = null;
        [SerializeField] private FakePlayerData fakePlayerData = null;
        [SerializeField] private Camera cam = null;
        [SerializeField] private Transform transformToFollow = null;
        private UIFrame uiFrame;
        
        private void Awake() {
            uiFrame = defaultUISettings.CreateUIInstance();
            // Signals.Get<StartDemoSignal>().AddListener(OnStartDemo);
            // Signals.Get<NavigateToWindowSignal>().AddListener(OnNavigateToWindow);
            // Signals.Get<ShowConfirmationPopupSignal>().AddListener(OnShowConfirmationPopup);
        }
    
    }
}

