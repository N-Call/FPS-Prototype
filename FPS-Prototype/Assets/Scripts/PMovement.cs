using System.ComponentModel;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class PMovement : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask playerMask;

    [Header("Movement Settings")]
    [SerializeField] float baseSpeed = 5f;
    [SerializeField] private float modSprint = 1.5f;

    [Header("Crouch Settings")]
    [SerializeField] private float crouchHeightMod = 0.5f;
    [SerializeField] private float crouchSpeedMod = 0.5f;
    [SerializeField] private bool crouchSprint = false;

    [Header("Gravity and Jumping")]
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float gravityMax = 10f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private int maxJumps = 1;

    //Store the primary and secondary weapon's gameobjects
    [Header("Weapon Settings")]
    [SerializeField] private GameObject primWeapon;
    [SerializeField] private GameObject SecWeapon;

    private Vector3 inputDir;
    private Vector3 moveDir;
    private Vector3 vertVel;
    private int currJumpCount = 0;
    private float currentSpeed;
    private float originalHeight;
    private bool isCrouching;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalHeight = controller.height;
    }

    // Update is called once per frame
    void Update()
    {
        // This checks the ipnut of the direction being pressed in question, as well as movement.
        handleMovement();

        //this method is for the inputs related to weapons
        WeaponInput();
    }

    void handleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // Find the input direction in world space.
        inputDir = (transform.right * h + transform.forward * v);

        // Determine sprint state and speed
        bool isSprinting = Input.GetButton("Sprint");
        currentSpeed = baseSpeed * ((isSprinting && (!isCrouching || (isCrouching && crouchSprint))) ? modSprint : 1f);

        if (controller.isGrounded)
        {
            // This is done, so once you touch the ground, your jumps count resets.
            currJumpCount = 0;
            vertVel = Vector3.zero;

            // This is needed to not just keep the player grounded
            vertVel.y = -1f;

            // Handle crouching
            if (Input.GetButton("Crouch"))
            {

                Crouch();
            }

            // Handle un-crouching
            if (Input.GetButtonUp("Crouch") || (!Input.GetButton("Crouch") && isCrouching))
            {
                UnCrouch();
            }

            // This checks the input for jumping.
            if (Input.GetButtonDown("Jump"))
            {
                if (isCrouching)
                {
                    UnCrouch();
                }

                vertVel.y = jumpForce;
                currJumpCount++;
            }

        }
        else
        {

            // This allows the jump to be done in mid-air, when under the max jump count.
            if (Input.GetButtonDown("Jump") && currJumpCount < maxJumps)
            {
                vertVel.y = jumpForce;
                currJumpCount++;
            }

            // This applies gravity, when not on the ground.
            vertVel.y -= gravity * Time.deltaTime;

            // This clamps the fall speed to avoid exceeding the maxFallspeed.
            vertVel.y = Mathf.Max(vertVel.y, -gravityMax);
        }

        // Apply current speed directly
        moveDir = inputDir * currentSpeed;

        // This grabs the H-movement and Fall speed.
        Vector3 moveFinal = moveDir + vertVel;

        // Now move the player using the controller itself after all of that is said and done.
        controller.Move(moveFinal * Time.deltaTime);

    }

    void WeaponInput()
    {
        //check for primary weapon
        if (Input.GetButtonDown("Fire1") && primWeapon != null)
        {
            //launch attack method
            primWeapon.GetComponent<IWeapon>().Attack(playerMask, cam);
        }

        //Change weapon if pressed
        if (Input.GetButtonDown("Fire3"))
        {
            ChangeWeapon();
        }

        if (Input.GetButtonDown("Reload"))
        {
            IReloadable reloadable = primWeapon.GetComponent<IReloadable>();
            reloadable?.Reload();
        }
    }

    void ChangeWeapon()
    {
        //swap the primary and secondary Weapons
        GameObject temp = primWeapon;
        primWeapon = SecWeapon;
        SecWeapon = temp;

        //set the seconday to inactive
        SecWeapon.SetActive(false);
        primWeapon.SetActive(true);
    }

    void Crouch()
    {
        controller.height = originalHeight * crouchHeightMod;
        currentSpeed *= crouchSpeedMod;
        isCrouching = true;
    }

    void UnCrouch()
    {
        controller.height = originalHeight;
        currentSpeed /= crouchSpeedMod;
        isCrouching = false;
    }

}
