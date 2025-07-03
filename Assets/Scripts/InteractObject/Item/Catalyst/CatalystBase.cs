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
        protected TrapBase _trap;

        public void Init()
        {
        }

        public override void InteractNegative(GameObject interactor)
        {
            if (_trap == null) return;
            _trap.TriggerByCatalyst(this, interactor);
            ChangeState(TrapState.Dead);
        }

        public override void InteractPositive(GameObject interactor)
        {
        }


        public virtual void SetTrap(TrapBase trap)
        {
            _trap = trap;
        }
    }
}