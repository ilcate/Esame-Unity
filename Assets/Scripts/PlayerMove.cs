using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Threading.Tasks;

public class PlayerMove : NetworkBehaviour
{
    public float speed = 10f;
    public float rotationSpeed = 360f;
    public bool isMoving = false;
    public bool isCharging = false;

    public NetworkVariable<bool> isAlive = new NetworkVariable<bool>(true);

    private NetworkVariable<bool> isDisabled = new NetworkVariable<bool>(false);

    Animator animator;
    Rigidbody rb;

    public static PlayerMove Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log(OwnerClientId);
        if (IsOwner)
        {
            SetInitialPosition();
        }
    }

    private void SetInitialPosition()
    {
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
        MovePlayers();
    }

    private void MovePlayers()
    {
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
        if (!IsOwner || !isAlive.Value) return;

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


    [ServerRpc(RequireOwnership = false)]
    public void DisableMovementServerRpc()
    {
        if (IsServer)
        {
            isAlive.Value = false;

        }

        rb.velocity = Vector3.zero;
        animator.SetBool("IsMoving", false);
        animator.SetBool("IsDead", true);
        animator.SetTrigger("Die");
        DisableClientRpc();
        
    }

    [ClientRpc]
    public void DisableClientRpc()
    {
        isAlive.Value = false;
        rb.velocity = Vector3.zero;
        animator.SetBool("IsMoving", false);
        animator.SetBool("IsDead", true); 
        animator.SetTrigger("Die");
    }

    [ServerRpc(RequireOwnership = false)]
    public void ReviveServerRpc()
    {
        Debug.Log($"ReviveServerRpc called for client {OwnerClientId}");
        isAlive.Value = true;
        rb.velocity = Vector3.zero;
       
        TpToMapClientRpc();
    }

    [ClientRpc]
    public void ReviveClientRpc()
    {
        Debug.Log($"ReviveClientRpc called for client {OwnerClientId}");
        isAlive.Value = true;
        rb.velocity = Vector3.zero;
    }

    public void RevivePlayers()
    {
        animator.SetBool("IsDead", false);

        ReviveServerRpc();
        ReviveClientRpc();
    }


}
