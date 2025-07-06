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
        protected bool _needCatalyst; // 是否需要媒介触发

        public override void Enter()
        {
            _isTimeValid = trap.TrapData.validType == TrapValidType.TimeDelay;
            _validTime = trap.TrapData.validTime;
            _validTimer = 0;
            _needCatalyst = trap.TrapData.triggerType == TrapTriggerType.Negative;
            //设置指示器材质颜色
            trap.ReadyIndicator.material.SetColor("_Color", trap.NoReadyColor);
            //如果既不需要时间也不需要媒介，直接变为Ready状态
            if (!_isTimeValid && !_needCatalyst)
            {
                trap.ChangeState(TrapState.Ready);
            }
        }

        public override void Update()
        {
            //处理需要媒介的情况
            if (_needCatalyst)
            {
                //如果有媒介存在
                if (trap.Catalyst != null)
                {
                    //如果需要时间验证且时间未到，只更新时间
                    if (_isTimeValid && _validTimer < _validTime)
                    {
                        _validTimer += Time.deltaTime;
                    }
                    //如果不需要时间验证，或者时间已经满足，变为Ready状态
                    else if (!_isTimeValid || _validTimer >= _validTime)
                    {
                        trap.ChangeState(TrapState.Ready);
                    }
                }
                //如果没有媒介存在，但需要时间验证，更新时间
                else if (_isTimeValid)
                {
                    _validTimer += Time.deltaTime;
                }
            }
            //处理不需要媒介但需要时间验证的情况
            else if (_isTimeValid)
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