using System;
using Unity.Netcode;
using UnityEngine;
public class RoundCounter : NetworkBehaviour
{
    //Synced round count
    public NetworkVariable<int> currentRound = new NetworkVariable<int>();
    public int enemiesLeftToSpawn;
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
        endRoundCheckRPC();
    }

    //Only server athority can change network variables
    [Rpc(SendTo.Server)]
    public void IncreaseRoundsRPC()
    {
        currentRound.Value++;
    }

    [Rpc(SendTo.Server)]
    void endRoundCheckRPC()
    {
        if(enemiesLeftToSpawn <= 0 && currentEnemyCount <= 0)
        {
            IncreaseRoundsRPC();
            OnRoundEnd();
        }
    }
    public void OnRoundEnd()
    {
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
        enemiesLeftToSpawn = Convert.ToInt32(Mathf.Round(enemiesLeftToSpawn));
    }
}
