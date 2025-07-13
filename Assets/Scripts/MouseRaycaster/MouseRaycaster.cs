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
            //�������  ����������õ��������л� ���֪��
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

            // �������� �����ײ
            if (Physics.Raycast(ray, out hitInfo, raycastMaxDistance))
            {
                // ������ײ�������� ��ȡ��ײ���Ľ���
                return hitInfo.point;
            }
            else return Vector3.negativeInfinity;
        }

    }
}

