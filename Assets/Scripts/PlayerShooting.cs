using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerShooting : NetworkBehaviour
{
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private Transform shootTransform;
    [SerializeField] private float chargeTime = 1.0f; // Tempo di caricamento
    [SerializeField] private float maxChargeTime = 3.0f; // Tempo massimo di caricamento

    [SerializeField] private List<GameObject> spawnedFireBalls = new List<GameObject>();

    private PlayerMove playerMove; // Riferimento a PlayerMove
    private Coroutine chargeCoroutine = null;
    private bool isChargingComplete = false;
    private bool isMaxChargeComplete = false;
    private bool isDisabled = false; // Variabile per tenere traccia se il player ? disabilitato

    private void Start()
    {
        playerMove = GetComponent<PlayerMove>(); // Ottieni il componente PlayerMove
    }

    private void Update()
    {
        if (!IsOwner || isDisabled) return;

        if (Input.GetButtonDown("Fire1")) // Tasto quadrato
        {
            if (chargeCoroutine == null)
            {
                chargeCoroutine = StartCoroutine(ChargeAndShoot());
            }
        }

        if (Input.GetButtonUp("Fire1")) // Rilasciato il tasto quadrato
        {
            if (chargeCoroutine != null)
            {
                StopCoroutine(chargeCoroutine);
                chargeCoroutine = null;
                playerMove.isCharging = false;
            }

            if (isChargingComplete)
            {
                ShootServerRpc();
                isChargingComplete = false;
                isMaxChargeComplete = false;
            }
        }
    }

    private IEnumerator ChargeAndShoot()
    {
        playerMove.isCharging = true; // Inizia il caricamento
        float startTime = Time.time;

        while (Time.time - startTime < maxChargeTime)
        {
            if (Time.time - startTime >= chargeTime && !isChargingComplete)
            {
                isChargingComplete = true;
            }
            yield return null;
        }

        playerMove.isCharging = false; // Resetta lo stato di caricamento
        isMaxChargeComplete = true;
        isChargingComplete = false;
        ShootServerRpc(); // Spara automaticamente dopo maxChargeTime

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

    public void DisableShooting()
    {
        isDisabled = true;
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
