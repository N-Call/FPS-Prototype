using System;
using UnityEngine;

public class PlayerSettings : MonoBehaviour
{

    [Header("Sensitivities")]
    public Vector2 mouseSensitivity;
    public Vector2 controllerSensitivity;

    string keybinds;

    private void OnLevelWasLoaded(int level)
    {
        GameManager.instance.playerSettings = this;
    }

    #region Save and Load
    public void Save(ref SettingsSaveData data)
    {
        data.mouseSensitivity = mouseSensitivity;
        data.controllerSensitivity = controllerSensitivity;
        data.keybinds = keybinds;
    }

    public void Load(SettingsSaveData data)
    {
        mouseSensitivity = data.mouseSensitivity;
        controllerSensitivity = data.controllerSensitivity;
        keybinds = data.keybinds;
        
        if (InputActionManager.instance != null)
        {
            InputActionManager.instance.SetSavedKeybinds(keybinds);
        }
    }
    #endregion

    #region Getters & Setters    
    public string GetKeybinds()
    {
        return keybinds;
    }

    public void UpdateKeybinds()
    {
        keybinds = InputActionManager.instance.GetSavedKeybinds();
    }
    #endregion

}

[System.Serializable]
public struct SettingsSaveData
{
    public Vector2 mouseSensitivity;
    public Vector2 controllerSensitivity;
    public string keybinds;
}