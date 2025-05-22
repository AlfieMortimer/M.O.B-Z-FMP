using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Networking;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PointsCollection : NetworkBehaviour, IEquatable<int[]>
{
    public NetworkList<int> playerPoints;

    public bool complete = false;
    private void Awake()
    {
        playerPoints = new NetworkList<int>();
    }

    private void Update()
    {
        if ((NetworkManager.IsConnectedClient || NetworkManager.IsHost) && complete == false)
        {
            if (NetworkManager.IsHost)
            {
                playerPoints.Add(0);
                playerPoints.Add(0);
                playerPoints.Add(0);
                playerPoints.Add(0);
            }
            Debug.Log(playerPoints.Count);
            if (playerPoints.Count > 0)
            {
                complete = true;
            }
        }
    }


    [Rpc(SendTo.Server)]
    public void collectPointsRpc(int clientID, int pointsToChange)
    {
       
        //Points.Value[clientID] += pointsToChange;
        playerPoints[clientID] += pointsToChange;
        Debug.Log(clientID + " has " + playerPoints[clientID] + " Points");
        //playerPoints.OnListChanged += OnListChanged;

    }

    public bool Equals(int[] other)
    {
        throw new NotImplementedException();
    }

}
