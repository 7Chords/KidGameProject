using KidGame.Core;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// ´¥Ã½»ùÀà
    /// </summary>
    public class CatalystBase : MapItem
    {
        [SerializeField] protected TrapBase _trap;

        public void Init()
        {
        }

        public override void InteractNegative()
        {
            if (_trap == null) return;
            _trap.TriggerByCatalyst(this);
        }

        public override void InteractPositive()
        {
        }

        public override void Pick()
        {
        }

        public virtual void SetTrap(TrapBase trap)
        {
            _trap = trap;
        }
    }
}