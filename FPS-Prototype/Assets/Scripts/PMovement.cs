using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class PMovement : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private float walkAccel = 5f;
    [SerializeField] private float walkSpeedMax = 10f;
    [SerializeField] private float groundFriction = 0.25f;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float gravityMax = 36f;

    // This is used to determine how much upward force your character jumps.
    [SerializeField] private float jumpForce = 16f;
    [SerializeField] private int maxJumps = 1;

    private Vector3 currVel;
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
        // This checks the ipnut of the direction being pressed in question.
        inputHandle();

        // This applies movement after checking everything within the function.
        moveApply();
    }

    void inputHandle()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // Find the input direction in world space.
        inputDir = (transform.right * h + transform.forward * v).normalized;
    }

    void moveApply()
    {
        if (controller.isGrounded)
        {
            // This is done, so once you touch the ground, your jumps count resets.
            currJumpCount = 0;
            vertVel = Vector3.zero;

            // This is needed to not just keep the player grounded
            vertVel.y = -1f;

            if (inputDir.magnitude > 0)
            {
                // Accelerate towards the direction of the input in question.
                // Grabs the Max Walk Speed, if not at that speed, then grab the walk acceleration value.
                currVel = Vector3.MoveTowards(currVel, inputDir * walkSpeedMax, walkAccel * Time.deltaTime);
            }
            else
            {
                // Appplies friction, if and only if there's no input involved.
                currVel = Vector3.MoveTowards(currVel, Vector3.zero, groundFriction * Time.deltaTime);
            }

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
        Vector3 moveFinal = currVel + vertVel;

        // Now move the player using the controller itself after all of that is said and done.
        controller.Move(moveFinal * Time.deltaTime);

    }
}
