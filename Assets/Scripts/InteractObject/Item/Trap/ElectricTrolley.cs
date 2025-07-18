using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    public class ElectricTrolley : TrapBase
    {
        public float Power;
        public float PowerCostPerSecond;
        public float Speed;
        public float Damage;
        public Vector3 HalfDamageArea;
        public BuffData Buff;

        private Vector3 dir;
        private bool hasCausedDamage;
        public int Score;
        public override void Trigger()
        {
            base.Trigger();
            trapData.deadDelayTime = Power / PowerCostPerSecond;
            Vector3 noFixDir = (transform.position - interactor.transform.position).normalized;
            dir = new Vector3(noFixDir.x, 0, noFixDir.z);
            transform.rotation = Quaternion.LookRotation(dir);
            //transform.LookAt(dir);
            this.OnFixedUpdate(PerformTick);
        }

        public override void Discard()
        {
            this.RemoveFixedUpdate(PerformTick);
            base.Discard();
        }

        private void PerformTick()
        {
            if(Rb) Rb.velocity = dir * Speed;

            if(!hasCausedDamage)//todo:fix
            {
                Collider[] colls = Physics.OverlapBox(transform.position, HalfDamageArea);
                foreach(var coll in colls)
                {
                    if (coll == null) return;
                    if(coll.gameObject.tag == "Enemy")
                    {
                        hasCausedDamage = true;
                        coll.GetComponent<EnemyController>().TakeDamage(
                            new DamageInfo(gameObject, Damage, Buff ? new BuffInfo(Buff, coll.gameObject) : null));
                        GameManager.Instance.AddScore(Score);
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
