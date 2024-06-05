using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameObject fireball;
    [SerializeField] private Transform shootTransform;

    // Update is called once per frame
    void Update()
    {
        //if (!IsOwner) return;
        if (Input.GetKeyDown(KeyCode.E))
        {
            //GameObject go =
            //go.GetComponent<NetworkObject>().Spawn();
            Instantiate(fireball, shootTransform.position, shootTransform.rotation);
        }
    }
}
