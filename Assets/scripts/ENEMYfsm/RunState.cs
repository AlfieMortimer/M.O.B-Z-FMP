
using Enemy;
using UnityEngine;
namespace Enemy
{
    public class RunState : State
    {
        // constructor
        public RunState(BasicEnemyStats enemy, StateMachine sm) : base(enemy, sm)
        {
        }

        public override void Enter()
        {
            base.Enter();
            enemy.animator.Play("Run", 0, 0);
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
            CheckState();
            //animspeed();
            base.LogicUpdate();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
        void CheckState()
        {
            if (enemy.en.agent.remainingDistance < 1.5)
            {
                sm.ChangeState(enemy.attackState);
            }
            else if (enemy.en.agent.velocity.magnitude <= .5)
            {
                sm.ChangeState(enemy.idleState);
            }

        }

        /* void animspeed()
        {
            float velocity = enemy.en.agent.velocity.magnitude / enemy.en.agent.speed;
            enemy.animator.speed = velocity;
           
        } */
    }
}