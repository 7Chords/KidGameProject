using Utils;
using UnityEngine;
using UnityEngine.UI;

namespace UIFramework.Examples
{
    public class PopupExampleWindowController : WindowController
    {
        [SerializeField] 
        private Image exampleImage = null;
        
        private int currentPopupExample;
        private Color originalColor;

        /// <summary>
        /// 本身根上是继承MonoBehaviour的，所以Awake这类的函数都能用
        /// </summary>
        protected override void Awake() {
            base.Awake();
            originalColor = exampleImage.color;
        }

        public void UI_ShowPopup() {
            Signals.Get<ShowConfirmationPopupSignal>().Dispatch(GetPopupData());
        }

        private ConfirmationPopupProperties GetPopupData() {
            ConfirmationPopupProperties testProps = null;
            
            switch (currentPopupExample) {
                case 0:
                    testProps = new ConfirmationPopupProperties("标题测试", 
                        "点下按钮试试",
                        "明白");
                    break;
                case 1:
                    testProps = new ConfirmationPopupProperties("小测试", 
                        "随便选个颜色",
                        "蓝色", OnBlueSelected, 
                        "红色", OnRedSelected);
                    break;
                case 2:
                    testProps = new ConfirmationPopupProperties("恢复颜色",
                        "恢复成原来的颜色吧",
                        "好的", OnRevertColors);
                    break;
                case 3:
                    testProps = new ConfirmationPopupProperties("重置", 
                        "随便测一下", "重置");
                    break;
            }

            currentPopupExample++;
            if (currentPopupExample > 3) {
                currentPopupExample = 0;
            }

            return testProps;
        }

        private void OnRevertColors() {
            exampleImage.color = originalColor;
        }

        private void OnRedSelected() {
            exampleImage.color = Color.red;
        }

        private void OnBlueSelected() {
            exampleImage.color = Color.blue;
        }
    }
}