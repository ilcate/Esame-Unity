using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ProjectileMove : NetworkBehaviour
{
    public PlayerShooting parent;
    [SerializeField] private float speed = 20f;
    private Rigidbody rb;

    // Network variables for synchronization 
    private NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>();
    private NetworkVariable<Vector3> networkVelocity = new NetworkVariable<Vector3>();

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Initialize network variables 
        if (IsServer)
        {
            networkPosition.Value = transform.position;
            networkVelocity.Value = rb.velocity;
        }
    }

    public void Initialize(Vector3 direction)
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = direction * speed;

        // Initialize network variables 
        if (IsServer)
        {
            networkPosition.Value = transform.position;
            networkVelocity.Value = rb.velocity;
        }
    }

    void Update()
    {
        if (IsServer)
        {
            // Update network variables with the current state 
            networkPosition.Value = transform.position;
            networkVelocity.Value = rb.velocity;
        }
        else
        {
            // Update the projectile's state based on network variables 
            transform.position = networkPosition.Value;
            rb.velocity = networkVelocity.Value;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!IsOwner) return;

        // Check if the collision object is the shooter 
        if (collision.gameObject == parent.gameObject)
        {
            // Ignore the collision with the shooter 
            return;
        }

        // Check if the collision object is a player 
        PlayerMove playerMove = collision.gameObject.GetComponent<PlayerMove>();
        if (playerMove != null)
        {
            playerMove.DisableMovement();

            PlayerShooting playerShooting = collision.gameObject.GetComponent<PlayerShooting>();
            if (playerShooting != null)
            {
                playerShooting.DisableShooting();
            }

            Animator targetAnimator = collision.gameObject.GetComponent<Animator>();
            if (targetAnimator != null)
            {
                targetAnimator.SetTrigger("Die"); // Trigger death animation
            }
        }

        Debug.Log(collision.collider);
        parent.DestroyServerRpc();
    }
}
