using KidGame.Core;
using KidGame.Interface;
using UnityEngine;

namespace KidGame.Core
{
    public class SmallBoom : TrapBase
    {
        public float damage;
        public float force;
        public float damageArea;
        public BuffData buffData;
        public override void Trigger()
        {
            Collider[] colls = Physics.OverlapSphere(transform.position, damageArea);
            IDamageable damageable;
            Vector3 dir = Vector3.zero;
            foreach(var coll in colls)
            {
                if (coll == null) continue;
                damageable = coll.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    dir = (coll.transform.position - transform.position).normalized;
                    damageable.TakeDamage(new DamageInfo(gameObject, damage, 
                        new BuffInfo(buffData,coll.gameObject,new object[] { dir * force })));//���⴫��һ�����Ĳ���
                }
            }
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, damageArea);
        }
    }
}