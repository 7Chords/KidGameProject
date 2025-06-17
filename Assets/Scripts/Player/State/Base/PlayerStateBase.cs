using System.Collections;
using System.Collections.Generic;
using KidGame.Core;
using UnityEngine;

namespace KidGame.Core
{
    public class PlayerStateBase : StateBase
    {
        protected PlayerController player;

        public override void Init(IStateMachineOwner owner, int stateType, StateMachine stateMachine)
        {
            base.Init(owner, stateType, stateMachine);
            player = (PlayerController)owner;
        }

        public override void Update()
        {
            if (player.InputSettings.GetInteractDown())
            {
                player.PlayerInteraction();
            }
        }
    }
}