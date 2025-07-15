using KidGame.Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KidGame.UI
{

    /// <summary>
    /// ���Ԥ���������������Ϸ��������������Ƴ�����ʾԤ��
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class MousePreviewDetector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private IMouseShowDetail previewTarget;
        private GameObject currentPreview;
        private bool isPreviewShowing = false;

        private void Start()
        {
            // ��ȡʵ����IMouseShowPreview�ӿڵ����
            previewTarget = GetComponent<IMouseShowDetail>();

            if (previewTarget == null)
            {
                Debug.LogError($"MousePreviewDetector��Ҫһ��ʵ����IMouseShowPreview�ӿڵ����������{gameObject.name}��δ�ҵ���");
                enabled = false;
            }
        }

        // ��������¼�
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (previewTarget == null || isPreviewShowing) return;
            isPreviewShowing = true;
            previewTarget.ShowDetail();
        }

        // ����Ƴ��¼�
        public void OnPointerExit(PointerEventData eventData)
        {
            if (!isPreviewShowing) return;
            isPreviewShowing = false;
            previewTarget.HideDetail();
        }



        //// ����Ԥ��
        //private void HidePreview()
        //{
        //    // ����Ԥ�����͵�����Ӧ�����ط���
        //    UIHelper.Instance.RemoveBubbleInfoFromList(gameObject);

        //    // ���������������Ԥ���������߼�

        //    isPreviewShowing = false;
        //}
    }
}
