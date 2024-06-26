using Unity.Netcode;
using UnityEngine;

public class ProjectileMove : NetworkBehaviour
{
    public PlayerShooting parent;
    [SerializeField] private float speed = 20f;
    private Rigidbody rb;
    private Collider projectileCollider;

    private NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>(writePerm: NetworkVariableWritePermission.Server);
    private NetworkVariable<Vector3> networkVelocity = new NetworkVariable<Vector3>(writePerm: NetworkVariableWritePermission.Server);

    public bool isBounceShot = false;
    public int remainingBounces = 3;

    public float lastSyncTime = 0f;
    private float syncInterval = 0.1f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        projectileCollider = GetComponent<Collider>();

        if (IsServer)
        {
            networkPosition.Value = transform.position;
            networkVelocity.Value = rb.velocity;
        }
    }

    public void Initialize(Vector3 direction)
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = direction * speed;

        if (IsServer)
        {
            networkPosition.Value = transform.position;
            networkVelocity.Value = rb.velocity;
        }
    }

    void Update()
    {
        if (IsServer)
        {
            if (Time.time - lastSyncTime > syncInterval)
            {
                networkPosition.Value = transform.position;
                networkVelocity.Value = rb.velocity;
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

    void FixedUpdate()
    {
        if (IsServer)
        {
            rb.MovePosition(rb.position + rb.velocity * Time.fixedDeltaTime);

            networkPosition.Value = rb.position;
            networkVelocity.Value = rb.velocity;
        }
    }

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

        PlayerMove playerMove = collision.gameObject.GetComponent<PlayerMove>();
        if (playerMove != null && playerMove.isAlive.Value)
        {
            playerMove.DisableMovementServerRpc();

            PlayerShooting playerShooting = collision.gameObject.GetComponent<PlayerShooting>();
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
            networkPosition.Value = transform.position;
            networkVelocity.Value = Vector3.zero;

            parent.DestroyServerRpc();
        }
    }

}
