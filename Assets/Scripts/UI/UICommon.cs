using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KidGame.UI
{
    /// <summary>
    /// UI通用的一些方法封装
    /// </summary>
    public static class UICommon
    {
        #region RichText

        /// <summary>
        /// 为富文本字符串添加颜色
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string AddColorForRichText(string txt, Color color)
        {
            string richTextColor = "#" + ColorToString(color);
            return string.Format("<color={0}>{1}</color>", richTextColor, txt);//富文本封装
        }

        /// <summary>
        /// 增加斜体标签
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string AddItalicLineTag(string str)
        {
            return $"<i>{str}</i>";
        }

        /// <summary>
        /// 添加粗体标签
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string AddBoldTag(string str)
        {
            return $"<b>{str}</b>";
        }

        #endregion


        /// <summary>
        /// 添加某部分字符串的响应方法 比如描述中的buff 可以鼠标进到buff字符串上查看详细描述
        /// </summary>
        public static void AddEventToStrInText(Text text, string str, EventTriggerType triggerType, UnityAction<BaseEventData> callBack)
        {
            GameObject buttonObj = null;
            Transform tran = text.transform.Find("TextStrEventMask");
            if (tran != null) buttonObj = tran.gameObject;
            if (buttonObj == null)
            {
                // 创建透明按钮
                buttonObj = new GameObject("TextStrEventMask");
                buttonObj.transform.SetParent(text.transform, false);
                //// 添加透明图像（按钮需要Image组件才能工作）
                Image image = buttonObj.AddComponent<Image>();
                image.color = new Color(0, 0, 0, 0); // 完全透明
            }

            AddUIListener(buttonObj, triggerType, callBack);

            // 强制刷新Text Generator
            TextGenerator textGen = text.cachedTextGenerator;
            TextGenerationSettings generationSettings = text.GetGenerationSettings(text.rectTransform.rect.size);
            textGen.Populate(text.text, generationSettings);
            // 获取文本中特定字符串的位置信息
            int strIndex = text.text.IndexOf(str);
            if (strIndex < 0) return;

            Vector2 startPos = textGen.GetCharactersArray()[strIndex].cursorPos;
            Vector2 endPos = textGen.GetCharactersArray()[strIndex + str.Length].cursorPos;

            // 计算按钮位置和大小
            float width = (endPos.x - startPos.x);
            float height = textGen.fontSizeUsedForBestFit * 1.2f; // 稍微比字体大一点

            // 设置按钮位置和大小
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0, 1);
            buttonRect.anchorMax = new Vector2(0, 1);
            buttonRect.pivot = new Vector2(0, 1);
            buttonRect.localPosition = new Vector2(startPos.x, startPos.y);
            buttonRect.sizeDelta = new Vector2(width, height);
        }



        /// <summary>
        /// 为组件添加监听事件
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="eventTriggerType"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static void AddUIListener(GameObject obj, EventTriggerType eventTriggerType, UnityAction<BaseEventData> callback)
        {
            EventTrigger trigger = obj.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = obj.gameObject.AddComponent<EventTrigger>();
            }
            if (trigger.triggers.Count == 0)
            {
                trigger.triggers = new List<EventTrigger.Entry>();
            }

            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = eventTriggerType
            };
            entry.callback.AddListener(callback);

            trigger.triggers.Add(entry);
        }

        /// <summary>
        /// 颜色转字符串
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private static string ColorToString(Color color)
        {
            int r = Mathf.RoundToInt(color.r * 255.0f);
            int g = Mathf.RoundToInt(color.g * 255.0f);
            int b = Mathf.RoundToInt(color.b * 255.0f);
            int a = Mathf.RoundToInt(color.a * 255.0f);
            string hex = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", r, g, b, a);
            return hex;
        }
    }
}

