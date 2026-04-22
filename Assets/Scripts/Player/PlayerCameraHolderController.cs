using UnityEngine;

public class PlayerCameraHolderController : MonoBehaviour
{
    [SerializeField] private Transform cameraPoint;

    void Update()
    {
        transform.SetPositionAndRotation(cameraPoint.position, cameraPoint.rotation);
    }
}
