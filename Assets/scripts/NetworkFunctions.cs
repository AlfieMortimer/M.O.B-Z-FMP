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
    GameObject[] playerObjects;
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
        playerObjects = GameObject.FindGameObjectsWithTag("Player");
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
        if (menuScene == false)
        {
            checkPlayersAlive();
        }
    }

    //sends all clients to a new scene and spawns new players if needed
    public void SendClientsToNewScene(string sceneName)
    {
        playerObjects = GameObject.FindGameObjectsWithTag("Player");
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

    public void checkPlayersAlive()
    {
        int knockCount = 0;
        int playerCount = 0;
        foreach(GameObject players in playerObjects)
        {
            playerCount++;
            PlayerHealth ph = players.GetComponent<PlayerHealth>();
            Debug.Log($"{playerCount} PlayerCount as shown by variable");

            Debug.Log($"{playerObjects.Length} playerObjects Length");

            if (ph.state == PlayerHealth.State.Knocked)
            {
                knockCount++;
                Debug.Log($"{playerCount} players are present. {knockCount} are currently downed");
            }
            else
            {
                knockCount--;
                Debug.Log($"{playerCount} players are present. {knockCount} are currently downed");
            }

        }
        if (playerCount > 0)
        {
            DeathCheck(knockCount, playerCount);
            Debug.Log("DeathCheck Working");
        }
        
    }
    public void DeathCheck(int knockCount, int playerCount)
    {
        if (knockCount == playerCount && SceneManager.GetActiveScene().name == /*Current Level name*/ "ENemyTesting")
        {
            //End Game
            Debug.Log($"All Players Down {knockCount} : {playerCount}");
            SendClientsToNewScene("DeathScreen");
            menuScene = true;
        }
    }
}
