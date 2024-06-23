using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    public NetworkVariable<bool> inGame = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);

    private CameraManager cameraManager;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        cameraManager = FindObjectOfType<CameraManager>();
    }

    public void StartGame()
    {
        inGame.Value = true;
        TeleportAllPlayers();
        Debug.Log("Game started!");
    }

    private void TeleportAllPlayers()
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            var playerMove = client.PlayerObject.GetComponent<PlayerMove>();
            if (playerMove != null)
            {
                playerMove.TpToMapClientRpc();
            }
        }
    }

    public Task ActivateGameCam()
    {
        return cameraManager.ActivateGameCam();
    }
}
