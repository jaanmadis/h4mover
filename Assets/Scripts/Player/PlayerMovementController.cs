using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private CapsuleCollider capsuleCollider;
    [SerializeField] private Transform orientation;

    private const float ASCEND_COOLDOWN = 1f;
    private const float ASCEND_FORCE = 1000f;
    private const float DESCEND_FORCE = 1000f;
    private const float DESCEND_COOLDOWN = 1f;
    private const float GROUNDED_THRESHOLD = 0.2f;
    private const float HORIZONTAL_MOVEMENT_DECAY = 0.8f;
    private const float HORIZONTAL_MOVEMENT_FACTOR = 1.1f;

    private float altitude = 0f;
    private bool ascendReady = true;
    private bool ascendRequested = false;
    private bool descendReady = true;
    private bool descendRequested = false;
    private bool grounded = false;
    private float horizontalSpeed = 0f;
    private float verticalSpeed = 0f;
    private float moveInputX;
    private float moveInputZ;
    private Rigidbody rigidBody;
    private Vector3 groundNormal;

    public float Altitude => altitude;
    //public float AngleToCapsule => angleToCapsule;
    //public float DistanceToCapsule => distanceToCapsule;
    public float HorizontalSpeed => horizontalSpeed;
    public float VerticalSpeed => verticalSpeed;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.freezeRotation = true;
    }

    void FixedUpdate()
    {
        float radius = capsuleCollider.radius - 0.1f;
        if (Physics.SphereCast(transform.position, radius, -transform.up, out RaycastHit hitler, 100f))
        {
            altitude = hitler.distance + radius - Constants.PLAYER_HEIGHT * 0.5f;
        }

        Debug.Log($"groundNormal={groundNormal}, hit.normal={hitler.normal}");

        // Split current velocity
        Vector3 currentVelocity = rigidBody.linearVelocity;
        Vector3 horizontalComponent = Vector3.ProjectOnPlane(currentVelocity, groundNormal);
        Vector3 verticalComponent = Vector3.Project(currentVelocity, groundNormal);

        horizontalSpeed = horizontalComponent.magnitude;
        verticalSpeed = verticalComponent.magnitude * Mathf.Sign(Vector3.Dot(verticalComponent, groundNormal));

        grounded = altitude <= GROUNDED_THRESHOLD;
        if (grounded)
        {
            // Get player move direction based in which way they are facing and user inputs
            Vector3 moveDirection = orientation.forward * moveInputZ + orientation.right * moveInputX;
            if (moveDirection.sqrMagnitude > 0f)
            {
                // Apply movement to horizontal component
                Vector3 moveDirOnGround = Vector3.ProjectOnPlane(moveDirection, groundNormal).normalized;
                horizontalComponent = Vector3.ClampMagnitude(horizontalComponent + moveDirOnGround * HORIZONTAL_MOVEMENT_FACTOR, 4f);
            }
            else
            {
                // Decay horizontalComponent if moveDir is zero
                horizontalComponent *= HORIZONTAL_MOVEMENT_DECAY;
            }

            // Recombine components and add back to current velocity
            rigidBody.linearVelocity = horizontalComponent + verticalComponent;
        }

        if (ascendRequested)
        {
            ascendRequested = false;
            Vector3 up = PhysicsUtils.Up(transform);
            rigidBody.linearVelocity = new Vector3(rigidBody.linearVelocity.x, 0f, rigidBody.linearVelocity.z);
            rigidBody.AddForce(up * ASCEND_FORCE, ForceMode.Impulse);
            StartCoroutine(AscendCooldown());
        }

        if (descendRequested)
        {
            descendRequested = false;
            Vector3 up = PhysicsUtils.Up(transform);
            rigidBody.linearVelocity = new Vector3(rigidBody.linearVelocity.x, 0f, rigidBody.linearVelocity.z);
            rigidBody.AddForce(-up * DESCEND_FORCE, ForceMode.Impulse);
            StartCoroutine(DescendCooldown());
        }
    }

    void OnCollisionStay(Collision collision)
    {
        Vector3 normal = Vector3.zero;
        for (int i = 0; i < collision.contactCount; i++)
        {
            normal += collision.contacts[i].normal;
        }
        groundNormal = (normal / collision.contactCount).normalized;
    }

    public void HandleAscend()
    {
        if (grounded && ascendReady)
        {
            ascendReady = false;
            ascendRequested = true;
        }
    }

    public void HandleDescend()
    {
        if (!grounded && descendReady)
        {
            descendReady = false;
            descendRequested = true;
        }
    }

    public void HandleMovement()
    {
        moveInputX = Input.GetAxisRaw("Horizontal");
        moveInputZ = Input.GetAxisRaw("Vertical");
    }

    private IEnumerator AscendCooldown()
    {
        yield return new WaitForSeconds(ASCEND_COOLDOWN);
        ascendReady = true;
    }

    private IEnumerator DescendCooldown()
    {
        yield return new WaitForSeconds(DESCEND_COOLDOWN);
        descendReady = true;
    }
}
