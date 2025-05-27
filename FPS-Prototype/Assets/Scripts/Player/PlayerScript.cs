using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour, IDamage, IElemental
{
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask playerMask;

    [Header("Health")]
    [SerializeField] int HP;
    [SerializeField] float invincHitTime;
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
    [SerializeField] LayerMask wallRunMask;
    [SerializeField] float wallRunDur;
    [SerializeField] float wallRunGravity;
    [SerializeField] float wallJumpForce;
    [SerializeField] float wallCheckDist;
    [SerializeField] float wallJumpHoriForce;
    [SerializeField] float wallRunCooldown;
    [SerializeField] float wallStickForce;

    [SerializeField]
    [Tooltip("Provides the player with an additional jump if they used all of them before running on the wall")]
    bool provideExtraJumpIfNeeded;

    [Header("Elements")]
    [SerializeField] float speedElemMod;
    [SerializeField] float speedElemFOVMod;
    [SerializeField] float speedElemModTime;
    [SerializeField] float jumpElemMod;
    [SerializeField] float jumpElemModTime;
    [SerializeField] int shieldElemMod;

    bool isWallRunning;         // Is the player wall jumping?
    bool wallJumped;            // Did the player wall jump?
    float wallRunTimer;         // Timer for the active wall run.
    float wallRunCooldownTimer; // Cooldown before another wall run can be made.
    Vector3 wallNormal;         // Normal of the wall being run on in question.
    Vector3 wallJumpVel;        // Horizontal force being applied for a wall jump.

    CameraController camControl;// TThis is referencing the CameraController for the tilting capabilities during wall running.

    [Header("Head Bobbing")]
    [SerializeField][Tooltip("The amplitude of the head bobbing when walking.")] float walkBobAmp;
    [SerializeField][Tooltip("The frequency of the head bobbing when walking.")] float walkBobFreq;
    [SerializeField][Tooltip("The amplitude of the head bobbing when sprinting.")] float sprintBobAmp;
    [SerializeField][Tooltip("The frequency of the head bobbing when sprinting.")] float sprintBobFreq;
    [SerializeField][Tooltip("The speed of the camera returning to its original position.")] float bobReturnSpeed;

    private Vector3 cameraLocalPosOrig;
    private float bobTimer;

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
    float currSpeed;
    float iFrameTimer;
    // Element Timers
    float speedBuffTimer;
    float jumpBuffTimer;
    float speedDebuffTimer;
    float jumpDebuffTimer;

    int originalHP;
    int checkPointHP;
    int jumpCount;

    bool isSprinting;
    bool isCrouching;
    bool isSliding;
    bool invulnerable = false;
    bool wallJumpOccurredThisFrame = false;
    //Element Buff/Debuff
    bool speedBuffed;
    bool jumpBuffed;
    bool speedDebuffed;
    bool jumpDebuffed;
    bool elemInversed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalHP = HP;
        checkPointHP = HP;
        originalHeight = controller.height;
        camControl = Camera.main.GetComponent<CameraController>();
        origFOV = Camera.main.fieldOfView;
        baseFOV = origFOV;
        GameManager.instance.SetSpawnPosition(transform.position);
        UpdatePlayerUI();
        cameraLocalPosOrig = Camera.main.transform.localPosition;
        Application.targetFrameRate = 60;
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
        HandleHeadBobbing();

        if (speedBuffed || jumpBuffed || speedDebuffed || jumpDebuffed)
        {
            HandleElements();
        }
        if (invulnerable)
        {
            iFrameTimer += Time.deltaTime;
            if (iFrameTimer >= invincHitTime)
            {
                invulnerable = false;
            }
        }
    }

    void HandleHeadBobbing()
    {
        float currAmp = 0f;
        float currFreq = 0f;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        bool isMoveInput = (Mathf.Abs(horizontalInput) > 0.01f || Mathf.Abs(verticalInput) > 0.01f);

        //Debug.Log($"Frame: {Time.frameCount} | isMoveInput: {isMoveInput} | isGrounded: {controller.isGrounded}");

        // This determines the amplitude and frequency based on the movement state.
        // And is only applied while grounded.
        if (controller.isGrounded)
        {
            if (isMoveInput)
            {
                if (isSprinting) // Is the player sprinting?
                {
                    currAmp = sprintBobAmp;
                    currFreq = sprintBobFreq;
                    //Debug.Log($"Sprinting - Amp: {currAmp}, Freq: {currFreq}");
                }
                else // Is the player walking?
                {
                    currAmp = walkBobAmp;
                    currFreq = walkBobFreq;
                    //Debug.Log($"Walking - Amp: {currAmp}, Freq: {currFreq}");
                }
            }
        }

        if (currAmp > 0f)
        {
            // This timer increments based on the frequency;
            bobTimer += Time.deltaTime * currFreq;

            // This calculateds the bobbing effect offset using a sine wave system.
            // The Mathf.Sin function allows me to create a smooth, oscillating value betrween -1 and 1.
            // Also, multiplying by the currAmp scales this oscillation to any desired bobbing height as you wish.
            float bobbingOffset = Mathf.Sin(bobTimer) * currAmp;
            //Debug.Log($"Bob Timer: {bobTimer}, Bobbing Offset: {bobbingOffset}");

            // Then I apply the offset to the camera's local Y position.
            Vector3 newCamLocalPos = cameraLocalPosOrig;
            newCamLocalPos.y += bobbingOffset;

            Camera.main.transform.localPosition = newCamLocalPos;
            //Debug.Log($"[Bobbing Active] Frame: {Time.frameCount} | bobTimer: {bobTimer:F4} | Offset: {bobbingOffset:F4} | Final Local Pos: {Camera.main.transform.localPosition:F4}");
        }
        else // If the player is either not moving at all, or is in the air. This includes wall running, jumping, falling, etc.
        {
            bobTimer = 0f; // This resets the timer, when not moving
            // This smoothly returns the camera back to its original position.
            Camera.main.transform.localPosition = Vector3.Lerp(Camera.main.transform.localPosition, cameraLocalPosOrig, Time.deltaTime * bobReturnSpeed);
            //Debug.Log($"[Bobbing Reset] Frame: {Time.frameCount} | Final Local Pos: {Camera.main.transform.localPosition:F4}");
        }
    }

    void WallRunCheck()
    {
        // Stop running if grounded
        if (controller.isGrounded)
        {
            //Debug.Log("Grounded: stopping wall run.");
            StopWallRun();
            wallJumped = false;
            camControl.SetWallRunTilt(0f);
            return;
        }

        // Stop if they wall-jumped or the cooldown isn't over yet
        if (wallJumped || wallRunCooldownTimer > 0f)
        {
            //Debug.Log("Wall jump cooldown or already wall jumped.");
            StopWallRun();
            return;
        }

        float forwardInput = Input.GetAxis("Vertical");
        // Stop wall running if they stop moving forward
        if (forwardInput <= 0.2f)
        {
            //Debug.Log("No forward input. Cancelling wall run.");
            StopWallRun();
            return;
        }

        RaycastHit hit;
        bool wallDetectedThisFrame = false;

        // Check if a runnable wall is on the left or right of the player
        // Start wall running if so
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

        // Stop wall running if they reach the end of the wall
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

            // Stop wall running if they wall run passed the allowed duration
            if (wallRunTimer > wallRunDur)
            {
                //Debug.Log("Wall run duration exceeded.");
                StopWallRun();
                return;
            }

            // Apply gravity
            verticalVelocity.y = -wallRunGravity;

            // Handle wall jump
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

        if (provideExtraJumpIfNeeded && jumpCount == maxJumps)
        {
            jumpCount -= 1;
        }

        float tilt = Vector3.Dot(wallNormal, -transform.right) > 0 ? 1 : -1;
        camControl.SetWallRunTilt(tilt * 15f);
    }

    void StopWallRun()
    {
        if (isWallRunning)
        {
            camControl.SetWallRunTilt(0f);
            wallRunCooldownTimer = wallRunCooldown;
        }
        wallJumped = false;
        isWallRunning = false;
        wallJumped = false;
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

        if (speedModifier < 1)
        {
            currSpeed = speed + (speed * speedModifier);
        }
        else
        {
            currSpeed = speed *= speedModifier;
        }

        // Return the calculated speed, and factor in external speed modifiers
        return currSpeed;
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
            if (jumpModifier < 1 && jumpModifier != 0)
            {
                verticalVelocity.y = jumpForce + (jumpForce * -(1.0f + jumpModifier));
            }
            else if (jumpModifier > 0)
            {
                verticalVelocity.y = jumpForce * jumpModifier;
            }
            else
            {
                verticalVelocity.y = jumpForce;
            }
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
            return;
        }

        SoundManager.instance.PlaySFX("playerHurt", 0.2f);

        if (isShielded > 0)
        {
            isShielded -= 1;
        }
        else
        {
            HP -= amount;
            StartCoroutine(FlashDamageScreen());
        }

        UpdatePlayerUI();
        iFrameTimer = 0;
        invulnerable = true;

        if (HP <= 0)
        {
            GameManager.instance.YouLose();
        }
    }

    public void ResetPlayerStats()
    {
        speedModifier = 0.0f;
        jumpModifier = 0.0f;
        verticalVelocity.y = 0.0f;
        HP = checkPointHP;
        invulnerable = false;
        ResetFOV();
        UpdatePlayerUI();
    }

    public void UpdateCheckpointHealth()
    {
        checkPointHP = HP;
    }

    public void UpdatePlayerUI()
    {
        // update player health bar at full and when taking damage
        GameManager.instance.playerHPbar.fillAmount = (float)HP / originalHP;

        // update player shield bar at full and when taking damage
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
        if (shieldAmount > shieldMax)
        {
            return;
        }
        else
        {
            isShielded = shieldAmount;
        }
    }

    public void AddHP(int amount)
    {
        if (amount < 1)
        {
            return;
        }

        HP += amount;
        if (HP > originalHP)
        {
            HP = originalHP;
        }

        UpdatePlayerUI();
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

    // Element Work
    void HandleElements()
    {
        if (speedBuffed)
        {
            speedBuffTimer += Time.deltaTime;
            if (speedBuffTimer >= speedElemModTime)
            {
                SpeedBuff();
            }
        }
        if (jumpBuffed)
        {
            jumpBuffTimer += Time.deltaTime;
            if (jumpBuffTimer >= jumpElemModTime)
            {
                JumpBuff();
            }
        }
        if (speedDebuffed)
        {
            speedDebuffTimer += Time.deltaTime;
            if (speedDebuffTimer >= speedElemModTime)
            {
                SpeedDebuff();
            }
        }
        if (jumpDebuffed)
        {
            jumpDebuffTimer += Time.deltaTime;
            if (jumpDebuffTimer >= jumpElemModTime)
            {
                JumpDebuff();
            }
        }
    }

    public void ElementBuff(int elem)
    {
        switch (elem)
        {
            case 1:

                if (elemInversed) { SpeedDebuff(); }
                else { SpeedBuff(); }
                break;
            case 2:
                Debug.Log("Jump Buff");
                if (elemInversed) { JumpDebuff(); }
                else { JumpBuff(); }
                break;
            case 3:
                Debug.Log("Shield Buff");
                if (elemInversed) { ShieldDebuff(); }
                else { ShieldBuff(); }
                break;
        }
    }
    public void ElementDebuff(int elem)
    {
        switch (elem)
        {
            case 1:

                if (!elemInversed) { SpeedDebuff(); }
                else { SpeedBuff(); }
                break;
            case 2:
                Debug.Log("Jump Debuff");
                if (!elemInversed) { JumpDebuff(); }
                else { JumpBuff(); }
                break;
            case 3:
                Debug.Log("Shield Debuff");
                if (!elemInversed) { ShieldDebuff(); }
                else { ShieldBuff(); }
                break;
        }
    }
    public void ElementInverse()
    {
        Debug.Log("Inversing");
        if (elemInversed)
        {
            elemInversed = false;
            GameManager.instance.playerInInverseScreen.SetActive(false);
        }
        else
        {
            elemInversed = true;
            GameManager.instance.playerInInverseScreen.SetActive(true);
        }
        //SwapBuffs();
    }

    public void SpeedBuff()
    {
        if (speedBuffed == false && speedBuffTimer < speedElemModTime)
        {
            Debug.Log("Speed Buff");
            SoundManager.instance.PlaySFX("powerUp", 0.3f);
            GameManager.instance.BuffSprintIcon(speedElemModTime);
            AddModifier(speedElemMod);
            SetBaseFOV(baseFOV + speedElemFOVMod);
            particleSpMod.gameObject.SetActive(true);
            speedBuffTimer = 0;
            speedBuffed = true;
        }
        else
        {
            particleSpMod.gameObject.SetActive(false);
            AddModifier(-speedElemMod);
            ResetFOV();
            speedBuffed = false;
        }
    }
    private void JumpBuff()
    {
        if (jumpBuffed == false && jumpBuffTimer < jumpElemModTime)
        {
            SoundManager.instance.PlaySFX("powerUp", 0.3f);
            GameManager.instance.BuffJumpIcon(jumpElemModTime);
            AddModifier(0.0f, jumpElemMod);
            particleJpMod.gameObject.SetActive(true);
            jumpBuffTimer = 0;
            jumpBuffed = true;
        }
        else
        {
            AddModifier(0.0f, -jumpElemMod);
            particleJpMod.gameObject.SetActive(false);
            jumpBuffed = false;
        }

    }
    private void SpeedDebuff()
    {
        if (speedDebuffed == false && speedDebuffTimer < speedElemModTime)
        {
            Debug.Log("Speed Debuff");
            SoundManager.instance.PlaySFX("debuff", 0.4f);
            GameManager.instance.DeBuffSprintIcon(speedElemModTime);
            AddModifier(-1 / speedElemMod);
            SetBaseFOV(baseFOV - speedElemFOVMod);
            speedDebuffTimer = 0;
            speedDebuffed = true;
        }
        else
        {
            AddModifier(1 / speedElemMod);
            ResetFOV();
            speedDebuffed = false;
        }
    }
    private void JumpDebuff()
    {
        if (jumpDebuffed == false && jumpDebuffTimer < jumpElemModTime)
        {
            SoundManager.instance.PlaySFX("debuff", 0.4f);
            GameManager.instance.DeBuffJumpIcon(jumpElemModTime);
            AddModifier(0.0f, -1 / jumpElemMod);
            jumpBuffTimer = 0;
            jumpDebuffed = true;
        }
        else
        {
            AddModifier(0.0f, 1 / jumpElemMod);
            jumpDebuffed = false;
        }
    }
    private void ShieldBuff()
    {
        SoundManager.instance.PlaySFX("powerUp", 0.3f);

        SetShield(isShielded = shieldElemMod);

        UpdatePlayerUI();
    }
    private void ShieldDebuff()
    {
        SoundManager.instance.PlaySFX("debuff", 0.4f);

        SetShield(isShielded - shieldElemMod);

        UpdatePlayerUI();
    }
    private void SwapBuffs()
    {
        //Swaps bools
        bool temp1 = speedBuffed;
        bool temp2 = speedDebuffed;
        speedBuffed = temp2;
        speedDebuffed = temp1;
        bool temp3 = jumpBuffed;
        bool temp4 = jumpDebuffed;
        jumpBuffed = temp4;
        jumpDebuffed = temp3;

        //Swap Timers
        float timer1 = speedBuffTimer;
        float timer2 = speedDebuffTimer;
        speedBuffTimer = timer2;
        speedDebuffTimer = timer1;
        float timer3 = jumpBuffTimer;
        float timer4 = jumpDebuffTimer;
        jumpBuffTimer = timer4;
        jumpDebuffTimer = timer3;
    }

}
