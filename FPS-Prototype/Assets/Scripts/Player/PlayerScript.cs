using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour, IDamage
{
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask playerMask;

    [Header("Health")]
    [SerializeField] int HP;
    [SerializeField] public int shieldMax;

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
    [SerializeField] float fovMod;

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

    [Header("Camera")]
    [SerializeField] float changeRate;
    [SerializeField] float tickRate;

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

    int originalHP;
    int jumpCount;
    public int isShielded;

    bool isSprinting;
    bool isCrouching;
    bool isSliding;
    bool invulnerable;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalHP = HP;
        originalHeight = controller.height;
        origFOV = Camera.main.fieldOfView;
        GameManager.instance.SetSpawnPosition(transform.position);
        UpdatePlayerUI();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Jump();
        Sprint();
        Crouch();
        WeaponInput();
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
        
        // Move the player the direction and speed
        controller.Move(inputDirection * speed * Time.deltaTime);
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
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumps)
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

        verticalVelocity.y -= gravity * Time.deltaTime;
        verticalVelocity.y = Mathf.Max(verticalVelocity.y, -maxGravity);
        controller.Move(verticalVelocity * Time.deltaTime);

        // Reset jumps, slide jump speed bonus, and applied gravity
        if (controller.isGrounded)
        {
            verticalVelocity.y = 0.0f;
            jumpCount = 0;
            jumpSpeedBonus = 0.0f;
        }
    }

    // Handle sprint inputs
    void Sprint()
    {
        if (Input.GetButtonDown("Sprint") && controller.isGrounded && !isSliding)
        {
            isSprinting = true;
            SetFOV(fovMod);

        }
        else if (Input.GetButtonUp("Sprint"))
        {
            isSprinting = false;
            SetFOV(-fovMod);
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
    }

    public void UpdatePlayerUI()
    {
        // update player health bar at full and when taking damage
        GameManager.instance.playerHPbar.fillAmount = (float)HP / originalHP;

        //GameManager.instance.playerShieldBar.fillAmount = (float)isShielded / shieldMax;
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

    public void SetFOV(float modifier = 0)
    {
        StartCoroutine(FieldOfViewChange(modifier));
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
        yield return new WaitForSeconds(0.1f);
        GameManager.instance.playerDamageScreen.SetActive(false);
    }

    IEnumerator FieldOfViewChange(float modifier = 0)
    {
        float temp = Camera.main.fieldOfView + modifier;

        if (Camera.main.fieldOfView > temp)
        {
            Debug.Log("Lowering FOV");
            while (Camera.main.fieldOfView > temp)
            {
                Camera.main.fieldOfView -= changeRate;
                yield return new WaitForSeconds(tickRate);
            }
        }
        else
        {
            while (Camera.main.fieldOfView < temp)
            {
                Camera.main.fieldOfView += changeRate;
                yield return new WaitForSeconds(tickRate);
            }
        }
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

    #endregion
}

[System.Serializable]
public struct PlayerSaveData
{
    public Vector3 position;
}
