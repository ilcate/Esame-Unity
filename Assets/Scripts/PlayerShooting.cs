using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerShooting : NetworkBehaviour
{
    [SerializeField] private GameObject fireball;
    [SerializeField] private Transform shootTransform;

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
       if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            GameObject go = Instantiate(fireball, shootTransform.position, shootTransform.rotation);
            go.GetComponent<NetworkObject>().Spawn();
        }
    }
}
