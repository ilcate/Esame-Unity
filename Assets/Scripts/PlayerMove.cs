using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class PlayerMove : NetworkBehaviour
{
    public float speed = 10f;
    public float rotationSpeed = 360f;
    private static string passed;
    public bool isCharging = false; // Variabile per tenere traccia dello stato di caricamento

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
        if (isCharging)
        {
            // Permetti solo la rotazione
            float moveHorizontal = Input.GetAxis("Horizontal");

            if (moveHorizontal != 0)
            {
                float targetAngle = moveHorizontal > 0 ? 90f : -90f;
                Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            animator.SetBool("IsMoving", false);
            rb.velocity = Vector3.zero; // Disabilita il movimento
        }
        else
        {
            // Movimento normale
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
}
