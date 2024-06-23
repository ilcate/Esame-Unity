using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerShooting : NetworkBehaviour
{
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private Transform shootTransform;
    [SerializeField] private float chargeTime = 1.0f;

    [SerializeField] private List<GameObject> spawnedFireBalls = new List<GameObject>();

    private PlayerMove playerMove;
    private bool canShoot = true;
    private bool isDisabled = false;


    public string shootType = "Standard";



    private Animator animator;

    private void Start()
    {
        playerMove = GetComponent<PlayerMove>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {


            if (!IsOwner || isDisabled) return;


            if (Input.GetButtonDown("Fire1") && !playerMove.isMoving)
            {


            if (GameManager.Instance.inGame.Value)
            {
                animator.SetBool("IsCharging", true);
                playerMove.isCharging = true;
            }

            }

            if (Input.GetButtonUp("Fire1"))
            {


            if (GameManager.Instance.inGame.Value)
            {
                if (canShoot)
                {
                    animator.SetTrigger("Shoot");


                }
                canShoot = false;

                playerMove.isCharging = false;
                animator.SetBool("IsCharging", false);

            }

            }
   
      
    }

    [ServerRpc]
    private void ShootServerRpc()
    {
        if (isDisabled) return;

        if (shootType != "Standard")
        {
            ShootProjectile(shootTransform.position, shootTransform.forward);
        }
        else
        {
            Vector3 baseDirection = shootTransform.forward;

            // Central projectile
            ShootProjectile(shootTransform.position, baseDirection);

            // Left spread projectile
            Vector3 leftDirection = Quaternion.Euler(0, -30, 0) * baseDirection;
            ShootProjectile(shootTransform.position, leftDirection);

            // Right spread projectile
            Vector3 rightDirection = Quaternion.Euler(0, 30, 0) * baseDirection;
            ShootProjectile(shootTransform.position, rightDirection);
        }
    }

    private void ShootProjectile(Vector3 position, Vector3 direction)
    {
        Quaternion rotation = Quaternion.LookRotation(direction);
        GameObject go = Instantiate(fireballPrefab, position, rotation);
        spawnedFireBalls.Add(go);

        ProjectileMove projectileMove = go.GetComponent<ProjectileMove>();
        projectileMove.parent = this;
        Debug.Log(direction);
        projectileMove.Initialize(direction);

        go.GetComponent<NetworkObject>().Spawn(true);
    }


    public void DisableShooting()
    {
        isDisabled = true;
    }

    public void EnableShooting()
    {
        isDisabled = false;
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


    private void CanShoot()
    {
        canShoot = true;
    }
}
