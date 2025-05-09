using UnityEngine;
using Unity.Netcode;
using Enemy;
using UnityEngine.InputSystem.LowLevel;


public class BasicEnemyStats : NetworkBehaviour
{
    [SerializeField]
    int health;
    bool respawning;
    MeshRenderer mr;
    PointsCollection points;
    public int worth;
    UiPointsClient UIPoints;
    RoundCounter rc;
    public EnemyNavigation en;
    public int damage;

    public StateMachine sm;

    public Animator animator;

    // variables holding the different player states
    public IdleState idleState;
    public RunState runState;
    public AttackState attackState;

    //public deathState deathState;

    private void Start()
    {
        sm = gameObject.AddComponent<StateMachine>();
        en = gameObject.GetComponent<EnemyNavigation>();
        mr = gameObject.GetComponent<MeshRenderer>();
        UIPoints = GameObject.FindWithTag("Player").GetComponent<UiPointsClient>();
        points = GameObject.FindWithTag("NetworkFunctions").GetComponent<PointsCollection>();
        rc = GameObject.FindWithTag("NetworkFunctions").GetComponent<RoundCounter>();

        // add new states here
        idleState = new IdleState(this, sm);
        runState = new RunState(this, sm);
        attackState = new AttackState(this, sm);

        // initialise the statemachine with the default state
        sm.Init(idleState);
    }

    private void Update()
    {
        sm.CurrentState.LogicUpdate();
    }

    void FixedUpdate()
    {
        sm.CurrentState.PhysicsUpdate();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void TakeDamageRpc(int ClientID, int damage)
    {
        health -= damage;

        if (health <= 0 && !respawning)
        {
            if (IsServer)
            {
                rc.currentEnemyCount--;
                points.collectPointsRpc(ClientID, worth);
                EnemyDeathRPC();
            }
            
        }
    }

    [Rpc(SendTo.Server)]
    public void EnemyDeathRPC()
    {
        var instanceNetworkObject = NetworkObject;
        instanceNetworkObject.Despawn();
    }
}
