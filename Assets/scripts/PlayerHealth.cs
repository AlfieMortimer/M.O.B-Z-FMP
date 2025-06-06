using Newtonsoft.Json.Bson;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using static PlayerMovementAdvanced;

public class PlayerHealth : NetworkBehaviour, IInteractable
{
    [SerializeField]
    NetworkVariable<int> health = new NetworkVariable<int>();
    NetworkVariable<float> KnockedHealth = new NetworkVariable<float>();

    //work out how to change network variable write permissions.
    //alternatively change health in a server RPC method. Probably easier.

    Rigidbody rb;
    PlayerMovementAdvanced pma;
    bool knocked = false;

    public enum State
    {
        Alive,
        Knocked,
        Dead,
    }

    public State state;

    public void Start()
    {
        
        rb = GetComponent<Rigidbody>();
        pma = GetComponent<PlayerMovementAdvanced>();
        state = State.Alive;
        NextState();
    }

    public void Interact(GameObject p)
    {
        if(state == State.Knocked)
        {
            revivePlayerRPC();
        }
    }

    IEnumerator KnockedState()
    {
        Debug.Log("knocked: Enter");
        while (state == State.Knocked)
        {
            transform.localScale = new Vector3(transform.localScale.x, .60f, transform.localScale.z);
            pma.readyToJump = false;
            pma.desiredMoveSpeed = 0;

            KnockedHealthRPC();
            yield return 0;

            if(health.Value > 0)
            {
                state = State.Alive;
                transform.localScale = new Vector3(transform.localScale.x, 1f, transform.localScale.z);
                knocked = false;
                pma.readyToJump = true;
            }

            KillPlayer();

        }
        Debug.Log("Knocked: Exit");
        NextState();
    }

    IEnumerator AliveState()
    {
        Debug.Log("Alive: Enter");
        while (state == State.Alive)
        {
            if (health.Value <= 0 && knocked == false)
            {
                state = State.Knocked;
                knocked = true;
            }
            //check for alive. if not go to knocked state
            yield return 0;
        }
        Debug.Log("Alive: Exit");
        NextState();
    }

    IEnumerator DeathState()
    {
        Debug.Log("Death: Enter");
        while (state == State.Dead)
        {

            yield return 0;
        }
        Debug.Log("Death: Exit");
        NextState();
    }
    private void Awake()
    {
        health.Value = 50;
        KnockedHealth.Value = 20;
    }


    [Rpc(SendTo.Server)]
    public void LoseHealthRPC(int damage)
    {
        health.Value -= damage;
        Debug.Log(health.Value);
    }


    [Rpc(SendTo.Server)]
    public void revivePlayerRPC()
    {
            health.Value = 50;
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
        /*if (KnockedHealth.Value <= 0)
        {
            state = State.Dead;
        } */
    }

    [Rpc(SendTo.Server)]
    public void KnockedHealthRPC()
    {
        KnockedHealth.Value -= Time.deltaTime;
    }
}
