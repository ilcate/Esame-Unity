using Unity.Netcode;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    public NetworkVariable<bool> inGame = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);

    private CameraManager cameraManager;

    public GameObject MultiShotPrefab;
    public GameObject SplitShotPrefab;

    private float secondsToWait = 20f;

    private List<GameObject> activePowerUps = new List<GameObject>();

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
        TeleportAllPlayers();

        inGame.Value = true;
        StartCoroutine(SpawnPowerUps());
    }

    public void RestartGame()
    {
        inGame.Value = false;

        foreach (var powerUp in activePowerUps)
        {
            if (powerUp != null)
            {
                powerUp.GetComponent<NetworkObject>().Despawn();
                Destroy(powerUp);
            }
        }
        activePowerUps.Clear();

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            var playerMove = client.PlayerObject.GetComponent<PlayerMove>();
            var playerShooting = client.PlayerObject.GetComponent<PlayerShooting>();

            playerShooting.shootType = "Standard";

            playerMove.RevivePlayers();
            playerShooting.EnableShooting();
        }

        UIManager.Instance.UIRestartGame();
        StartGame();
    }

    private void Update()
    {
        CheckPlayersAlive();
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

    private IEnumerator SpawnPowerUps()
    {
        while (inGame.Value)
        {
            yield return new WaitForSeconds(secondsToWait);

            if (!inGame.Value)
            {
                yield break;
            }

            SpawnRandomPowerUpServerRpc();
            secondsToWait = Random.Range(20f, 40f);
        }
    }

    private void CheckPlayersAlive()
    {
        if (!IsServer) return;

        if (inGame.Value)
        {
            int aliveCount = 0;
            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                var playerMove = client.PlayerObject.GetComponent<PlayerMove>();
                if (playerMove != null && playerMove.isAlive.Value)
                {
                    aliveCount++;
                }
            }

            if (aliveCount <= 1)
            {
                foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
                {
                    var playerMove = client.PlayerObject.GetComponent<PlayerMove>();
                    var playerShooting = client.PlayerObject.GetComponent<PlayerShooting>();

                    playerShooting.DisableShooting();

                    if (playerMove != null)
                    {
                        SendVictoryOrDefeatClientRpc(playerMove.OwnerClientId, playerMove.isAlive.Value);
                    }
                }
            }
        }
    }

    [ClientRpc]
    private void SendVictoryOrDefeatClientRpc(ulong clientId, bool isAlive)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            if (isAlive)
            {
                UIManager.Instance.displayWin();
            }
            else
            {
                UIManager.Instance.displayLose();
            }
        }

        UIManager.Instance.showRestart();
        StopAllCoroutines();
    }

    [ServerRpc]
    private void SpawnRandomPowerUpServerRpc()
    {
        Vector3 spawnPosition = new Vector3(
            Random.Range(-3f, 2.5f),
            1f,
            Random.Range(1f, -6f)
        );

        GameObject prefabToSpawn = Random.value > 0.5f ? MultiShotPrefab : SplitShotPrefab;
        GameObject powerUp = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        powerUp.GetComponent<NetworkObject>().Spawn();

        activePowerUps.Add(powerUp);

        var powerUpComponent = powerUp.GetComponent<PowerUp>();
        powerUpComponent.powerUpName = prefabToSpawn.name;
        powerUpComponent.ammo = 10;

        StartCoroutine(DestroyPowerUpAfterDelay(powerUp, Random.Range(7f, 15f)));
    }

    private IEnumerator DestroyPowerUpAfterDelay(GameObject powerUp, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (powerUp != null && powerUp.GetComponent<NetworkObject>().IsSpawned)
        {
            powerUp.GetComponent<NetworkObject>().Despawn();
            Destroy(powerUp);
            activePowerUps.Remove(powerUp); 
        }
    }
}
