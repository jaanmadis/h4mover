using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class OrientationController : MonoBehaviour
{
    private const float UPRIGHT_SPEED = 5f;
    private Rigidbody rigidBody;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (rigidBody.isKinematic)
        {
            return;
        }

        // Get relative up direction
        Vector3 up = PhysicsUtils.Up(transform);

        // Force upright orientation
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, up) * rigidBody.rotation;
        rigidBody.MoveRotation(Quaternion.Slerp(rigidBody.rotation, targetRotation, Time.fixedDeltaTime * UPRIGHT_SPEED));
    }
}
