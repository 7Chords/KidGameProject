using System.Collections.Generic;
using KidGame.Core;
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
        public UICircularScrollView materialScrollView;

        public RecipeListData recipes;
        private List<RecipeData> trapRecipes = new List<RecipeData>();
        private List<RecipeData> equipRecipes = new List<RecipeData>();
        private Dictionary<int, List<RecipeData>> recipesDictionary = new Dictionary<int, List<RecipeData>>();
        private string[] names = new[] { "陷阱", "手持道具" };

        public Image recipeImage;
        public TextMeshProUGUI recipeNameText;
        public TextMeshProUGUI recipeDescriptionText;

        private List<MakeMenuButton> makeMenuButtons = new List<MakeMenuButton>();
        private RecipeData currentRecipe;
        public Button makeButton;
        protected override void Awake()
        {
            base.Awake();

            Signals.Get<ShowRecipeSignal>().AddListener(ShowRecipe);
            makeButton.onClick.AddListener(OnClickMake);
        }

        protected override void OnPropertiesSet()
        {
            recipeImage = transform.Find("Right/Icon").GetComponent<Image>();
            //根据so初始化列表和其中按钮即可
            //scrollView.Init(_materialSlotInfos.Count + _trapSlotInfos.Count,OnCellUpdate,null);

            //todo.获取最新数据

            SetRecipeList();
            firstSelection.Init(2, (cell, index) =>
            {
                MakeMenuButton button = cell.GetComponent<MakeMenuButton>();
                button.InitFirstSelect(secondSelection, names[index - 1], recipesDictionary[index - 1]);
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
                }
                else if (recipe.recipeType == RecipeType.Equip)
                {
                    equipRecipes.Add(recipe);
                }
            }

            recipesDictionary.Add(0, trapRecipes);
            recipesDictionary.Add(1, equipRecipes);
        }

        private void ShowRecipe(RecipeData recipe)
        {
            currentRecipe = recipe;
            recipeNameText.text = recipe.trapData.trapName;
            if (recipe.recipeType == RecipeType.Trap)
                recipeImage.sprite = Resources.Load<Sprite>(recipe.trapData.trapIconPath);
            else if (recipe.recipeType == RecipeType.Equip)
                recipeImage.sprite = Resources.Load<Sprite>(recipe.trapData.trapIconPath);//todo
            recipeDescriptionText.text = recipe.trapData.trapDesc;

            materialScrollView.Init(recipe.materialDatas.Count, (cell, index) =>
            {
                CellUI cellUI = cell.GetComponent<CellUI>();
                if (index - 1 < recipe.materialDatas.Count)
                {
                    cellUI.SetUIWithMaterial(recipe.materialDatas[index - 1]);
                }
            });
        }

        private void OnClickMake()
        {
            foreach(var slotInfo in currentRecipe.materialDatas)
            {
                //先检查 不能直接try delete 不然检查到一半才发现发现不够了 之前删掉了回不来了
                if(!PlayerBag.Instance.CheckItemEnoughInCombineBag(slotInfo.ItemData.Id, slotInfo.Amount))
                {
                    UIHelper.Instance.ShowOneTipWithParent(new TipInfo("材料不足！", gameObject), transform);
                    break;
                }
            }

            foreach (var slotInfo in currentRecipe.materialDatas)
            {
                PlayerBag.Instance.DeleteItemInCombineBag(slotInfo.ItemData.Id, slotInfo.Amount);
            }
            // todo.填写具体陷阱数量
            PlayerBag.Instance.AddItemToCombineBag(currentRecipe.trapData.id, UseItemType.trap,1);
            UIHelper.Instance.ShowOneTipWithParent(new TipInfo("打造了" + currentRecipe.trapData.trapName, gameObject), transform);
        }
    }
}