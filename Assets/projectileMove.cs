using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class projectileMove : NetworkBehaviour
{
    public PlayerShooting parent;
    [SerializeField] private float speed = 10f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        rb.velocity = rb.transform.forward * speed;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Debug.Log(collision.collider);
        // Destroy(gameObject);
        if(!IsOwner) return;
        parent.DestroyServerRpc();
    }
}
