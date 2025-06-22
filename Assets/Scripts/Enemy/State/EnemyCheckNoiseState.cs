using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    public class EnemyCheckNoiseState : EnemyStateBase
    {
        private Vector3 _soundPos;
        private bool _searched;
        private RoomType _roomType;
        

        public override void Enter()
        {
            _searched = false;
            // enemy.PlayAnimation("Walk");
        }

        public override void Update()
        {
            if (!_searched)
            {
                Vector3 dir = (_soundPos - enemy.transform.position).normalized;
                enemy.Rb.velocity = dir * enemy.EnemyBaseData.MoveSpeed;

                if (Vector3.Distance(enemy.transform.position, _soundPos) < 0.3f)
                {
                    enemy.Rb.velocity = Vector3.zero;
                    _searched = true;
                    enemy.StartCoroutine(SearchRoutine());
                }
            }
        }

        private IEnumerator SearchRoutine()
        {
            // ���ݷ������ͽ��в�ͬ�������߼�
            switch (_roomType)
            {
                case RoomType.Bedroom:
                    // ���贲�ڷ����һ�ǣ����д��»��������
                    yield return new WaitForSeconds(1.0f);
                    CheckUnderBedOrCabinet();
                    break;
                case RoomType.LivingRoom:
                    // ���ɳ���¡����ӹ��
                    yield return new WaitForSeconds(1.0f);
                    CheckBehindFurniture();
                    break;
                default:
                    yield return new WaitForSeconds(1.0f);
                    break;
            }

            if (enemy.PlayerInSight())  // �ҵ���ң�����׷��״̬
            {
                enemy.Stun(0.5f);
                yield return new WaitForSeconds(0.5f);
                enemy.ChangeState(EnemyState.Chase);
            }
            else
            {
                enemy.ChangeState(EnemyState.Patrol);  // û���ҵ���ң��ص�Ѳ��״̬
            }
        }

        #region ��鷿��

        // ���ݷ������Ͳ�ͬ��ִ�в�ͬ��������Ϊ
        private void CheckUnderBedOrCabinet()
        {
            // ģ�ⴲ�º͹��ӵ������߼�
            Debug.Log("���ڼ�鴲�»����...");
            // �����������ڣ����˻����ȼ�鴲�»����
        }

        private void CheckBehindFurniture()
        {
            // ģ����ɳ������ӹ���߼�
            Debug.Log("���ڼ��ɳ������...");
            // ���ڿ����ڣ����˻���ɳ�������ӹ��ȵط�
        }        

        #endregion
        
        public override void Exit()
        {
            enemy.Rb.velocity = Vector3.zero;
        }
    }
}
