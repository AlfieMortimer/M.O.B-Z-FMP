using Newtonsoft.Json.Bson;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using static PlayerMovementAdvanced;

public class PlayerHealth : NetworkBehaviour
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

    IEnumerator KnockedState()
    {
        Debug.Log("knocked: Enter");
        while (state == State.Knocked)
        {
            transform.localScale = new Vector3(transform.localScale.x, .60f, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            pma.desiredMoveSpeed = pma.crouchSpeed;

            KnockedHealth.Value -= Time.deltaTime;
            yield return 0;

            if(health.Value > 0)
            {
                state = State.Alive;
                transform.localScale = new Vector3(transform.localScale.x, 1f, transform.localScale.z);
                knocked = false;
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

    private void Update()
    {
        revivePlayer();
    }

    public void revivePlayer()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            health.Value = 50;
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
        if (KnockedHealth.Value <= 0)
        {
            state = State.Dead;
        }
    }
}
