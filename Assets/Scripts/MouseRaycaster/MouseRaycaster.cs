using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    public class MouseRaycaster : Singleton<MouseRaycaster>
    {
        public float raycastMaxDistance = 100f;
        private Vector3 _m_vMousePosition;
        public Vector3 _mousePosition
        {
            get 
            { 
                return _m_vMousePosition;
            }
        }

        private Camera mainCamera;

        private void Start()
        {
            //主摄像机  如果后续会用到多个相机切换 请告知我
            mainCamera = GetComponent<Camera>();
        }
        void Update()
        {
            _m_vMousePosition = GetMousePosi();
        }

        private Vector3 GetMousePosi()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            RaycastHit hitInfo;

            // 发射射线 检测碰撞
            if (Physics.Raycast(ray, out hitInfo, raycastMaxDistance))
            {
                // 射线碰撞到了物体 获取碰撞到的交点
                return hitInfo.point;
            }
            else return Vector3.negativeInfinity;
        }

    }
}

