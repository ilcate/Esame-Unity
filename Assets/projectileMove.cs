using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProjectileMove : NetworkBehaviour
{
    public PlayerShooting parent;
    [SerializeField] private float speed = 20f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 movement = rb.transform.forward * speed * Time.deltaTime;
        rb.MovePosition(rb.transform.position + movement);
    }

    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.collider);
        //Destroy(gameObject);
        if (!IsOwner) return;
        parent.DestroyServerRpc();
    }
}
