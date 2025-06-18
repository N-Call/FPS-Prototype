using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;

public class InputActionManager : MonoBehaviour
{

    public static InputActionManager instance { get; private set; }

    [SerializeField] InputActionAsset menuInputActionMap;
    [SerializeField] InputActionAsset playerInputActionMap;
    [SerializeField] InputActionAsset rebindInputActionMap;
    [SerializeField] InputActionAsset debugInputActionMap;

    [SerializeField] bool onStartScreen = false;

    [SerializeField] bool toggleSprint;
    [SerializeField] bool toggleCrouch;

    bool isSprinting;
    bool isCrouching;

    public bool isUsingGamepad { get; private set; }

    //
    // Menu Inputs
    //
    InputAction menuUnpauseAction;
    InputAction menuNavigateAction;
    InputAction menuConfirmAction;
    InputAction menuCancelAction;
    InputAction menuClickAction;
    InputAction menuPointAction;
    InputAction menuScrollAction;
    
    public bool menuUnpause { get; private set; }
    public Vector2 menuNavigate { get; private set; }
    public bool menuConfirm { get; private set; }
    public bool menuCancel { get; private set; }
    public Vector2 menuPoint { get; private set; }
    public Vector2 menuScroll { get; private set; }

    //
    // Player Inputs
    //
    InputAction playerWalkAction;
    InputAction playerSprintAction;
    InputAction playerJumpAction;
    InputAction playerCrouchAction;
    InputAction playerLookAction;
    InputAction playerInteractAction;
    InputAction playerShootAction;
    InputAction playerAimAction;
    InputAction playerReloadAction;
    InputAction playerSwapAction;
    InputAction playerPistolAction;
    InputAction playerBowAction;
    InputAction playerSwordAction;
    InputAction playerPauseAction;

    public Vector2 playerWalk { get; private set; }
    public bool playerSprint { get; private set; }
    public bool playerJump { get; private set; }
    public bool playerCrouch { get; private set; }
    public Vector2 playerLook { get; private set; }
    public bool playerInteract { get; private set; }
    public bool playerShoot { get; private set; }
    public bool playerShooting { get; private set; }
    public bool playerAim { get; private set; }
    public bool playerReload { get; private set; }
    public float playerSwap { get; private set; }
    public bool playerPistol { get; private set; }
    public bool playerBow { get; private set; }
    public bool playerSword { get; private set; }
    public bool playerPause { get; private set; }

    //
    // Rebind Inputs
    //
    InputAction rebindCancelAction;
    InputAction rebindResetAction;

    public bool rebindCancel { get; private set; }
    public bool rebindReset { get; private set; }

    InputAction debugInputAction;

    InputDevice lastInputDevice;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;

        InputSystem.onEvent += OnInputEvent;

        menuUnpauseAction = menuInputActionMap["Unpause"];
        menuNavigateAction = menuInputActionMap["Navigate"];
        menuConfirmAction = menuInputActionMap["Confirm"];
        menuCancelAction = menuInputActionMap["Cancel"];
        menuClickAction = menuInputActionMap["Click"];
        menuPointAction = menuInputActionMap["Point"];
        menuScrollAction = menuInputActionMap["Scroll"];

        rebindCancelAction = rebindInputActionMap["Cancel"];
        rebindResetAction = rebindInputActionMap["Reset"];

        playerWalkAction = playerInputActionMap["Walk"];
        playerSprintAction = playerInputActionMap["Sprint"];
        playerJumpAction = playerInputActionMap["Jump"];
        playerCrouchAction = playerInputActionMap["Crouch"];
        playerLookAction = playerInputActionMap["Look"];
        playerInteractAction = playerInputActionMap["Interact"];
        playerShootAction = playerInputActionMap["Shoot"];
        playerAimAction = playerInputActionMap["Aim"];
        playerReloadAction = playerInputActionMap["Reload"];
        playerSwapAction = playerInputActionMap["Swap"];
        playerPistolAction = playerInputActionMap["Pistol"];
        playerSwordAction = playerInputActionMap["Sword"];
        playerBowAction = playerInputActionMap["Bow"];
        playerPauseAction = playerInputActionMap["Pause"];

        if (debugInputActionMap != null)
        {
            debugInputAction = debugInputActionMap["Debug"];
            debugInputAction.Enable();
        }

        if (onStartScreen)
        {
            EnableMenuInput();
        }
    }

    private void Update()
    {
        if (
            debugInputActionMap != null
            && debugInputActionMap.enabled
            && debugInputAction.WasPressedThisFrame()
            && SceneManager.GetActiveScene() != SceneManager.GetSceneByName("Showcase (Debug)")
        ) {
            SceneManager.LoadScene("Showcase (Debug)");
            return;
        }

        bool menu = menuInputActionMap.enabled;
        menuUnpause = menu ? menuUnpauseAction.WasPressedThisFrame() : false;
        menuNavigate = menu ? menuNavigateAction.ReadValue<Vector2>() : Vector2.zero;
        menuConfirm = menu ? menuConfirmAction.WasPressedThisFrame() : false;
        menuCancel = menu ? menuCancelAction.WasPressedThisFrame() : false;
        menuPoint = menu ? menuPointAction.ReadValue<Vector2>() : Vector2.zero;
        menuScroll = menu ? menuScrollAction.ReadValue<Vector2>() : Vector2.zero;

        bool rebind = rebindInputActionMap.enabled;
        rebindCancel = rebind ? rebindCancelAction.WasPressedThisFrame() : false;
        rebindReset = rebind ? rebindResetAction.WasPressedThisFrame() : false;

        bool player = playerInputActionMap.enabled;
        playerWalk = player ? playerWalkAction.ReadValue<Vector2>() : Vector2.zero;

        if (player)
        {
            if (toggleSprint && playerSprintAction.WasPressedThisFrame())
            {
                isSprinting = !isSprinting;
                playerSprint = isSprinting;
            }
            else if (!toggleSprint)
            {
                playerSprint = playerSprintAction.IsPressed();
            }

            if (toggleCrouch && playerCrouchAction.WasPressedThisFrame())
            {
                isCrouching = !isCrouching;
                playerCrouch = isCrouching;
            }
            else if (!toggleCrouch)
            {
                playerCrouch = playerCrouchAction.IsPressed();
            }
        }
        else
        {
            playerSprint = false;
            playerCrouch = false;
        }

        playerJump = player ? playerJumpAction.WasPressedThisFrame() : false;
        playerLook = player ? playerLookAction.ReadValue<Vector2>() : Vector2.zero;
        playerInteract = player ? playerInteractAction.WasPressedThisFrame() : false;
        playerShoot = player ? playerShootAction.WasPressedThisFrame() : false;
        playerShooting = player ? playerShootAction.IsPressed() : false;
        playerAim = player ? playerAimAction.IsPressed() : false;
        playerReload = player ? playerReloadAction.WasPressedThisFrame() : false;

        // If is player -> is using gamepad? Otherwise return 0
        // If is using gamepad -> check if it was pressed this frame -> return 1. Otherwise return 0
        // If not using gamepad -> return the read float value
        playerSwap = player
            ? isUsingGamepad
                ? playerSwapAction.WasPressedThisFrame()
                    ? 1
                    : 0
                : playerSwapAction.ReadValue<float>()
            : 0;

        playerPistol = player ? playerPistolAction.WasPressedThisFrame() : false;
        playerBow = player ? playerBowAction.WasPressedThisFrame() : false;
        playerSword = player ? playerSwordAction.WasPressedThisFrame() : false;

        playerPause = player ? playerPauseAction.WasPressedThisFrame() : false;
    }

    private void OnDisable()
    {
        DisableRebindInput();
        DisableMenuInput();
        DisablePlayerInput();
    }

    public void EnablePauseInput()
    {
        if (menuInputActionMap.enabled)
        {
            menuUnpauseAction.Enable();
        }
        else
        {
            playerPauseAction.Enable();
        }
    }

    public void DisablePauseInput()
    {
        menuUnpauseAction.Disable();
        playerPauseAction.Disable();
    }

    public void EnableMenuInput()
    {
        menuInputActionMap.Enable();
        menuUnpauseAction.Enable();
        menuNavigateAction.Enable();
        menuConfirmAction.Enable();
        menuCancelAction.Enable();
        menuClickAction.Enable();
        menuPointAction.Enable();
        menuScrollAction.Enable();
    }

    public void DisableMenuInput()
    {
        menuUnpauseAction.Disable();
        menuNavigateAction.Disable();
        menuConfirmAction.Disable();
        menuCancelAction.Disable();
        menuClickAction.Disable();
        menuPointAction.Disable();
        menuScrollAction.Disable();
        menuInputActionMap.Disable();
    }

    public void EnableRebindInput()
    {
        DisableMenuInput();
        rebindInputActionMap.Enable();
        rebindCancelAction.Enable();
        rebindResetAction.Enable();
    }

    public void DisableRebindInput()
    {
        rebindCancelAction.Disable();
        rebindResetAction.Disable();
        rebindInputActionMap.Disable();
        EnableMenuInput();
    }

    public void EnablePlayerInput()
    {
        playerInputActionMap.Enable();
        playerWalkAction.Enable();
        playerSprintAction.Enable();
        playerJumpAction.Enable();
        playerCrouchAction.Enable();
        playerLookAction.Enable();
        playerInteractAction.Enable();
        playerShootAction.Enable();
        playerAimAction.Enable();
        playerReloadAction.Enable();
        playerSwapAction.Enable();
        playerPauseAction.Enable();
    }

    public void DisablePlayerInput()
    {
        playerWalkAction.Disable();
        playerSprintAction.Disable();
        playerJumpAction.Disable();
        playerCrouchAction.Disable();
        playerLookAction.Disable();
        playerInteractAction.Disable();
        playerShootAction.Disable();
        playerAimAction.Disable();
        playerReloadAction.Disable();
        playerSwapAction.Disable();
        playerPauseAction.Disable();
        playerInputActionMap.Disable();
    }

    void OnInputEvent(InputEventPtr eventPtr, InputDevice device)
    {
        if (device == lastInputDevice || (!isUsingGamepad && (device is Mouse || device is Keyboard)))
        {
            return;
        }

        if (!device.added || !device.enabled)
        {
            return;
        }

        if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>())
        {
            return;
        }

        foreach (InputControl control in device.allControls)
        {
            if (control is ButtonControl button)
            {
                if (button.ReadValueFromEvent(eventPtr, out float value) && value > 0.1f)
                {
                    RegisterDevice(device);
                    return;
                }
            }

            else if (control is AxisControl axis)
            {
                if (axis.ReadValueFromEvent(eventPtr, out float value) && Mathf.Abs(value) > 0.1f)
                {
                    RegisterDevice(device);
                    return;
                }
            }

            else if (control is Vector2Control vector)
            {
                if (vector.ReadValueFromEvent(eventPtr, out Vector2 value) && value.magnitude > 0.1f)
                {
                    RegisterDevice(device);
                    return;
                }
            }
        }

    }

    void RegisterDevice(InputDevice device)
    {
        lastInputDevice = device;
        isUsingGamepad = !(device is Mouse) && !(device is Keyboard);
        //Debug.Log($"[{Time.deltaTime}]\t\tSwitched to " + (isUsingGamepad ? "Gamepad" : "Keyboard & Mouse"));
    }

    private void OnDestroy()
    {
        InputSystem.onEvent -= OnInputEvent;
    }

}
