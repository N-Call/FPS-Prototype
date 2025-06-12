using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputActionManager : MonoBehaviour
{

    public static InputActionManager instance { get; private set; }

    public bool isUsingGamepad { get; private set; }

    //
    // Menu Input Actions
    //
    Dictionary<MenuInputs, InputAction> menuInputActionsByEnum;
    Dictionary<MenuInputs, HashSet<Action<InputAction.CallbackContext>>> menuInputMethods;

    public enum MenuInputs
    {
        Unpause,
        Navigate,
        Confirm,
        Cancel
    }

    MenuInputActions menuInputActions;
    public InputAction menuUnpauseAction { get; private set; }
    public InputAction menuNavigateAction { get; private set; }
    public InputAction menuConfirmAction { get; private set; }
    public InputAction menuCancelAction { get; private set; }

    //
    // Player Input Actions
    //
    Dictionary<PlayerInputs, InputAction> playerInputActionsByEnum;
    Dictionary<PlayerInputs, HashSet<Action<InputAction.CallbackContext>>> playerInputMethods;

    public enum PlayerInputs
    {
        Walk,
        Sprint,
        Jump,
        Crouch,
        Look,
        Interact,
        Shoot,
        Aim,
        Reload,
        Swap,
        Pause
    }

    PlayerInputActions playerInputActions;
    public InputAction playerWalkAction { get; private set; }
    public InputAction playerSprintAction { get; private set; }
    public InputAction playerJumpAction { get; private set; }
    public InputAction playerCrouchAction { get; private set; }
    public InputAction playerLookAction { get; private set; }
    public InputAction playerInteractAction { get; private set; }
    public InputAction playerShootAction { get; private set; }
    public InputAction playerAimAction { get; private set; }
    public InputAction playerReloadAction { get; private set; }
    public InputAction playerSwapAction { get; private set; }
    public InputAction playerPauseAction { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;

        menuInputActionsByEnum = new Dictionary<MenuInputs, InputAction>();
        menuInputMethods = new Dictionary<MenuInputs, HashSet<Action<InputAction.CallbackContext>>>();
        menuInputActions = new MenuInputActions();

        playerInputActionsByEnum = new Dictionary<PlayerInputs, InputAction>();
        playerInputMethods = new Dictionary<PlayerInputs, HashSet<Action<InputAction.CallbackContext>>>();
        playerInputActions = new PlayerInputActions();

        InputSystem.onActionChange += (obj, change) =>
        {
            if (change != InputActionChange.ActionStarted)
            {
                return;
            }

            InputAction action = (InputAction)obj;
            InputControl control = action.activeControl;
            InputDevice device = control.device;
            int deviceId = device.deviceId;
            //Debug.Log($"{Time.deltaTime}\t\tDevice: {device.displayName} ({deviceId})\t\tGamepad: {isUsingGamepad}");

            if (deviceId < 1 || deviceId > 2)
            {
                isUsingGamepad = true;
            }
            else
            {
                isUsingGamepad = false;
            }
        };
    }

    private void OnDisable()
    {
        DisableMenuInput();
        DisablePlayerInput();
    }

    public void EnableMenuInput()
    {
        menuInputActions.Enable();

        menuUnpauseAction = menuInputActions.Unpause.Unpause;
        menuUnpauseAction.Enable();
        menuInputActionsByEnum[MenuInputs.Unpause] = menuUnpauseAction;

        menuNavigateAction = menuInputActions.Navigate.Navigate;
        menuNavigateAction.Enable();
        menuInputActionsByEnum[MenuInputs.Navigate] = menuNavigateAction;

        menuConfirmAction = menuInputActions.Navigate.Confirm;
        menuConfirmAction.Enable();
        menuInputActionsByEnum[MenuInputs.Confirm] = menuConfirmAction;

        menuCancelAction = menuInputActions.Navigate.Cancel;
        menuCancelAction.Enable();
        menuInputActionsByEnum[MenuInputs.Cancel] = menuCancelAction;

        foreach (var entry in menuInputMethods)
        {
            HashSet<Action<InputAction.CallbackContext>> actions = entry.Value;
            if (actions.Count == 0)
            {
                continue;
            }

            MenuInputs input = entry.Key;
            foreach (Action<InputAction.CallbackContext> action in actions)
            {
                menuInputActionsByEnum[input].performed += action;
            }
        }
    }

    public void DisableMenuInput()
    {
        foreach (var entry in menuInputMethods)
        {
            HashSet<Action<InputAction.CallbackContext>> actions = entry.Value;
            if (actions.Count == 0)
            {
                continue;
            }

            MenuInputs input = entry.Key;
            foreach (Action<InputAction.CallbackContext> action in actions)
            {
                if (menuInputActionsByEnum.ContainsKey(input))
                {
                    menuInputActionsByEnum[input].performed -= action;
                }
            }
        }

        menuUnpauseAction.Disable();
        menuNavigateAction.Disable();
        menuConfirmAction.Disable();
        menuCancelAction.Disable();
        menuInputActions.Disable();
        menuInputActionsByEnum.Clear();
    }

    public void EnablePlayerInput()
    {
        playerInputActions.Enable();

        playerWalkAction = playerInputActions.Movement.Walk;
        playerWalkAction.Enable();
        playerInputActionsByEnum[PlayerInputs.Walk] = playerWalkAction;

        playerSprintAction = playerInputActions.Movement.Sprint;
        playerSprintAction.Enable();
        playerInputActionsByEnum[PlayerInputs.Sprint] = playerSprintAction;

        playerJumpAction = playerInputActions.Movement.Jump;
        playerJumpAction.Enable();
        playerInputActionsByEnum[PlayerInputs.Jump] = playerJumpAction;

        playerCrouchAction = playerInputActions.Movement.Crouch;
        playerCrouchAction.Enable();
        playerInputActionsByEnum[PlayerInputs.Crouch] = playerCrouchAction;

        playerLookAction = playerInputActions.Look.Look;
        playerLookAction.Enable();
        playerInputActionsByEnum[PlayerInputs.Look] = playerLookAction;

        playerInteractAction = playerInputActions.Interact.Interact;
        playerInteractAction.Enable();
        playerInputActionsByEnum[PlayerInputs.Interact] = playerInteractAction;

        playerShootAction = playerInputActions.Weapons.Shoot;
        playerShootAction.Enable();
        playerInputActionsByEnum[PlayerInputs.Shoot] = playerShootAction;

        playerAimAction = playerInputActions.Weapons.Aim;
        playerAimAction.Enable();
        playerInputActionsByEnum[PlayerInputs.Aim] = playerAimAction;

        playerReloadAction = playerInputActions.Weapons.Reload;
        playerReloadAction.Enable();
        playerInputActionsByEnum[PlayerInputs.Reload] = playerReloadAction;

        playerSwapAction = playerInputActions.Weapons.Swap;
        playerSwapAction.Enable();
        playerInputActionsByEnum[PlayerInputs.Swap] = playerSwapAction;

        playerPauseAction = playerInputActions.Pause.Pause;
        playerPauseAction.Enable();
        playerInputActionsByEnum[PlayerInputs.Pause] = playerPauseAction;

        foreach (var entry in playerInputMethods)
        {
            HashSet<Action<InputAction.CallbackContext>> actions = entry.Value;
            if (actions.Count == 0)
            {
                continue;
            }

            PlayerInputs input = entry.Key;
            foreach (Action<InputAction.CallbackContext> action in actions)
            {
                playerInputActionsByEnum[input].performed += action;
            }
        }
    }

    public void DisablePlayerInput()
    {
        foreach (var entry in playerInputMethods)
        {
            HashSet<Action<InputAction.CallbackContext>> actions = entry.Value;
            if (actions.Count == 0)
            {
                continue;
            }

            PlayerInputs input = entry.Key;
            foreach (Action<InputAction.CallbackContext> action in actions)
            {
                if (playerInputActionsByEnum.ContainsKey(input))
                {
                    playerInputActionsByEnum[input].performed -= action;
                }
            }
        }

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
        playerInputActions.Disable();
        playerInputActionsByEnum.Clear();
    }

    public void AddMenuPerform(MenuInputs input, Action<InputAction.CallbackContext> action)
    {
        if (!menuInputMethods.ContainsKey(input))
        {
            menuInputMethods[input] = new HashSet<Action<InputAction.CallbackContext>>();
        }

        menuInputMethods[input].Add(action);
    }

    public void RemoveMenuPerform(MenuInputs input, Action<InputAction.CallbackContext> action)
    {
        if (!menuInputMethods.ContainsKey(input))
        {
            return;
        }

        menuInputMethods[input].Remove(action);
    }

    public void AddPlayerPerform(PlayerInputs input, Action<InputAction.CallbackContext> action)
    {
        if (!playerInputMethods.ContainsKey(input))
        {
            playerInputMethods[input] = new HashSet<Action<InputAction.CallbackContext>>();
        }

        playerInputMethods[input].Add(action);
    }

    public void RemovePlayerPerform(PlayerInputs input, Action<InputAction.CallbackContext> action)
    {
        if (!playerInputMethods.ContainsKey(input))
        {
            return;
        }

        playerInputMethods[input].Remove(action);
    }

    //void InputDeviceChanged(InputDevice device, InputDeviceChange change)
    //{

    //}

}
