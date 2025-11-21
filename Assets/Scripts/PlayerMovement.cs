using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    // Camera the player looks through
    public Camera playerCamera;

    // Movement speeds
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;

    // Gravity force
    public float gravity = 10f;

    // Mouse look sensitivity
    public float lookSpeed = 2f;

    // Vertical camera rotation limit
    public float lookXLimit = 45f;

    // Height values for standing / crouching
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;

    // Internal movement variables
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;

    // Character controller reference
    private CharacterController characterController;

    // Whether the player is allowed to move/look
    private bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Lock the cursor to the center and hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Forward and right directions based on player rotation
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // ---- CROUCH ----
        
        bool isCrouching = Input.GetKey(KeyCode.LeftControl);

        // Adjust character height for crouch/stand
        characterController.height = isCrouching ? crouchHeight : defaultHeight;

        // Pick correct speed depending on crouch
        float curWalk = isCrouching ? crouchSpeed : walkSpeed;
        float curRun = isCrouching ? crouchSpeed : runSpeed;

        // ---- RUNNING ----
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        // Movement input on vertical and horizontal axes
        float curSpeedX = canMove ? (isRunning ? curRun : curWalk) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? curRun : curWalk) * Input.GetAxis("Horizontal") : 0;

        // Preserve Y movement (jumping/gravity)
        float movementDirectionY = moveDirection.y;

        // Calculate movement direction
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        // ---- JUMP ----
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // Apply gravity if not on ground
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the player controller
        characterController.Move(moveDirection * Time.deltaTime);

        // ---- CAMERA LOOK ----
        if (canMove)
        {
            // Vertical camera rotation (up/down)
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;

            // Clamp to limit look angle
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

            // Apply vertical rotation to camera
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

            // Horizontal rotation (turning left/right)
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }
}
