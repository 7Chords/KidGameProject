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
        protected bool _needCatalyst; // �Ƿ���Ҫý�鴥��

        public override void Enter()
        {
            _isTimeValid = trap.TrapData.validType == TrapValidType.TimeDelay;
            _validTime = trap.TrapData.validTime;
            _validTimer = 0;
            _needCatalyst = trap.TrapData.triggerType == TrapTriggerType.Negative;
            //����ָʾ��������ɫ
            trap.ReadyIndicator.material.SetColor("_Color", trap.NoReadyColor);
            //����Ȳ���Ҫʱ��Ҳ����Ҫý�飬ֱ�ӱ�ΪReady״̬
            if (!_isTimeValid && !_needCatalyst)
            {
                trap.ChangeState(TrapState.Ready);
            }
        }

        public override void Update()
        {
            //������Ҫý������
            if (_needCatalyst)
            {
                //�����ý�����
                if (trap.Catalyst != null)
                {
                    //�����Ҫʱ����֤��ʱ��δ����ֻ����ʱ��
                    if (_isTimeValid && _validTimer < _validTime)
                    {
                        _validTimer += Time.deltaTime;
                    }
                    //�������Ҫʱ����֤������ʱ���Ѿ����㣬��ΪReady״̬
                    else if (!_isTimeValid || _validTimer >= _validTime)
                    {
                        trap.ChangeState(TrapState.Ready);
                    }
                }
                //���û��ý����ڣ�����Ҫʱ����֤������ʱ��
                else if (_isTimeValid)
                {
                    _validTimer += Time.deltaTime;
                }
            }
            //������Ҫý�鵫��Ҫʱ����֤�����
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