using KidGame.Core;
using KidGame.Interface;
using UnityEngine;

namespace KidGame.Core
{
    public class SmallBoom : TrapBase
    {
        public override void Trigger()
        {
            Collider[] colls = Physics.OverlapSphere(transform.position, 5);
            IDamageable damageable;
            foreach(var coll in colls)
            {
                damageable = coll.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(new DamageInfo(gameObject, 5));
                }
            }
            Debug.Log("TestTrap_1¥•∑¢¡À");
            Destroy(gameObject);
        }
    }
}