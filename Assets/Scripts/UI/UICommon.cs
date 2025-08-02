using System.Collections.Generic;
using KidGame.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KidGame.UI
{
    /// <summary>
    /// UIͨ�õ�һЩ������װ
    /// </summary>
    public static class UICommon
    {
        #region RichText

        /// <summary>
        /// Ϊ���ı��ַ���������ɫ
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string AddColorForRichText(string txt, Color color)
        {
            string richTextColor = "#" + ColorToString(color);
            return string.Format("<color={0}>{1}</color>", richTextColor, txt);//���ı���װ
        }

        /// <summary>
        /// ����б���ǩ
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string AddItalicLineTag(string str)
        {
            return $"<i>{str}</i>";
        }

        /// <summary>
        /// ���Ӵ����ǩ
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string AddBoldTag(string str)
        {
            return $"<b>{str}</b>";
        }

        #endregion


        /// <summary>
        /// ����ĳ�����ַ�������Ӧ���� ���������е�buff ����������buff�ַ����ϲ鿴��ϸ����
        /// </summary>
        public static void AddEventToStrInText(Text text, string str, EventTriggerType triggerType, UnityAction<BaseEventData> callBack)
        {
            GameObject buttonObj = null;
            Transform tran = text.transform.Find("TextStrEventMask");
            if (tran != null) buttonObj = tran.gameObject;
            if (buttonObj == null)
            {
                // ����͸����ť
                buttonObj = new GameObject("TextStrEventMask");
                buttonObj.transform.SetParent(text.transform, false);
                //// ����͸��ͼ�񣨰�ť��ҪImage������ܹ�����
                Image image = buttonObj.AddComponent<Image>();
                image.color = new Color(0, 0, 0, 0); // ��ȫ͸��
            }

            AddUIListener(buttonObj, triggerType, callBack);

            // ǿ��ˢ��Text Generator
            TextGenerator textGen = text.cachedTextGenerator;
            TextGenerationSettings generationSettings = text.GetGenerationSettings(text.rectTransform.rect.size);
            textGen.Populate(text.text, generationSettings);
            // ��ȡ�ı����ض��ַ�����λ����Ϣ
            int strIndex = text.text.IndexOf(str);
            if (strIndex < 0) return;

            Vector2 startPos = textGen.GetCharactersArray()[strIndex].cursorPos;
            Vector2 endPos = textGen.GetCharactersArray()[strIndex + str.Length].cursorPos;

            // ���㰴ťλ�úʹ�С
            float width = (endPos.x - startPos.x);
            float height = textGen.fontSizeUsedForBestFit * 1.2f; // ��΢�������һ��

            // ���ð�ťλ�úʹ�С
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0, 1);
            buttonRect.anchorMax = new Vector2(0, 1);
            buttonRect.pivot = new Vector2(0, 1);
            buttonRect.localPosition = new Vector2(startPos.x, startPos.y);
            buttonRect.sizeDelta = new Vector2(width, height);
        }



        /// <summary>
        /// Ϊ������Ӽ����¼�
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
        /// ��ɫת�ַ���
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

        /// <summary>
        /// MaterailItem转ISlotInfo
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="eventTriggerType"></param>
        /// <param name="callback"></param>
        public static List<ISlotInfo> MaterialItemsToSlotInfo(IList<MaterialItem> materials)
        {
            List<ISlotInfo> result = new List<ISlotInfo>();
            foreach (var material in materials)
            {
                result.Add(new MaterialSlotInfo(material.data,material.amount));
            }
            return result;
        }
    }
}

