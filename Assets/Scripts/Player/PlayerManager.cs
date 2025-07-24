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