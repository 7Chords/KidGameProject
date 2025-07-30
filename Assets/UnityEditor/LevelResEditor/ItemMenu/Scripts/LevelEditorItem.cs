using UnityEngine.UIElements;

namespace KidGame.Editor
{
    public class LevelEditorItem
    {
        private LevelEditorItemStyle itemStyle;

        private string itemName;
        public string ItemName => itemName;

        public void Init(VisualElement parent, string name)
        {
            itemName = name;

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