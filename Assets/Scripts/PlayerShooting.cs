using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class PlayerShooting : NetworkBehaviour
{
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private Transform shootTransform;
    [SerializeField] private List<GameObject> spawnedFireBalls = new List<GameObject>();

    private PlayerMove playerMove;
    public bool canShoot = true;
    private bool isDisabled = false;

    public string shootType = "Standard";
    private int ammo = 0;

    private Animator animator;

    private void Start()
    {
        playerMove = GetComponent<PlayerMove>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!IsOwner || isDisabled || !GameManager.Instance.inGame.Value) return;

        if (Input.GetButtonDown("Fire1") && !playerMove.isMoving)
        {
            animator.SetBool("IsCharging", true);
            playerMove.isCharging = true;
        }

        if (Input.GetButtonUp("Fire1"))
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

    public void OnShootAnimationEvent()
    {
        if (IsOwner)
        {
            ShootServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ShootServerRpc()
    {
        if (isDisabled) return;


        if (shootType == "Standard")
        {
            ShootProjectile(shootTransform.position, shootTransform.forward);
        }

        if (ammo > 0)
        {
            ammo--;

            if (shootType == "MultiShot")
            {
                Vector3 baseDirection = shootTransform.forward;
                ShootProjectile(shootTransform.position, baseDirection);
                Vector3 leftDirection = Quaternion.Euler(0, -20, 0) * baseDirection;
                ShootProjectile(shootTransform.position, leftDirection);
                Vector3 rightDirection = Quaternion.Euler(0, 20, 0) * baseDirection;
                ShootProjectile(shootTransform.position, rightDirection);
            }
            else if (shootType == "BounceShot")
            {
                ShootProjectile(shootTransform.position, shootTransform.forward, true);
            }

        }

        if (ammo <= 0)
        {
            Debug.Log("setted to standard");
            SetShootType("Standard");
            canShoot = true;
        }
    }

    public void ShootProjectile(Vector3 position, Vector3 direction, bool isBounceShot = false)
    {
        Quaternion rotation = Quaternion.LookRotation(direction);
        GameObject go = Instantiate(fireballPrefab, position, rotation);
        spawnedFireBalls.Add(go);

        ProjectileMove projectileMove = go.GetComponent<ProjectileMove>();
        projectileMove.parent = this;
        projectileMove.isBounceShot = isBounceShot;
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
            StartCoroutine(DestroyAfterDelay(0.27f));
        }
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (spawnedFireBalls.Count > 0)
        {
            GameObject toDestroy = spawnedFireBalls[0];
            toDestroy.GetComponent<NetworkObject>().Despawn();
            spawnedFireBalls.RemoveAt(0);
            Destroy(toDestroy);
        }
    }

    public void SetShootType(string newShootType, int newAmmo = 0)
    {
        shootType = newShootType;
        ammo = newAmmo;
    }

    public void CanShoot()
    {
        canShoot = true;
    }
}
