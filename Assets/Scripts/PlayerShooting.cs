using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerShooting : NetworkBehaviour
{
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private Transform shootTransform;
    [SerializeField] private float chargeTime = 2.0f; // Tempo di caricamento

    [SerializeField] private List<GameObject> spawnedFireBalls = new List<GameObject>();

    private PlayerMove playerMove; // Riferimento a PlayerMove

    private void Start()
    {
        playerMove = GetComponent<PlayerMove>(); // Ottieni il componente PlayerMove
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.E) && !playerMove.isCharging)
        {
            StartCoroutine(ChargeAndShoot());
        }
    }

    private IEnumerator ChargeAndShoot()
    {
        playerMove.isCharging = true; // Inizia il caricamento
        yield return new WaitForSeconds(chargeTime); // Aspetta per il tempo di caricamento

        if (Input.GetKey(KeyCode.E)) // Controlla se il tasto è ancora premuto
        {
            ShootServerRpc(); // Spara il colpo
        }

        playerMove.isCharging = false; // Resetta lo stato di caricamento
    }

    [ServerRpc]
    private void ShootServerRpc()
    {
        GameObject go = Instantiate(fireballPrefab, shootTransform.position, shootTransform.rotation);
        spawnedFireBalls.Add(go);

        ProjectileMove projectileMove = go.GetComponent<ProjectileMove>();
        projectileMove.parent = this;
        projectileMove.Initialize(shootTransform.forward);

        go.GetComponent<NetworkObject>().Spawn(true);
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
