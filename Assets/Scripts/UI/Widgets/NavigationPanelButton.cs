using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KidGame.UI.Game
{
    [RequireComponent(typeof(Button))]
    public class NavigationPanelButton : MonoBehaviour
    {
        [SerializeField] private Text buttonLabel = null;
        [SerializeField] private Image icon = null;

        public event Action<NavigationPanelButton> ButtonClicked;

        private NavigationPanelEntry navigationData = null;
        private Button _button = null;

        private Button button
        {
            get
            {
                if (_button == null)
                {
                    _button = GetComponent<Button>();
                }

                return _button;
            }
        }

        public string Target
        {
            get { return navigationData.TargetScreen; }
        }

        public void SetData(NavigationPanelEntry target)
        {
            navigationData = target;
            buttonLabel.text = target.ButtonText;
            icon.sprite = target.Sprite;
        }

        public void SetCurrentNavigationTarget(NavigationPanelButton selectedButton)
        {
            button.interactable = selectedButton != this;
        }

        public void SetCurrentNavigationTarget(string screenId)
        {
            if (navigationData != null)
            {
                button.interactable = navigationData.TargetScreen == screenId;
            }
        }

        public void UI_Click()
        {
            if (ButtonClicked != null)
            {
                Debug.Log("12121");
                ButtonClicked(this);
                UIController.Instance.uiCanvas.GetComponent<GraphicRaycaster>().enabled = true;
            }
        }
    }
}