using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;
using TMPro;



public class PlayerMove : NetworkBehaviour
{
    public float speed = 10f;
    public float rotationSpeed = 360f;
    private static string passed;
    

    Animator animator;
    public static PlayerMove Instance { get; private set; }

    Rigidbody rb;

    //[SerializeField]
    //Texture2D rougeTexture;

    //[SerializeField]
    //GameObject armLeft;

    //[SerializeField]
    //GameObject armRight;

    //[SerializeField]
    //GameObject body;

    //[SerializeField]
    //GameObject head;

    //[SerializeField]
    //GameObject legLeft;

    //[SerializeField]
    //GameObject legRight;





    public static void PassName(string inputUsername)
    {
        passed = inputUsername;
    }

    public override void OnNetworkSpawn()
    {
        //conto quanti ce ne sono prima e così capisco il numero del mio player
        //e da li do spawnpoint custom



        if (!IsOwner) Destroy(this);

        //Texture2D textureRouge = (Texture2D)Resources.Load("Assets/PlayerModels/fbx/rogue_texture.png");

        //Debug.Log(textureRouge);
        //hat.GetComponent<Renderer>().material.mainTexture = rougeTexture;
        //ChangeSkin();
        //transform.position = new Vector3(Random.Range(-20, 20), 0f, Random.Range(-20, 20));
        transform.position = new Vector3(0f, 0f, 0f);
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }


    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        if (moveHorizontal != 0 || moveVertical != 0)
        {
            // Calcola l'angolo di rotazione basato sulla direzione del movimento
            float targetAngle = Mathf.Atan2(moveHorizontal, moveVertical) * Mathf.Rad2Deg;
            // Calcola la rotazione desiderata
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);

            // Ruota gradualmente il personaggio verso l'angolo desiderato
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
