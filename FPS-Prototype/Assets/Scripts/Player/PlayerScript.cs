using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour, IDamage, IPickup
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
    //[SerializeField] float wallRunCooldown;
    [SerializeField] float wallStickForce;
    [SerializeField] float minWallRunHeight;

    [SerializeField]
    [Tooltip("Provides the player with an additional jump if they used all of them before running on the wall")]
    bool provideExtraJumpIfNeeded;

    bool isWallRunning;         // Is the player wall jumping?
    bool wallJumped;            // Did the player wall jump?
    float wallRunTimer;         // Timer for the active wall run.
    //float wallRunCooldownTimer; // Cooldown before another wall run can be made.
    Vector3 wallNormal;         // Normal of the wall being run on in question.
    Vector3 wallJumpVel;        // Horizontal force being applied for a wall jump.
    private bool wallDetectedThisFrame;
    private GameObject wallRunLockedWall = null;

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
    float walkHorizontalDirection;
    float walkVerticalDirection;
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

    // Element
    float speedElemMod;
    float jumpElemMod;

    int originalHP;
    int jumpCount;

    bool isSprinting;
    bool isCrouching;
    bool isSliding;
    bool invulnerable = false;
    bool wallJumpOccurredThisFrame = false;
    //Element Buff/Debuff
    public bool speedBuffed;
    public bool jumpBuffed;
    public bool speedDebuffed;
    public bool jumpDebuffed;
    bool elemInversed;
    bool isPlayingStep;

    private void OnEnable()
    {
        InputActionManager.instance.AddPlayerPerform(InputActionManager.PlayerInputs.Pause, PerformPause);
        InputActionManager.instance.EnablePlayerInput();
    }

    private void OnDisable()
    {
        InputActionManager.instance.DisablePlayerInput();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalHP = HP;
        originalHeight = controller.height;
        camControl = Camera.main.GetComponent<CameraController>();
        origFOV = Camera.main.fieldOfView;
        GameManager.instance.SetSpawnPosition(transform.position);
        UpdatePlayerUI();
        cameraLocalPosOrig = Camera.main.transform.localPosition;
        Application.targetFrameRate = 60;
        baseFOV = origFOV;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 walkInputDirection = InputActionManager.instance.playerWalkAction.ReadValue<Vector2>();
        walkHorizontalDirection = walkInputDirection.x;
        walkVerticalDirection = walkInputDirection.y;

        wallJumpOccurredThisFrame = false;
        if (controller.isGrounded)
        {
            wallRunLockedWall = null;
        }
        //Debug.DrawRay(transform.position, -transform.right * wallCheckDist, Color.blue);
        //Debug.DrawRay(transform.position, transform.right * wallCheckDist, Color.red);

        WallRunCheck();
        Jump();
        Sprint();
        Crouch();
        WeaponInput();
        SetCurrentFOV();
        HandleHeadBobbing();

        if (invulnerable)
        {
            iFrameTimer += Time.deltaTime;
            if (iFrameTimer >= invincHitTime)
            {
                invulnerable = false;
            }
        }
    }

    void FixedUpdate()
    {
        Movement();
    }

    void HandleHeadBobbing()
    {
        float currAmp = 0f;
        float currFreq = 0f;

        bool isMoveInput = (Mathf.Abs(walkHorizontalDirection) > 0.01f || Mathf.Abs(walkVerticalDirection) > 0.01f);

        //Debug.Log($"Frame: {Time.frameCount} | isMoveInput: {isMoveInput} | isGrounded: {controller.isGrounded}");

        // This determines the amplitude and frequency based on the movement state.
        // And is only applied while grounded.
        if (controller.isGrounded && !isCrouching && !isSliding)
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

        if (currAmp > 0f && !isCrouching && !isSliding)
        {
            // This timer increments based on the frequency;
            bobTimer += Time.deltaTime * currFreq;

            // This calculateds the bobbing effect offset using a sine wave system.
            // The Mathf.Sin function allows me to create a smooth, oscillating value betrween -1 and 1.
            // Also, multiplying by the currAmp scales this oscillation to any desired bobbing height as you wish.
            float bobbingOffset = (-0.5f * Mathf.Sin(bobTimer) - 0.5f) * currAmp;
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
        // Immediately stop wall run if player is grounded while wall running
        if (isWallRunning && controller.isGrounded)
        {
            StopWallRun(null, false);
            return;
        }

        RaycastHit groundHit;
        float distToGnd = Mathf.Infinity;

        if (Physics.Raycast(transform.position, Vector3.down, out groundHit, 100f, ~0))
            distToGnd = groundHit.distance;

        wallDetectedThisFrame = false;
        Vector3 currWallNormal = Vector3.zero;
        GameObject hitWallObject = null;
        RaycastHit wallHit;

        // Check if a runnable wall is on the left or right of the player
        if (Physics.Raycast(transform.position, -transform.right, out wallHit, wallCheckDist, wallRunMask))
        {
            currWallNormal = wallHit.normal;
            hitWallObject = wallHit.collider.gameObject;
            wallDetectedThisFrame = true;
        }
        else if (Physics.Raycast(transform.position, transform.right, out wallHit, wallCheckDist, wallRunMask))
        {
            currWallNormal = wallHit.normal;
            hitWallObject = wallHit.collider.gameObject;
            wallDetectedThisFrame = true;
        }

        bool tryToRunOnLockedWall = (wallRunLockedWall != null && hitWallObject == wallRunLockedWall);
        bool canInitiateWallRun = wallDetectedThisFrame && !controller.isGrounded && distToGnd > minWallRunHeight && !wallJumped && !tryToRunOnLockedWall && walkHorizontalDirection > 0.2f && verticalVelocity.y < 0f;
        bool canContinueWallRun = isWallRunning && wallDetectedThisFrame && !controller.isGrounded;

        if (canInitiateWallRun)
        {
            StartWallRun(currWallNormal, hitWallObject);
        }
        else if (canContinueWallRun)
        {
            wallNormal = currWallNormal;
            wallRunTimer += Time.deltaTime;
            verticalVelocity.y = -wallRunGravity;

            if (wallRunTimer > wallRunDur)
            {
                StopWallRun(hitWallObject, false);
                return;
            }

            // Handle Wall Jump
            if (InputActionManager.instance.playerJumpAction.IsPressed())
            {
                if (!wallJumped)
                {
                    SoundManager.instance.PlaySFX("playerJump");
                    verticalVelocity.y = wallJumpForce;
                    wallJumpVel = (wallNormal + transform.forward).normalized * wallJumpHoriForce;
                    wallJumped = true;
                    StopWallRun(hitWallObject, true);

                    wallJumpOccurredThisFrame = true;
                }
            }
        }
        else if (isWallRunning)
        {
            StopWallRun(null, false);
        }
    }

    void StartWallRun(Vector3 hitNormal, GameObject wallObject)
    {
        isWallRunning = true;
        wallNormal = hitNormal;
        wallRunTimer = 0f;
        wallRunLockedWall = wallObject;

        if (provideExtraJumpIfNeeded && jumpCount == maxJumps)
        {
            jumpCount -= 1;
        }

        float tilt = Vector3.Dot(wallNormal, -transform.right) > 0 ? 1 : -1;
        camControl.SetWallRunTilt(tilt * 15f);
    }

    void StopWallRun(GameObject wallToLock = null, bool wasJump = false)
    {
        bool wasWallRunning = isWallRunning;
        isWallRunning = false;
        wallRunTimer = 0f;

        wallNormal = Vector3.zero;

        if (!wasJump)
        {
            verticalVelocity.y = -1f;
            wallJumpVel = Vector3.zero;
        }

        if (wasWallRunning)
        {
            camControl.SetWallRunTilt(0f);
        }

        if (wallToLock != null)
            wallRunLockedWall = wallToLock;
    }

    void Movement()
    {
        // If moving sideways
        bool sideways = walkHorizontalDirection != 0;

        // If moving forward
        bool forward = walkVerticalDirection > 0;
        // If moving backwards
        bool backwards = walkVerticalDirection < 0;

        // The direction the player is going
        Vector3 direction = transform.right * walkHorizontalDirection + transform.forward * walkVerticalDirection;

        // Vertical & horizontal speed
        verticalSpeed = forward && backwards ? 0.0f : forward ? walkForwardSpeed : backwards ? walkBackwardsSpeed : 0.0f;
        horizontalSpeed = sideways ? walkSidewaysSpeed : 0.0f;

        // The current speed calculated
        float speed = CalculateSpeed();

        // Debug added here to track state in FixedUpdate

        if (isWallRunning)
        {
            Vector3 wallRunMoveDirection = Vector3.ProjectOnPlane(direction, wallNormal).normalized;
            Vector3 stickToWallForce = Vector3.zero;
            if (wallNormal != Vector3.zero)
            {
                stickToWallForce = -wallNormal * wallStickForce;
            }
            controller.Move((wallRunMoveDirection * speed + stickToWallForce) * Time.deltaTime);
        }
        else
        {
            controller.Move(direction * speed * Time.deltaTime);
            if (direction != Vector3.zero && !isPlayingStep && controller.isGrounded)
            {
                StartCoroutine(PlaySteps());
            }
        }

        if (wallJumpVel != Vector3.zero)
        {
            controller.Move(wallJumpVel * Time.deltaTime);
            wallJumpVel = Vector3.Lerp(wallJumpVel, Vector3.zero, 5f * Time.deltaTime);
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

        if (isCrouching && controller.isGrounded)
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
        if (InputActionManager.instance.playerJumpAction.WasPressedThisFrame() && jumpCount < maxJumps && !wallJumpOccurredThisFrame)
        {
            SoundManager.instance.PlaySFX("playerJump");

            // Handle slide jump
            if (isSliding)
            {
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

        // Apply normal gravity when not wall running
        if (!isWallRunning)
        {
            verticalVelocity.y -= gravity * Time.deltaTime;
            verticalVelocity.y = Mathf.Max(verticalVelocity.y, -maxGravity);
        }

        // Debug added here to track state before vertical move
        controller.Move(verticalVelocity * Time.deltaTime);

        // Reset jumps, slide jump speed bonus, and applied gravity
        if (controller.isGrounded)
        {
            if (verticalVelocity.y < 0f)
                verticalVelocity.y = 0.0f;

            jumpCount = 0;
            jumpSpeedBonus = 0.0f;
            wallJumpVel = Vector3.zero;
            wallJumped = false;
        }

        wallJumpOccurredThisFrame = false;
    }

    // Handle sprint inputs
    void Sprint()
    {
        if (InputActionManager.instance.playerSprintAction.IsPressed() && controller.isGrounded && !isSliding && !isCrouching)
        {
            isSprinting = true;
            particleSpRun.gameObject.SetActive(true);
        }
        else if (!InputActionManager.instance.playerSprintAction.IsPressed())
        {
            isSprinting = false;
            particleSpRun.gameObject.SetActive(false);

        }
    }

    // Handle crouch and slide inputs
    void Crouch()
    {
        // Crouched
        if (InputActionManager.instance.playerCrouchAction.IsPressed())
        {
            if (isSprinting && controller.isGrounded)
            {
                isSliding = true;
                currentSlideSpeed = slideSpeedBonus;
                isSprinting = false;
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

        // Uncrouched
        else
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
        if (InputActionManager.instance.playerShootAction.IsPressed() && weaponList != null)
        {
            //launch attack method
            weaponList[0].GetComponent<IWeapon>()?.AttackBegin(playerMask);

        }

        if (!InputActionManager.instance.playerShootAction.IsPressed() && weaponList != null)
        {
            //launch attack method
            weaponList[0].GetComponent<IWeapon>()?.AttackEnd(playerMask);

        }

        //Change weapon if pressed
        float scrollDirection = InputActionManager.instance.playerSwapAction.ReadValue<float>();
        if (scrollDirection != 0)
        {
            ChangeWeapon(scrollDirection);
        }

        if (InputActionManager.instance.playerReloadAction.IsPressed())
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
        //if (invulnerable)
        //{
        //    Debug.Log("Invulnerable Hit");
        //    return;
        //}

        SoundManager.instance.PlaySFX("playerHurt");

        if (isShielded > 0 && !invulnerable)
        {
            isShielded -= 1;
        }
        else if (!invulnerable)
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
        HP = originalHP;
        invulnerable = false;

        ResetElems();
        ResetFOV();
        UpdatePlayerUI();
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
        if (isShielded + shieldAmount > shieldMax)
        {
            return;
        }
        else
        {
            isShielded += shieldAmount;
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
        else // NEW Uncrouching conditions
        {
            float targetHeight = originalHeight;
            float heightDiff = targetHeight - originalHeight;

            Vector3 rayOrigin = transform.position + controller.center + Vector3.up * (controller.height / 2f);
            float rayLength = heightDiff;

            RaycastHit hit;
            if (Physics.Raycast(rayOrigin, Vector3.up, out hit, rayLength, playerMask))
            {
                isCrouching = true;
                unCrouchCoroutine = null;
                yield break;
            }

            while (controller.height < originalHeight)
            {
                controller.height += crouchRate;
                yield return new WaitForSeconds(crouchWaitTimer);
            }
        }
        unCrouchCoroutine = null; // Safety net to ensure the reference is cleared once completed.
    }

    IEnumerator FlashDamageScreen()
    {
        GameManager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        GameManager.instance.playerDamageScreen.SetActive(false);
    }

    IEnumerator PlaySteps()
    {
        isPlayingStep = true;

        SoundManager.instance.PlaySFX("footsteps");

        if (isSprinting)
        {
            yield return new WaitForSeconds(0.25f);
        }
        else
        {
            yield return new WaitForSeconds(0.45f);
            
        }
        isPlayingStep = false;


    }

    // Element Work
    public void ApplyElement(int elem, bool buffStatus, float speedMod, float jumpMod)
    {
        if (buffStatus)
        {
            switch (elem)
            {
                case 1:
                    speedBuffed = true;
                    break;
                case 2:
                    jumpBuffed = true;
                    break;
                default:
                    return;
            }
        }
        else
        {
            switch (elem)
            {
                case 1:
                    speedDebuffed = true;
                    break;
                case 2:
                    jumpDebuffed = true;
                    break;
                default:
                    return;
            }
        }
        speedElemMod = speedMod;
        jumpElemMod = jumpMod;
    }

    public void ElementReverse()
    {
        if (speedBuffed && GameManager.instance.speedBuffTimer >= GameManager.instance.speedBuffLimit)
        {
            particleSpMod.gameObject.SetActive(false);
            GameManager.instance.BuffSprintIcon(false);
            AddModifier(-speedElemMod);
            ResetFOV();
            speedBuffed = false;
        }
        if (jumpBuffed && GameManager.instance.jumpBuffTimer >= GameManager.instance.jumpBuffLimit)
        {
            AddModifier(0.0f, -jumpElemMod);
            particleJpMod.gameObject.SetActive(false);
            GameManager.instance.BuffJumpIcon(false);
            jumpBuffed = false;
        }
        if (speedDebuffed && GameManager.instance.speedDebuffTimer >= GameManager.instance.speedDebuffLimit)
        {
            GameManager.instance.DeBuffSprintIcon(false);
            AddModifier(1 / speedElemMod);
            ResetFOV();
            speedDebuffed = false;
        }
        if (jumpDebuffed && GameManager.instance.jumpDebuffTimer >= GameManager.instance.jumpDebuffLimit)
        {
            GameManager.instance.DeBuffJumpIcon(false);
            AddModifier(0.0f, 1 / jumpElemMod);
            jumpDebuffed = false;
        }
    }

    public void ElementInverse()
    {
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
    }

    void ResetElems()
    {
        speedBuffed = false;
        jumpBuffed = false;
        speedDebuffed = false;
        jumpDebuffed = false;
        speedElemMod = 0.0f;
        jumpElemMod = 0.0f;
        particleSpMod.gameObject.SetActive(false);
        particleJpMod.gameObject.SetActive(false);
    }

    public void CollectScrap(int amount)
    {
        GameManager.instance.AddScrap(amount);
    }

    void PerformPause(InputAction.CallbackContext context)
    {
        GameManager.instance.StatePause(true);
    }

}
