using System;
using System.Collections;
using System.Collections.Generic;
using KidGame.Core;
using KidGame.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KidGame.UI
{
    public class MakeMenuButton : MonoBehaviour
    {
        private UICircularScrollView secondSelectScroll;
        private List<RecipeData> recipes;
        private Button makeMenuButton;
        public TextMeshProUGUI makeMenuText;

        private void Awake()
        {
            makeMenuButton = GetComponentInChildren<Button>();
            makeMenuText = GetComponentInChildren<TextMeshProUGUI>();
            makeMenuButton.onClick.AddListener(InitSecondSelect);
        }

        public void InitFirstSelect(UICircularScrollView secondSelectScroll,string name,List<RecipeData> recipes)
        {
            this.secondSelectScroll = secondSelectScroll;
            makeMenuText.text = name;
            this.recipes = recipes;
            
        }

        public void InitSecondSelect()
        {
            secondSelectScroll.Init(recipes.Count, (cell, index) =>
            {
                SecMakeMenuBtn button = cell.GetComponent<SecMakeMenuBtn>();
                button.InitBtnData(recipes[index-1]);
                if(index-1==0)button.ShowDetail();
            });
        }
    }
}
