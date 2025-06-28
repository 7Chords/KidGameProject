using KidGame.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.UI
{
    /// <summary>
    /// ������Ϣ
    /// </summary>
    public class BubbleInfo : IComparable<BubbleInfo>
    {

        public ControlType controlType; //�������ͣ�����/�ֱ�

        public List<InputActionType> actionTypeList;
        public GameObject creator { get; set; }//���ݴ����ߣ�������
        public GameObject player { get; set; }//���
        public string content { get; set; } //�ı�

        public BubbleInfo(ControlType controlType, List<InputActionType> actionTypeList, GameObject creator, GameObject player, string content)
        {
            this.controlType = controlType;
            this.actionTypeList = actionTypeList;
            this.creator = creator;
            this.player = player;
            this.content = content;
        }

        public int CompareTo(BubbleInfo other)//�Զ���������򣺸��ݺ���ҵľ�������
        {
            float myDist = Vector3.Distance(creator.transform.position, player.transform.position);
            float otherDist = Vector3.Distance(other.creator.transform.position, other.player.transform.position);
            if (myDist <= otherDist) return -1;
            else return 1;
        }
    }

    /// <summary>
    /// ���ݹ�����
    /// </summary>
    public class BubbleManager : Singleton<BubbleManager>
    {
        public GameObject BubblePrefab; // ���ݵ�Ԥ����

        private GameObject currentBubble;//��ǰ��ʾ������
        private BubbleInfo currentBubbleInfo;

        /// <summary>
        /// ������Ϣ���� ����װ�����п�����ʾ������ 
        /// ��������ͬʱֻ����ʾһ��������ֻ����������Ч ������ʾ�Ķ��Ǿ�����������
        /// </summary>
        public List<BubbleInfo> bubbleInfoList;


        //������
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
        /// �������� ��Ҫ���� �Ƿ��ж������ͬʱ���ڵ������
        /// </summary>
        /// <param name="info"></param>
        private void RefreshBubble()
        {
            if (bubbleInfoList == null || bubbleInfoList.Count == 0) return;

            //ȡ��������������bubbleinfo
            BubbleInfo tmpBubbleInfo = bubbleInfoList[0];
            //���ڻ�������������
            if (currentBubbleInfo != null && tmpBubbleInfo.creator == currentBubbleInfo.creator) return;
            // �������е�����
            if (currentBubble != null)
            {
                DestroyBubble();
            }
            //���µ�ǰ������Ϣ
            currentBubbleInfo = tmpBubbleInfo;
            // ʵ��������
            currentBubble = Instantiate(BubblePrefab);

            UIBubbleItem bubbleItem = currentBubble.GetComponent<UIBubbleItem>();
            currentBubble.transform.parent = transform;

            string keyStr = "";//������ʾ�ı�
            if (bubbleItem != null)
            {
                for (int i = 0; i < tmpBubbleInfo.actionTypeList.Count; i++)
                {
                    keyStr += PlayerUtil.Instance.GetSettingKey(tmpBubbleInfo.actionTypeList[i], tmpBubbleInfo.controlType);
                    if (i < tmpBubbleInfo.actionTypeList.Count - 1) keyStr += "/";
                }
                bubbleItem.Init(tmpBubbleInfo, keyStr); // ��ʼ������
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
            //����������� ����Ҫ���� ��Ϊ�����Ѿ���ÿ֡���ã��Ż�����
            bubbleInfoList.Add(info);
            RefreshBubble();
        }

        public void RemoveBubbleInfoFromList(BubbleInfo info)
        {
            if (bubbleInfoList == null) return;
            BubbleInfo tmpInfo = bubbleInfoList.Find(x => x.creator == info.creator);
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
    }
}