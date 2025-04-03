using UnityEngine;
using Unity.Netcode;
using Unity.Networking;
using UnityEditor.PackageManager;
using System;
using System.Collections.Generic;

public class PointsCollection : NetworkBehaviour, IEquatable<int[]>
{
    //public NetworkVariable<int[]> Points = new NetworkVariable<int[]>();

    NetworkList<int> playerPoints;

    private void Awake()
    {
        playerPoints = new NetworkList<int>();
    }
    private void Start()
    {
            playerPoints.Add(0);
            playerPoints.Add(0);
            playerPoints.Add(0);
            playerPoints.Add(0);
            Debug.Log(playerPoints.Count);
    }
    private void Update()
    {

    }


    [Rpc(SendTo.Server)]
    public void collectPointsRpc(int clientID, int pointsToChange)
    {
        if (playerPoints.Count-1 <= clientID)
        {
            playerPoints.Add(0);
            Debug.Log(playerPoints.Count);
        }
       
        //Points.Value[clientID] += pointsToChange;
        playerPoints[clientID] += pointsToChange;
        Debug.Log(clientID + " has " + playerPoints[clientID] + " Points");
        //playerPoints.OnListChanged += OnListChanged;

    }

    /*
    private void OnSomeValueChanged(int previous, int current)
    {
        Debug.Log($"Detected NetworkVariable Change: Previous: {previous} | Current: {current}");
    }



    public void OnListChanged(NetworkListEvent<NetworkList<int>> changeEvent)
    {
        Debug.Log("The Value has changed");
    }
   */
    public bool Equals(int[] other)
    {
        throw new NotImplementedException();
    }

}
