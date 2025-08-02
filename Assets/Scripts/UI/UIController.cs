using System;
using System.Collections.Generic;
using KidGame.Core;
using Utils;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace KidGame.UI.Game
{
    public class UIController : SingletonPersistent<UIController>
    {
        [SerializeField] private UISettings defaultUISettings = null;
        [SerializeField] private FakePlayerData fakePlayerData = null;
        [SerializeField] private Camera cam = null;
        [SerializeField] private Transform transformToFollow = null;
        public Canvas uiCanvas =  null;

        private UIFrame uiFrame;

        protected override void Awake()
        {
            base.Awake();
            
            uiFrame = defaultUISettings.CreateUIInstance();
            uiCanvas = uiFrame.GetComponent<Canvas>();
            Signals.Get<StartGameSignal>().AddListener(OnToGameScene);
            Signals.Get<ToSettingsWindowSignal>().AddListener(OnToSettingsWindow);
            Signals.Get<ContinueGameSignal>().AddListener(OnToSaveWindow);
            
            Signals.Get<GameFailSignal>().AddListener(OnShowFailPanel);
            Signals.Get<OpenPauseSignal>().AddListener(OnShowPausePanel);

            Signals.Get<GotoSelectedPanel>().AddListener(OnNavigateToWindow);
            Signals.Get<ShowConfirmationPopupSignal>().AddListener(OnShowConfirmationPopup);
        }

        private void OnDestroy()
        {
            Signals.Get<StartGameSignal>().RemoveListener(OnToGameScene);
            Signals.Get<ToSettingsWindowSignal>().RemoveListener(OnToSettingsWindow);
            
            Signals.Get<GameFailSignal>().RemoveListener(OnShowFailPanel);
            Signals.Get<OpenPauseSignal>().RemoveListener(OnShowPausePanel);
            
            Signals.Get<GotoSelectedPanel>().RemoveListener(OnNavigateToWindow);
            Signals.Get<ShowConfirmationPopupSignal>().RemoveListener(OnShowConfirmationPopup);
        }

        private void Start()
        {
            // 开始游戏立即打开开始面板
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                uiFrame.OpenWindow(ScreenIds.StartGameWindow);
            }
            else
            {
                UICameraBindingVertexCamera();
            }
            
        }

        #region 开关

        private void OnToGameScene()
        {
            // uiFrame.ShowPanel(ScreenIds.GamePlayPanel);
        }

        private void OnToSettingsWindow()
        {
            uiFrame.OpenWindow(ScreenIds.SettingWindow);
        }

        private void OnToSaveWindow()
        {
            uiFrame.OpenWindow(ScreenIds.SaveWindow);
        }

        private void OnNavigateToWindow(string windowId)
        {
            uiFrame.CloseCurrentWindow();

            switch (windowId)
            {
                case ScreenIds.SettingWindow:
                    uiFrame.OpenWindow(windowId);
                    break;
                case ScreenIds.BackpackWindow:
                    uiFrame.OpenWindow(windowId);
                    break;
                default:
                    uiFrame.OpenWindow(windowId);
                    break;
            }
        }

        public void CloseCurrentWindow()
        {
            uiFrame.CloseCurrentWindow();
        }

        private void OnShowConfirmationPopup(ConfirmationPopupProperties popupPayload)
        {
            uiFrame.OpenWindow(ScreenIds.ConfirmationPopup, popupPayload);
        }
        
        private void OnShowPausePanel()
        {
            uiFrame.OpenWindow(ScreenIds.PauseWindow);
        }

        private void OnShowFailPanel()
        {
            uiFrame.OpenWindow(ScreenIds.GameFailWindow);
        }

        public void ShowPopItemContainerWindow(List<MaterialItem> materialHoldList,int rowCount,int columnCount)
        {
            PopItemContainerProp popItemContainerProp = new PopItemContainerProp();
            
            popItemContainerProp.items = UICommon.MaterialItemsToSlotInfo(materialHoldList);
            popItemContainerProp.row = rowCount;
            popItemContainerProp.column = columnCount;
            uiFrame.OpenWindow(ScreenIds.BackpackWindow,new BackpackProp(popItemContainerProp.items));
            uiFrame.OpenWindow(ScreenIds.PopItemContainerWindow,popItemContainerProp);
        }
        
        #endregion

        #region 转场

        public TransitionPanelController GetTransitionPanel()
        {
            uiFrame.ShowPanel(ScreenIds.TransitionPanel);
            return uiFrame.GetPanel<TransitionPanelController>(ScreenIds.TransitionPanel);
        }

        public void HideTransitionPanel()
        {
            uiFrame.HidePanel(ScreenIds.TransitionPanel);
        }
        
        public void UICameraBindingVertexCamera()
        {
            Camera vertexCamera = GameObject.Find("ViewCamera").GetComponent<Camera>();
            UniversalAdditionalCameraData baseCameraData = 
                vertexCamera.GetUniversalAdditionalCameraData();
            UniversalAdditionalCameraData UICameraData = 
                uiFrame.UICamera.GetUniversalAdditionalCameraData();
            UICameraData.renderType = CameraRenderType.Overlay;
            // 将Overlay相机加入堆叠
            baseCameraData.cameraStack.Add(uiFrame.UICamera);
        }
        #endregion
    }
}