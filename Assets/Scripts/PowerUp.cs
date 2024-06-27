using Unity.Netcode;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerUp : NetworkBehaviour
{
    public string powerUpName;
    public int ammo;

    public static PowerUp Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HandleCollisionServerRpc(other.GetComponent<NetworkObject>().NetworkObjectId, powerUpName, ammo);
            gameObject.SetActive(false);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void HandleCollisionServerRpc(ulong playerNetworkObjectId, string powerUpName, int ammo)
    {
        var playerObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerNetworkObjectId];
        if (playerObject != null)
        {
            var playerShooting = playerObject.GetComponent<PlayerShooting>();
            if (playerShooting != null)
            {
                playerShooting.SetShootType(powerUpName, ammo);
            }
            Destroy(gameObject);
        }
    }

    [ServerRpc]
    public void SpawnRandomPowerUpServerRpc(int powerUpType)
    {
        GameObject prefabToSpawn = powerUpType == 0 ? GameManager.Instance.MultiShotPrefab : GameManager.Instance.SplitShotPrefab;

        Vector3 spawnPosition = new Vector3(
            Random.Range(-3f, 2.5f),
            1f,
            Random.Range(1f, -6f)
        );

        GameObject powerUp = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        powerUp.GetComponent<NetworkObject>().Spawn();

        GameManager.Instance.activePowerUps.Add(powerUp);

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
            GameManager.Instance.activePowerUps.Remove(powerUp);
        }
    }
}
