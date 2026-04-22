using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityController : MonoBehaviour
{
    private Rigidbody rigidBody;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        rigidBody.useGravity = false;
    }

    void FixedUpdate()
    {
        // Add gravity force towards asteroid center
        rigidBody.AddForce(-1 * Constants.GRAVITY_STRENGTH_ASTEROID * PhysicsUtils.Up(transform), ForceMode.Acceleration);
    }
}
