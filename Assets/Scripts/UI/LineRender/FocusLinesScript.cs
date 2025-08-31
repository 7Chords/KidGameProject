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
        private Transform startPosi; // 起点
        private bool isHoldPress = false;
        [Header("射线引用")]
        public LineRenderer leftLine; // 左射线
        public LineRenderer rightLine; // 右射线

        [Header("聚焦参数")]
        public float lineLength = 20f; // 射线长度 这里应该读配表的Distance
        public float startAngle = 50f; // 初始夹角（度）
        public float focusSpeed = 50f; // 每秒减少的角度（度）
        private float currentRotation = 0f; // 当前旋转角度
        private float currentAngle; // 当前夹角
        public float lineWidth;
        void Start()
        {
            Initialized();
        }
         
        void Update()
        {
            // 当玩家HoldPress的时候 更新主要逻辑：计算左右两条线的终点
            if(isHoldPress)
            {
                UpdateRotationTowardsMouse(); // 更新面朝位置
                UpdateLinePositions();
            }
            else
            {
                ChangeBackAngle();
            }
           
        }

        private void updateCurrentStartPosi()
        {
            // 初始化开始位置 player的位置
            startPosi = PlayerController.Instance.transform;
            // 初始化鼠标位置

        }
        void UpdateLinePositions()
        {

            // 持续更新起始位置
            updateCurrentStartPosi();
            // 角度逐渐减小（最小到10）
            currentAngle = Mathf.Max(10, currentAngle - focusSpeed * Time.deltaTime);

            // 玩家位置作为起点
            Vector3 origin = startPosi.position;
            origin.y = 0.1f;
            // 计算左右射线的角度（以玩家面向方向为0°，向两侧张开）
            // 假设玩家面向Z轴正方向，左射线为+currentAngle/2，右射线为-currentAngle/2
            // 由于Unity的数学库三角函数只接受弧度 所以这里转为弧度制
            float halfAngle = currentAngle / 2f * Mathf.Deg2Rad;
            float rotationRad = currentRotation * Mathf.Deg2Rad;

            Vector3 leftDir = new Vector3(
                Mathf.Sin(rotationRad + halfAngle), // 理解成俯视角 算一个以玩家为圆心的园的三角函数的归一化值
                0.1f, // 俯视理论上要高于地面否则会被遮挡
                Mathf.Cos(rotationRad + halfAngle)
            ).normalized;

            // 左射线终点 = origin + 长度 * 方向向量
            Vector3 leftEnd = origin + leftDir * lineLength;

            // 右射线终点 同上
            Vector3 rightDir = new Vector3(
                Mathf.Sin(rotationRad - halfAngle),
                0.1f, // 俯视理论上要高于地面否则会被遮挡
                Mathf.Cos(rotationRad - halfAngle)
            ).normalized;

            Vector3 rightEnd = origin + rightDir * lineLength;

            // 更新线段位置 0这个点为玩家位置 1为线段终点位置
            leftLine.SetPosition(0, origin);
            leftLine.SetPosition(1, leftEnd);

            rightLine.SetPosition(0, origin);
            rightLine.SetPosition(1, rightEnd);
        }

        //创建这个Prefab的时候 想办法让
        public void SetIsHoldRelease()
        {
            isHoldPress = false;
        }
        public void SetIsHoldPress()
        {
            isHoldPress = true;
        }

        private void ChangeBackAngle()
        {
            currentAngle = startAngle;
            SetLineRenderPoint2Default();
        }

        private void SetLineRenderPoint2Default()
        {
            leftLine.SetPosition(0, Vector3.zero);
            leftLine.SetPosition(1, Vector3.zero);
            rightLine.SetPosition(0, Vector3.zero);
            rightLine.SetPosition(1, Vector3.zero);
        }

        // 更新面向鼠标的旋转角度
        private void UpdateRotationTowardsMouse()
        {
            if (startPosi == null) return;

            // 获取鼠标在世界地图上的位置
            Vector3 mouseWorldPos = MouseRaycaster.Instance.GetMousePosi();
            mouseWorldPos.y = startPosi.position.y; // 保持在同一高度

            // 计算从玩家到鼠标的方向向量
            Vector3 direction = (mouseWorldPos - startPosi.position).normalized;

            // 计算这个方向的角度
            currentRotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        }

        private void Initialized()
        {
            // 线宽 初始化
            if(leftLine != null && rightLine != null)
            {
                leftLine.startWidth = lineWidth;
                leftLine.endWidth = lineWidth;
                rightLine.startWidth = lineWidth;
                rightLine.endWidth = lineWidth;
            }


            // 初始化开始位置 player的位置
            updateCurrentStartPosi();
            // 初始化线段点个数
            leftLine.positionCount = 2;
            rightLine.positionCount = 2;

            // 初始角度设为最大夹角
            currentAngle = startAngle;

            SetLineRenderPoint2Default();

            // 订阅鼠标释放这个消息
            MsgCenter.RegisterMsgAct(MsgConst.ON_USE_LONG_PRESS_RELEASE, SetIsHoldRelease);
            MsgCenter.RegisterMsgAct(MsgConst.ON_USE_LONG_PRESS, SetIsHoldPress);
        }
        private void OnDestroy()
        {
            // 取消订阅
            MsgCenter.UnregisterMsgAct(MsgConst.ON_USE_LONG_PRESS_RELEASE, SetIsHoldRelease);
            MsgCenter.UnregisterMsgAct(MsgConst.ON_USE_LONG_PRESS, SetIsHoldPress);
        }
    }

}

