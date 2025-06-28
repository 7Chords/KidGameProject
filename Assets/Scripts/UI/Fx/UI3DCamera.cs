using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

namespace KidGame.UI
{


    /// <summary>
    /// 用于背包里显示玩家模型
    /// </summary>
    public class UI3DCamera : MonoBehaviour
    {
        // 显示的目标
        public Transform target;

        // 物体围绕旋转的点
        public Transform pivot;

        // 与旋转的偏移值
        public Vector3 pivotOffset = Vector3.zero;

        // 摄像机距离目标的距离
        public float distance = 10.0f;

        // 最短、最长距离
        public float minDistance = 2f;

        public float maxDistance = 15f;

        // 缩放速度
        public float zoomSpeed = 1f;

        // x，y轴的旋转速度
        public float xSpeed = 250.0f;

        public float ySpeed = 250.0f;

        // y轴的最大、最小偏移值
        public float yMinLimit = -90f;

        public float yMaxLimit = 90f;

        // 是否允许y轴方向旋转
        public bool allowYTilt = true;

        // 记录相机与最后要移动的距离
        private float targetDistance;

        // 记录摄像机的x,y轴旋转
        private float x = 0.0f;

        private float y = 0.0f;

        // 记录摄像机的x,y轴旋转的目标值
        private float targetX = 0f;

        private float targetY = 0f;

        // x,y相对缓冲减速
        private float xVelocity = 1f;

        private float yVelocity = 1f;

        // 缩放相对缓冲减速
        private float zoomVelocity = 1f;

        private void Start()
        {
            // 记录现在相机的旋转角度
            var angles = transform.eulerAngles;
            // 刚开始相机的x,y轴旋转
            targetX = x = angles.x;
            // targetY = y = ClampAngle(angles.y, yMinLimit, yMaxLimit); // 原注释有误，应使用Mathf.Clamp
            targetY = y = Mathf.Clamp(angles.y, yMinLimit, yMaxLimit); // 修正注释：使用Mathf.Clamp限制y轴角度
            // 设置目标距离
            targetDistance = distance;
        }

        private void LateUpdate()
        {
            // 如果没有旋转点，不执行旋转和缩放
            if (!pivot) return;
            // 获取鼠标滚轮的偏移
            var scroll = Input.GetAxis("Mouse ScrollWheel");
            // 如果有值，就将targetDistance增大、缩小，即距离摄像机的远近
            if (scroll > 0.0f) targetDistance -= zoomSpeed;
            else if (scroll < 0.0f)
                targetDistance += zoomSpeed;

            // 距离目标距离在最大值、最小值之间取值
            targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);

            // 如果按下鼠标右键，或者（鼠标左键的同时按下左边的ctrl）或者（鼠标左键的同时按下右边的ctrl）
            if (Input.GetMouseButton(1) || (Input.GetMouseButton(0) &&
                                            (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))))
            {
                // 获取水平方向的偏移值
                targetX += Input.GetAxis("Mouse X") * xSpeed * 0.02f;

                // 如果允许Y轴偏移，获取鼠标在y轴上的偏移，记录改变的摄像机y轴的角度
                if (allowYTilt)
                {
                    targetY -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
                    // targetY = ClampAngle(targetY, yMinLimit, yMaxLimit); // 原注释有误，应使用Mathf.Clamp
                    targetY = Mathf.Clamp(targetY, yMinLimit, yMaxLimit); // 修正注释：使用Mathf.Clamp限制y轴角度
                }
            }

            // Mathf.SmoothDamp用于做相机的缓冲跟踪
            x = Mathf.SmoothDampAngle(x, targetX, ref xVelocity, 0.3f);
            y = allowYTilt ? Mathf.SmoothDampAngle(y, targetY, ref yVelocity, 0.3f) : targetY;
            distance = Mathf.SmoothDamp(distance, targetDistance, ref zoomVelocity, 0.5f);
            // 旋转
            Quaternion rotation = Quaternion.Euler(y, x, 0);

            // 摄像机的最终位置
            Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + pivot.position + pivotOffset;
            // 设置摄像机的位置和旋转
            transform.rotation = rotation;
            transform.position = position;
        }
    }
}