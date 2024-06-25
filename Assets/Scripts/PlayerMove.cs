using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using System.Threading.Tasks;

public class PlayerMove : NetworkBehaviour
{
    public float speed = 10f;
    public float rotationSpeed = 360f;
    public bool isMoving = false;
    public bool isCharging = false;


   

    private NetworkVariable<bool> isDisabled = new NetworkVariable<bool>(false);

    Animator animator;

    public static PlayerMove Instance { get; private set; }

    Rigidbody rb;

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log(OwnerClientId);
        switch (OwnerClientId)
        {
            case 0:
                transform.position = new Vector3(110f, 0f, -15f);
                break;
            case 1:
                transform.position = new Vector3(113f, 0f, -15f);
                break;
            case 2:
                transform.position = new Vector3(115f, 0f, -15f);
                break;
            case 3:
                transform.position = new Vector3(117f, 0f, -15f);
                break;
            default:
                transform.position = new Vector3(115f, 0f, -13f);
                break;
        }
    }

    [ClientRpc]
    public void TpToMapClientRpc()
    {
        TpToMapClientRpcAsync(); 
    }

    private async Task TpToMapClientRpcAsync()
    {
        await GameManager.Instance.ActivateGameCam();

        switch (OwnerClientId)
        {
            case 0:
                transform.position = new Vector3(-25f, 0f, 16f);
                break;
            case 1:
                transform.position = new Vector3(19f, 0f, -16f);
                break;
            case 2:
                transform.position = new Vector3(19f, 0f, 14f);
                break;
            case 3:
                transform.position = new Vector3(-25f, 0f, -19f);
                break;
            default:
                transform.position = new Vector3(0f, 0f, 0f);
                break;
        }
    }



    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!IsOwner) return;

        if (isDisabled.Value)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                ReviveServerRpc();
            }
            return;
        }

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        if (isCharging)
        {
            if (moveHorizontal != 0 || moveVertical != 0)
            {
                float rotationDirection = 0;
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                {
                    rotationDirection = -1;
                }
                else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                {
                    rotationDirection = 1;
                }

                transform.Rotate(Vector3.up, rotationDirection * rotationSpeed * Time.deltaTime);
             
                isMoving = false;
            }

            animator.SetBool("IsMoving", false);
            isMoving = false;
            rb.velocity = Vector3.zero;
        }
        else
        {
            if (moveHorizontal != 0 || moveVertical != 0)
            {
                float targetAngle = Mathf.Atan2(moveHorizontal, moveVertical) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                animator.SetBool("IsMoving", true);
                isMoving = true;
            }
            else
            {
                animator.SetBool("IsMoving", false);
                isMoving = false;
            }

            rb.velocity = new Vector3(moveHorizontal, 0, moveVertical) * speed;
        }
    }

    public void DisableMovement()
    {
        if (IsServer)
        {
            isDisabled.Value = true;
            rb.velocity = Vector3.zero;
            animator.SetBool("IsMoving", false);
            animator.SetTrigger("Die");
            DisableClientRpc();
        }
        else
        {
            DisableMovementServerRpc();
        }
    }

    [ServerRpc]
    public void DisableMovementServerRpc()
    {
        isDisabled.Value = true;
        rb.velocity = Vector3.zero;
        animator.SetBool("IsMoving", false);
        animator.SetTrigger("Die");
        DisableClientRpc();
    }

    [ClientRpc]
    private void DisableClientRpc()
    {
        rb.velocity = Vector3.zero;
        animator.SetBool("IsMoving", false);
        animator.SetTrigger("Die");
    }

    [ServerRpc]
    private void ReviveServerRpc()
    {
        isDisabled.Value = false;
        
        ReviveClientRpc();
    }

    [ClientRpc]
    private void ReviveClientRpc()
    {
        rb.velocity = Vector3.zero;
        animator.ResetTrigger("Die");
        StartCoroutine(RespawnPlayer());

        PlayerShooting playerShooting = GetComponent<PlayerShooting>();
        if (playerShooting != null)
        {
            playerShooting.EnableShooting();
        }
    }

    private IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(5); 

        Transform spawnPoint = FindObjectOfType<SpawnManager>().GetRandomSpawnPoint();
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;
    }

}
