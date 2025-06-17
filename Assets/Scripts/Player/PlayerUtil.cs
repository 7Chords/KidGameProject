using System;
using KidGame.Interface;

namespace KidGame.Core
{
    /// <summary>
    /// 玩家事件中心
    /// </summary>
    public class PlayerUtil : Singleton<PlayerUtil>
    {
        private InputSettings inputSettings;

        //玩家捡到场景物品（陷阱和道具）的事件
        private Action<IPickable> onPlayerPickItem;

        protected override void Awake()
        {
            base.Awake();
            inputSettings = GetComponent<InputSettings>();
        }

        public void Init()
        {
        }

        public void Discard()
        {
        }

        #region 呼叫事件

        public void CallPlayerPickItem(IPickable iPickable)
        {
            onPlayerPickItem?.Invoke(iPickable);
        }

        #endregion


        #region 注册和反注册


        public void RegPlayerPickItem(Action<IPickable> onPlayerPickItem)
        {
            this.onPlayerPickItem += onPlayerPickItem;
        }

        public void UnregPlayerPickItem(Action<IPickable> onPlayerPickItem)
        {
            this.onPlayerPickItem -= onPlayerPickItem;
        }

        #endregion
    }
}