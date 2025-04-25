using UnityEngine;
using Unity.Netcode;
public class Interactable : NetworkBehaviour
{
    public GameObject myTrigger;
    public int cost;
    public bool interactable;

    public PointsCollection points;
    public UiPointsClient UIPoints;
    private void Start()
    {
        points = GameObject.FindWithTag("NetworkFunctions").GetComponent<PointsCollection>();
        UIPoints = GameObject.FindWithTag("Player").GetComponent<UiPointsClient>();
        
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void removeDoorRpc()
    {
        myTrigger.SetActive(false);
        gameObject.SetActive(false);
    }
}
