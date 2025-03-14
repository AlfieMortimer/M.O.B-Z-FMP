using player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class runState : StateP
{
    public runState(playerscript pl, StateMachineP sm) : base(pl, sm)
    {
    }

    public override void Enter()
    {

    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void HandleInput()
    {
        base.HandleInput();
    }

    public override void LogicUpdate()
    {

    }

    public override void PhysicsUpdate()
    {

    }
}
