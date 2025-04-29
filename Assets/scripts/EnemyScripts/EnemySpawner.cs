using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;

public class EnemySpawner : NetworkBehaviour
{
    //Spawning mechanics
    public GameObject enemyPrefab;
    public bool spawnRoom;
    public GameObject spawnRoomExit;

    //Round Checks
    public RoundCounter roundCounter;

    //Timer and spawning
    [SerializeField] float roundStartDelay;
    [SerializeField] float enemySpawnDelay;
    [SerializeField] NetworkManager nM;

    private void Start()
    {
        if (IsHost)
        {
        roundCounter = GameObject.FindWithTag("NetworkFunctions").GetComponent<RoundCounter>();
        }
        nM = GameObject.FindWithTag("NetworkManager").GetComponent<NetworkManager>();
    }
    private void Update()
    {
        spawnerDelayRPC();
    }
    [Rpc(SendTo.ClientsAndHost)]
    void SpawnEnemyRPC()
    {
        //var instance = Instantiate(enemyPrefab, gameObject.transform.position, Quaternion.identity);
        //instance.GetComponent<EnemyNavigation>().FindMySpawn(spawnRoom, this);
        //var instanceNetworkObject = instance.GetComponent<NetworkObject>();
        if (IsServer)
        {
            //instanceNetworkObject.Spawn();
            NetworkObject.InstantiateAndSpawn(enemyPrefab, nM, 0, false, false, false, transform.position, Quaternion.identity);
            roundCounter.currentEnemyCount++;
            roundCounter.enemiesLeftToSpawn--;
            enemySpawnDelay = 5;
        }
    }
    [Rpc(SendTo.Server)]
    void spawnerDelayRPC()
    {
        if (roundStartDelay <= 0)
        {
            if (roundCounter != null && roundCounter.enemiesLeftToSpawn > 0 && roundCounter.currentEnemyCount < roundCounter.zombieLimit)
            {
                if (enemySpawnDelay <= 0)
                {
                    SpawnEnemyRPC();
                    Debug.Log(roundCounter.enemiesLeftToSpawn);
                }
                else
                {
                    enemySpawnDelay -= Time.deltaTime;
                }
            }
        }
        else if (roundStartDelay >= 0)
        {
            roundStartDelay -= Time.deltaTime;
            Debug.Log(roundStartDelay);
        }
    }
}
