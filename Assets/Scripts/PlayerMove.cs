using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

public class PlayerMove : NetworkBehaviour
{
    public float speed = 5f;
 

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) Destroy(this);
    }
    

    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical);

        transform.Translate(movement * speed/ 10);
    }
}
