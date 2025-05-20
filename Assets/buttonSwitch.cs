using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonSwitch : MonoBehaviour
{
    NetworkManager nM;
    public void Start()
    {
        nM = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>();
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
}
