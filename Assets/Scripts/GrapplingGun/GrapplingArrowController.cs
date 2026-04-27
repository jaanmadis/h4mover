using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(GravityController))]
[RequireComponent(typeof(Rigidbody))]
public class GrapplingArrowController : MonoBehaviour
{
    [SerializeField] private Transform cablePoint;
    [SerializeField] private GrapplingGunController grapplingGun; // qqq circular

    public enum State
    { 
        Loaded,
        Fired,
        Attached,
        Retracting,
    }

    private Collider arrowCollider;
    private GravityController gravityController;
    private Rigidbody rigidBody;
    private State state = State.Loaded;

    public Transform CablePoint => cablePoint;
    public State CurrentState => state;

    void Awake()
    {
        arrowCollider = GetComponent<Collider>();
        arrowCollider.enabled = false;
        gravityController = GetComponent<GravityController>();
        gravityController.enabled = false;
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.isKinematic = true;
    }

    void FixedUpdate()
    {
        if (state == State.Retracting)
        {
            Vector3 toGun = (grapplingGun.transform.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(-toGun);
            rigidBody.MoveRotation(Quaternion.Slerp(rigidBody.rotation, targetRotation, 10f * Time.fixedDeltaTime));

            rigidBody.linearVelocity = toGun * 25f;

            //rigidBody.AddForce((grapplingGun.transform.position - transform.position) * 2f, ForceMode.Acceleration);

            if (Vector3.Distance(grapplingGun.transform.position, transform.position) < 2f)
            {
                transform.SetParent(grapplingGun.transform, false);
                transform.SetLocalPositionAndRotation(new Vector3(0f, 0f, 0.5f), Quaternion.identity);
                transform.localScale = new Vector3(0.2f, 0.2f, 1f);
                rigidBody.linearVelocity = Vector3.zero;
                rigidBody.angularVelocity = Vector3.zero;
                rigidBody.interpolation = RigidbodyInterpolation.None;
                rigidBody.isKinematic = true;
                state = State.Loaded;
                grapplingGun.HandleArrowRetracted();

                // 1. arrow sould be butt towards gun always
                // 2. i can run away from arrow while its retracting, that feels off.


            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state == State.Fired)
        {
            state = State.Attached;
            rigidBody.linearVelocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
            rigidBody.interpolation = RigidbodyInterpolation.None;
            rigidBody.isKinematic = true;
            arrowCollider.enabled = false;
        }
    }

    public void FireArrow(Vector3 direction)
    {
        if (state == State.Loaded)
        {
            state = State.Fired;
            transform.SetParent(null);
            rigidBody.isKinematic = false;
            rigidBody.linearVelocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
            rigidBody.interpolation = RigidbodyInterpolation.Interpolate;
            rigidBody.AddForce(direction * 100f, ForceMode.Impulse);
            arrowCollider.enabled = true;
        }
    }

    public void RetractArrow()
    {
        if (state == State.Attached)
        {
            state = State.Retracting;
            rigidBody.isKinematic = false;
            gravityController.enabled = true;
        }
    }
}
