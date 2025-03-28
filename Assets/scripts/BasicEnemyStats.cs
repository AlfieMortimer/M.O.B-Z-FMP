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

    [Rpc(SendTo.Server)]
    public void TakeDamageRpc(ulong ClientID, int damage)
    {
        health -= damage;

        if (health <= 0 && !respawning)
        {
            mr.enabled = false;
            Invoke("respawnObjectRpc", 2);
            respawning = true;

            points.collectPointsRpc(ClientID, worth);
            UIPoints.UpdatePointsUIRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void respawnObjectRpc()
    {
        health = 5;
        mr.enabled = true;
        respawning = false;
    }
}
