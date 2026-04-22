using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementController : MonoBehaviour
{
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
    private float moveInputX;
    private float moveInputZ;
    private Rigidbody rigidBody;

    public float Altitude => altitude;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.freezeRotation = true;
    }

    void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 100f)) // qqq i can be holding on with one toe and the counted as "not grounded!"
        {
            altitude = hit.distance - Constants.PLAYER_HEIGHT * 0.5f;
        }

        grounded = altitude <= GROUNDED_THRESHOLD;
        if (grounded)
        {
            // Split current velocity
            Vector3 currentVelocity = rigidBody.linearVelocity;
            Vector3 verticalComponent = Vector3.Project(currentVelocity, hit.normal);
            Vector3 horizontalComponent = Vector3.ProjectOnPlane(currentVelocity, hit.normal);

            // Get player move direction based in which way they are facing and user inputs
            Vector3 moveDirection = orientation.forward * moveInputZ + orientation.right * moveInputX;
            if (moveDirection.sqrMagnitude > 0f)
            {
                // Apply movement to horizontal component
                Vector3 moveDirOnGround = Vector3.ProjectOnPlane(moveDirection, hit.normal).normalized;
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
