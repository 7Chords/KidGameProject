using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KidGame.UI
{
    public class UIBubbleItem : MonoBehaviour
    {
        public Text ContentText;//��ʾ�����ݣ��磺��ʹ��/Ͷ��/������
        public Image ContentImage;//��ʾ��ͼƬ����ʱû�ã�
        public Text KeyText;//��ʾ�ļ�λ

        private GameObject go_1;
        private GameObject go_2;

        private Tweener _tweener;

        public void Init(BubbleInfo info,string key)
        {
            go_1 = info.go_1;
            go_2 = info.go_2;
            ContentText.text = info.content;
            KeyText.text = key;

            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, 0.2f);

            //TODO:���ø�����
            //transform.parent = GameObject.FindWithTag("BubbleManager").transform;
        }

        private void Update()
        {
            if (go_1 && go_2)
            {
                SetPosition(go_1, go_2);
            }
        }

        /// <summary>
        /// ����Һ���Ҫ��������Ʒ��λ�ü����ø�����
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        public void SetPosition(GameObject obj1, GameObject obj2)
        {
            Vector3 pos = (obj1.transform.position + obj2.transform.position) / 2;
            transform.position = Camera.main.WorldToScreenPoint(pos);
        }

        public void DestoryBubble()
        {
            _tweener = transform.DOScale(0, 0.2f);
            StartCoroutine(WaitToDestory());
        }

        private IEnumerator WaitToDestory()
        {
            yield return _tweener.WaitForCompletion();
            Destroy(gameObject);
        }
    }



}
