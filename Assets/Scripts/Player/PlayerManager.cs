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

            //��ͼ��ת���µ�λ������
            playerGO.transform.RotateAround(new Vector3(GameManager.Instance.GameGeneratePoint.position.x, playerSpawnPos.y, GameManager.Instance.GameGeneratePoint.position.z),
                Vector3.up, 
                GameManager.Instance.GameGeneratePoint.rotation.eulerAngles.y);
            PlayerController.Instance.Init();
            PlayerBag.Instance.Init();
        }

        public void Discard()
        {
            PlayerController.Instance.Discard();
            PlayerBag.Instance.Discard();
        }
    }
}