using KidGame.Core;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// 触媒基类 触媒也算是陷阱类
    /// </summary>
    public class CatalystBase : TrapBase
    {
        protected TrapBase _connectTrap;

        public void Init()
        {
        }

        public override void InteractPositive(GameObject interactor)
        {
            if (_connectTrap == null) return;//没有关联时被触发不进入running状态
            base.InteractPositive(interactor);
        }

        public override void Trigger()
        {
            if (_connectTrap == null) return;
            _connectTrap.InteractNegative(this, interactor);
        }

        public virtual void SetTrap(TrapBase trap)
        {
            if(_connectTrap!=null)//先取消之前关联陷阱的媒介引用
            {
                _connectTrap.SetCatalyst(null);
            }
            _connectTrap = trap;
        }

        public override void Pick()
        {
            if(_connectTrap != null)
            {
                _connectTrap.SetCatalyst(null);//取消关联陷阱的媒介引用
            }
            base.Pick();
        }
    }
}