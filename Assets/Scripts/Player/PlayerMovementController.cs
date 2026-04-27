using System;
using System.Collections;
using UnityEngine;


// Flat surface
// Jump
// Descend
// Slope up
// Slope down
// Ladder
// Grappling

// OV Pharah style flight
// Alitutude coyote time?
// automatic landing softener?
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
    private const float MAX_VELOCITY_GRAPPLE = 6f;
    private const float FACTOR_GROUNDED = 10f;
    private const float FACTOR_LADDER = 10f;
    private const float FACTOR_LADDER_TOP = 5f;
    private const float FACTOR_GRAPPLE = 10f;
    private const float MIN_LINEAR_DAMPING = 1f;
    private const float MAX_LINEAR_DAMPING = 100f;
    private const float LINEAR_DAMPING_FACTOR = 2f;
    private const float MAX_SLOPE_ANGLE = 50f;
    private const float LADDER_TOP_TIME = 0.15f;

    public enum State
    {
        Grounded,
        Falling,
        Ascending,
        Slipping,
        Ladder,
        LadderTop,
        Grappling,
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
#if UNITY_EDITOR
    private State debugPrevState = State.Falling;
#endif
    private bool playerControl = true;
    private bool grapplePullRequested = false;
    private bool grappleRetractRequested = false;
    private Vector3 grapplePosition = Vector3.zero;

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

        /* State transitions */

        if (grapplePullRequested)
        {
            state = State.Grappling;
        }
        else if (facingLadder && (
            state == State.Grounded ||
            state == State.Falling || 
            state == State.Ascending || 
            state == State.Slipping))
        {
            state = State.Ladder;
        }

        switch (state)
        {
            case State.Grounded:
                if (!isGrounded)
                {
                    if (verticalSpeed <= 0)
                    {
                        state = State.Falling;
                    }
                    else
                    {
                        state = State.Ascending;
                    }
                }
                else if (slopeAngle > MAX_SLOPE_ANGLE)
                {
                    state = State.Slipping;
                }
                break;
            case State.Falling:
                if (isGrounded)
                {
                    state = State.Grounded;
                }
                break;
            case State.Ascending:
                if (isGrounded)
                {
                    state = State.Grounded;
                }
                else if (verticalSpeed <= 0)
                {
                    state = State.Falling;
                }
                break;
            case State.Slipping:
                if (slopeAngle <= MAX_SLOPE_ANGLE)
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
            case State.Grappling:
                if (grappleRetractRequested)
                {
                    state = State.Falling;
                }
                break;
        }

#if UNITY_EDITOR
        if (state != debugPrevState)
        {
            Debug.Log($"PrevState={debugPrevState} -> CurrState={state}");
            debugPrevState = state;
        }
#endif

        /* State behavior */

        if (state == State.Grounded)
        {
            if (ascendRequested && ascendReady)
            {
                rigidBody.linearDamping = 0;
                rigidBody.AddForce(up * ASCEND_FORCE, ForceMode.Impulse);
                StartCoroutine(AscendCooldown());
            }
            else
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
                    DampenMovement();
                }
            }
        }
        else if (state == State.Falling)
        {
            rigidBody.linearDamping = 0;
            if (descendRequested && descendReady)
            {
                rigidBody.AddForce(-up * DESCEND_FORCE, ForceMode.Impulse);
                StartCoroutine(DescendCooldown());
            }
        }
        else if (state == State.Ascending)
        {
            rigidBody.linearDamping = 0;
            if (descendRequested && descendReady)
            {
                rigidBody.AddForce(-up * DESCEND_FORCE, ForceMode.Impulse);
                StartCoroutine(DescendCooldown());
            }
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
                DampenMovement();
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
                DampenMovement();
            }
        }
        else if (state == State.Grappling)
        {
            Vector3 grappleDirection = grapplePosition - transform.position;
            Vector3 targetVelocity = grappleDirection.normalized * MAX_VELOCITY_GRAPPLE;
            Vector3 deltaVelocity = targetVelocity - currentVelocity;
            rigidBody.linearDamping = 0;
            rigidBody.AddForce(deltaVelocity * FACTOR_GRAPPLE, ForceMode.Acceleration);
        }

        ascendRequested = false;
        descendRequested = false;
        grapplePullRequested = false;
        grappleRetractRequested = false;
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
        if (playerControl)
        {
            ascendRequested = true;
        }
    }

    public void HandleDescend()
    {
        if (playerControl)
        {
            descendRequested = true;
        }
    }

    public void HandleMovement()
    {
        if (playerControl)
        {
            moveInputX = Input.GetAxisRaw("Horizontal");
            moveInputZ = Input.GetAxisRaw("Vertical");
        }
        else
        {
            moveInputX = 0;
            moveInputZ = 0;
        }
    }

    public void HandleGrapplePull(Vector3 position)
    {
        grapplePullRequested = true;
        grapplePosition = position;
    }

    public void HandleGrappleRetract()
    {
        grappleRetractRequested = true;
    }

    public void SetPlayerControl(bool newValue)
    {
        playerControl = newValue;
    }

    private IEnumerator AscendCooldown()
    {
        ascendReady = false;
        yield return new WaitForSeconds(ASCEND_COOLDOWN);
        ascendReady = true;
    }

    private IEnumerator DescendCooldown()
    {
        descendReady = false;
        yield return new WaitForSeconds(DESCEND_COOLDOWN);
        descendReady = true;
    }

    private void DampenMovement()
    {
        rigidBody.linearDamping = Mathf.Clamp(rigidBody.linearDamping * LINEAR_DAMPING_FACTOR, MIN_LINEAR_DAMPING, MAX_LINEAR_DAMPING);
    }
}
