using KidGame.Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KidGame.UI
{

    /// <summary>
    /// 鼠标预览检测器，用于游戏物体检测鼠标移入移出并显示预览
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class MousePreviewDetector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private IMouseShowPreview previewTarget;
        private GameObject currentPreview;
        private bool isPreviewShowing = false;

        private void Start()
        {
            // 获取实现了IMouseShowPreview接口的组件
            previewTarget = GetComponent<IMouseShowPreview>();

            if (previewTarget == null)
            {
                Debug.LogError($"MousePreviewDetector需要一个实现了IMouseShowPreview接口的组件，但在{gameObject.name}上未找到。");
                enabled = false;
            }
        }

        // 鼠标移入事件
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (previewTarget == null || isPreviewShowing) return;
            isPreviewShowing = true;
            previewTarget.ShowPreview();
        }

        // 鼠标移出事件
        public void OnPointerExit(PointerEventData eventData)
        {
            if (!isPreviewShowing) return;
            isPreviewShowing = false;
            previewTarget.HidePreview();
        }



        //// 隐藏预览
        //private void HidePreview()
        //{
        //    // 根据预览类型调用相应的隐藏方法
        //    UIHelper.Instance.RemoveBubbleInfoFromList(gameObject);

        //    // 可以添加其他类型预览的隐藏逻辑

        //    isPreviewShowing = false;
        //}
    }
}
