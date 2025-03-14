using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Vivox;
using Unity.Services.Multiplayer;

public class ChangeGameUsername : MonoBehaviour
{
    public TMP_InputField inputField;
    public LoginOptions options;
    //public ISession Session { get; set; }

    private void Awake()
    {
       
    inputField = GetComponentInChildren<TMP_InputField>();
        options = new LoginOptions();

    }
    void Update()
    {
        inputField.onEndEdit.AddListener(value =>
        {
            if (Input.GetKeyDown(KeyCode.Return) && !string.IsNullOrEmpty(value))
            {
                UpdateUsername();
            }
        });

        void UpdateUsername()
        {
            options.DisplayName = inputField.text;
            Debug.Log(options.DisplayName);
        }
    }
}
