using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KidGame.Core
{

    public class Hammer_Positive : TrapBase
    {
        public override void Trigger()
        {
            Rb.AddForce((transform.position - interactor.transform.position).normalized * 100f,
                ForceMode.Impulse);
        }

    }
}
