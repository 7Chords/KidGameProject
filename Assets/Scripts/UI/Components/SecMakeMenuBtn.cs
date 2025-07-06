using System;
using System.Collections;
using System.Collections.Generic;
using KidGame.Core;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace KidGame.UI
{
    public class RefreshSecBtnSignal : ASignal
    {
        
    }
    public class SecMakeMenuBtn : MonoBehaviour
    {
        private RecipeData recipeData;
        public Button makeMenuButton;
        public TextMeshProUGUI makeMenuText;

        private void Awake()
        {
            makeMenuButton = GetComponentInChildren<Button>();
            makeMenuText = GetComponentInChildren<TextMeshProUGUI>();
            makeMenuButton.onClick.AddListener(ShowDetail);
            Signals.Get<RefreshSecBtnSignal>().AddListener(RefreshButton);
        }

        private void OnDestroy()
        {
            Signals.Get<RefreshSecBtnSignal>().RemoveListener(RefreshButton);
        }

        public void InitBtnData(RecipeData recipeData)
        {
            
            this.recipeData = recipeData;
            makeMenuText.text = this.recipeData.trapData.trapName;
            
        }

        public void ShowDetail()
        {
            
            Signals.Get<RefreshSecBtnSignal>().Dispatch();
            Signals.Get<ShowRecipeSignal>().Dispatch(recipeData);
            makeMenuButton.interactable = false;
        }

        public void RefreshButton()
        {
            makeMenuButton.interactable = true;
        }
    }
    
    
}
