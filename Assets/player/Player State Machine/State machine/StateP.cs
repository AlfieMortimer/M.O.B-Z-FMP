
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace player
{
    public abstract class StateP
    {
        protected playerscript pl;
        protected StateMachineP sm;


        // base constructor
        protected StateP(playerscript player, StateMachineP sm)
        {
            this.pl = player;
            this.sm = sm;
        }

        // These methods are common to all states
        public virtual void Enter()
        {
            //Debug.Log("This is base.enter");
        }

        public virtual void HandleInput()
        {
        }

        public virtual void LogicUpdate()
        {
        }

        public virtual void PhysicsUpdate()
        {
        }

        public virtual void Exit()
        {
        }

    }

}