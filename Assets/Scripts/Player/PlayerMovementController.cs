using System.Collections;
using UnityEngine;


// Flat surface
// Jump
// Descend
// Slope up
// Slope down
// Ladder

// Grappling hoook
// Alitutude coyote time?

// Climbing
// Vaulting
// Stairs
// ledges
// sprint

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private CapsuleCollider capsuleCollider;
    [SerializeField] private Transform orientation;

    private const float ALTITUDE_CHECK_MAX_DISTANCE = 100f;
    private const float ASCEND_COOLDOWN = 1f;
    private const float ASCEND_FORCE = 600f;
    private const float DESCEND_FORCE = 600f;
    private const float DESCEND_COOLDOWN = 1f;
    private const float GROUNDED_THRESHOLD = 0.02f;
    private const float MAX_VELOCITY_GROUNDED = 4f;
    private const float MAX_VELOCITY_LADDER = 2f;
    private const float MAX_VELOCITY_LADDER_TOP = 1f;
    private const float FACTOR_GROUNDED = 10f;
    private const float FACTOR_LADDER = 10f;
    private const float FACTOR_LADDER_TOP = 5f;
    private const float MIN_LINEAR_DAMPING = 1f;
    private const float MAX_LINEAR_DAMPING = 100f;
    private const float LINEAR_DAMPING_FACTOR = 2f;
    private const float MAX_SLOPE_ANGLE = 50f;
    private const float LADDER_TOP_TIME = 0.15f;

    private enum State
    {
        Falling,
        Grounded,
        Ascending,
        Slipping,
        Ladder,
        LadderBottom,
        LadderTop
    }

    private float altitude = 0f;
    private bool ascendReady = true;
    private bool ascendRequested = false;
    private bool descendReady = true;
    private bool descendRequested = false;
    private float horizontalSpeed = 0f;
    private float verticalSpeed = 0f;
    private float moveInputX;
    private float moveInputZ;
    private Rigidbody rigidBody;
    private LadderController activeLadder = null;
    private float ladderTopTimer = 0f;
    private State state = State.Falling;

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
        Vector3 up = PhysicsUtils.Up(transform);

        float radius = capsuleCollider.radius - 0.01f;
        bool hasGround = Physics.SphereCast(transform.position, radius, -transform.up, out RaycastHit hit, ALTITUDE_CHECK_MAX_DISTANCE);
        if (hasGround)
        {
            altitude = hit.distance + radius - Constants.PLAYER_HEIGHT * 0.5f;
        }

        Vector3 currentVelocity = rigidBody.linearVelocity;
        Vector3 verticalComponent = Vector3.Project(currentVelocity, up);
        Vector3 horizontalComponent = currentVelocity - verticalComponent;
        horizontalSpeed = horizontalComponent.magnitude;
        verticalSpeed = verticalComponent.magnitude * Mathf.Sign(Vector3.Dot(verticalComponent, up));

        float slopeAngle = hasGround ? Vector3.Angle(up, hit.normal) : 90f;
        bool isGrounded = altitude <= GROUNDED_THRESHOLD;
        bool facingLadder = activeLadder != null && Vector3.Dot(activeLadder.transform.position - transform.position, orientation.forward) > 0;

        switch (state)
        {
            case State.Falling:
                if (facingLadder)
                {
                    state = State.Ladder;
                }
                else if (isGrounded) 
                {
                    state = State.Grounded;
                }
                break;
            case State.Grounded:
                if (facingLadder)
                {
                    state = State.Ladder;
                }
                else if (!isGrounded)
                {
                    state = State.Falling;
                }
                else if (slopeAngle > MAX_SLOPE_ANGLE)
                {
                    state = State.Slipping;
                }
                break;
            case State.Ascending:
                if (facingLadder)
                {
                    state = State.Ladder;
                }
                else if (isGrounded)
                {
                    state = State.Grounded;
                }
                else if (verticalSpeed < 0)
                {
                    state = State.Falling;
                }
                break;
            case State.Slipping:
                if (facingLadder)
                {
                    state = State.Ladder;
                }
                else if (slopeAngle <= MAX_SLOPE_ANGLE)
                {
                    if (isGrounded)
                    {
                        state = State.Grounded;
                    }
                    else
                    {
                        state = State.Falling;
                    }
                }
                break;
            case State.Ladder:
                if (!facingLadder)
                {
                    if (isGrounded)
                    {
                        state = State.Grounded;
                    }
                    else if (verticalSpeed <= 0)
                    {
                        state = State.Falling;
                    }
                    else if (verticalSpeed > 0)
                    {
                        state = State.LadderTop;
                        ladderTopTimer = 0;
                    }
                }
                break;
            case State.LadderTop:
                ladderTopTimer += Time.fixedDeltaTime;
                if (ladderTopTimer >= LADDER_TOP_TIME)
                {
                    state = State.Falling;
                }
                break;
        }

        //Debug.Log($"state={state}, rigidBody.linearDamping={rigidBody.linearDamping}");

        if (state == State.Grounded)
        {
            Vector3 moveDirection = orientation.forward * moveInputZ + orientation.right * moveInputX;
            if (moveDirection.sqrMagnitude > 0f)
            {
                Vector3 targetVelocity = moveDirection.normalized * MAX_VELOCITY_GROUNDED;
                Vector3 deltaVelocity = targetVelocity - currentVelocity;
                Vector3 deltaVelocityPlane = Vector3.ProjectOnPlane(deltaVelocity, hit.normal);
                rigidBody.linearDamping = 0;
                rigidBody.AddForce(deltaVelocityPlane * FACTOR_GROUNDED, ForceMode.Acceleration);
            }
            else
            {
                StopMoving();
            }
        }
        else if (state == State.Falling)
        {
            rigidBody.linearDamping = 0;
        }
        else if (state == State.Slipping)
        {
            rigidBody.linearDamping = 0;
        }
        else if (state == State.Ladder)
        {
            Vector3 moveDirection = orientation.up * moveInputZ + orientation.right * moveInputX;

            // Step away from ladder
            if (isGrounded && moveInputZ < 0)
            {
                moveDirection = orientation.forward * moveInputZ + orientation.right * moveInputX;
            }

            if (moveDirection.sqrMagnitude > 0f)
            {
                Vector3 targetVelocity = moveDirection.normalized * MAX_VELOCITY_LADDER;
                Vector3 deltaVelocity = targetVelocity - currentVelocity;
                rigidBody.linearDamping = 0;
                rigidBody.AddForce(deltaVelocity * FACTOR_LADDER, ForceMode.Acceleration);
            }
            else
            {
                StopMoving();
            }

        }
        else if (state == State.LadderTop)
        {
            Vector3 moveDirection = orientation.forward * moveInputZ + orientation.right * moveInputX;
            if (moveDirection.sqrMagnitude > 0f)
            {
                Vector3 targetVelocity = moveDirection.normalized * MAX_VELOCITY_LADDER_TOP;
                rigidBody.linearDamping = 0;
                rigidBody.AddForce(targetVelocity * FACTOR_LADDER_TOP, ForceMode.Acceleration);
            }
            else
            {
                StopMoving();
            }
        }

        if (ascendRequested)
        {
            ascendRequested = false;
            rigidBody.linearDamping = 0;
            rigidBody.AddForce(up * ASCEND_FORCE, ForceMode.Impulse);
            state = State.Ascending;
            StartCoroutine(AscendCooldown());
        }

        if (descendRequested)
        {
            descendRequested = false;
            rigidBody.linearDamping = 0;
            rigidBody.AddForce(-up * DESCEND_FORCE, ForceMode.Impulse);
            StartCoroutine(DescendCooldown());
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out LadderController ladderController))
        {
            activeLadder = ladderController;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out LadderController ladderController))
        {
            if (ladderController == activeLadder)
            {
                activeLadder = null;
            }
        }
    }

    public void HandleAscend()
    {
        if (state == State.Grounded && ascendReady)
        {
            ascendReady = false;
            ascendRequested = true;
        }
    }

    public void HandleDescend()
    {
        if ((state == State.Falling || state == State.Ascending) && descendReady)
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

    private void StopMoving()
    {
        rigidBody.linearDamping = Mathf.Clamp(rigidBody.linearDamping * LINEAR_DAMPING_FACTOR, MIN_LINEAR_DAMPING, MAX_LINEAR_DAMPING);
    }
}
