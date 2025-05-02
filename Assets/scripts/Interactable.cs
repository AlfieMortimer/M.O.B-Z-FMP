using UnityEngine;
using Unity.Netcode;
public class Interactable : NetworkBehaviour
{
    public GameObject myTrigger;
    public int cost;
    public bool interactable;
    public GameObject[] mySpawners;

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
        foreach(GameObject spawner in mySpawners)
        {
            spawner.SetActive(true);
        }
        
        myTrigger.SetActive(false);
        gameObject.SetActive(false);
    }
}
