using System;
using System.Collections;
using System.Collections.Generic;
using KidGame.Core;
using KidGame.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace KidGame.UI
{
    public class RefreshFirBtnSignal : ASignal
    {
        
    }
    public class MakeMenuButton : MonoBehaviour
    {
        private UICircularScrollView secondSelectScroll;
        private List<RecipeData> recipes;
        public Button makeMenuButton;
        public TextMeshProUGUI makeMenuText;
        private List<SecMakeMenuBtn> makeMenuBtns = new List<SecMakeMenuBtn>();
        private void Awake()
        {
            makeMenuButton = transform.GetChild(0).GetComponent<Button>();
            makeMenuText = GetComponentInChildren<TextMeshProUGUI>();
            makeMenuButton.onClick.AddListener(InitSecondSelect);
            Signals.Get<RefreshFirBtnSignal>().AddListener(RefreshButton);
        }

        private void OnDestroy()
        {
            Signals.Get<RefreshFirBtnSignal>().RemoveListener(RefreshButton);
        }

        public void InitFirstSelect(UICircularScrollView secondSelectScroll,string name,List<RecipeData> recipes)
        {
            this.secondSelectScroll = secondSelectScroll;
            makeMenuText.text = name;
            this.recipes = recipes;
            
        }

        public void InitSecondSelect()
        {
            Signals.Get<RefreshFirBtnSignal>().Dispatch();
            makeMenuButton.interactable = false;
            secondSelectScroll.Init(recipes.Count, (cell, index) =>
            {
                SecMakeMenuBtn button = cell.GetComponent<SecMakeMenuBtn>();
                button.InitBtnData(recipes[index-1]);
                makeMenuBtns.Add(button);
            });
            makeMenuBtns[0].ShowDetail();
        }

        public void RefreshButton()
        {
            makeMenuButton.interactable = true;
        }
    }
}
