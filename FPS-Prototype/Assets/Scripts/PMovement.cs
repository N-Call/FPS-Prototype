using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class PMovement : MonoBehaviour, IDamage
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private LayerMask playerMask;

    [Header("Health")]
    [SerializeField] public int HP;

    [Header("Movement Settings")]
    [SerializeField] public float baseSpeed = 5f;
    [SerializeField] private float modSprint = 1.5f;

    [Header("Crouch Settings")]
    [SerializeField] private float crouchHeightMod = 0.5f;
    [SerializeField] private float crouchSpeedMod = 0.5f;

    [Header("Slide Settings")]
    [SerializeField] private float slideTime = 0.67f;
    [SerializeField] private float slideSpeedBoost = 2.0f;

    [Header("Gravity and Jumping")]
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float gravityMax = 10f;
    [SerializeField] public float jumpForce = 8f;
    [SerializeField] private int maxJumps = 1;

    //Store the primary and secondary weapon's gameobjects
    [Header("Weapon Settings")]
    [SerializeField] public List<GameObject> weaponList;

    private Vector3 inputDir;
    private Vector3 moveDir;
    private Vector3 vertVel;
    
    
    private int currJumpCount = 0;
    
    private float currentSpeed;
    private float crouchSpeed;
    private float originalSpeed;
    private float originalJump;
    private float originalHeight;
    private float crouchHeight;
    private float elapsedSlideTime;

    private bool isSprinting;
    private bool isCrouching;
    private bool isSliding;

    public int origHealth;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        crouchSpeed = baseSpeed * crouchSpeedMod;
        originalHeight = controller.height;
        crouchHeight = originalHeight * crouchHeightMod;

        originalSpeed = baseSpeed;
        originalJump = jumpForce;

        origHealth = HP;
        GameManager.instance.SetSpawnPosition(transform.position);
       
    }

    // Update is called once per frame
    void Update()
    {
        // This checks the ipnut of the direction being pressed in question, as well as movement.
        HandleMovement();

        //this method is for the inputs related to weapons
        WeaponInput();
        // this is to show player health bar and when taking damage
        UpdatePlayerUI();
    }

    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // Find the input direction in world space.
        inputDir = (transform.right * h + transform.forward * v);

        // Determine sprint state and speed
        if (Input.GetButtonDown("Sprint") || (Input.GetButton("Sprint") && !isSliding && !isCrouching))
        {
            isSprinting = true;
        }
        if (Input.GetButtonUp("Sprint"))
        {
            isSprinting = false;
        }

        if (!isSliding && !isCrouching)
        {
            currentSpeed = baseSpeed * (isSprinting ? modSprint : 1f);
        }

        if (controller.isGrounded)
        {
            // This is done, so once you touch the ground, your jumps count resets.
            currJumpCount = 0;
            vertVel = Vector3.zero;

            // This is needed to not just keep the player grounded
            vertVel.y = -1f;

            // Handle crouching
            if (Input.GetButtonDown("Crouch"))
            {
                Crouch();
            }

            // Handle un-crouching
            if (Input.GetButtonUp("Crouch") || (!Input.GetButton("Crouch") && isCrouching))
            {
                UnCrouch();
            }

            // Handle sliding
            if ((isCrouching && isSprinting) || isSliding)
            {
                Slide();
            }

            // This checks the input for jumping.
            if (Input.GetButtonDown("Jump"))
            {
                // This allows the player to be able to cancel the slide into a jump.
                if (isSliding)
                {
                    isSliding = false;
                    controller.height = originalHeight;
                    currentSpeed = baseSpeed;
                }

                if (isCrouching)
                {
                    UnCrouch();
                }

                vertVel.y = jumpForce;
                currJumpCount++;
                SoundManager.instance.PlaySFX("playerJump");
            }

        }
        else
        {

            // This allows the jump to be done in mid-air, when under the max jump count.
            if (Input.GetButtonDown("Jump") && currJumpCount < maxJumps)
            {
                vertVel.y = jumpForce;
                currJumpCount++;
                SoundManager.instance.PlaySFX("playerJump");
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
        
        if(controller.isGrounded && inputDir.magnitude != 0)
        {
            SoundManager.instance.PlaySFX("footSteps");
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

    void Crouch()
    {
        controller.height = crouchHeight;
        currentSpeed *= crouchSpeedMod;
        isCrouching = true;
    }

    void UnCrouch()
    {
        controller.height = originalHeight;
        currentSpeed /= crouchSpeedMod;
        isCrouching = false;

        if (Input.GetButton("Sprint"))
        {
            isSprinting = true;
        }
    }

    void Slide()
    {
        if (!isSliding)
        {
            isSprinting = false;
            isCrouching = false;
            isSliding = true;
            elapsedSlideTime = 0.0f;
            currentSpeed = baseSpeed * modSprint + slideSpeedBoost;
        }

        elapsedSlideTime += Time.deltaTime;

        float delayBeforeLerp = 0.1f;
        if (elapsedSlideTime > delayBeforeLerp)
        {
            float eSTDBL = (elapsedSlideTime - delayBeforeLerp) / (slideTime - delayBeforeLerp);
            currentSpeed = Mathf.Lerp(baseSpeed + slideSpeedBoost, crouchSpeed, eSTDBL);
        }
        
        if (elapsedSlideTime > slideTime)
        {
            currentSpeed = crouchSpeed;
            controller.height = crouchHeight;
            isSliding = false;
            elapsedSlideTime = 0.0f;

            if (Input.GetButton("Sprint") && inputDir.z > 0)
            {
                isSprinting = true;
                isCrouching = false;
            }
            else
            {
                isSprinting = false;
                isCrouching = true;
            }
        }
    }

    public void AddMomentum(Vector3 direction, float speed)
    {
        controller.Move(direction * speed * Time.deltaTime);
    }

    public void TakeDamage(int amount)
    {
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
        baseSpeed = originalSpeed;
        jumpForce = originalJump;
        HP = origHealth;
    }

    public void UpdatePlayerUI()
    {
        // update player health bar at full and when taking damage
        GameManager.instance.playerHPbar.fillAmount = (float)HP/ origHealth;
    }

    IEnumerator FlashDamageScreen()
    {
        GameManager.instance.playerDamageScreen.SetActive(true);   
        yield return new WaitForSeconds(0.1f);
        GameManager.instance.playerDamageScreen.SetActive(false);
    }

}
