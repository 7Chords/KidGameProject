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

        private Vector3 dir;
        public override void Trigger()
        {
            trapData.deadDelayTime = Power / PowerCostPerSecond;
            Vector3 noFixDir = (transform.position - interactor.transform.position).normalized;
            dir = new Vector3(noFixDir.x, 0, noFixDir.z);
            transform.LookAt(dir);
            MonoManager.Instance.AddLateUpdateListener(PerformTick);
        }

        public override void Discard()
        {
            MonoManager.Instance.RemoveFixedUpdate(PerformTick);
            base.Discard();
        }

        private void PerformTick()
        {
            if(Rb) Rb.velocity = dir * Speed;
        }

    }
}
