using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using System.Threading.Tasks;

public class PlayerMove : NetworkBehaviour
{
    public float speed = 10f;
    public float rotationSpeed = 360f;
    public bool isMoving = false;
    public bool isCharging = false;

    public NetworkVariable<bool> isAlive = new NetworkVariable<bool>(true);
    private NetworkVariable<bool> isDisabled = new NetworkVariable<bool>(false);

    Animator animator;
    Rigidbody rb;

    public static PlayerMove Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log(OwnerClientId);
        Vector3[] spawnPositions = {
            new Vector3(110f, 0f, -15f),
            new Vector3(113f, 0f, -15f),
            new Vector3(115f, 0f, -15f),
            new Vector3(117f, 0f, -15f),
            new Vector3(115f, 0f, -13f) // Default spawn position
        };

        transform.position = spawnPositions[OwnerClientId];
    }

    [ClientRpc]
    public void TpToMapClientRpc()
    {
        _ = TpToMapClientRpcAsync();
    }

    private async Task TpToMapClientRpcAsync()
    {
        await GameManager.Instance.ActivateGameCam();

        Vector3[] mapPositions = {
            new Vector3(-25f, 0f, 16f),
            new Vector3(19f, 0f, -16f),
            new Vector3(19f, 0f, 14f),
            new Vector3(-25f, 0f, -19f),
            new Vector3(0f, 0f, 0f) // Default map position
        };

        transform.position = mapPositions[OwnerClientId];
    }

    void Update()
    {
        if (!IsOwner) return;

        if (isDisabled.Value)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                ReviveServerRpc();
            }
            return;
        }

        HandleMovement();
    }

    private void HandleMovement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        if (isCharging)
        {
            if (moveHorizontal != 0 || moveVertical != 0)
            {
                float rotationDirection = 0;
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                {
                    rotationDirection = -1;
                }
                else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                {
                    rotationDirection = 1;
                }

                transform.Rotate(Vector3.up, rotationDirection * rotationSpeed * Time.deltaTime);
                isMoving = false;
            }

            animator.SetBool("IsMoving", false);
            rb.velocity = Vector3.zero;
        }
        else
        {
            if (moveHorizontal != 0 || moveVertical != 0)
            {
                float targetAngle = Mathf.Atan2(moveHorizontal, moveVertical) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                animator.SetBool("IsMoving", true);
                isMoving = true;
            }
            else
            {
                animator.SetBool("IsMoving", false);
                isMoving = false;
            }

            rb.velocity = new Vector3(moveHorizontal, 0, moveVertical) * speed;
        }
    }

    public void DisableMovement()
    {
        if (IsServer)
        {
            SetPlayerState(false);
        }
        else
        {
            DisableMovementServerRpc();
        }
    }

    [ServerRpc]
    public void DisableMovementServerRpc()
    {
        SetPlayerState(false);
    }

    private void SetPlayerState(bool alive)
    {
        isAlive.Value = alive;
        rb.velocity = Vector3.zero;
        animator.SetBool("IsMoving", false);
        animator.SetBool("Die", !alive); 

        if (!alive)
        {
            DisableClientRpc();
        }
        else
        {
            ReviveClientRpc();
        }
    }

    [ClientRpc]
    private void DisableClientRpc()
    {
        rb.velocity = Vector3.zero;
        animator.SetBool("IsMoving", false);
        animator.SetBool("Die", true);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ReviveServerRpc()
    {
        isDisabled.Value = false;
        SetPlayerState(true);
    }

    [ClientRpc]
    private void ReviveClientRpc()
    {
        rb.velocity = Vector3.zero;
        animator.SetBool("IsMoving", false);
        _ = TpToMapClientRpcAsync();

        PlayerShooting playerShooting = GetComponent<PlayerShooting>();
        playerShooting?.EnableShooting();
    }
}
