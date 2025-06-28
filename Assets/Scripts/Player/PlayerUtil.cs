using System;
using KidGame.Interface;

namespace KidGame.Core
{
    /// <summary>
    /// ����¼�����
    /// </summary>
    public class PlayerUtil : Singleton<PlayerUtil>
    {
        private InputSettings inputSettings;

        //��Ҽ񵽳�����Ʒ������͵��ߣ����¼�
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

        #region �����¼�

        public void CallPlayerPickItem(IPickable iPickable)
        {
            onPlayerPickItem?.Invoke(iPickable);
        }

        #endregion

        #region ע��ͷ�ע��


        public void RegPlayerPickItem(Action<IPickable> onPlayerPickItem)
        {
            this.onPlayerPickItem += onPlayerPickItem;
        }

        public void UnregPlayerPickItem(Action<IPickable> onPlayerPickItem)
        {
            this.onPlayerPickItem -= onPlayerPickItem;
        }

        #endregion

        #region ������

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