using UnityEngine;
using Unity.Netcode;

public class BasicEnemyStats : NetworkBehaviour
{
    [SerializeField]
    int health;
    bool respawning;
    MeshRenderer mr;

    private void Start()
    {
        health = 10;
        mr = gameObject.GetComponent<MeshRenderer>();
    }
    private void Update()
    {
        if(health <= 0 && !respawning)
        {
            mr.enabled = false;
            Invoke("respawnObjectRpc", 2);
            respawning = true;
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void TakeDamageRpc(int damage)
    {
        health -= damage;
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void respawnObjectRpc()
    {
        health = 5;
        mr.enabled = true;
        respawning = false;
    }
}
