using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KidGame.Core
{
    public class Hammer_Passive : TrapBase
    {
        public override void Trigger()
        {
            Rb.AddForce((interactor.transform.position - transform.position).normalized * 100f,
                ForceMode.Impulse);
        }
    }

}
