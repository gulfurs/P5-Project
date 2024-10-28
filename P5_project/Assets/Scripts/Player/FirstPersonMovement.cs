using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float mouseSensitivity = 100f;

    public Transform cameraTransform; 
    private CharacterController controller;

    float xRotation = 0f;
    public Vector3 startingPosition;

    void Start()
    {
        // Lock the cursor to the game window initially
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        
        controller = GetComponent<CharacterController>();
        
        startingPosition = transform.position;
    }

    void Update()
    {
        // SHIFT TO CONTROL CURSOR
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            // Unlock the cursor and make it visible
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // LOCK THE CURSOR
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Move the player based on WASD keys
            MovePlayer();

            // ROTATE CAMERA
            RotateCamera();
        }
    }
    
    void MovePlayer()
    {
        // Get input for horizontal and vertical movement
        float moveX = Input.GetAxis("Horizontal");  // A/D or Left/Right arrow
        float moveZ = Input.GetAxis("Vertical");    // W/S or Up/Down arrow

        // Move direction in local space (relative to the player's forward and right)
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // Apply the movement using the character controller
        controller.Move(move * moveSpeed * Time.deltaTime);
    } 

    void RotateCamera()
    {
        // MOVEMENT INPUT
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // ROTATE X AND Y
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // Clamp vertical rotation

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // ROTATE AROUND Y AXIS
        transform.Rotate(Vector3.up * mouseX);
    }
}
