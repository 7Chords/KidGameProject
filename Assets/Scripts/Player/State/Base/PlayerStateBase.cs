using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateBase : StateBase
{
    protected PlayerController player;
    protected CharacterController characterController;
    public override void Init(IStateMachineOwner owner, int stateType, StateMachine stateMachine)
    {
        base.Init(owner, stateType, stateMachine);
        player = (PlayerController)owner;
        characterController = player.CharacterController;
    }
}
