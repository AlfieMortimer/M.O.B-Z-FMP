using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField]
    NetworkVariable<int> health = new NetworkVariable<int>();
    NetworkVariable<float> knockedHealh = new NetworkVariable<float>();

    bool knocked = false;

    public enum State
    {
        Alive,
        Knocked,
        Dead,
    }

    public State state;

    IEnumerator KnockedState()
    {
        Debug.Log("knocked: Enter");
        while (state == State.Knocked && IsServer)
        {
            knockedHealh.Value -= Time.deltaTime;
            yield return 0;

            KillPlayer();
        }
        Debug.Log("Knocked: Exit");
        NextState();
    }

    IEnumerator AliveState()
    {
        Debug.Log("Alive: Enter");
        while (state == State.Alive && IsServer)
        {
            //check for alive. if not go to knocked state
            yield return 0;
        }
        Debug.Log("Alive: Exit");
        NextState();
    }
    private void Awake()
    {
        health.Value = 50;
        knockedHealh.Value = 100;
    }

    private void Start()
    {
        state = State.Alive;
        NextState();
    }

    [Rpc(SendTo.Server)]
    public void LoseHealthRPC(int damage)
    {
        health.Value -= damage;
        Debug.Log(health.Value);
    }

    private void Update()
    {
        if (health.Value <= 0 && knocked == false)
        {

        }
    }

    private void OnTriggerEnter(Collider other)
    {

        Debug.Log(other.gameObject.layer);
        if (other.gameObject.layer == 9)
        {
            gameObject.GetComponent<PlayerHealth>().LoseHealthRPC(other.gameObject.GetComponentInParent<BasicEnemyStats>().damage);
            Debug.Log("Damage?");
        }
    }


    void NextState()
    {
        string methodName = state.ToString() + "State";
        System.Reflection.MethodInfo info = GetType().GetMethod(methodName,System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        StartCoroutine((IEnumerator)info.Invoke(this, null));
    }

    void KillPlayer()
    {
        if (knockedHealh.Value <= 0)
        {
            state = State.Dead;
        }
    }
}
