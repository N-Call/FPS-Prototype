using UnityEngine;
using UnityEngine.EventSystems;

public class PMovement : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;

    // Implement the speed with the sprinting modifier.
    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private float modSprint = 1.5f;

    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float gravityMax = 10f;

    // This is used to determine how much upward force your character jumps.
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private int maxJumps = 1;

    private Vector3 moveDir;
    private Vector3 inputDir;
    private Vector3 vertVel;
    private int currJumpCount = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // This checks the input of the direction being pressed in question, as well as overall movement.
        handleMovement();
    }

    void handleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // Find the input direction in world space.
        Vector3 inputDir = (transform.right * h + transform.forward * v);

        // This checks the sprinting on whether the 
        bool isSprinting = Input.GetButton("Sprint");
        float currSpeed = baseSpeed * (isSprinting ? modSprint : 1f);

        moveDir = inputDir * currSpeed;

        if (controller.isGrounded)
        {
            // This is done, so once you touch the ground, your jumps count resets.
            currJumpCount = 0;
            vertVel = Vector3.zero;

            // This is needed to keep the player grounded.
            vertVel.y = -1f;

            // This checks the input for jumping.
            if (Input.GetButtonDown("Jump"))
            {
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

        // This grabs the H-movement and Fall speed.
        Vector3 moveFinal = moveDir + vertVel;

        // Now move the player using the controller itself after all of that is said and done.
        controller.Move(moveFinal * Time.deltaTime);

    }
}
