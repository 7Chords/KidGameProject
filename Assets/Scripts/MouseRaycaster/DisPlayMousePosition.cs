using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KidGame.Core
{
    public class DisPlayMousePosition : MonoBehaviour
    {
        [SerializeField] private GameObject showMousePrefab;
        Vector3 lastFrameMousePosi = Vector3.zero;
        private GameObject _m_gMouseObj;
        void Start()
        {
            _m_gMouseObj = Object.Instantiate(showMousePrefab);
        }

        // Update is called once per frame
        void Update()
        {
            if (lastFrameMousePosi != MouseRaycaster.Instance.GetMousePosi()) updateMouseShow();
        }

        /// <summary>
        /// 如果鼠标位置更改了 那么更改鼠标物体位置
        /// </summary>
        private void updateMouseShow()
        {
            lastFrameMousePosi = MouseRaycaster.Instance.GetMousePosi();
            _m_gMouseObj.transform.position = lastFrameMousePosi;
        }
    }

}
