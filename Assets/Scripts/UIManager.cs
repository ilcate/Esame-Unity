using DilmerGames.Core.Singletons;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField]
    private Button startServerButton;

    [SerializeField]
    private Button startHostButton;

    [SerializeField]
    private Button startClientButton;

    [SerializeField]
    private TextMeshProUGUI playersInGameText;

    [SerializeField]
    private TMP_InputField joinCodeInput;

    [SerializeField]
    private Button executePhysicsButton;


    private void Awake()
    {
        Cursor.visible = true;
    }

 

    void Start()
    {
<<<<<<< HEAD
       
=======
        // START SERVER
        startServerButton?.onClick.AddListener(() =>
        {
            if (NetworkManager.Singleton.StartServer())
                Debug.Log("Server started...");
            else
                Debug.Log("Server started...");
        });
>>>>>>> 2b1a2a97473ec55908fa42093a3922ca61dbc25f

        // START HOST
        startHostButton?.onClick.AddListener(async () =>
        {
            // this allows the UnityMultiplayer and UnityMultiplayerRelay scene to work with and without
            // relay features - if the Unity transport is found and is relay protocol then we redirect all the 
            // traffic through the relay, else it just uses a LAN type (UNET) communication.
            if (RelayManager.Instance.isRelayEnabled)
                await RelayManager.Instance.SetupRelay();

            if (NetworkManager.Singleton.StartHost())
                Debug.Log("Host started...");
            else
                Debug.Log("Unable to start host...");
        });

        // START CLIENT
        startClientButton?.onClick.AddListener(async () =>
        {
            if (RelayManager.Instance.isRelayEnabled && !string.IsNullOrEmpty(joinCodeInput.text))
                await RelayManager.Instance.JoinRelay(joinCodeInput.text);

            if (NetworkManager.Singleton.StartClient())
                Debug.Log("Client started...");
            
            else
                Debug.Log("Unable to start client...");
        });
        // STATUS TYPE CALLBACKS
        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            Debug.Log($"{id} just connected...");
        };

        

    }
}