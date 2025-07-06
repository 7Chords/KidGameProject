using System;
using System.Collections;
using System.Collections.Generic;
using KidGame.Core;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace KidGame.UI
{
    public class SecMakeMenuBtn : MonoBehaviour
    {
        private RecipeData recipeData;
        private Button makeMenuButton;
        public TextMeshProUGUI makeMenuText;

        private void Awake()
        {
            makeMenuButton = GetComponentInChildren<Button>();
            makeMenuText = GetComponentInChildren<TextMeshProUGUI>();
            makeMenuButton.onClick.AddListener(ShowDetail);
        }
        public void InitBtnData(RecipeData recipeData)
        {
            
            this.recipeData = recipeData;
            makeMenuText.text = this.recipeData.trapData.trapName;
            
        }

        public void ShowDetail()
        {
            
        }
    }
    
    
}
