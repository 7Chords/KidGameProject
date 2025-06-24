using System;
using UnityEngine;

namespace KidGame.UI
{
    /// <summary>
    /// ������Ϣ
    /// </summary>
    public class BubbleInfo
    {
        public string myControlType; //�������ͣ�����/�ֱ�

        public string inputActionName; //��ʾ������inputaction������

        public BubbleInfo(string myControlType, string inputActionName, GameObject go_1, GameObject go_2, string content)
        {
            this.myControlType = myControlType;
            this.inputActionName = inputActionName;
            this.go_1 = go_1;
            this.go_2 = go_2;
            this.content = content;
        }

        public GameObject go_1 { get; set; }
        public GameObject go_2 { get; set; }
        public string content { get; set; } //�ı�
    }

    /// <summary>
    /// ���ݹ�����
    /// </summary>
    public class BubbleManager : Singleton<BubbleManager>
    {
        public GameObject BubblePrefab; // ���ݵ�Ԥ����

        private GameObject currentBubble;

        public event Action<BubbleInfo> onBubbleCreated;

        public void Init()
        {
            onBubbleCreated += CreateBubble;
        }

        public void Discard()
        {
            onBubbleCreated -= CreateBubble;
        }

        /// <summary>
        /// �������� ��Ҫ���� �Ƿ��ж������ͬʱ���ڵ������
        /// </summary>
        /// <param name="info"></param>
        public void CreateBubble(BubbleInfo info)
        {
            // �������е�����
            if (currentBubble != null)
            {
                Destroy(currentBubble);
            }

            // ʵ��������
            currentBubble = Instantiate(BubblePrefab);
            UIBubbleItem bubbleItem = currentBubble.GetComponent<UIBubbleItem>();
            currentBubble.transform.parent = transform;

            string keyStr = "";
            if (bubbleItem != null)
            {
                if (info.myControlType == "Keyboard") //����
                {
                    switch (info.inputActionName)
                    {
                        case "Interaction":
                            keyStr = "E";
                            break;
                        case "Recycle":
                            keyStr = "R";
                            break;
                        case "Throw":
                            keyStr = "����Ҽ�"; //�е㳤�� ��������еĿ�����ʾ�ļ�λ����ʾ��ͼ ��ͼƬ��ʾ
                            break;
                        case "Use":
                            keyStr = "������";
                            break;
                    }
                }
                else if (info.myControlType == "Gamepad") //�ֱ�
                {
                    switch (info.inputActionName)
                    {
                        case "Interaction":
                            keyStr = "A";
                            break;
                        case "Recycle":
                            keyStr = "Y";
                            break;
                        case "Throw":
                            keyStr = "B";
                            break;
                        case "Use":
                            keyStr = "X";
                            break;
                    }
                }

                bubbleItem.Init(info, keyStr); // ��ʼ������
            }
        }

        public void DestroyBubble()
        {
            if (currentBubble)
            {
                currentBubble.GetComponent<UIBubbleItem>().DestoryBubble();
            }
        }
    }
}