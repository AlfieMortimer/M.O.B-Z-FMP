using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
public class RoundCounter : NetworkBehaviour
{
    //Synced round count
    public NetworkVariable<int> currentRound = new NetworkVariable<int>();
    public int enemiesLeftToSpawn;
    public int enemiesLeftToSpawnDefault;
    public int playerCount;
    public int zombieLimit = 32;
    public bool gameStart = false;

    public int currentEnemyCount;
    private void Start()
    {
        currentRound.Value = 1;
    }

    private void Update()
    {
        if (IsServer)
        {
            EndRoundCheckRPC();
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name != "Map01")
        {
            enemiesLeftToSpawn = enemiesLeftToSpawnDefault;
            gameStart = false;
        }
    }

    //Only server athority can change network variables
    [Rpc(SendTo.Server)]
    public void IncreaseRoundsRPC()
    {
        currentRound.Value++;
    }

    [Rpc(SendTo.Server)]
    void EndRoundCheckRPC()
    {
        if (gameStart)
        {
            //Debug.Log($"checking for round end: CurrentEnemyCount - {currentEnemyCount} + enemies left to spawn {enemiesLeftToSpawn}");
            if (enemiesLeftToSpawn <= 0 && currentEnemyCount <= 0)
            {
                //Debug.Log("Next Round Started");
                //Debug.Log(currentRound.Value);
                //IncreaseRoundsRPC();
                OnRoundEndRPC();
            }
        }
    }
    [Rpc(SendTo.Server)]
    public void OnRoundEndRPC()
    {
        playerCount = NetworkManager.ConnectedClients.Count;
        float playerValue = 1.5f;
        float enemies;
        if (playerCount >= 3)
        {
            playerValue = 1;
        }
        else if (playerCount <= 1)
        {
            playerValue = 2.5f;
        }
        enemies = (playerCount * playerValue) * currentRound.Value + 6;
        enemiesLeftToSpawn = Convert.ToInt32(Mathf.Round(enemies));
        currentRound.Value++;
    }
}
