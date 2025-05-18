using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour, IDamage
{
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask playerMask;

    [Header("Health")]
    [SerializeField] int HP;

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

    int originalHP;
    int jumpCount;

    bool isSprinting;
    bool isCrouching;
    bool isSliding;
    bool invulnerable;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalHP = HP;
        originalHeight = controller.height;
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
        float horizontal = Input.GetAxis("Horizontal");
        bool sideways = horizontal != 0;

        float vertical = Input.GetAxis("Vertical");
        bool forward = vertical > 0;
        bool backwards = vertical < 0;

        Vector3 inputDirection = transform.right * horizontal + transform.forward * vertical;

        verticalSpeed = forward && backwards ? 0.0f : forward ? walkForwardSpeed : backwards ? walkBackwardsSpeed : 0.0f;
        horizontalSpeed = sideways ? walkSidewaysSpeed : 0.0f;

        float speed = CalculateSpeed();
        
        controller.Move(inputDirection * speed * Time.deltaTime);
    }

    float CalculateSpeed()
    {
        float speed = verticalSpeed > 0 ? verticalSpeed : horizontalSpeed;
        
        if (isSprinting)
        {
            speed += sprintSpeed;
        }

        if (isCrouching)
        {
            speed *= crouchSpeedMultiplier;
        }

        if (jumpSpeedBonus > 0.0f)
        {
            speed += jumpSpeedBonus;
            if (jumpSpeedBonus > slideJumpMinimumSpeed)
            {
                jumpSpeedBonus -= slideJumpRate;
            }
        }

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

        return speed + (speed * speedModifier);
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumps)
        {
            if (isSliding)
            {
                jumpSpeedBonus = slideJumpSpeedBonus;
            }

            verticalVelocity.y = jumpForce + (jumpForce * jumpModifier);
            jumpCount++;
        }

        verticalVelocity.y -= gravity * Time.deltaTime;
        verticalVelocity.y = Mathf.Max(verticalVelocity.y, -maxGravity);
        controller.Move(verticalVelocity * Time.deltaTime);

        if (controller.isGrounded)
        {
            verticalVelocity.y = 0.0f;
            jumpCount = 0;
            jumpSpeedBonus = 0.0f;
        }
    }

    void Sprint()
    {
        if (Input.GetButtonDown("Sprint") && controller.isGrounded && !isSliding)
        {
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            isSprinting = false;
        }
    }

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

        SoundManager.instance.PlaySFX("playerHurt");

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

    public void SetInvulnerable(bool isInvulnerable)
    {
        invulnerable = isInvulnerable;
    }

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

}
