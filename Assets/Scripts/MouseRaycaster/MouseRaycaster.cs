using KidGame.UI.Game;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace KidGame.Core
{
    public class MouseRaycaster : Singleton<MouseRaycaster>
    {
        public float raycastMaxDistance = 100f;
        private Vector3 _m_vMousePosition;
        int layerIndex;
        int layerMask;
        //public Vector3 _mousePosition
        //{
        //    get 
        //    { 
        //        return _m_vMousePosition;
        //    }
        //}

        private Camera mainCamera;
        private RaycastHit hitInfo;
        private Ray ray;
        private void Start()
        {
            layerIndex = LayerMask.NameToLayer("Ground");
            layerMask = 1 << layerIndex;
            //主摄像机  如果后续会用到多个相机切换 请告知我
            mainCamera = GetComponent<Camera>();
        }
        //void Update()
        //{
        //    _m_vMousePosition = GetMousePosi();
        //}

        public Vector3 GetMousePosi()
        {
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            // 发射射线 检测碰撞
            if (Physics.Raycast(ray, out hitInfo, raycastMaxDistance, layerMask))
            {
                Debug.DrawLine(this.transform.position,hitInfo.point);
                // 射线碰撞到了物体 获取碰撞到的交点
                return hitInfo.point;
            }
            else return Vector3.zero;
        }

    }
}

