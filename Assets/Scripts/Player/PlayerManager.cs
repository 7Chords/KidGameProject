using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// ��ҹ����� ������Ҹ���ģ��ĳ�ʼ������ ����˳��
    /// ��ͬ��ҲҪ��GameManager��ʼ��
    /// </summary>
    public class PlayerManager : Singleton<PlayerManager>
    {
        [SerializeField]
        private GameObject playerPrefab;
        public void Init()
        {
            //todo
            Instantiate(playerPrefab);

            PlayerUtil.Instance.Init();
            PlayerController.Instance.Init();
            PlayerBag.Instance.Init();
        }

        public void Discard()
        {
            PlayerUtil.Instance.Discard();
            PlayerController.Instance.Discard();
            PlayerBag.Instance.Discard();
        }
    }
}