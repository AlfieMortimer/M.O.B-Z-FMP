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
        nM.SceneManager.LoadScene(SceneName, LoadSceneMode.Single);
    }

}
