using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : NetworkBehaviour
{
    public static UIManager Instance { get; private set; }
    public RawImage video;
    public TextMeshProUGUI title;
    public Button AudioOn;
    public Button AudioOff;
    public Button startHostButton;
    public Button startClientButton;
    public TMP_InputField joinCodeInput;
    public TextMeshProUGUI CodeDisplay;
    public TextMeshProUGUI WinOrLose;
    public Button restartGame;
    public Button startGame;
    public TextMeshProUGUI errorMessage;
    public MusicController musicController;

    private void Awake()
    {
        Instance = this;
        Cursor.visible = true;
    }


    [ServerRpc(RequireOwnership = false)]
    private void CheckPlayersCountServerRpc()
    {
        int playerCount = NetworkManager.Singleton.ConnectedClientsList.Count;
        UpdateStartGameButtonClientRpc(playerCount);
    }


    public void DisplayLose()
    {
        WinOrLose.text = "You lose";
        WinOrLose.gameObject.SetActive(true);
    }

    public void DisplayWin()
    {
        WinOrLose.text = "You win";
        WinOrLose.gameObject.SetActive(true);
    }


    [ClientRpc]
    private void RestartGameClientRpc()
    {
        WinOrLose.gameObject.SetActive(false);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RestartGameServerRpc()
    {
        RestartGameClientRpc();
        GameManager.Instance.RestartGame();
    }


    public void ShowRestart()
    {
        if (IsHost)
        {
            restartGame.gameObject.SetActive(true);
        }
    }

    void Start()
    {
        startGame.gameObject.SetActive(false);
        restartGame.gameObject.SetActive(false);
        WinOrLose.gameObject.SetActive(false);
        errorMessage.gameObject.SetActive(false);
        AudioOff.gameObject.SetActive(false);

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

            startHostButton.gameObject.SetActive(false);
            startClientButton.gameObject.SetActive(false);
            joinCodeInput.gameObject.SetActive(false);
            CodeDisplay.text = RelayManager.Instance.code;
            video.gameObject.SetActive(false);
            title.gameObject.SetActive(false);
        });

        startClientButton?.onClick.AddListener(async () =>
        {

            if (!string.IsNullOrEmpty(joinCodeInput.text))
            {
                if (RelayManager.Instance.isRelayEnabled)
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
                errorMessage.gameObject.SetActive(false);
                joinCodeInput.gameObject.SetActive(false);
                video.gameObject.SetActive(false);
                title.gameObject.SetActive(false);

            }
            else
            {
                errorMessage.gameObject.SetActive(true);
                errorMessage.text = "insert a valid code";
            }

            CheckPlayersCountServerRpc();
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

        musicController = FindAnyObjectByType<MusicController>();
        AudioOn?.onClick.AddListener(() =>
        {
            musicController.PauseMusic();
            AudioOn.gameObject.SetActive(false);
            AudioOff.gameObject.SetActive(true);
        });

        AudioOff?.onClick.AddListener(() =>
        {
            musicController.ResumeMusic();
            AudioOff.gameObject.SetActive(false);
            AudioOn.gameObject.SetActive(true);
        });

        NetworkManager.Singleton.OnClientConnectedCallback += (clientId) =>
        {
            CheckPlayersCountServerRpc();
            errorMessage.gameObject.SetActive(false);
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += (clientId) =>
        {
            CheckPlayersCountServerRpc();
        };
    }

    [ClientRpc]
    private void StartGameClientRpc()
    {
        CodeDisplay.gameObject.SetActive(false);
    }


    [ServerRpc(RequireOwnership = false)]
    private void StartGameServerRpc()
    {
        CodeDisplay.gameObject.SetActive(false);
        StartGameClientRpc();
        GameManager.Instance.StartGame();
    }

    public void UIRestartGame()
    {
        restartGame.gameObject.SetActive(false);
        WinOrLose.gameObject.SetActive(false);
    }

    [ClientRpc]
    private void UpdateStartGameButtonClientRpc(int playerCount)
    {
        if (IsHost)
        {
            startGame.gameObject.SetActive(playerCount > 1);
        }

    }

}
