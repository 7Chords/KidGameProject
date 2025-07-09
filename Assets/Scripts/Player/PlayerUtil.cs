using System;
using KidGame.Interface;

namespace KidGame.Core
{
    /// <summary>
    /// 玩家util 事件
    /// </summary>
    public class PlayerUtil : Singleton<PlayerUtil>
    {
        private InputSettings inputSettings;
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

        #region 订阅和反订阅事件


        public void RegPlayerPickItem(Action<IPickable> onPlayerPickItem)
        {
            this.onPlayerPickItem += onPlayerPickItem;
        }

        public void UnregPlayerPickItem(Action<IPickable> onPlayerPickItem)
        {
            this.onPlayerPickItem -= onPlayerPickItem;
        }

        #endregion

        #region 功能

        /// <summary>
        /// ��ȡ������õļ�λ
        /// </summary>
        /// <param name="actionName">��Ϊ��</param>
        /// <param name="controlType">��������</param>
        /// <returns></returns>
        public string GetSettingKey(InputActionType actionType, ControlType controlType)
        {
            return inputSettings.GetSettingKey(actionType, (int)controlType);
        }
        #endregion
    }
}