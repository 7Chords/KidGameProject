using KidGame.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    /// ��ʾ��Ϣ��
    /// </summary>
    public class TipInfo
    {
        public string content;
        public GameObject creator;
        public float showTime; // ��ʾʱ��

        public TipInfo(string content, GameObject creator, float showTime = 0.75f)
        {
            this.content = content;
            this.creator = creator;
            this.showTime = showTime;
        }
    }

    /// <summary>
    /// ��ʾͼ����
    /// </summary>
    public class SignInfo 
    {
        public string signIconPath;
        public GameObject creator;
        public float showTime;

        public SignInfo(string signIconPath, GameObject creator, float showTime = 0.75f)
        {
            this.signIconPath = signIconPath;
            this.creator = creator;
            this.showTime = showTime;
        }
    }


    /// <summary>
    /// ����һЩС�Ķ�����СUI
    /// </summary>
    public class UIHelper : Singleton<UIHelper>
    {
        //��������ֶ�
        public GameObject BubblePrefab;
        private GameObject currentBubble;
        private BubbleInfo currentBubbleInfo;
        public List<BubbleInfo> bubbleInfoList;


        // ������ʾ����ֶ�
        public GameObject TipPrefab;
        private Queue<TipInfo> tipQueue = new Queue<TipInfo>();
        private bool isShowingTip = false;
        private Coroutine showTipCoroutine;

        //ͼ����ʾ����ֶ�
        public GameObject SignPrefab;

        //�̶�UIλ������ֶ�
        public GameObject SideTextPrefab;
        public Transform SideShowPos;
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

        /// <summary>
        /// �����ʾ������
        /// </summary>
        /// <param name="content">��ʾ����</param>
        /// <param name="creator">������</param>
        /// <param name="showTime">��ʾʱ��</param>
        public void ShowTipByQueue(TipInfo info,float intervalTime = 0.5f)
        {
            tipQueue.Enqueue(info);

            // ���û��������ʾ����ʾ��������ʼ��ʾ
            if (!isShowingTip && showTipCoroutine == null)
            {
                showTipCoroutine = StartCoroutine(ProcessTipQueue(intervalTime));
            }
        }

        /// <summary>
        /// ������ʾ���е�Э��
        /// </summary>
        private IEnumerator ProcessTipQueue(float intervalTime)
        {
            isShowingTip = true;

            while (tipQueue.Count > 0)
            {
                TipInfo tipInfo = tipQueue.Dequeue();
                ShowOneTip(tipInfo);
                // ����������һ����ʾ���ȴ����ʱ��
                yield return new WaitForSeconds(intervalTime);
            }

            isShowingTip = false;
            showTipCoroutine = null;
        }

        /// <summary>
        /// ֱ��չʾһ����ʾ��Ϣ
        /// </summary>
        /// <param name="tipInfo"></param>
        public void ShowOneTip(TipInfo tipInfo)
        {
            GameObject tipGO = Instantiate(TipPrefab);
            tipGO.transform.SetParent(transform);
            tipGO.GetComponent<UITipItem>().Init(tipInfo.creator, tipInfo.content);
            Destroy(tipGO, tipInfo.showTime);
        }
        
        public void ShowOneTipWithParent(TipInfo tipInfo,Transform parent)
        {
            GameObject tipGO = Instantiate(TipPrefab);
            tipGO.transform.SetParent(parent);
            tipGO.GetComponent<UITipItem>().InitWithRectTransform(tipInfo.creator, tipInfo.content);

            DOVirtual.DelayedCall(tipInfo.showTime, () =>
            {
                tipGO.transform.SetParent(transform);
                Destroy(tipGO);
            });
        }

        
        /// <summary>
        /// �����ʾ����
        /// </summary>
        public void ClearTipQueue()
        {
            tipQueue.Clear();

            if (showTipCoroutine != null)
            {
                StopCoroutine(showTipCoroutine);
                showTipCoroutine = null;
            }

            isShowingTip = false;
        }
        #endregion

        #region Sign

        /// <summary>
        /// ֱ��չʾһ��ͼ����Ϣ
        /// </summary>
        /// <param name="signInfo"></param>
        public void ShowOneSign(SignInfo signInfo)
        {
            GameObject signGO = Instantiate(SignPrefab);
            signGO.transform.SetParent(transform);
            signGO.GetComponent<UISignItem>().Init(signInfo.creator, signInfo.signIconPath);
            Destroy(signGO, signInfo.showTime);
        }
        #endregion

        #region SideText
        /// <summary>
        /// չʾһ����Ļ��ߵ�UI�ı������ڷ����ȣ�
        /// </summary>
        public void ShowOneSildUIText(string content,float showTime)
        {
            GameObject sideTextGO = Instantiate(SideTextPrefab);
            sideTextGO.transform.SetParent(transform);
            sideTextGO.GetComponent<UISideTextItem>().Init(content);
            Destroy(sideTextGO, showTime);
        }

        #endregion

        #region Detail

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
