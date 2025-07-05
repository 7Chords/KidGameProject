using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KidGame.Core
{

    public class Hammer_Positive : TrapBase
    {
        //public override void Init(TrapData trapData)
        //{
        //    this.trapData = trapData;

        //    stateMachine = PoolManager.Instance.GetObject<StateMachine>();
        //    stateMachine.Init(this);
        //    //³õÊ¼»¯ÎªIdle×´Ì¬
        //    ChangeState(TrapState.NoReady);
        //}
        public override void Trigger()
        {
            Rb.AddForce((transform.position - interactor.transform.position).normalized * 100f,
                ForceMode.Impulse);
        }

    }
}
