using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonSwitch : MonoBehaviour
{
    NetworkManager nM;
    PointsCollection pc;
    RoundCounter rc;


    public void Start()
    {
        nM = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>();
        pc = GameObject.FindGameObjectWithTag("NetworkFunctions").GetComponent<PointsCollection>();
        rc = GameObject.FindGameObjectWithTag("NetworkFunctions").GetComponent<RoundCounter>();

        Debug.Log(pc.ToString());
    }
    public void LoadScene(string SceneName)
    {
        SceneManager.LoadScene(SceneName, LoadSceneMode.Single);
    }
    public void ShutDownServer()
    {
        if (nM != null)
        {
            if (nM.IsServer)
            {
                NetworkManager.Singleton.Shutdown();
            }
        }
    }

    public void ClientDisconnect()
    {
        if (nM != null)
        {
            if (nM.IsConnectedClient)
            {
                NetworkManager.Singleton.DisconnectClient(nM.LocalClientId);
            }
        }
    }

    public void ResetGame()
    {
        pc.playerPoints.Clear();
        rc.ResetRound();
        pc.complete = false;
    }
}
