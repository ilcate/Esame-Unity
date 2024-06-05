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
            ShootServerRpc();            
        }
    }

    [ServerRpc]
    private void ShootServerRpc()
    {
        GameObject go = Instantiate(fireball, shootTransform.position, shootTransform.rotation);
        shootList.Add(go);
        go.GetComponent<projectileMove>().parent = this;
        go.GetComponent<NetworkObject>().Spawn();
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyServerRpc()
    {
        GameObject toDestroy = shootList[0];
        toDestroy.GetComponent<NetworkObject>().Despawn();
        shootList.Remove(toDestroy);
        Destroy(toDestroy);
    }
}
