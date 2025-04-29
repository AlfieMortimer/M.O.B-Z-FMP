using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Multiplayer.Widgets;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.Collections;

public class NetworkFunctions : NetworkBehaviour
{
    NetworkManager nM;
    public bool menuScene;
    string lastWorkingCode;
    TextMeshProUGUI codeText;
    //Singleton Creation
    public static NetworkFunctions instance;
    RoundCounter roundCounter;

    //Singleton Below
    void Awake()
    {

        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    public void OnLobbyJoin(TextMeshProUGUI menuCodeText)
    {
        StartCoroutine(GetCode(menuCodeText));
        
    }

    IEnumerator GetCode(TextMeshProUGUI menuCodeText)
    {
        while (lastWorkingCode == null || lastWorkingCode == "Code")
        {
            lastWorkingCode = menuCodeText.text;

            if (lastWorkingCode != null && lastWorkingCode != "Code")
            {
                Debug.LogWarning("The server code is: " + lastWorkingCode);
            }

            yield return new WaitForSeconds(0.01f);
        }



    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Optomise at some point
        if (GameObject.Find("CodeDisplay") && menuScene == false)
        {
            codeText = GameObject.Find("CodeDisplay").GetComponent<TextMeshProUGUI>();
            codeText.text = "Code: " + lastWorkingCode;
        }
        //And this gameobject find needs optomising at some point
        /*else if (menuScene == true && GameObject.Find("CreateButton"))
        {
            
        }*/
    }

    void Start()
    {
        nM = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>();
        roundCounter = gameObject.GetComponent<RoundCounter>();
    }

    private void Update()
    {
        //Testing Only remove after.
        if (Input.GetKeyDown("0") && nM.IsHost)
        {
            SendClientsToNewScene("ENemyTesting");
            //SendClientsToNewScene("LoadnewSceneTests");
            menuScene = false;
        }
    }

    //sends all clients to a new scene and spawns new players if needed
    public void SendClientsToNewScene(string sceneName)
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        Debug.Log(nM.IsHost);
        if (nM.IsHost)
        {
            nM.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            menuScene = false;
            roundCounter.gameStart = true;
            Debug.Log("Host has loaded menuScene on false");
        }
        else if (!nM.IsHost)
        {
            menuScene = false;
            Debug.Log("Client has loaded menuScene on false");

        }

    }
}
