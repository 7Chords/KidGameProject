using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    public class TrapNoReadyStateBase : TrapStateBase
    {
        protected bool _isTimeValid; // 是否时间型陷阱（需要时间"暖机"后生效）
        protected float _validTime;  // 生效所需时间
        protected float _validTimer; // 上者的计时器

        public override void Enter()
        {
            _isTimeValid = trap.TrapData.validType == TrapValidType.TimeDelay;
            _validTime = trap.TrapData.validTime;
            _validTimer = 0;
            //设置指示器材质颜色
            trap.ReadyIndicator.material.SetColor("_Color", trap.NoReadyColor);
            //如果不需要时间等待，直接变为Ready状态
            if (!_isTimeValid)
            {
                trap.ChangeState(TrapState.Ready);
            }
        }

        public override void Update()
        {
            if (_isTimeValid)
            {
                _validTimer += Time.deltaTime;
                if (_validTimer >= _validTime)
                {
                    trap.ChangeState(TrapState.Ready);
                }
            }
        }
    }
}