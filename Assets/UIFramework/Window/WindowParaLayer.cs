using UnityEngine;
using System.Collections.Generic;

namespace UIFramework {
    /// <summary>
    /// 这是一个“辅助”层级，以便显示优先级更高的窗口。
    /// 默认情况下，它包含任何标记为弹出窗口的窗口。它由 WindowUILayer 控制
    /// </summary>
    public class WindowParaLayer : MonoBehaviour {
        [SerializeField] 
        private GameObject darkenBgObject = null;

        private List<GameObject> containedScreens = new List<GameObject>();
        
        public void AddScreen(Transform screenRectTransform) {
            screenRectTransform.SetParent(transform, false);
            containedScreens.Add(screenRectTransform.gameObject);
        }

        public void RefreshDarken() {
            for (int i = 0; i < containedScreens.Count; i++) {
                if (containedScreens[i] != null) {
                    if (containedScreens[i].activeSelf) {
                        darkenBgObject.SetActive(true);
                        return;
                    }
                }
            }

            darkenBgObject.SetActive(false);
        }

        public void DarkenBG() {
            darkenBgObject.SetActive(true);
            darkenBgObject.transform.SetAsLastSibling();
        }
    }
}
