using KidGame.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.UI
{
    /// <summary>
    /// 气泡信息
    /// </summary>
    public class BubbleInfo : IComparable<BubbleInfo>
    {

        public ControlType controlType; //控制类型：键盘/手柄

        public string inputActionName; //提示操作的inputaction的名字，如”Interaction“
        public GameObject creator { get; set; }//气泡创造者，如陷阱
        public GameObject player { get; set; }//玩家
        public string content { get; set; } //文本

        public BubbleInfo(ControlType controlType, string inputActionName, GameObject creator, GameObject player, string content)
        {
            this.controlType = controlType;
            this.inputActionName = inputActionName;
            this.creator = creator;
            this.player = player;
            this.content = content;
        }

        public int CompareTo(BubbleInfo other)//自定义排序规则：根据和玩家的距离排序
        {
            float myDist = Vector3.Distance(creator.transform.position, player.transform.position);
            float otherDist = Vector3.Distance(other.creator.transform.position, other.player.transform.position);
            if (myDist <= otherDist) return -1;
            else return 1;
        }
    }

    /// <summary>
    /// 气泡管理器
    /// </summary>
    public class BubbleManager : Singleton<BubbleManager>
    {
        public GameObject BubblePrefab; // 气泡的预制体

        private GameObject currentBubble;//当前显示的气泡
        private BubbleInfo currentBubbleInfo;

        /// <summary>
        /// 气泡信息队列 里面装着所有可以显示的气泡 
        /// 但是由于同时只会显示一个气泡且只对它交互有效 所以显示的都是距离玩家最近的
        /// </summary>
        public List<BubbleInfo> bubbleInfoList;


        //测试用
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

        /// <summary>
        /// 创建气泡 需要完善 是否有多个气泡同时存在的情况？
        /// </summary>
        /// <param name="info"></param>
        private void RefreshBubble()
        {
            if (bubbleInfoList == null || bubbleInfoList.Count == 0) return;

            //取出距离玩家最近的bubbleinfo
            BubbleInfo tmpBubbleInfo = bubbleInfoList[0];
            //现在还是这个气泡最近
            if (currentBubbleInfo != null && tmpBubbleInfo.creator == currentBubbleInfo.creator) return;
            // 销毁已有的气泡
            if (currentBubble != null)
            {
                DestroyBubble();
            }
            //更新当前气泡信息
            currentBubbleInfo = tmpBubbleInfo;
            // 实例化气泡
            currentBubble = Instantiate(BubblePrefab);

            UIBubbleItem bubbleItem = currentBubble.GetComponent<UIBubbleItem>();
            currentBubble.transform.parent = transform;

            string keyStr = "";//按键提示文本
            if (bubbleItem != null)
            {
                keyStr = PlayerUtil.Instance.GetSettingKey(tmpBubbleInfo.inputActionName, tmpBubbleInfo.controlType);
                bubbleItem.Init(tmpBubbleInfo, keyStr); // 初始化气泡
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
            if (bubbleInfoList.Find(x=>x.creator == info.creator) != null) return;
            //单纯加入就行 不需要排序 因为排序已经会每帧调用（优化？）
            bubbleInfoList.Add(info);
            RefreshBubble();
        }

        public void RemoveBubbleInfoFromList(BubbleInfo info)
        {
            if (bubbleInfoList == null) return;
            BubbleInfo tmpInfo = bubbleInfoList.Find(x => x.creator == info.creator);
            if (tmpInfo == null) return;
            bubbleInfoList.Remove(tmpInfo);
            if(bubbleInfoList.Count == 0)
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
    }
}