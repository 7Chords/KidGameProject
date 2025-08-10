using UnityEngine.UIElements;

namespace KidGame.Editor
{
    public class LevelEditorItem
    {
        private LevelEditorItemStyle itemStyle;

        private string itemName;
        public string ItemName => itemName;

        private string id;
        public string Id => id;

        public int rare;

        public int Rare => rare;


        public void Init(VisualElement parent, string id,string name,int rare)
        {
            itemName = name;
            this.id = id;
            this.rare = rare;

            itemStyle = new LevelEditorItemStyle();
            itemStyle.Init(parent, name);

            ((Button)itemStyle.SelfRoot).clicked += OnItemButtonClicked;
        }

        private void OnItemButtonClicked()
        {
            LevelResEditorWindow.Instance.SelectOneItem(this);
        }


        public void Select()
        {
            LevelResEditorWindow.Instance.SetButtonBorderColor(((Button)itemStyle.SelfRoot),
                LevelResEditorWindow.Instance.selectColor);
        }

        public void UnSelect()
        {
            LevelResEditorWindow.Instance.SetButtonBorderColor(((Button)itemStyle.SelfRoot),
                LevelResEditorWindow.Instance.unSelectColor);
        }

        public void Destory()
        {
            if (itemStyle != null)
            {
                itemStyle.Destory();
            }
        }
    }
}