
using Enemy;
using UnityEngine;
namespace Enemy
{
    public class AttackState : State
    {
        // constructor
        public AttackState(BasicEnemyStats enemy, StateMachine sm) : base(enemy, sm)
        {
        }

        public override void Enter()
        {
            base.Enter();
            enemy.animator.speed = 1;
            enemy.animator.Play("Zombie Attack", 0, 0);
            enemy.animator.speed = 1;
            enemy.en.agent.velocity = new Vector3(0,0,0);
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
            checkState();
            base.LogicUpdate();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

        void checkState()
        {
            if (enemy.en.agent.remainingDistance > 1.5)
            {
                sm.ChangeState(enemy.runState);
            }
           /* else if(enemy.en.agent.velocity.magnitude <= .5 || enemy.en.agent.remainingDistance < 1.5)
            {
                sm.ChangeState(enemy.idleState);
            } */
        }
    }
}