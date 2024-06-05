using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerShooting : NetworkBehaviour
{
    [SerializeField] private GameObject fireball;
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
        GameObject go = Instantiate(fireball, shootTransform.position, shootTransform.rotation);
        shootList.Add(go);
        go.GetComponent<ProjectileMove>().parent = this;
        go.GetComponent<NetworkObject>().Spawn();
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyServerRpc()
    {
        if (shootList.Count > 0)
        {
            GameObject toDestroy = shootList[0];
            toDestroy.GetComponent<NetworkObject>().Despawn();
            shootList.Remove(toDestroy);
            Destroy(toDestroy);
        }
    }
}
