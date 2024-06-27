using Unity.Netcode;
using UnityEngine;

public class ProjectileMove : NetworkBehaviour
{
    public PlayerShootingManager parent;
    [SerializeField] private float speed = 20f;
    private Rigidbody rb;
    private Collider projectileCollider;

    private NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>(writePerm: NetworkVariableWritePermission.Server);
    private NetworkVariable<Vector3> networkVelocity = new NetworkVariable<Vector3>(writePerm: NetworkVariableWritePermission.Server);

    public bool isBounceShot = false;
    public int remainingBounces = 3;

    public float lastSyncTime = 0f;
    private float syncInterval = 0.1f;


    void OnCollisionEnter(Collision collision)
    {
        if (!IsServer) return;

        if (collision.gameObject == parent.gameObject)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Projectile"))
        {
            return;
        }

        PlayerMoveSet player = collision.gameObject.GetComponent<PlayerMoveSet>();
        if (player != null && player.isAlive.Value)
        {
            player.DisableMovementServerRpc();

            PlayerShootingManager playerShooting = collision.gameObject.GetComponent<PlayerShootingManager>();
            if (playerShooting != null)
            {
                playerShooting.DisableShooting();
            }
        }

        if (isBounceShot && remainingBounces > 0)
        {
            remainingBounces--;
        }

        else
        {
           
            SetPositionAndVelocity(transform.position, Vector3.zero);

            parent.DestroyServerRpc();
        }
    }

    void FixedUpdate()
    {
        if (IsServer)
        {
            rb.MovePosition(rb.position + rb.velocity * Time.fixedDeltaTime);

  
            SetPositionAndVelocity(rb.position, rb.velocity);
        }
    }


    public void Initialize(Vector3 direction)
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = direction * speed;

        if (IsServer)
        {
            SetPositionAndVelocity(transform.position, rb.velocity);
        }
    }


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        projectileCollider = GetComponent<Collider>();

        if (IsServer)
        {
            

            SetPositionAndVelocity(transform.position, rb.velocity);
        }
    }

    void Update()
    {
        if (IsServer)
        {
            if (Time.time - lastSyncTime > syncInterval)
            {
                SetPositionAndVelocity(transform.position, rb.velocity);
                lastSyncTime = Time.time;
            }
        }
        else
        {
            float interpolationFactor = (Time.time - lastSyncTime) / syncInterval;
            transform.position = Vector3.Lerp(transform.position, networkPosition.Value, interpolationFactor);
            rb.velocity = Vector3.Lerp(rb.velocity, networkVelocity.Value, interpolationFactor);
        }
    }

    private void SetPositionAndVelocity(Vector3 transform, Vector3 rigidbody)
    {
        networkPosition.Value = transform;
        networkVelocity.Value = rigidbody;
    }



}
