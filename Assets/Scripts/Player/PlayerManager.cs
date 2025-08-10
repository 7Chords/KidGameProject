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
        public void Init(Vector3 playerSpawnPos)
        {
            GameObject playerGO = Instantiate(playerPrefab);
            playerGO.transform.position = new Vector3(playerSpawnPos.x, playerSpawnPos.y, -playerSpawnPos.z)
                + GameManager.Instance.GameGeneratePoint.position;

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