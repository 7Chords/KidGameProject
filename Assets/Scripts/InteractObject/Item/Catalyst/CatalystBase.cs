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

        public override void Trigger()
        {
            if (_connectTrap == null) return;
            _connectTrap.InteractNegative(this, interactor);
        }

        public virtual void SetTrap(TrapBase trap)
        {
            _connectTrap = trap;
        }
    }
}