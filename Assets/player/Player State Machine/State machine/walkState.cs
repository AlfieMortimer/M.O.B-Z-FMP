using player;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
namespace player
{

    public class walkState : StateP
    {

        public walkState(playerscript pl, StateMachineP sm) : base(pl, sm)
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
            pl.MyInput();

            // calculate movement direction
            pl.moveDirection = pl.orientation.forward * pl.verticalInput + pl.orientation.right * pl.horizontalInput;

            // on slope
            if (pl.OnSlope() && !pl.exitingSlope)
            {
                pl.rb.AddForce(pl.GetSlopeMoveDirection(pl.moveDirection) * pl.moveSpeed * 20f, ForceMode.Force);

                if (pl.rb.linearVelocity.y > 0)
                    pl.rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }

            // on ground
            else if (pl.grounded)
                pl.rb.AddForce(pl.moveDirection.normalized * pl.moveSpeed * 10f, ForceMode.Force);

            // in air
            else if (!pl.grounded)
                pl.rb.AddForce(pl.moveDirection.normalized * pl.moveSpeed * 10f * pl.airMultiplier, ForceMode.Force);

            // turn gravity off while on slope
            pl.rb.useGravity = !pl.OnSlope();
        }

        public override void PhysicsUpdate()
        {
        
        }

        public void StateChange()
        {
            if (pl.verticalInput == 0 && pl.horizontalInput == 0)
            {
                sm.ChangeState(pl.Is);
            }
        }
    }
}
