using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KidGame.Core
{
    public class Hammer_Passive : TrapBase
    {
        //public override void Init(TrapData trapData)
        //{
        //    this.trapData = trapData;

        //    stateMachine = PoolManager.Instance.GetObject<StateMachine>();
        //    stateMachine.Init(this);
        //    //��ʼ��ΪIdle״̬
        //    ChangeState(TrapState.NoReady);
        //}
        public override void Trigger()
        {
            Rb.AddForce((interactor.transform.position - transform.position).normalized * 1000f,
                ForceMode.Impulse);
        }
    }

}
