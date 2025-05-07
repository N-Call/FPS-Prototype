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

    private Vector3 currVel;
    private Vector3 inputDir;

    private Vector3 vertVel;

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
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // Find the input direction in world space.
        inputDir = (transform.right * h + transform.forward * v).normalized;
    }

    void moveApply()
    {
        if (controller.isGrounded)
        {
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
        }
        else
        {
            // This applies gravity, when not on the ground.
            vertVel.y -= gravity * Time.deltaTime;

            // This clamps the fall speed to avoid exceeding the maxFallspeed.
            vertVel.y = Mathf.Max(vertVel.y, -gravityMax);
        }

        // This grabs the H-movement and Fall speed.
        Vector3 moveFinal = currVel + Vector3.up * Time.deltaTime;

        // Now move the player using the controller itself after all of that is said and done.
        controller.Move(moveFinal * Time.deltaTime);

    }
}
