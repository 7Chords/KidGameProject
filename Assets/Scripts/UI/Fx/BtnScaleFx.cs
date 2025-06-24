using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using KidGame.Core;
using UnityEngine.EventSystems;

namespace KidGame.UI
{
    public class UIButtonEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        private Vector3 originalScale; // 存储按钮的原始大小
        private float scaleFactor = 1.1f;
        private float duration = 0.1f;

        private void Start()
        {
            originalScale = transform.localScale;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            transform.DOScale(originalScale * scaleFactor, duration).SetUpdate(true).SetEase(Ease.OutBack);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            transform.DOScale(originalScale, duration).SetUpdate(true).SetEase(Ease.InBack);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            AudioManager.Instance.PlaySfx("click");
        }
    }
}