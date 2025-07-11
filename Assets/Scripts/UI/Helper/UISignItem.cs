using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KidGame.UI
{
    public class UISignItem : MonoBehaviour
    {
        public Image IconImg;
        private RectTransform rectTran;
        private Vector2 screenPos;

        private GameObject creator;

        private void Awake()
        {
            rectTran = GetComponent<RectTransform>();
        }
        public void Init(GameObject creator, string iconPath)
        {
            this.creator = creator;
            IconImg.sprite = Resources.Load<Sprite>(iconPath);
            // 计算位置
            screenPos = Camera.main.WorldToScreenPoint(creator.transform.position) * (1080f / 300);
            rectTran.localPosition = UIHelper.Instance.ScreenPointToUIPoint(rectTran, screenPos);

        }
        private void Update()
        {
            if (creator != null)
            {
                screenPos = Camera.main.WorldToScreenPoint(creator.transform.position) * (1080f / 300);
                Vector2 newPos = UIHelper.Instance.ScreenPointToUIPoint(rectTran, screenPos);
                rectTran.localPosition = new Vector2(newPos.x, newPos.y + 150f); // 保持Y轴偏移
            }
        }
    }
}
