using System;
using Unity.Netcode;
using UnityEngine;
public class Interactable : NetworkBehaviour, IInteractable
{
    public int cost;
    public bool interactable;
    public GameObject[] mySpawners;

    public PointsCollection points;
    public UiPointsClient UIPoints;

    public void Interact(GameObject p)
    {
        removeDoorRpc();
        PlayerWeapons weapons = p.GetComponent<PlayerWeapons>();

        weapons.am.playsfx(weapons.am.sounds[5]);

    }
    private void Start()
    {
        points = GameObject.FindWithTag("NetworkFunctions").GetComponent<PointsCollection>();
        UIPoints = GameObject.FindWithTag("Player").GetComponent<UiPointsClient>();
        
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void removeDoorRpc()
    {
        //PointsCollection points = GameObject.FindWithTag("NetworkFunctions").GetComponent<PointsCollection>();
        if (cost <= points.playerPoints[Convert.ToInt32(OwnerClientId.ToString())])
        {
            
            points.collectPointsRpc(Convert.ToInt32(OwnerClientId.ToString()), -cost);
            foreach (GameObject spawner in mySpawners)
            {
                spawner.SetActive(true);
            }

            RemoveDoorModelRPC();
        }

    }

    [Rpc(SendTo.ClientsAndHost)]
    public void RemoveDoorModelRPC()
    {
        gameObject.SetActive(false);
    }
}
