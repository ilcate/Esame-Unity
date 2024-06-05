using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerShooting : NetworkBehaviour
{
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private Transform shootTransform;

    [SerializeField] private List<GameObject> spawnedFireBalls = new List<GameObject>();

    private void Update()
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
        Debug.Log("Shoot");
        GameObject go = Instantiate(fireballPrefab, shootTransform.position, shootTransform.rotation);
        spawnedFireBalls.Add(go);

        // Set the parent reference and initial velocity 
        ProjectileMove projectileMove = go.GetComponent<ProjectileMove>();
        projectileMove.parent = this;
        projectileMove.Initialize(shootTransform.forward);  // Initialize with the forward direction of the shoot transform 

        // Spawn the networked object 
        go.GetComponent<NetworkObject>().Spawn(true); // Ensuring ownership is set correctly 
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyServerRpc()
    {
        if (spawnedFireBalls.Count > 0)
        {
            GameObject toDestroy = spawnedFireBalls[0];
            toDestroy.GetComponent<NetworkObject>().Despawn();
            spawnedFireBalls.RemoveAt(0);
            Destroy(toDestroy);
        }
    }
}