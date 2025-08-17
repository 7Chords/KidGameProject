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
            //�������  ����������õ��������л� ���֪��
            mainCamera = GetComponent<Camera>();
        }
        //void Update()
        //{
        //    _m_vMousePosition = GetMousePosi();
        //}

        public Vector3 GetMousePosi()
        {
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            // �������� �����ײ
            if (Physics.Raycast(ray, out hitInfo, raycastMaxDistance))
            {
                Debug.DrawLine(this.transform.position,hitInfo.point);
                // ������ײ�������� ��ȡ��ײ���Ľ���
                return hitInfo.point;
            }
            else return Vector3.zero;
        }

    }
}

