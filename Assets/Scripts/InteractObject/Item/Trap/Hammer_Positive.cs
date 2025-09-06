using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KidGame.Core
{

    public class Hammer_Positive : TrapBase
    {
        public float Damage;
        public Vector3 HalfDamageArea;
        public BuffData Buff;
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
            Vector3 dir = (transform.position - interactor.transform.position).normalized;
            dir.y = 0;
            Rb.AddForce(dir.normalized * 100f,ForceMode.Impulse);
            this.OnFixedUpdate(PerformTick);
        }

        public override void Discard()
        {
            this.RemoveFixedUpdate(PerformTick);
            base.Discard();
        }

        private void PerformTick()
        {
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
                        ScoreManager.Instance.AddScore(trapData.trapScore);
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
