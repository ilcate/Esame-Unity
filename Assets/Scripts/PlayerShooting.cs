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
    private Coroutine chargeCoroutine = null;

    private void Start()
    {
        playerMove = GetComponent<PlayerMove>(); // Ottieni il componente PlayerMove
    }

    private void Update()
    {
        if (!IsOwner) return;

        float rightStickHorizontal = Input.GetAxis("RightStickHorizontal");
        float rightStickVertical = Input.GetAxis("RightStickVertical");

        if (rightStickHorizontal != 0 || rightStickVertical != 0)
        {
            if (chargeCoroutine == null)
            {
                chargeCoroutine = StartCoroutine(ChargeAndShoot());
            }
        }
        else
        {
            if (chargeCoroutine != null)
            {
                StopCoroutine(chargeCoroutine);
                chargeCoroutine = null;
                playerMove.isCharging = false;
            }
        }
    }

    private IEnumerator ChargeAndShoot()
    {
        playerMove.isCharging = true; // Inizia il caricamento
        float startTime = Time.time;

        while (Time.time - startTime < chargeTime)
        {
            yield return null;
        }

        ShootServerRpc(); // Spara il colpo

        playerMove.isCharging = false; // Resetta lo stato di caricamento
        chargeCoroutine = null;
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
