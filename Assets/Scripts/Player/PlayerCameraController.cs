using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private Transform orientation;

    private const float MOUSE_SENSITIVITY = 300f;
    private const float DEFAULT_XCLAMP = 90f;

    private float xRotation = 0f;
    private float yRotation = 0f;

    private bool locked = false;
    private float xLocked;
    private float yLocked;

    public void HandleMouseLook()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * MOUSE_SENSITIVITY;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * MOUSE_SENSITIVITY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -DEFAULT_XCLAMP, DEFAULT_XCLAMP);

        if (locked)
        {
            float xOffset = xRotation - xLocked;
            float yOffset = yRotation - yLocked;

            xOffset = Mathf.Clamp(xOffset, -30f, 30f);
            yOffset = Mathf.Clamp(yOffset, -30f, 30f);

            xRotation = xLocked + xOffset;
            yRotation = yLocked + yOffset;
        }

        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        orientation.localRotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    public void LockView()
    {
        locked = true;
        xLocked = xRotation;
        yLocked = yRotation;
    }

    public void UnlockView()
    {
        locked = false;
    }
}
