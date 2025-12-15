using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{

    // Weapons
    public WeaponDamage mace;
    public WeaponDamage axe;
    private WeaponDamage currentWeapon;

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
    public float defaultHeight = 3.5f;
    public float crouchHeight = 1.5f;
    public float crouchSpeed = 3f;

    // Internal movement variables
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;

    // Character controller reference
    private CharacterController characterController;

    // Whether the player is allowed to move/look
    private bool canMove = true;

    private Animator animator;

    // audio for picking weapon
    public AudioSource pickWeapon;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // weapons
        currentWeapon = mace;
        mace.gameObject.SetActive(true);
        axe.gameObject.SetActive(false);

        // Lock the cursor to the center and hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Forward and right directions based on player rotation
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right   = transform.TransformDirection(Vector3.right);

        // ---- CROUCH ----
        bool isCrouching = Input.GetKey(KeyCode.LeftControl);

        // Adjust character height for crouch/stand
        characterController.height = isCrouching ? crouchHeight : defaultHeight;

        // keep the capsule centered so the bottom stays on the ground
        characterController.center = new Vector3(
            characterController.center.x,
            characterController.height / 2f,
            characterController.center.z
        );

        // Pick correct speed depending on crouch
        float curWalk = isCrouching ? crouchSpeed : walkSpeed;
        float curRun  = isCrouching ? crouchSpeed : runSpeed;

        // ---- RUNNING INPUT ----
        bool isRunningInput = Input.GetKey(KeyCode.LeftShift);

        // Movement input on vertical and horizontal axes
        float inputVertical   = canMove ? Input.GetAxis("Vertical")   : 0f;
        float inputHorizontal = canMove ? Input.GetAxis("Horizontal") : 0f;

        float curSpeedX = (isRunningInput ? curRun : curWalk) * inputVertical;
        float curSpeedY = (isRunningInput ? curRun : curWalk) * inputHorizontal;

        // Preserve Y movement (jumping/gravity)
        float movementDirectionY = moveDirection.y;

        // Calculate movement direction
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        // horizontal speed for animation
        Vector3 horizontalMove = new Vector3(moveDirection.x, 0f, moveDirection.z);
        float speed = horizontalMove.magnitude; // 0 = idle, >0 = moving

      if (animator != null)
{
    animator.SetFloat("Speed", speed);

    bool pressingW  = Input.GetKey(KeyCode.W);
    bool pressingS  = Input.GetKey(KeyCode.S);
    bool pressingA  = Input.GetKey(KeyCode.A);
    bool pressingD  = Input.GetKey(KeyCode.D);
    bool shiftHeld  = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    bool isGrounded = characterController.isGrounded;

    // Base values
    bool runningForward = false;
    bool walkingForward = false;
    bool walkingBack    = false;
    bool walkingLeft    = false;
    bool walkingRight   = false;

    if (isGrounded && canMove)
    {
        // PRIORITY: Forward > Back > Left > Right

        if (pressingW)
        {
            // Run vs walk
            runningForward = shiftHeld;
            walkingForward = !shiftHeld;
        }
        else if (pressingS)
        {
            walkingBack = true;
        }
        else if (pressingA)
        {
            walkingLeft = true;
        }
        else if (pressingD)
        {
            walkingRight = true;
        }
    }

    // Apply to Animator (only ONE of these will be true)
    animator.SetBool("RunForward", runningForward);
    animator.SetBool("isWalking",  walkingForward);
    animator.SetBool("walkBack",   walkingBack);
    animator.SetBool("leftS",      walkingLeft);
    animator.SetBool("rightS",     walkingRight);
    animator.SetBool("walkBack", walkingBack);                                // <= NEW
}


        // ---- JUMP ----
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            animator.SetTrigger("Jump");
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

        // weapon switching
        if (Input.GetKeyDown(KeyCode.Alpha1))   // key "1"
        {
            EquipWeapon(mace);   // mace
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))   // key "2"
        {
            EquipWeapon(axe);   // axe
        }

        // Attack
        if (Input.GetMouseButtonDown(0))    //leftClick
        {
            if (currentWeapon == null) { return; }
            animator.SetTrigger("Attack");
            currentWeapon.ResetSwing();
            currentWeapon.PlaySwing();
            StartCoroutine(EnableWeaponHitbox());
        }

        // Downward Attack
        if (Input.GetMouseButtonDown(1))
        {
            //if (currentWeapon == null) { return; };
            animator.SetTrigger("DownAttack");
            currentWeapon.ResetSwing();
            currentWeapon.PlaySwing();
            StartCoroutine(EnableWeaponHitbox());
        }
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

    private IEnumerator EnableWeaponHitbox()
    {
        if (currentWeapon == null) yield break;
        yield return new WaitForSeconds(.25f);
        currentWeapon.canDealDamage = true;
        yield return new WaitForSeconds(0.25f);   // big window, tweak if needed
        currentWeapon.canDealDamage = false;
    }

    void EquipWeapon(WeaponDamage newWeapon)
    {
        if (currentWeapon != null)
        {
            currentWeapon.gameObject.SetActive(false);
        }

        currentWeapon = newWeapon;
        currentWeapon.gameObject.SetActive(true);
        currentWeapon.ResetSwing();

        pickWeapon.Play();
    }
}
