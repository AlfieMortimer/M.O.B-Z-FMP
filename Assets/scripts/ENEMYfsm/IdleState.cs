
using Enemy;
using Unity.VisualScripting;
using UnityEngine;
namespace Enemy
{
    public class IdleState : State
    {
        // constructor
        public IdleState(BasicEnemyStats enemy, StateMachine sm) : base(enemy, sm)
        {
        }

        public override void Enter()
        {
            base.Enter();
            enemy.animator.Play("Idle", 0, 0);
            enemy.animator.speed = 1;

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
            if (enemy.en.agent.velocity.magnitude > .5)
            {
                sm.ChangeState(enemy.runState);
            }
        }
    }
}