using KidGame.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KidGame.core
{
    public class FocusLinesScript : MonoBehaviour
    {
        //理论上这个点要在地板图层之上 为了和下面同一 在玩家脚底位置 的y分量上 + 0.1f
        public Transform startPosi; // 起点
        private bool isHoldPress = false;
        [Header("射线引用")]
        public LineRenderer leftLine; // 左射线
        public LineRenderer rightLine; // 右射线

        [Header("聚焦参数")]
        public float lineLength = 10f; // 射线长度 这里应该读配表的Distance
        public float startAngle = 60f; // 初始夹角（度）
        public float focusSpeed = 20f; // 每秒减少的角度（度）

        private float currentAngle; // 当前夹角 

        void Start()
        {
            Initialized();
        }

        void Update()
        {
            // 角度逐渐减小（最小到0°）
            currentAngle = Mathf.Max(0, currentAngle - focusSpeed * Time.deltaTime);

            // 当玩家HoldPress的时候 计算左右两条线的终点
            if(isHoldPress)
            {
                UpdateLinePositions();
            }
            else
            {
                ChangeBack();
            }
           
        }

        void UpdateLinePositions()
        {
            // 玩家位置作为起点
            Vector3 origin = startPosi.position;

            // 计算左右射线的角度（以玩家面向方向为0°，向两侧张开）
            // 假设玩家面向Z轴正方向，左射线为+currentAngle/2，右射线为-currentAngle/2
            // 由于Unity的数学库三角函数只接受弧度 所以这里转为弧度制
            float halfAngle = currentAngle / 2f * Mathf.Deg2Rad; 

            Vector3 leftDir = new Vector3(
                Mathf.Sin(halfAngle), // 理解成俯视角 算一个以玩家为圆心的园的三角函数的归一化值
                0.1f, // 俯视理论上要高于地面否则会被遮挡
                Mathf.Cos(halfAngle)
            ).normalized;

            // 左射线终点 = origin + 长度 * 方向向量
            Vector3 leftEnd = origin + leftDir * lineLength;

            // 右射线终点 同上
            Vector3 rightDir = new Vector3(
                Mathf.Sin(-halfAngle),
                0.1f, // 俯视理论上要高于地面否则会被遮挡
                Mathf.Cos(-halfAngle)
            ).normalized;

            Vector3 rightEnd = origin + rightDir * lineLength;

            // 更新线段位置 0这个点为玩家位置 1为线段终点位置
            leftLine.SetPosition(0, origin);
            leftLine.SetPosition(1, leftEnd);

            rightLine.SetPosition(0, origin);
            rightLine.SetPosition(1, rightEnd);
        }

        //创建这个Prefab的时候 想办法让
        public void SetisHoldPressOrNot(bool isHold)
        {
            isHoldPress = isHold;
        }

        private void ChangeBack()
        {
            currentAngle = startAngle;
        }


        private void Initialized()
        {

            // 初始化线段点个数
            leftLine.positionCount = 2;
            rightLine.positionCount = 2;

            // 初始角度设为最大夹角
            currentAngle = startAngle;
            // 订阅鼠标释放这个消息
            PlayerController.Instance.OnMouseBtnReleaseAction += SetisHoldPressOrNot;
        }
        private void OnDestroy()
        {
            // 取消订阅
            PlayerController.Instance.OnMouseBtnReleaseAction -= SetisHoldPressOrNot;
        }
    }

}

