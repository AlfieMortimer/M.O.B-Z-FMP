using UnityEngine;
using Unity.Netcode;

public class BasicEnemyStats : NetworkBehaviour
{
    [SerializeField]
    int health;
    bool respawning;
    MeshRenderer mr;
    PointsCollection points;
    public int worth;
    UiPointsClient UIPoints;

    private void Start()
    {
        health = 10;
        mr = gameObject.GetComponent<MeshRenderer>();
        UIPoints = GameObject.FindWithTag("Player").GetComponent<UiPointsClient>();
        points = GameObject.FindWithTag("NetworkFunctions").GetComponent<PointsCollection>();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void TakeDamageRpc(int ClientID, int damage)
    {
        health -= damage;

        if (health <= 0 && !respawning)
        {
            if (IsServer)
            {
                points.collectPointsRpc(ClientID, worth);
            }
            EnemyDeathRPC();

        }
    }

    [Rpc(SendTo.Server)]
    public void EnemyDeathRPC()
    {
        var instanceNetworkObject = gameObject.GetComponent<NetworkObject>();
        instanceNetworkObject.Despawn();
    }
}
