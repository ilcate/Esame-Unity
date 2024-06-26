using Unity.Netcode;
using UnityEngine;

public class PowerUp : NetworkBehaviour
{
    public string powerUpName;
    public int ammo;

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
}
