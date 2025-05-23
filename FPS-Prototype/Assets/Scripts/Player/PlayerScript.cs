using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour, IDamage
{
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask playerMask;

    [Header("Health")]
    [SerializeField] int HP;
    [SerializeField] int isShielded;
    [SerializeField] int shieldMax;

    [Header("Walking")]
    [SerializeField][Tooltip("The walk speed of the player walking forwards")] float walkForwardSpeed = 8.0f;
    [SerializeField][Tooltip("The walk speed of the player walking sideways")] float walkSidewaysSpeed = 4.0f;
    [SerializeField][Tooltip("The walk speed of the player walking backwards")] float walkBackwardsSpeed = 4.0f;

    [Header("Jumping")]
    [SerializeField][Tooltip("The maximum amount of times the player can jump")] int maxJumps;
    [SerializeField][Tooltip("The jump height")] float jumpForce;
    [SerializeField] float gravity;
    [SerializeField][Tooltip("The fastest speed the player can fall")] float maxGravity;

    [Header("Sprinting")]
    [SerializeField] float sprintSpeed;

    [Header("Crouching")]
    [SerializeField] float crouchSpeedMultiplier;
    [SerializeField] float crouchHeightMultiplier;
    [SerializeField] float crouchRate = 0.05f;
    [SerializeField] float crouchWaitTimer = 0.001f;

    [Header("Sliding")]
    [SerializeField] float slideSpeedBonus;
    [SerializeField] float slideRate;
    [SerializeField] float slideJumpSpeedBonus;
    [SerializeField] float slideJumpRate;
    [SerializeField] float slideJumpMinimumSpeed;

    [Header("Weapon Settings")]
    [SerializeField] public List<GameObject> weaponList;

    [Header("Wall Running/Jumping")]
    [SerializeField] float wallRunDur;
    [SerializeField] float wallRunGravity;
    [SerializeField] float wallJumpForce;
    [SerializeField] float wallCheckDist;
    [SerializeField] float wallJumpHoriForce;
    [SerializeField] float wallRunCooldown;
    [SerializeField] LayerMask wallRunMask;
    [SerializeField] float wallStickForce;

    bool isWallRunning;         // Is the player wall jumping?
    bool wallJumped;            // Did the player wall jump?
    float wallRunTimer;         // TImer for the active wall run.
    float wallRunCooldownTimer; // Cooldown before another wall run can be made.
    Vector3 wallNormal;         // Normal of the wall being run on in question.
    Vector3 wallJumpVel;        // Horizontal force being applied for a wall jump.

    CameraController camControl;// TThis is referencing the CameraController for the tilting capabilities during wall running.

    [Header("Camera")]
    [SerializeField] float sprintFOVMod;
    [SerializeField] float changeRate;

    [Header("VFXParticleSystem")]
    public ParticleSystem particleSpMod;
    public ParticleSystem particleSpRun;
    public ParticleSystem particleJpMod;
    

    Coroutine crouchCoroutine;
    Coroutine unCrouchCoroutine;

    Vector3 verticalVelocity;

    float originalHeight;
    float horizontalSpeed;
    float verticalSpeed;
    float currentSlideSpeed;
    float jumpSpeedBonus;
    float speedModifier;
    float jumpModifier;
    float origFOV;
    float baseFOV;

    int originalHP;
    int jumpCount;

    bool isSprinting;
    bool isCrouching;
    bool isSliding;
    bool invulnerable;
    bool wallJumpOccurredThisFrame = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalHP = HP;
        originalHeight = controller.height;
        camControl = Camera.main.GetComponent<CameraController>();
        origFOV = Camera.main.fieldOfView; 
        baseFOV = origFOV;
        GameManager.instance.SetSpawnPosition(transform.position);
        UpdatePlayerUI();
    }

    // Update is called once per frame
    void Update()
    {
        wallJumpOccurredThisFrame = false;
        if (wallRunCooldownTimer > 0f)
            wallRunCooldownTimer -= Time.deltaTime;

        //Debug.DrawRay(transform.position, -transform.right * wallCheckDist, Color.blue);
        //Debug.DrawRay(transform.position, transform.right * wallCheckDist, Color.red);

        WallRunCheck();
        Movement();
        Jump();
        Sprint();
        Crouch();
        WeaponInput();
        SetCurrentFOV();
    }

//    public void getDialogue(DialogueSO text)
//    {
//        lines = text.lines;
//        speaker = text.speaker;
//        textSpeed = text.textSpeed;
//        speedBetweenText = text.speedBetweenText;

//}
    void WallRunCheck()
    {
        if (controller.isGrounded)
        {
            //Debug.Log("Grounded: stopping wall run.");
            StopWallRun();
            wallJumped = false;
            camControl.SetWallRunTilt(0f);
            return;
        }

        if (wallJumped || wallRunCooldownTimer > 0f)
        {
            //Debug.Log("Wall jump cooldown or already wall jumped.");
            StopWallRun();
            return;
        }

        float forwardInput = Input.GetAxis("Vertical");
        if (forwardInput <= 0.2f)
        {
            //Debug.Log("No forward input. Cancelling wall run.");
            StopWallRun();
            return;
        }

        RaycastHit hit;
        bool wallDetectedThisFrame = false;

        if (Physics.Raycast(transform.position, -transform.right, out hit, wallCheckDist, wallRunMask))
        {
            //Debug.Log("Wall detected on left");
            StartWallRun(hit.normal);
            wallDetectedThisFrame = true;
        }
        else if (Physics.Raycast(transform.position, transform.right, out hit, wallCheckDist, wallRunMask))
        {
            //Debug.Log("Wall detected on right");
            StartWallRun(hit.normal);
            wallDetectedThisFrame = true;
        }

        if (isWallRunning && !wallDetectedThisFrame)
        {
            //Debug.Log("No direct wall detected. Checking for edge.");

            //Debug.Log("Wall run ended due to no continuous wall detected.");
            StopWallRun();
            return;
        }

        if (isWallRunning)
        {
            //Debug.Log("Wall running...");
            wallRunTimer += Time.deltaTime;

            if (wallRunTimer > wallRunDur)
            {
                //Debug.Log("Wall run duration exceeded.");
                StopWallRun();
                return;
            }

            verticalVelocity.y = -wallRunGravity;

            if (Input.GetButtonDown("Jump"))
            {
                if (!wallJumped && wallRunCooldownTimer <= 0f)
                {
                    //Debug.Log("Wall jump triggered.");
                    SoundManager.instance.PlaySFX("playerJump", 0.3f);
                    verticalVelocity.y = wallJumpForce;
                    wallJumpVel = (wallNormal + transform.forward).normalized * wallJumpHoriForce;
                    wallJumped = true;
                    wallRunCooldownTimer = wallRunCooldown;
                    StopWallRun();

                    wallJumpOccurredThisFrame = true;
                    //Debug.Log($"Wall Jump - Wall Normal: {wallNormal}");
                    //Debug.Log($"Wall Jump - Calculated Wall Jump Velocity: {wallJumpVel}");
                }
            }
        }
    }

    void StartWallRun(Vector3 hitNormal)
    {
        isWallRunning = true;
        wallNormal = hitNormal;
        wallRunTimer = 0f;

        float tilt = Vector3.Dot(wallNormal, -transform.right) > 0 ? 1 : -1;
        camControl.SetWallRunTilt(tilt * 15f);
    }

    void StopWallRun()
    {
        if (isWallRunning)
            camControl.SetWallRunTilt(0f);

        isWallRunning = false;
        wallRunTimer = 0f;
    }

    void Movement()
    {
        // Horizontal direction
        float horizontal = Input.GetAxis("Horizontal");
        // If moving sideways
        bool sideways = horizontal != 0;

        // Vertical direction
        float vertical = Input.GetAxis("Vertical");
        // If moving forward
        bool forward = vertical > 0;
        // If moving backwards
        bool backwards = vertical < 0;

        // The direction the player is going
        Vector3 inputDirection = transform.right * horizontal + transform.forward * vertical;

        // Vertical & horizontal speed
        verticalSpeed = forward && backwards ? 0.0f : forward ? walkForwardSpeed : backwards ? walkBackwardsSpeed : 0.0f;
        horizontalSpeed = sideways ? walkSidewaysSpeed : 0.0f;

        // The current speed calculated
        float speed = CalculateSpeed();

        if (isWallRunning)
        {
            Vector3 wallRunMoveDirection = Vector3.ProjectOnPlane(inputDirection, wallNormal).normalized;

            Vector3 stickToWallForce = -wallNormal * wallStickForce;
            controller.Move((wallRunMoveDirection * speed + stickToWallForce) * Time.deltaTime);
        }
        else
        {
            // Move the player the direction and speed
            controller.Move(inputDirection * speed * Time.deltaTime);
        }

        // This applies the wall jump directional momentum
        if (wallJumpVel != Vector3.zero)
        {
            controller.Move(wallJumpVel * Time.deltaTime);
            wallJumpVel = Vector3.Lerp(wallJumpVel, Vector3.zero, 5f * Time.deltaTime); // This creates a fade out force as a result.
        }
    }

    float CalculateSpeed()
    {
        // The base speed (vertical prioritized over horizontal)
        float speed = verticalSpeed > 0 ? verticalSpeed : horizontalSpeed;
        
        if (isSprinting)
        {
            speed += sprintSpeed;
        }

        if (isCrouching)
        {
            speed *= crouchSpeedMultiplier;
        }

        // If there's a jump speed bonus...
        if (jumpSpeedBonus > 0.0f)
        {
            // Apply jump speed bonus
            speed += jumpSpeedBonus;

            if (jumpSpeedBonus > slideJumpMinimumSpeed)
            {
                // Gradually decrease the jump speed bonus
                // until it is <= slide jump minimum speed
                jumpSpeedBonus -= slideJumpRate;
            }
        }

        // Handle slide speed
        if (isSliding)
        {
            float calculatedCrouchSpeed = speed * crouchSpeedMultiplier;
            speed += currentSlideSpeed;

            if (speed <= calculatedCrouchSpeed)
            {
                isSliding = false;
                isCrouching = true;
                speed = calculatedCrouchSpeed;
            }

            currentSlideSpeed -= slideRate;
        }

        // Return the calculated speed, and factor in external speed modifiers
        return speed + (speed * speedModifier);
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumps && !wallJumpOccurredThisFrame)
        {

            SoundManager.instance.PlaySFX("playerJump", 0.3f);
            // Handle slide jump
            if (isSliding)
            {
                // Apply slide jump speed boost
                jumpSpeedBonus = slideJumpSpeedBonus;
            }
            // Handle jump force (with external jump multiplier factored in)
            verticalVelocity.y = jumpForce + (jumpForce * jumpModifier);
            jumpCount++;
        }

        if (!isWallRunning)
        {
            verticalVelocity.y -= gravity * Time.deltaTime;
            verticalVelocity.y = Mathf.Max(verticalVelocity.y, -maxGravity);
        }

        controller.Move(verticalVelocity * Time.deltaTime);

        // Reset jumps, slide jump speed bonus, and applied gravity
        if (controller.isGrounded)
        {
            if (verticalVelocity.y < 0f)
                verticalVelocity.y = 0.0f;
            jumpCount = 0;
            jumpSpeedBonus = 0.0f;
            wallJumpVel = Vector3.zero;
        }
    }

    // Handle sprint inputs
    void Sprint()
    {
        if (Input.GetButton("Sprint") && controller.isGrounded && !isSliding)
        {
            isSprinting = true;
            particleSpRun.gameObject.SetActive(true);
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            isSprinting = false;
            particleSpRun.gameObject.SetActive(false);

        }
    }

    // Handle crouch and slide inputs
    void Crouch()
    {
        if (Input.GetButtonDown("Crouch"))
        {
            if (isSprinting && controller.isGrounded)
            {
                isSliding = true;
                currentSlideSpeed = slideSpeedBonus;
            }

            else
            {
                isCrouching = true;
            }

            if (unCrouchCoroutine != null)
            {
                StopCoroutine(unCrouchCoroutine);
                unCrouchCoroutine = null;
            }

            if (crouchCoroutine == null)
            {
                crouchCoroutine = StartCoroutine(HandleCrouchHeight(true));
            }
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            if (crouchCoroutine != null)
            {
                StopCoroutine(crouchCoroutine);
                crouchCoroutine = null;
            }

            isCrouching = false;
            isSliding = false;

            if (unCrouchCoroutine == null)
            {
                unCrouchCoroutine = StartCoroutine(HandleCrouchHeight(false));
            }
        }
    }

    void WeaponInput()
    {
        //check for primary weapon
        if (Input.GetButtonDown("Fire1") && weaponList != null)
        {
            //launch attack method
            weaponList[0].GetComponent<IWeapon>()?.AttackBegin(playerMask);

        }

        if (Input.GetButtonUp("Fire1") && weaponList != null)
        {
            //launch attack method
            weaponList[0].GetComponent<IWeapon>()?.AttackEnd(playerMask);

        }

        //Change weapon if pressed
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            ChangeWeapon(Input.GetAxis("Mouse ScrollWheel"));
        }

        if (Input.GetButtonDown("Reload"))
        {
            IReloadable reloadable = weaponList[0].GetComponent<IReloadable>();
            reloadable?.Reload();
        }
    }

    void ChangeWeapon(float scroll)
    {
        weaponList[0].SetActive(false);
        if (scroll > 0)
        {
            //move the primary down the list
            GameObject temp = weaponList[0];
            weaponList.RemoveAt(0);
            weaponList.Add(temp);
        }
        else
        {
            //up the primary up the list
            GameObject temp = weaponList[weaponList.Count - 1];
            weaponList.RemoveAt(weaponList.Count - 1);
            weaponList.Insert(0, temp);
        }

        //set the seconday to inactive
        weaponList[0].SetActive(true);
    }

    public void TakeDamage(int amount)
    {
        if (invulnerable)
        {
            invulnerable = false;
            return;
        }

        SoundManager.instance.PlaySFX("playerHurt", 0.2f);

        HP -= amount;
        UpdatePlayerUI();
        StartCoroutine(FlashDamageScreen());

        if (HP <= 0)
        {
            GameManager.instance.YouLose();
            //GameManager.instance.Respawn();
        }
    }

    public void ResetPlayerStats()
    {
        speedModifier = 0.0f;
        jumpModifier = 0.0f;
        verticalVelocity.y = 0.0f;
        HP = originalHP;
        invulnerable = false;
        ResetFOV();
    }

    public void UpdatePlayerUI()
    {
        // update player health bar at full and when taking damage
        GameManager.instance.playerHPbar.fillAmount = (float)HP / originalHP;

        GameManager.instance.playerShieldbar.fillAmount = (float)isShielded / shieldMax;
    }

    public void SetSpeedModifier(float modifier)
    {
        speedModifier = modifier;
    }

    public void SetJumpModifier(float modifier)
    {
        jumpModifier = modifier;
    }

    public void AddModifier(float speed = 0.0f, float jump = 0.0f)
    {
        speedModifier += speed;
        jumpModifier += jump;
    }

    public void SetShield(int shieldAmount)
    {
        if (isShielded < shieldMax)
        {
            isShielded += shieldAmount;
        }
    }

    public void SetCurrentFOV()
    {
        if (isSprinting == true)
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, baseFOV + sprintFOVMod, changeRate * Time.deltaTime);
        }
        else
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, baseFOV, changeRate * Time.deltaTime);
        }
    }

    public void SetBaseFOV(float target)
    {
        baseFOV = target;
    }
    public void ResetFOV()
    {
        baseFOV = origFOV;
    }

    // Gradually crouch and uncrouch
    IEnumerator HandleCrouchHeight(bool crouch)
    {
        if (crouch)
        {
            while (controller.height > originalHeight * crouchHeightMultiplier)
            {
                controller.height -= crouchRate;
                yield return new WaitForSeconds(crouchWaitTimer);
            }
        }
        else
        {
            while (controller.height < originalHeight)
            {
                controller.height += crouchRate;
                yield return new WaitForSeconds(crouchWaitTimer);
            }
        }
    }

    IEnumerator FlashDamageScreen()
    {
        GameManager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        GameManager.instance.playerDamageScreen.SetActive(false);
    }

    #region Save and Load
    public void Save(ref PlayerSaveData data)
    {
        data.position = transform.position;
    }

    public void Load(PlayerSaveData data)
    {
        transform.position = data.position;
    }

    public void getDialogue(Dialogue text)
    {
        throw new System.NotImplementedException();
    }

    #endregion
}

[System.Serializable]
public struct PlayerSaveData
{
    public Vector3 position;
}
