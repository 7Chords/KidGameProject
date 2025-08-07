using UnityEngine.UIElements;

namespace KidGame.Editor
{
    public class F2MConfigItem 
    {

        private F2MConfigItemStyle itemStyle;

        private TextField MaterialIDField;
        private FloatField MaterialAmountMaxField;
        private FloatField MaterialAmountMinField;
        private FloatField MaterialSpawnChanceField;

        private string materialIDStr;
        private float materialAmountMaxStr;
        private float materialAmountMinStr;
        private float materialSpawnChanceStr;

        public void Init(VisualElement parent)
        {

            itemStyle = new F2MConfigItemStyle();
            itemStyle.Init(parent);

            MaterialIDField = parent.Q<TextField>(nameof(MaterialIDField));
            MaterialIDField.RegisterValueChangedCallback(MaterialIDFieldValueChanged);

            MaterialAmountMaxField = parent.Q<FloatField>(nameof(MaterialAmountMaxField));
            MaterialAmountMaxField.RegisterValueChangedCallback(MaterialAmountMaxFieldValueChanged);

            MaterialAmountMinField = parent.Q<FloatField>(nameof(MaterialAmountMinField));
            MaterialAmountMinField.RegisterValueChangedCallback(MaterialAmountMinFieldValueChanged);

            MaterialSpawnChanceField = parent.Q<FloatField>(nameof(MaterialSpawnChanceField));
            MaterialSpawnChanceField.RegisterValueChangedCallback(MaterialSpawnChanceFieldValueChanged);

        }
        private void MaterialIDFieldValueChanged(ChangeEvent<string> evt)
        {
            materialIDStr = evt.newValue;
        }
        private void MaterialAmountMaxFieldValueChanged(ChangeEvent<float> evt)
        {
            materialAmountMaxStr = evt.newValue;
        }

        private void MaterialAmountMinFieldValueChanged(ChangeEvent<float> evt)
        {
            materialAmountMinStr = evt.newValue;
        }
        private void MaterialSpawnChanceFieldValueChanged(ChangeEvent<float> evt)
        {
            materialSpawnChanceStr = evt.newValue;
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
