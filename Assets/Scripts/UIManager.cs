using DilmerGames.Core.Singletons;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : NetworkBehaviour
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

    [SerializeField]
    private TextMeshProUGUI CodeDisplay;

    [SerializeField]
    private Button startGame;

    private void Awake()
    {
        Cursor.visible = true;
    }

    void Start()
    {
        startGame.gameObject.SetActive(false);

        startHostButton?.onClick.AddListener(async () =>
        {
            if (RelayManager.Instance.isRelayEnabled)
                await RelayManager.Instance.SetupRelay();

            if (NetworkManager.Singleton.StartHost())
            {
                Debug.Log("Host started...");
            }
            else
            {
                Debug.Log("Unable to start host...");
            }

            startGame.gameObject.SetActive(true);
            startHostButton.gameObject.SetActive(false);
            startClientButton.gameObject.SetActive(false);
            joinCodeInput.gameObject.SetActive(false);
            CodeDisplay.text = RelayManager.Instance.code;
        });

        startClientButton?.onClick.AddListener(async () =>
        {
            if (RelayManager.Instance.isRelayEnabled && !string.IsNullOrEmpty(joinCodeInput.text))
                await RelayManager.Instance.JoinRelay(joinCodeInput.text);

            if (NetworkManager.Singleton.StartClient())
            {
                Debug.Log("Client started...");
            }
            else
            {
                Debug.Log("Unable to start client...");
            }

            CodeDisplay.text = RelayManager.Instance.code;
            startHostButton.gameObject.SetActive(false);
            startClientButton.gameObject.SetActive(false);
            joinCodeInput.gameObject.SetActive(false);
        });

        startGame?.onClick.AddListener(() =>
        {
            GameManager.Instance.StartGame();
            StartGameServerRpc();
            startGame.gameObject.SetActive(false);
        });
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartGameServerRpc()
    {
        GameManager.Instance.StartGame();
        CodeDisplay.gameObject.SetActive(false);
    }

    
}
