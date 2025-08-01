using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KidGame.Core
{
    public class Hammer_Passive : TrapBase
    {
        public float Damage;
        public Vector3 HalfDamageArea;
        public BuffData Buff;
        private bool hasCausedDamage;
        public int Score;
        public override void Trigger()
        {
            base.Trigger();
            Vector3 dir = (interactor.transform.position - transform.position).normalized;
            dir.y = 0;
            Rb.AddForce(dir * 100f,ForceMode.Impulse);
            this.OnFixedUpdate(PerformTick);
        }

        public override void Discard()
        {
            this.RemoveFixedUpdate(PerformTick);
            base.Discard();
        }

        private void PerformTick()
        {
            if (!hasCausedDamage)//todo:fix
            {
                Collider[] colls = Physics.OverlapBox(transform.position, HalfDamageArea);
                foreach (var coll in colls)
                {
                    if (coll == null) return;
                    if (coll.gameObject.tag == "Enemy")
                    {
                        hasCausedDamage = true;
                        coll.GetComponent<EnemyController>().TakeDamage(
                            new DamageInfo(gameObject, Damage, Buff?new BuffInfo(Buff, coll.gameObject):null));
                        GameManager.Instance.AddScore(trapData.trapScore);

                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, HalfDamageArea);
        }
    }

}
