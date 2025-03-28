using UnityEngine;
using Unity.Netcode;
public class PointsCollection : NetworkBehaviour
{
    
    public int[] playerPoints;

    [Rpc(SendTo.ClientsAndHost)]
    public void collectPointsRpc(ulong clientID, int pointsToChange)
    {
        playerPoints[clientID] += pointsToChange;
        Debug.Log(clientID + " has " + playerPoints[clientID] + " Points");

    }

    [Rpc(SendTo.Server)]
    public void checkPointsRpc(int pointsChecking, ulong ClientID)
    {
        if (playerPoints[ClientID] >= pointsChecking)
        {
            
        }
        else
        {

        }
    }

}
