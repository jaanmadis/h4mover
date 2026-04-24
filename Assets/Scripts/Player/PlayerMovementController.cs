using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

// Flat surface

// Slope up
// Slope down

// Jump
// Descend
// Stairs
// Ladder
// Grappling hoook
// ledges

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private CapsuleCollider capsuleCollider;
    [SerializeField] private Transform orientation;

    private const float ASCEND_COOLDOWN = 1f;
    private const float ASCEND_FORCE = 1000f;
    private const float DESCEND_FORCE = 1000f;
    private const float DESCEND_COOLDOWN = 1f;
    private const float GROUNDED_THRESHOLD = 0.02f;
    private const float HORIZONTAL_MOVEMENT_DECAY = 0.8f;
    private const float HORIZONTAL_MOVEMENT_FACTOR = 1.1f;

    private enum State
    {
        Detached,
        Grounded,
        Ascending,
        Descending,
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
    private Vector3 groundNormal;
    private State state = State.Detached;

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

    void FixedUpdateThisIsFine()
    {
        Vector3 up = PhysicsUtils.Up(transform);

        float radius = capsuleCollider.radius;
        if (Physics.SphereCast(transform.position, radius, -transform.up, out RaycastHit hit, 100f))
        {
            altitude = hit.distance + radius - Constants.PLAYER_HEIGHT * 0.5f;
        }

        // Split current velocity
        Vector3 currentVelocity = rigidBody.linearVelocity;
        Vector3 verticalComponent = Vector3.Project(currentVelocity, up);
        Vector3 horizontalComponent = currentVelocity - verticalComponent;
        horizontalSpeed = horizontalComponent.magnitude;
        verticalSpeed = verticalComponent.magnitude * Mathf.Sign(Vector3.Dot(verticalComponent, up));

        float angle = Vector3.Angle(up, hit.normal);
        Vector3 upp = Vector3.Project(up, hit.normal).normalized;

        bool isGrounded = altitude <= GROUNDED_THRESHOLD;

        if (isGrounded && (state == State.Detached || state == State.Descending))
        {
            state = State.Grounded;
        }
        else if (!isGrounded && state == State.Grounded)
        {
            state = State.Detached;
        }
        else if (verticalSpeed < 0 && state == State.Ascending)
        {
            state = State.Descending;
        }

        Debug.Log($"state={state}, currentVelocity={currentVelocity}, currentVelocity.magnitude={currentVelocity.magnitude}, up={up}, upp={upp}, hn={hit.normal}, verticalComponent={verticalComponent}, horizontalComponent={horizontalComponent}, angle={angle}");

        if (state == State.Grounded)
        {
            Vector3 moveDirection = orientation.forward * moveInputZ + orientation.right * moveInputX;
            if (moveDirection.sqrMagnitude > 0f)
            {
                Vector3 tergetVelocity = moveDirection.normalized * 4f;
                Vector3 deltaVelocity = tergetVelocity - currentVelocity;
                rigidBody.linearDamping = 0;
                rigidBody.AddForce(deltaVelocity * 10f, ForceMode.Acceleration);
            }
            else
            {
                rigidBody.linearDamping = Mathf.Clamp(rigidBody.linearDamping * 2f, 1f, 100f);
            }
        }
        else if (state == State.Detached)
        {
            rigidBody.AddForce(-hit.normal * 10f, ForceMode.Acceleration);
        }

        if (ascendRequested)
        {
            ascendRequested = false;
            rigidBody.linearDamping = 0;
            rigidBody.AddForce(up * ASCEND_FORCE, ForceMode.Impulse); // qqq unrealistic, landing will hurt
            state = State.Ascending;
            StartCoroutine(AscendCooldown());
        }

        if (descendRequested)
        {
            descendRequested = false;
            rigidBody.linearDamping = 0;
            rigidBody.AddForce(-up * DESCEND_FORCE, ForceMode.Impulse); // qqq unrealistic, landing will hurt
            StartCoroutine(DescendCooldown());
        }
    }

    void FixedUpdateX()
    {
        Vector3 up = PhysicsUtils.Up(transform);

        float radius = capsuleCollider.radius;
        if (Physics.SphereCast(transform.position, radius, -transform.up, out RaycastHit hit, 100f))
        {
            altitude = hit.distance + radius - Constants.PLAYER_HEIGHT * 0.5f;
        }

        /*

        //if (Physics.CapsuleCast())
        //{
        //    altitude = hit.distance + radius - Constants.PLAYER_HEIGHT * 0.5f;
        //}
        */

        // Split current velocity
        Vector3 currentVelocity = rigidBody.linearVelocity;
        Vector3 verticalComponent = Vector3.Project(currentVelocity, up);
        Vector3 horizontalComponent = currentVelocity - verticalComponent;

        Debug.Log($"currentVelocity={currentVelocity}, up={up}, verticalComponent={verticalComponent}, horizontalComponent={horizontalComponent}");

        horizontalSpeed = horizontalComponent.magnitude;
        verticalSpeed = verticalComponent.magnitude * Mathf.Sign(Vector3.Dot(verticalComponent, up));

        //grounded = altitude <= GROUNDED_THRESHOLD;
        //if (grounded)
        //{
            // Get player move direction based in which way they are facing and user inputs
            Vector3 moveDirection = orientation.forward * moveInputZ + orientation.right * moveInputX;
            if (moveDirection.sqrMagnitude > 0f)
            {
                /*
                    // Apply movement to horizontal component
                    Vector3 moveDirOnGround = Vector3.ProjectOnPlane(moveDirection, groundNormal).normalized;
                    horizontalComponent = Vector3.ClampMagnitude(horizontalComponent + moveDirOnGround * HORIZONTAL_MOVEMENT_FACTOR, 4f);
                */

                horizontalComponent = Vector3.ClampMagnitude(horizontalComponent + moveDirection, 4f);
            }
            else
            {
                // Decay horizontalComponent if moveDir is zero
                //horizontalComponent *= HORIZONTAL_MOVEMENT_DECAY;
                //horizontalComponent *= 0.8f;
                horizontalComponent = Vector3.zero;
            }

            // Recombine components and add back to current velocity
            rigidBody.linearVelocity = horizontalComponent + verticalComponent;
            Debug.Log($"2o..forward={orientation.transform.forward}, o..right={orientation.transform.right}, moveDirection={moveDirection}, rigidBody.linearVelocity={rigidBody.linearVelocity}, horizontalComponent={horizontalComponent}, verticalComponent={verticalComponent}");

            /*
            Debug.Log($"horizontalComponent={horizontalComponent}");
            Debug.Log($"verticalComponent={verticalComponent}");
            Debug.Log($"groundNormal={groundNormal}");
            */
        //}
        //else
        //{
            //rigidBody.AddForce(-1 * Constants.GRAVITY_STRENGTH_ASTEROID * up, ForceMode.Acceleration);
        //}

        if (ascendRequested)
        {
            ascendRequested = false;
            rigidBody.linearVelocity = new Vector3(rigidBody.linearVelocity.x, 0f, rigidBody.linearVelocity.z);
            rigidBody.AddForce(up * ASCEND_FORCE, ForceMode.Impulse);
            StartCoroutine(AscendCooldown());
        }

        if (descendRequested)
        {
            descendRequested = false;
            rigidBody.linearVelocity = new Vector3(rigidBody.linearVelocity.x, 0f, rigidBody.linearVelocity.z);
            rigidBody.AddForce(-up * DESCEND_FORCE, ForceMode.Impulse);
            StartCoroutine(DescendCooldown());
        }
    }

    //void OnCollisionStay(Collision collision)
    //{
    //    Vector3 normal = Vector3.zero;
    //    for (int i = 0; i < collision.contactCount; i++)
    //    {
    //        normal += collision.contacts[i].normal;
    //    }
    //    groundNormal = (normal / collision.contactCount).normalized;

    //    Debug.Log($"groundNormal={groundNormal}, collision.contactCount={collision.contactCount}");

    //    // qqq what if I hit a vertical wall? then result is 45d?
    //}

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
        if ((state == State.Ascending || state == State.Descending) && descendReady)
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
