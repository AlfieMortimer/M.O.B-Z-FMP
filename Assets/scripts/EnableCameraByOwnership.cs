using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnableCameraByOwnership : NetworkBehaviour
{
    public NetworkManager NM;
    public GameObject myCamera;
    public NetworkObject NetworkSelf;
    public NetworkFunctions NetworkFunctions;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NetworkSelf = GetComponent<NetworkObject>();
        NetworkFunctions = GameObject.FindGameObjectWithTag("NetworkFunctions").GetComponent<NetworkFunctions>();
        NM = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        print("New Scene Loaded");

        int activeScene = SceneManager.GetActiveScene().buildIndex;
        if (activeScene != 0)
        {
            NetworkFunctions.menuScene = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            NetworkFunctions.menuScene = true;
        }

        if (NetworkFunctions == null)
        {
            NetworkFunctions = GameObject.FindGameObjectWithTag("NetworkFunctions").GetComponent<NetworkFunctions>();
            
        }

        if (NetworkSelf.IsOwner && NetworkFunctions.menuScene == false)
        {
            print("IsOwner: " + NetworkSelf.IsOwner + " - MenuScene: " + NetworkFunctions.menuScene);
            myCamera.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            print("IsOwner: " + NetworkSelf.IsOwner + " - MenuScene: " + NetworkFunctions.menuScene);
            myCamera.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if(NetworkFunctions == null)
        {
            NetworkFunctions = GameObject.FindGameObjectWithTag("NetworkFunctions").GetComponent<NetworkFunctions>();
        }

        if (NetworkSelf.IsOwner && NetworkFunctions.menuScene == false)
        {
            myCamera.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;

        }
        else
        {
            myCamera.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.None;

        }
    }
}
