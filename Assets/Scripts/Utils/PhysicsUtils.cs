using UnityEngine;

public static class PhysicsUtils
{
    public static Vector3 Up(Transform transform)
    {
        return (transform.position - Constants.ASTEROID_CENTER).normalized;
    }
}
