using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMove : NetworkBehaviour
{
    public float speed = 10f;
    public float rotationSpeed = 360f;
    private static string passed;
    public bool isMoving = false;
    public bool isCharging = false;

    private NetworkVariable<bool> isDisabled = new NetworkVariable<bool>(false);

    Animator animator;
    public static PlayerMove Instance { get; private set; }

    Rigidbody rb;

    public static void PassName(string inputUsername)
    {
        passed = inputUsername;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        transform.position = new Vector3(0f, 0f, 0f);
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!IsOwner) return;

        if (isDisabled.Value)
        {
            if (Input.GetKeyDown(KeyCode.R) || Input.GetButtonDown("Triangle"))
            {
                ReviveServerRpc();
            }
            return;
        }

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        if (isCharging)
        {
            if (moveHorizontal != 0 || moveVertical != 0)
            {
                float targetAngle = Mathf.Atan2(moveHorizontal, moveVertical) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                isMoving = false;
            }

            animator.SetBool("IsMoving", false);
            isMoving = false;
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
            isDisabled.Value = true;
            rb.velocity = Vector3.zero;
            animator.SetBool("IsMoving", false);
            animator.SetTrigger("Die");
            DisableClientRpc();
        }
        else
        {
            DisableMovementServerRpc();
        }
    }

    [ServerRpc]
    public void DisableMovementServerRpc()
    {
        isDisabled.Value = true;
        rb.velocity = Vector3.zero;
        animator.SetBool("IsMoving", false);
        animator.SetTrigger("Die");
        DisableClientRpc();
    }

    [ClientRpc]
    private void DisableClientRpc()
    {
        rb.velocity = Vector3.zero;
        animator.SetBool("IsMoving", false);
        animator.SetTrigger("Die");
    }

    [ServerRpc]
    private void ReviveServerRpc()
    {
        isDisabled.Value = false;
        ReviveClientRpc();
    }

    [ClientRpc]
    private void ReviveClientRpc()
    {
        rb.velocity = Vector3.zero;
        animator.ResetTrigger("Die");
        animator.SetBool("IsMoving", false);

        // Call EnableShooting on the PlayerShooting component
        PlayerShooting playerShooting = GetComponent<PlayerShooting>();
        if (playerShooting != null)
        {
            playerShooting.EnableShooting();
        }
    }
}
