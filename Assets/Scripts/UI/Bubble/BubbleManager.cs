using System;
using UnityEngine;

namespace KidGame.UI
{
    /// <summary>
    /// 气泡信息
    /// </summary>
    public class BubbleInfo
    {
        public string myControlType; //控制类型：键盘/手柄

        public string inputActionName; //提示操作的inputaction的名字

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
        public string content { get; set; } //文本
    }

    /// <summary>
    /// 气泡管理器
    /// </summary>
    public class BubbleManager : Singleton<BubbleManager>
    {
        public GameObject BubblePrefab; // 气泡的预制体

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
        /// 创建气泡 需要完善 是否有多个气泡同时存在的情况？
        /// </summary>
        /// <param name="info"></param>
        public void CreateBubble(BubbleInfo info)
        {
            // 销毁已有的气泡
            if (currentBubble != null)
            {
                Destroy(currentBubble);
            }

            // 实例化气泡
            currentBubble = Instantiate(BubblePrefab);
            UIBubbleItem bubbleItem = currentBubble.GetComponent<UIBubbleItem>();
            currentBubble.transform.parent = transform;

            string keyStr = "";
            if (bubbleItem != null)
            {
                if (info.myControlType == "Keyboard") //键盘
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
                            keyStr = "鼠标右键"; //有点长了 建议把所有的可能提示的键位都画示意图 用图片提示
                            break;
                        case "Use":
                            keyStr = "鼠标左键";
                            break;
                    }
                }
                else if (info.myControlType == "Gamepad") //手柄
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

                bubbleItem.Init(info, keyStr); // 初始化气泡
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