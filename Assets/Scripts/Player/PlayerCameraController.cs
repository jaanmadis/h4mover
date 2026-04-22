using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private Transform orientation;

    private const float MOUSE_SENSITIVITY = 300f;

    private float xRotation = 0f;
    private float yRotation = 0f;

    public void HandleMouseLook()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * MOUSE_SENSITIVITY;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * MOUSE_SENSITIVITY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        orientation.localRotation = Quaternion.Euler(0f, yRotation, 0f);
    }
}
