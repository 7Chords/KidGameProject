using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    public class TrapNoReadyStateBase : TrapStateBase
    {
        protected bool _isTimeValid; // �Ƿ�ʱ�������壨��Ҫʱ��"ů��"����Ч��
        protected float _validTime;  // ��Ч����ʱ��
        protected float _validTimer; // ���ߵļ�ʱ��

        public override void Enter()
        {
            _isTimeValid = trap.TrapData.validType == TrapValidType.TimeDelay;
            _validTime = trap.TrapData.validTime;
            _validTimer = 0;
            //����ָʾ��������ɫ
            trap.ReadyIndicator.material.SetColor("_Color", trap.NoReadyColor);
            //�������Ҫʱ��ȴ���ֱ�ӱ�ΪReady״̬
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