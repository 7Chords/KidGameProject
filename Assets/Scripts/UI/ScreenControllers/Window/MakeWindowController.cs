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
    public class ShowRecipeSignal : ASignal<RecipeData>
    {
        
    }
    public class MakeWindowController : WindowController
    {
        public UICircularScrollView firstSelection;
        public UICircularScrollView secondSelection;

        public RecipeListData recipes;
        private List<RecipeData> trapRecipes = new List<RecipeData>();
        private List<RecipeData> equipRecipes = new List<RecipeData>();
        private Dictionary<int, List<RecipeData>> recipesDictionary = new Dictionary<int, List<RecipeData>>();
        private string[] names = new[] { "陷阱", "手持道具" };
        
        public Image recipeImage;
        public TextMeshProUGUI recipeNameText;
        public TextMeshProUGUI recipeDescriptionText;
        
        private List<MakeMenuButton> makeMenuButtons = new List<MakeMenuButton>();
        
        protected override void Awake()
        {
            base.Awake();
            
            Signals.Get<ShowRecipeSignal>().AddListener(ShowRecipe);
        }

        protected override void OnPropertiesSet()
        {
            recipeImage = transform.Find("Right/Icon").GetComponent<Image>();
            
            //根据so初始化列表和其中按钮即可
            //scrollView.Init(_materialSlotInfos.Count + _trapSlotInfos.Count,OnCellUpdate,null);
            
            //todo获取最新数据
            
            SetRecipeList();
            firstSelection.Init(2, (cell, index) =>
            {
                MakeMenuButton button = cell.GetComponent<MakeMenuButton>();
                button.InitFirstSelect(secondSelection,names[index-1],recipesDictionary[index-1]);
                makeMenuButtons.Add(button);
            });
            makeMenuButtons[0].InitSecondSelect();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Signals.Get<ShowRecipeSignal>().RemoveListener(ShowRecipe);
        }

        private void SetRecipeList()
        {
            recipesDictionary.Clear();
            trapRecipes.Clear();
            equipRecipes.Clear();
            foreach (var recipe in recipes.recipes)
            {
                if (recipe.recipeType == RecipeType.Trap)
                {
                    trapRecipes.Add(recipe);
                }else if (recipe.recipeType == RecipeType.Equip)
                {
                    equipRecipes.Add(recipe);
                }
            }
            recipesDictionary.Add(0,trapRecipes);
            recipesDictionary.Add(1,equipRecipes);
        }

        private void ShowRecipe(RecipeData recipe)
        {
            recipeNameText.text = recipe.trapData.name;
            if (recipe.recipeType == RecipeType.Trap)
                recipeImage.sprite = recipe.trapData.trapIcon;
            else if (recipe.recipeType == RecipeType.Equip)
                recipeImage.sprite = recipe.trapData.trapIcon;//todo
            recipeDescriptionText.text = recipe.trapData.trapDesc;
        }
        
    }
}