using KidGame.Core;
using KidGame.Interface;
using UnityEngine;

namespace KidGame.Core
{
    public class SmallBoom : TrapBase
    {
        public float damage;
        public float force;
        public BuffData buffData;
        public override void Trigger(GameObject interactor)
        {
            Collider[] colls = Physics.OverlapSphere(transform.position, 5);
            IDamageable damageable;
            Vector3 dir = Vector3.zero;
            foreach(var coll in colls)
            {
                damageable = coll.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    dir = (coll.transform.position - transform.position).normalized;
                    damageable.TakeDamage(new DamageInfo(gameObject, damage, 
                        new BuffInfo(buffData,coll.gameObject,new object[] { dir * force })));//额外传递一个力的参数
                }
            }

            Destroy(gameObject);
        }
    }
}