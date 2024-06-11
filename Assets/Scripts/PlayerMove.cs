using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMove : NetworkBehaviour
{
    public float speed = 10f;
    public float rotationSpeed = 360f;
    private static string passed;
    public bool isCharging = false; // Variable to track charging state
    private bool isDisabled = false; // Variable to track if the player is disabled

    Animator animator;
    public static PlayerMove Instance { get; private set; }

    Rigidbody rb;

    public static void PassName(string inputUsername)
    {
        passed = inputUsername;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) enabled = false;
        transform.position = new Vector3(0f, 0f, 0f);
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!IsOwner || isDisabled) return;

        if (isCharging)
        {
            // Allow only rotation with the left analog stick
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            if (moveHorizontal != 0 || moveVertical != 0)
            {
                float targetAngle = Mathf.Atan2(moveHorizontal, moveVertical) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            animator.SetBool("IsMoving", false);
            rb.velocity = Vector3.zero; // Disable movement
        }
        else
        {
            // Normal movement with the left analog stick
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            if (moveHorizontal != 0 || moveVertical != 0)
            {
                float targetAngle = Mathf.Atan2(moveHorizontal, moveVertical) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                animator.SetBool("IsMoving", true);
            }
            else
            {
                animator.SetBool("IsMoving", false);
            }

            rb.velocity = new Vector3(moveHorizontal, 0, moveVertical) * speed;
        }
    }

    public void DisableMovement()
    {
        isDisabled = true;
        rb.velocity = Vector3.zero;
        animator.SetBool("IsMoving", false);
        animator.SetTrigger("Die"); // Trigger death animation
    }
}
