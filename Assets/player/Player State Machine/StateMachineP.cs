using UnityEngine;


namespace player
{
    public class StateMachineP : MonoBehaviour
    {
        public StateP CurrentState { get; private set; }
        public StateP LastState { get; private set; }

        public void Init(StateP startingState)
        {
            CurrentState = startingState;
            LastState = null;
            startingState.Enter();
        }

        public void ChangeState(StateP newState)
        {
            //Debug.Log("Changing state to " + newState);
            CurrentState.Exit();

            LastState = CurrentState;
            CurrentState = newState;
            newState.Enter();

            
        }

        public StateP GetState()
        {
            return CurrentState;
        }

    }
}