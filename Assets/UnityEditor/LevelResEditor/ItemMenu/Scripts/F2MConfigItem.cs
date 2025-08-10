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
        private int materialAmountMax;
        private int materialAmountMin;
        private float materialSpawnChance;

        public string MaterialID => materialIDStr;
        public int MaterialAmountMax => materialAmountMax;
        public int MaterialAmountMin => materialAmountMin;
        public float MaterialSpawnChance => materialSpawnChance;


        private Action onDestory;

        public void Init(VisualElement parent ,Action onDestory)
        {

            itemStyle = new F2MConfigItemStyle();
            itemStyle.Init(parent);

            MaterialIDField = itemStyle.SelfRoot.Q<TextField>(nameof(MaterialIDField));
            MaterialIDField.RegisterValueChangedCallback(MaterialIDFieldValueChanged);
            materialIDStr = "T001";

            MaterialAmountMaxField = itemStyle.SelfRoot.Q<IntegerField>(nameof(MaterialAmountMaxField));
            MaterialAmountMaxField.RegisterValueChangedCallback(MaterialAmountMaxFieldValueChanged);
            MaterialAmountMaxField.value = 10;
            materialAmountMax = 10;

            MaterialAmountMinField = itemStyle.SelfRoot.Q<IntegerField>(nameof(MaterialAmountMinField));
            MaterialAmountMinField.RegisterValueChangedCallback(MaterialAmountMinFieldValueChanged);
            MaterialAmountMinField.value = 0;
            materialAmountMin = 0;

            MaterialSpawnChanceField = itemStyle.SelfRoot.Q<FloatField>(nameof(MaterialSpawnChanceField));
            MaterialSpawnChanceField.RegisterValueChangedCallback(MaterialSpawnChanceFieldValueChanged);
            MaterialSpawnChanceField.value = 0.2f;
            materialSpawnChance = 0.2f;

            DeleteConfigButton = itemStyle.SelfRoot.Q<Button>(nameof(DeleteConfigButton));
            DeleteConfigButton.clicked += OnDeleteConfigButtonClicked;

            this.onDestory = onDestory;
        }

        public void SetInfo(string materialId,int materialSpawnMaxAmount, int materialSpawnMinAmount,float materialSpawnChance)
        {
            materialIDStr = materialId;
            materialAmountMax = materialSpawnMaxAmount;
            materialAmountMin = materialSpawnMinAmount;
            this.materialSpawnChance = materialSpawnChance;
            MaterialIDField.value = materialIDStr;
            MaterialAmountMaxField.value = materialAmountMax;
            MaterialAmountMinField.value = materialAmountMin;
            MaterialSpawnChanceField.value = this.materialSpawnChance;
        }


        private void MaterialIDFieldValueChanged(ChangeEvent<string> evt)
        {
            materialIDStr = evt.newValue;
        }
        private void MaterialAmountMaxFieldValueChanged(ChangeEvent<int> evt)
        {
            materialAmountMax = evt.newValue;
        }

        private void MaterialAmountMinFieldValueChanged(ChangeEvent<int> evt)
        {
            materialAmountMin = evt.newValue;
        }
        private void MaterialSpawnChanceFieldValueChanged(ChangeEvent<float> evt)
        {
            materialSpawnChance = evt.newValue;
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
