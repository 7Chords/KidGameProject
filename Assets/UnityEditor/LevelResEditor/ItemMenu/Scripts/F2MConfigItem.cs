using System;
using UnityEngine.UIElements;

namespace KidGame.Editor
{
    public class F2MConfigItem 
    {

        private F2MConfigItemStyle itemStyle;

        private TextField MaterialIDField;
        private IntegerField MaterialAmountMaxField;
        private IntegerField MaterialAmountMinField;
        private FloatField MaterialSpawnChanceField;
        private Button DeleteConfigButton;

        private string materialIDStr;
        private int materialAmountMaxStr;
        private int materialAmountMinStr;
        private float materialSpawnChanceStr;

        public string MaterialID => materialIDStr;
        public int MaterialAmountMax => materialAmountMaxStr;
        public int MaterialAmountMin => materialAmountMinStr;
        public float MaterialSpawnChance => materialSpawnChanceStr;


        private Action onDestory;

        public void Init(VisualElement parent ,Action onDestory)
        {

            itemStyle = new F2MConfigItemStyle();
            itemStyle.Init(parent);

            MaterialIDField = parent.Q<TextField>(nameof(MaterialIDField));
            MaterialIDField.RegisterValueChangedCallback(MaterialIDFieldValueChanged);

            MaterialAmountMaxField = parent.Q<IntegerField>(nameof(MaterialAmountMaxField));
            MaterialAmountMaxField.RegisterValueChangedCallback(MaterialAmountMaxFieldValueChanged);

            MaterialAmountMinField = parent.Q<IntegerField>(nameof(MaterialAmountMinField));
            MaterialAmountMinField.RegisterValueChangedCallback(MaterialAmountMinFieldValueChanged);

            MaterialSpawnChanceField = parent.Q<FloatField>(nameof(MaterialSpawnChanceField));
            MaterialSpawnChanceField.RegisterValueChangedCallback(MaterialSpawnChanceFieldValueChanged);

            DeleteConfigButton = itemStyle.SelfRoot.Q<Button>(nameof(DeleteConfigButton));
            DeleteConfigButton.clicked += OnDeleteConfigButtonClicked;

            this.onDestory = onDestory;
        }

        public void SetInfo(string materialId,int materialSpawnMaxAmount, int materialSpawnMinAmount,float materialSpawnChance)
        {
            materialIDStr = materialId;
            materialAmountMaxStr = materialSpawnMaxAmount;
            materialAmountMinStr = materialSpawnMinAmount;
            materialSpawnChanceStr = materialSpawnChance;
            MaterialIDField.value = materialIDStr;
            MaterialAmountMaxField.value = materialAmountMaxStr;
            MaterialAmountMinField.value = materialAmountMinStr;
            MaterialSpawnChanceField.value = materialSpawnChanceStr;
        }


        private void MaterialIDFieldValueChanged(ChangeEvent<string> evt)
        {
            materialIDStr = evt.newValue;
        }
        private void MaterialAmountMaxFieldValueChanged(ChangeEvent<int> evt)
        {
            materialAmountMaxStr = evt.newValue;
        }

        private void MaterialAmountMinFieldValueChanged(ChangeEvent<int> evt)
        {
            materialAmountMinStr = evt.newValue;
        }
        private void MaterialSpawnChanceFieldValueChanged(ChangeEvent<float> evt)
        {
            materialSpawnChanceStr = evt.newValue;
        }

        private void OnDeleteConfigButtonClicked()
        {
            Destory();
            onDestory?.Invoke();
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
