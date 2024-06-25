using Unity.Netcode;
using UnityEngine;

public class PowerUp : NetworkBehaviour
{
    public string powerUpName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HandleCollisionServerRpc(other.GetComponent<NetworkObject>().NetworkObjectId, powerUpName);

            gameObject.SetActive(false);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void HandleCollisionServerRpc(ulong playerNetworkObjectId, string powerUpName)
    {
        var playerObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerNetworkObjectId];

        if (playerObject != null)
        {

            var playerShooting = playerObject.GetComponent<PlayerShooting>();
            if (playerShooting != null)
            {
                playerShooting.SetShootType(powerUpName);
            }

            Destroy(gameObject);
        }
    }
}
