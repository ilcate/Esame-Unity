using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ProjectileMove : NetworkBehaviour
{
    public PlayerShooting parent;
    [SerializeField] private float speed = 20f;
    private Rigidbody rb;
    private Collider projectileCollider;

    private NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>();
    private NetworkVariable<Vector3> networkVelocity = new NetworkVariable<Vector3>();

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        projectileCollider = GetComponent<Collider>();

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
            networkPosition.Value = transform.position;
            networkVelocity.Value = rb.velocity;
        }
        else
        {
            transform.position = networkPosition.Value;
            rb.velocity = networkVelocity.Value;
        }
    }

    void FixedUpdate()
    {
        if (IsServer)
        {
            rb.MovePosition(rb.position + rb.velocity * Time.fixedDeltaTime);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!IsServer) return;

        if (collision.gameObject == parent.gameObject)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Projectile"))
        {
            return;
        }

        PlayerMove playerMove = collision.gameObject.GetComponent<PlayerMove>();
        if (playerMove != null)
        {
            playerMove.DisableMovement();
            PlayerShooting playerShooting = collision.gameObject.GetComponent<PlayerShooting>();
            if (playerShooting != null)
            {
                playerShooting.DisableShooting();
            }
        }
        else
        {
            Debug.LogWarning("PlayerMove component missing on target");
        }

        // Destroy the projectile after collision
        Destroy(gameObject);
    }
}
