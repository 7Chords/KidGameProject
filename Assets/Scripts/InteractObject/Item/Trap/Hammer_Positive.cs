using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KidGame.Core
{

    public class Hammer_Positive : TrapBase
    {
        public override void Trigger()
        {
            Rb.AddForce((transform.position - PlayerController.Instance.transform.position).normalized * 10f,
                ForceMode.Impulse);
            Destroy(gameObject, 2f);
        }

    }
}
