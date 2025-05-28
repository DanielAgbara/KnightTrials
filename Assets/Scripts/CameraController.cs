using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform cameraPitch;
    public float mouseSensitivity = 2f;
    public float verticalLimit = 60f;

    private float pitch = 0f;

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Yaw (horizontal)
        transform.Rotate(Vector3.up * mouseX);

        // Pitch (vertical, clamped)
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -40f, verticalLimit);
        cameraPitch.localEulerAngles = new Vector3(pitch, 0f, 0f);
    }
}
