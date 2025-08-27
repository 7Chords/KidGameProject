using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    public class ElectricTrolley : TrapBase
    {
        //public float Power;
        //public float PowerCostPerSecond;
        public float Speed;
        public float Damage;
        public Vector3 HalfDamageArea;
        public BuffData Buff;

        private Vector3 dir;
        private List<GameObject> hasHurtEnemyList;
        private Collider[] colls;

        public override void Init(TrapData trapData)
        {
            base.Init(trapData);
            hasHurtEnemyList = new List<GameObject>();
        }
        public override void Trigger()
        {
            base.Trigger();
            //trapData.deadDelayTime = Power / PowerCostPerSecond;
            Vector3 noFixDir = (transform.position - interactor.transform.position).normalized;
            dir = new Vector3(noFixDir.x, 0, noFixDir.z);
            transform.rotation = Quaternion.LookRotation(dir);
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
            if (hasHurtEnemyList == null) return;
            colls = Physics.OverlapBox(transform.position, HalfDamageArea);
            if (colls.Length == 0) return;
            foreach (var coll in colls)
            {
                if (coll == null) return;
                if (coll.gameObject.tag == "Enemy")
                {
                    if(!hasHurtEnemyList.Contains(coll.gameObject))
                    {
                        hasHurtEnemyList.Add(coll.gameObject);
                        coll.GetComponent<EnemyController>().TakeDamage(
                            new DamageInfo(gameObject, Damage, Buff ? new BuffInfo(Buff, coll.gameObject) : null));
                        GameManager.Instance.AddScore(trapData.trapScore);
                        DeadByExternal();
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
