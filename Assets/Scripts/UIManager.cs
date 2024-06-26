using DilmerGames.Core.Singletons;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : NetworkBehaviour
{
    public static UIManager Instance { get; private set; }

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
    private TextMeshProUGUI WinOrLose;

    [SerializeField]
    private Button restartGame;

    [SerializeField]
    private Button startGame;

    private void Awake()
    {
        Instance = this;
        Cursor.visible = true;
    }

    void Start()
    {
        startGame.gameObject.SetActive(false);
        restartGame.gameObject.SetActive(false);
        WinOrLose.gameObject.SetActive(false);

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
            StartGameServerRpc();
            startGame.gameObject.SetActive(false);
        });

        restartGame?.onClick.AddListener(() =>
        {
            RestartGameServerRpc();
        });
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartGameServerRpc()
    {
        CodeDisplay.gameObject.SetActive(false);
        StartGameClientRpc();
        GameManager.Instance.StartGame();
    }

    [ClientRpc]
    private void StartGameClientRpc()
    {
        CodeDisplay.gameObject.SetActive(false);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RestartGameServerRpc()
    {
        RestartGameClientRpc();
        GameManager.Instance.RestartGame();
    }

    [ClientRpc]
    private void RestartGameClientRpc()
    {
        WinOrLose.gameObject.SetActive(false);
    }

    public void showRestart()
    {
        if (IsHost)
        {
            restartGame.gameObject.SetActive(true);
        }
    }

    public void displayLose()
    {
        WinOrLose.text = "You lose";
        WinOrLose.gameObject.SetActive(true);
    }

    public void displayWin()
    {
        WinOrLose.text = "You win";
        WinOrLose.gameObject.SetActive(true);
    }


    public void UIRestartGame()
    {
        restartGame.gameObject.SetActive(false);

        WinOrLose.gameObject.SetActive(false);
    }
}
