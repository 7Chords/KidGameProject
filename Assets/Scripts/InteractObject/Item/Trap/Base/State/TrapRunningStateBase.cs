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

            // ��ʼ������
            if (_conditions.Contains(TrapDeadType.TimeDelay))
            {
                _deadTimer = trap.TrapData.deadDelayTime;
            }

            // ���ֻ����Immediate����û����������������
            if (_conditions.Contains(TrapDeadType.Immediate) ||
                _conditions == null || _conditions.Count == 0)
            {
                trap.ChangeState(TrapState.Dead);
            }
        }

        public override void Update()
        {

            // ���ʱ���ӳ�����
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