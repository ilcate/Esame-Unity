using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProjectileMove : NetworkBehaviour
{
    public PlayerShooting parent;
    [SerializeField] private float speed = 40f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (IsOwner)
        {
            Vector3 movement = transform.forward * speed * Time.deltaTime;
            rb.MovePosition(rb.transform.position + movement);
        }
    }

   

    /*void OnCollisionEnter(Collision collision)
    {
        if (!IsOwner) return;
        parent.DestroyServerRpc();
    }*/
}
