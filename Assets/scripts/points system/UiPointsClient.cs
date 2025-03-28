using UnityEngine;
using Unity.Netcode;
using TMPro;
public class UiPointsClient : NetworkBehaviour
{
    public TextMeshProUGUI p1UI, p2UI, p3UI, P4UI;
    public PointsCollection allPoints;

    public NetworkObject playerOwner;
    private void Start()
    {
        allPoints = GameObject.FindGameObjectWithTag("NetworkFunctions").GetComponent<PointsCollection>();
    }
    [Rpc(SendTo.ClientsAndHost)]
    public void UpdatePointsUIRpc()
    {
        p1UI.text = allPoints.playerPoints[playerOwner.OwnerClientId].ToString();
        p2UI.text = allPoints.playerPoints[playerOwner.OwnerClientId + 1].ToString();


    }
}
