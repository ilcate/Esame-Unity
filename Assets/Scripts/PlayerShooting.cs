using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerShooting : NetworkBehaviour
{
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private Transform shootTransform;

    private List<GameObject> shootList = new List<GameObject>();

    private void Update()
    {
        if(!IsOwner) enabled = false;
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Shoot");
            ShootServerRpc();
        }
    }

    [ServerRpc]
    private void ShootServerRpc()
    {
        GameObject fireball = Instantiate(fireballPrefab, shootTransform.position, shootTransform.rotation);
        NetworkObject networkObject = fireball.GetComponent<NetworkObject>();
        networkObject.Spawn();
        //fireball.GetComponent<ProjectileMove>().parent = this;
        //CIAO
        shootList.Add(fireball);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyServerRpc()
    {
 
            GameObject toDestroy = shootList[0];
            shootList.RemoveAt(0);
            NetworkObject networkObject = toDestroy.GetComponent<NetworkObject>();
            networkObject.Despawn();
            Destroy(toDestroy);
 
    }

}
