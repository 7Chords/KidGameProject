using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// 玩家管理器 负责玩家各个模块的初始化调用 控制顺序
    /// 其同样也要被GameManager初始化
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

            //地图旋转导致的位置修正
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