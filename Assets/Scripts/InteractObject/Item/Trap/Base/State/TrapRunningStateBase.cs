using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    public class TrapRunningStateBase : TrapStateBase
    {
        private float _deadTimer = 0f;
        private List<TrapDeadType> _conditions;

        public override void Enter()
        {
            trap.Trigger();

            _conditions = trap.TrapData.deadTypeList;

            // 初始化条件
            if (_conditions.Contains(TrapDeadType.TimeDelay))
            {
                _deadTimer = trap.TrapData.deadDelayTime;
            }

            // 如果只包含Immediate或者没有条件，立即死亡
            if (_conditions.Contains(TrapDeadType.Immediate) ||
                _conditions == null || _conditions.Count == 0)
            {
                trap.ChangeState(TrapState.Dead);
            }
        }

        public override void Update()
        {

            // 检查时间延迟条件
            if (_conditions.Contains(TrapDeadType.TimeDelay))
            {
                _deadTimer -= Time.deltaTime;
                if (_deadTimer <= 0)
                {
                    trap.ChangeState(TrapState.Dead);
                }
            }
        }




    }
}