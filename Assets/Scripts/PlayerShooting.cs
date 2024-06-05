using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerShooting : NetworkBehaviour
{
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private Transform shootTransform;

    [SerializeField] private List<GameObject> shootList = new List<GameObject>();

    void Update()
    {
        if (!IsOwner) return;
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Spara");
            ShootServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ShootServerRpc()
    {
        GameObject go = Instantiate(fireballPrefab, shootTransform.position, shootTransform.rotation);
        NetworkObject networkObject = go.GetComponent<NetworkObject>();
        networkObject.Spawn(true);
        go.GetComponent<ProjectileMove>().parent = this;
        shootList.Add(go);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyServerRpc()
    {
        if (shootList.Count > 0)
        {
            GameObject toDestroy = shootList[0];
            shootList.RemoveAt(0);
            NetworkObject networkObject = toDestroy.GetComponent<NetworkObject>();
            networkObject.Despawn(true);
            Destroy(toDestroy);
        }
    }
}
