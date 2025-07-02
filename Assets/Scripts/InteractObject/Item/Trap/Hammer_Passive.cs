using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KidGame.Core
{
    public class Hammer_Passive : TrapBase
    {
        public override void Trigger(GameObject interactor)
        {
            Rb.AddForce((interactor.transform.position - transform.position).normalized * 1000f,
                ForceMode.Impulse);
            Destroy(gameObject, 2f);
        }
    }

}
