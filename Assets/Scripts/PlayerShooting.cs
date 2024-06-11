using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerShooting : NetworkBehaviour
{
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private Transform shootTransform;
    [SerializeField] private float chargeTime = 1.0f; // Charging time
    [SerializeField] private float maxChargeTime = 3.0f; // Maximum charging time

    [SerializeField] private List<GameObject> spawnedFireBalls = new List<GameObject>();

    private PlayerMove playerMove; // Reference to PlayerMove
    private Coroutine chargeCoroutine = null;
    private bool isChargingComplete = false;
    private bool isMaxChargeComplete = false;
    private bool isDisabled = false; // Variable to track if the player is disabled

    private Animator animator;

    private void Start()
    {
        playerMove = GetComponent<PlayerMove>(); // Get the PlayerMove component
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!IsOwner || isDisabled) return;

        if (Input.GetButtonDown("Fire1")) // Square button
        {
            if (chargeCoroutine == null)
            {
                chargeCoroutine = StartCoroutine(ChargeAndShoot());
            }
        }

        if (Input.GetButtonUp("Fire1")) // Released the square button
        {
            if (chargeCoroutine != null)
            {
                StopCoroutine(chargeCoroutine);
                chargeCoroutine = null;
                playerMove.isCharging = false;
                animator.SetBool("IsCharging", false); // Stop charging animation
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
        playerMove.isCharging = true; // Start charging
        animator.SetBool("IsCharging", true); // Start charging animation
        float startTime = Time.time;

        while (Time.time - startTime < maxChargeTime)
        {
            if (Time.time - startTime >= chargeTime && !isChargingComplete)
            {
                isChargingComplete = true;
            }
            yield return null;
        }

        playerMove.isCharging = false; // Reset charging state
        isMaxChargeComplete = true;
        isChargingComplete = false;
        ShootServerRpc(); // Automatically shoot after maxChargeTime

        chargeCoroutine = null;
        animator.SetBool("IsCharging", false); // Stop charging animation
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
        animator.SetTrigger("Shoot"); // Trigger shoot animation
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
