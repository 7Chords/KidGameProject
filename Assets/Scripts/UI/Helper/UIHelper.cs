using KidGame.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KidGame.UI
{
    /// <summary>
    /// ������Ϣ��
    /// </summary>
    public class BubbleInfo : IComparable<BubbleInfo>
    {

        public ControlType controlType; //��ҵĿ������ͣ��ֱ�/����...

        public List<InputActionType> actionTypeList;//Ҫ��ʾ�Ľ�����λ�����б�
        public GameObject creator { get; set; }//���ݵĴ�����
        public GameObject player { get; set; }
        public string content { get; set; } //��ʾ��������ʾ�ı�

        public string itemName;//�������ݵ�������

        public BubbleInfo(ControlType controlType, List<InputActionType> actionTypeList, GameObject creator, GameObject player, string content, string itemName)
        {
            this.controlType = controlType;
            this.actionTypeList = actionTypeList;
            this.creator = creator;
            this.player = player;
            this.content = content;
            this.itemName = itemName;
        }

        public int CompareTo(BubbleInfo other)
        {
            float myDist = Vector3.Distance(creator.transform.position, player.transform.position);
            float otherDist = Vector3.Distance(other.creator.transform.position, other.player.transform.position);
            if (myDist <= otherDist) return -1;
            else return 1;
        }
    }



    /// <summary>
    /// ����һЩС�Ķ�����СUI
    /// </summary>
    public class UIHelper : Singleton<UIHelper>
    {
        public GameObject BubblePrefab;
        private GameObject currentBubble;
        private BubbleInfo currentBubbleInfo;
        public List<BubbleInfo> bubbleInfoList;

        public GameObject TipPrefab;
        private void Start()
        {
            Init();
        }
        public void Init()
        {
            bubbleInfoList = new List<BubbleInfo>();
        }
        public void Discard()
        {
            bubbleInfoList.Clear();
            bubbleInfoList = null;
        }

        private void Update()
        {
            SortBubbleQueueByDist();
        }

        #region Bubble
        private void RefreshBubble()
        {
            if (bubbleInfoList == null || bubbleInfoList.Count == 0) return;

            BubbleInfo tmpBubbleInfo = bubbleInfoList[0];
            if (currentBubbleInfo != null && tmpBubbleInfo.creator == currentBubbleInfo.creator) return;
            if (currentBubble != null)
            {
                DestroyBubble();
            }
            currentBubbleInfo = tmpBubbleInfo;
            currentBubble = Instantiate(BubblePrefab);

            UIBubbleItem bubbleItem = currentBubble.GetComponent<UIBubbleItem>();
            currentBubble.transform.SetParent(transform);

            string keyStr = "";
            if (bubbleItem != null)
            {
                for (int i = 0; i < tmpBubbleInfo.actionTypeList.Count; i++)
                {
                    keyStr += PlayerUtil.Instance.GetSettingKey(tmpBubbleInfo.actionTypeList[i], tmpBubbleInfo.controlType);
                    if (i < tmpBubbleInfo.actionTypeList.Count - 1) keyStr += "/";
                }
                bubbleItem.Init(tmpBubbleInfo, keyStr);
            }
        }

        public void DestroyBubble()
        {
            if (currentBubble)
            {
                currentBubble.GetComponent<UIBubbleItem>().DestoryBubble();
            }
        }

        public void AddBubbleInfoToList(BubbleInfo info)
        {
            if (bubbleInfoList == null) return;
            if (bubbleInfoList.Find(x => x.creator == info.creator) != null) return;
            bubbleInfoList.Add(info);
            RefreshBubble();
        }

        public void RemoveBubbleInfoFromList(GameObject creator)
        {
            if (bubbleInfoList == null) return;
            BubbleInfo tmpInfo = bubbleInfoList.Find(x => x.creator == creator);
            if (tmpInfo == null) return;
            bubbleInfoList.Remove(tmpInfo);
            if (bubbleInfoList.Count == 0)
            {
                currentBubbleInfo = null;
                DestroyBubble();
            }
            RefreshBubble();
        }

        public void SortBubbleQueueByDist()
        {
            if (bubbleInfoList == null) return;
            bubbleInfoList.Sort();
        }

        #endregion

        #region Tip
        public void ShowTip(string content,GameObject creator)
        {
            GameObject tipGO = Instantiate(TipPrefab);
            tipGO.transform.SetParent(transform);
            tipGO.GetComponent<UITipItem>().Init(creator, content);
        }
        #endregion

        #region Util
        public Vector2 ScreenPointToUIPoint(RectTransform rt, Vector2 screenPoint)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rt.parent as RectTransform,
                screenPoint,
                null,
                out localPoint
            );
            return localPoint;
        }

        #endregion
    }
}
