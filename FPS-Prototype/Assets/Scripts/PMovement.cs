//using UnityEngine;

//public class PMovement : MonoBehaviour
//{
//    [SerializeField] CharacterController controller;
//    [SerializeField] LayerMask ignoreLayer;

//    [SerializeField] float speed;
//    [SerializeField] float sprintMod;
//    [SerializeField] int jumpMax;
//    [SerializeField] float jumpForce;
//    [SerializeField] float gravity;

//    [SerializeField] int shootDamage;
//    [SerializeField] float shootRate;
//    [SerializeField] int shootDist;

//    float shootTimer;

//    int jumpCount;

//    Vector3 moveDir;
//    Vector3 playerVel; // This is for Y Velocity only!

//    bool isSprinting;

//    // Start is called once before the first execution of Update after the MonoBehaviour is created
//    void Start()
//    {

//    }

//    // Update is called once per frame
//    void Update()
//    {
//        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

//        movement();
//        sprint();
//    }
//    void movement()
//    {
//        shootTimer += Time.deltaTime;

//        if (controller.isGrounded)
//        {
//            jumpCount = 0;
//            playerVel = Vector3.zero;
//        }

//        moveDir = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward);

//        //transform.position += moveDir * speed * Time.deltaTime;

//        controller.Move(moveDir * speed * Time.deltaTime);

//        jump();

//        controller.Move(playerVel * Time.deltaTime);
//        playerVel.y -= gravity * Time.deltaTime;

//        if (Input.GetButton("Fire1") && shootTimer > shootRate)
//            shoot();
//    }
//    void sprint()
//    {
//        if (Input.GetButtonDown("Sprint"))
//        {
//            speed *= sprintMod;
//            isSprinting = true;
//        }
//        else if (Input.GetButtonUp("Sprint"))
//        {
//            speed /= sprintMod;
//            isSprinting = false;
//        }
//    }

//    void jump()
//    {
//        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
//        {
//            jumpCount++;
//            playerVel.y = jumpForce;

//        }
//    }

//    void shoot()
//    {
//        shootTimer = 0;

//        RaycastHit hit;
//        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
//        {
//            Debug.Log(hit.collider.name);
//            IDamage dmg = hit.collider.GetComponent<IDamage>();

//            if (dmg != null)
//            {
//                dmg.takeDamage(shootDamage);
//            }
//        }
//    }
//}

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
