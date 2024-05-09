using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

public class PlayerMove : NetworkBehaviour
{
    public float speed = 10f;
    Rigidbody rb;


    public override void OnNetworkSpawn()
    {
        //conto quanti ce ne sono prima e così capisco il numero del mio player
        //e da li do spawnpoint custom

        if (!IsOwner) Destroy(this);

        transform.position = new Vector3(-10, 0f, -5);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    void Update()
    {

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");


        rb.velocity = new Vector3(moveHorizontal, 0, moveVertical) * speed;

    }
}
