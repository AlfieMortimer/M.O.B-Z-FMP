using UnityEngine;
using Unity.Netcode;
using TMPro;
using System;

public class UiPointsClient : NetworkBehaviour
{
    public TextMeshProUGUI p1UI, p2UI, p3UI, P4UI;
    public PointsCollection allPoints;

    public NetworkObject playerOwner;
    public PointsCollection pointsCollection;
    private void Start()
    {
        allPoints = GameObject.FindGameObjectWithTag("NetworkFunctions").GetComponent<PointsCollection>();
    }
    private void Update()
    {
        if (allPoints != null)
        {
             // p1UI.text = allPoints.playerPoints[Convert.ToInt32(OwnerClientId.ToString())].ToString();
              //p2UI.text = allPoints.playerPoints[1].ToString();


        }
    }
}
