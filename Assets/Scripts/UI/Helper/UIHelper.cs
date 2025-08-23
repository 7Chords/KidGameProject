using KidGame.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using KidGame.UI.Game;
using TMPro;
using UnityEngine;


namespace KidGame.UI
{

    public enum FixedUIPosType
    {
        Left,Right,Top,Bottom,Center
    }

    /// <summary>
    /// 气泡信息类
    /// </summary>
    public class BubbleInfo : IComparable<BubbleInfo>
    {

        public ControlType controlType; //玩家的控制类型：手柄/键盘...

        public List<InputActionType> actionTypeList;//要提示的交互键位类型列表
        public GameObject creator { get; set; }//气泡的创建者
        public GameObject player { get; set; }
        public string content { get; set; } //显示交互的提示文本

        public string itemName;//产生气泡的物体名

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
    /// 提示信息类
    /// </summary>
    public class TipInfo
    {
        public string content;
        public Vector3 creatorPos;
        public float showTime; // 显示时长

        public TipInfo(string content, Vector3 creatorPos, float showTime = 0.75f)
        {
            this.content = content;
            this.creatorPos = creatorPos;
            this.showTime = showTime;
        }
    }

    /// <summary>
    /// 提示图标类
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
    /// 管理一些小的独立的小UI
    /// </summary>
    public class UIHelper : Singleton<UIHelper>
    {
        [Header("Bubble")]
        //气泡相关字段
        public GameObject BubblePrefab;
        private GameObject currentBubble;
        private BubbleInfo currentBubbleInfo;
        public List<BubbleInfo> bubbleInfoList;

        [Header("Tip")]
        // 文字提示相关字段
        public GameObject TipPrefab;
        public float ShowTipInterval;
        public Queue<TipInfo> TipQueue;
        //private bool isShowingTip;
        public float QuickReplaceThreshold = 0.3f; // 快速替换阈值
        private GameObject currentTipGO; // 当前显示的Tip对象
        private float lastTipShowTime; // 上次显示Tip的时间

        [Header("Sign")]
        //图标提示相关字段
        public GameObject SignPrefab;

        [Header("FixedPos")]
        //固定UI位置相关字段
        public GameObject FixedPosTextPrefab;
        public Transform LeftShowPos;
        public Transform RightShowPos;
        public Transform TopShowPos;
        public Transform BottomShowPos;
        public Transform CenterShowPos;

        [Header("Progress")]
        //圆形进度条相关字段
        public GameObject CircleProgressPrefab;
        private GameObject currentProgressGO;

        private void Start()
        {
            //todo
            Init();
        }
        public void Init()
        {
            bubbleInfoList = new List<BubbleInfo>();
            TipQueue = new Queue<TipInfo>();
            
            detailPanel = UIController.Instance.uiCanvas.transform.Find("PriorityPanelLayer/DetailPanel").gameObject;
            detailText = detailPanel.transform.Find("DetailText").GetComponent<TextMeshProUGUI>();
            detailPanel.SetActive(false);

            this.OnUpdate(SortBubbleQueueByDist);
        }
        public void Discard()
        {
            bubbleInfoList.Clear();
            bubbleInfoList = null;
            this.RemoveUpdate(SortBubbleQueueByDist);

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
            if (bubbleInfoList == null || bubbleInfoList.Count == 0) return;
            BubbleInfo oldNearestBubble = bubbleInfoList[0];
            bubbleInfoList.Sort();
            if(oldNearestBubble != bubbleInfoList[0])
            {
                RefreshBubble();
            }
        }

        #endregion

        #region Tip

        /// <summary>
        /// 直接展示一个提示信息
        /// </summary>
        /// <param name="tipInfo"></param>
        public void ShowOneTip(TipInfo tipInfo)
        {
            float currentTime = Time.time;
            float timeSinceLastTip = currentTime - lastTipShowTime;

            // 如果在快速替换阈值内且有当前显示的Tip，则替换内容
            if (timeSinceLastTip <= QuickReplaceThreshold &&
                //isShowingTip &&
                currentTipGO != null)
            {
                // 更新当前显示的Tip内容
                UITipItem tipItem = currentTipGO.GetComponent<UITipItem>();
                if (tipItem != null)
                {
                    tipItem.ResetTip(tipInfo.creatorPos, tipInfo.content);

                    lastTipShowTime = currentTime;
                    return;
                }
            }
            TipQueue.Enqueue(tipInfo);
            //if (isShowingTip) return;
            StartCoroutine(ShowTipCoroutine());
        }

        private IEnumerator ShowTipCoroutine()
        {
            //isShowingTip = true;
            TipInfo tmpInfo = null;
            while(TipQueue.Count > 0)
            {
                tmpInfo = TipQueue.Dequeue();
                lastTipShowTime = Time.time;
                currentTipGO = Instantiate(TipPrefab);
                currentTipGO.transform.SetParent(transform);
                currentTipGO.GetComponent<UITipItem>().Init(tmpInfo.creatorPos, tmpInfo.content, tmpInfo.showTime);
                yield return new WaitForSeconds(ShowTipInterval);
            }
            //isShowingTip = false;
        }
        
        //public void ShowOneTipWithParent(TipInfo tipInfo,Transform parent)
        //{
        //    GameObject tipGO = Instantiate(TipPrefab);
        //    tipGO.transform.SetParent(parent);
        //    tipGO.GetComponent<UITipItem>().InitWithRectTransform(tipInfo.creatorPos, tipInfo.content);

        //    DOVirtual.DelayedCall(tipInfo.showTime, () =>
        //    {
        //        tipGO.transform.SetParent(transform);
        //        Destroy(tipGO);
        //    });
        //}

        
        #endregion

        #region Sign

        /// <summary>
        /// 直接展示一个图标信息
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

        #region FixedPosUI
        /// <summary>
        /// 展示一个屏幕侧边的UI文本（用于分数等）
        /// </summary>
        public void ShowOneFixedPosUIText(FixedUIPosType posType,string content,float showTime)
        {
            GameObject fixedPosTextGO = Instantiate(FixedPosTextPrefab);
            fixedPosTextGO.transform.SetParent(transform);
            fixedPosTextGO.GetComponent<UIFixedPosTextItem>().Init(posType,content);
            Destroy(fixedPosTextGO, showTime);
        }

        #endregion

        #region Detail
        
        private GameObject detailPanel;
        private TextMeshProUGUI detailText;
        /// <summary>
        /// 展示物品详情
        /// </summary>
        /// <param name="cellUI"></param>
        public void ShowItemDetail(CellUI cellUI)
        {
            detailPanel.transform.SetAsLastSibling();
            detailPanel.transform.position = cellUI.detailPoint.position;
            detailText.text = cellUI.detailText;
            detailPanel.SetActive(true);
        }
        
        public void HideItemDetail()
        {
            detailPanel.SetActive(false);
        }
        
        #endregion

        #region Progress
        public void ShowCircleProgress(GameObject creator,float duration)
        {
            GameObject circleProgressGO = Instantiate(CircleProgressPrefab);
            circleProgressGO.transform.SetParent(transform);
            circleProgressGO.GetComponent<UICommonProgressItem>().Init(creator,duration);
            currentProgressGO = circleProgressGO;
            Destroy(currentProgressGO, duration);
        }

        public void DestoryCurrentCircleProgress()
        {
            if(currentProgressGO != null)//todo：和destory延迟销毁存在冲突问题？
            {
                Destroy(currentProgressGO);
            }
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
