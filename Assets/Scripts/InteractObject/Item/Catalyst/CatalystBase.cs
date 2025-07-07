using KidGame.Core;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// ��ý���� ��ýҲ����������
    /// </summary>
    public class CatalystBase : TrapBase
    {
        protected TrapBase _connectTrap;

        public void Init()
        {
        }

        public override void InteractPositive(GameObject interactor)
        {
            if (_connectTrap == null) return;//û�й���ʱ������������running״̬
            base.InteractPositive(interactor);
        }

        public override void Trigger()
        {
            if (_connectTrap == null) return;
            _connectTrap.InteractNegative(this, interactor);
        }

        public virtual void SetTrap(TrapBase trap)
        {
            if(_connectTrap!=null)//��ȡ��֮ǰ���������ý������
            {
                _connectTrap.SetCatalyst(null);
            }
            _connectTrap = trap;
        }

        public override void Pick()
        {
            if(_connectTrap != null)
            {
                _connectTrap.SetCatalyst(null);//ȡ�����������ý������
            }
            base.Pick();
        }
    }
}