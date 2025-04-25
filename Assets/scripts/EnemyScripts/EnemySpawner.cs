using Unity.Netcode;
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
        nM = GameObject.FindWithTag("NetworkManager").GetComponent<NetworkManager>();
        roundCounter = GameObject.FindWithTag("NetworkFunctions").GetComponent<RoundCounter>();
    }
    private void Update()
    {
        spawnerDelayRPC();
    }
    [Rpc(SendTo.Server)]
    void SpawnEnemyRPC()
    {
        var instance = Instantiate(enemyPrefab, gameObject.transform.position, Quaternion.identity);
        instance.GetComponent<EnemyNavigation>().FindMySpawn(spawnRoom, this);
        var instanceNetworkObject = instance.GetComponent<NetworkObject>();
        instanceNetworkObject.Spawn();
        roundCounter.currentEnemyCount++;
        roundCounter.enemiesLeftToSpawn--;
    }
    [Rpc(SendTo.Server)]
    void spawnerDelayRPC()
    {
        if (roundStartDelay <= 0)
        {
            if (roundCounter != null && roundCounter.enemiesLeftToSpawn > 0 && roundCounter.currentEnemyCount < roundCounter.zombieLimit)
            {
                SpawnEnemyRPC();
                Debug.Log(roundCounter.enemiesLeftToSpawn);
            }
        }
        else if (roundStartDelay >= 0)
        {
            roundStartDelay -= Time.deltaTime;
            Debug.Log(roundStartDelay);
        }
    }
}
