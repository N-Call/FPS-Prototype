using System;
using UnityEngine;

public class InputUpdateHandler : MonoBehaviour
{
    
    public void SetMouseSensitivityX(float sensitivity)
    {
        if (GameManager.instance.playerSettings == null)
        {
            return;
        }

        GameManager.instance.playerSettings.mouseSensitivity.x = MathF.Round(sensitivity, 1);
    }

    public void SetMouseSensitivityX(string sensitivity)
    {
        float val;
        try
        {
            val = float.Parse(sensitivity);
        }
        catch
        {
            return;
        }

        SetMouseSensitivityX(val);
    }

    public void SetMouseSensitivityY(float sensitivity)
    {
        if (GameManager.instance.playerSettings == null)
        {
            return;
        }

        GameManager.instance.playerSettings.mouseSensitivity.y = MathF.Round(sensitivity, 1);
    }

    public void SetMouseSensitivityY(string sensitivity)
    {
        float val;
        try
        {
            val = float.Parse(sensitivity);
        }
        catch
        {
            return;
        }

        SetMouseSensitivityY(val);
    }

    public void SetControllerSensitivityX(float value)
    {
        if (GameManager.instance.playerSettings == null)
        {
            return;
        }

        GameManager.instance.playerSettings.controllerSensitivity.x = MathF.Round(value, 1);
    }

    public void SetControllerSensitivityX(string value)
    {
        float val;
        try
        {
            val = float.Parse(value);
        }
        catch
        {
            return;
        }

        SetControllerSensitivityX(val);
    }

    public void SetControllerSensitivityY(float value)
    {
        if (GameManager.instance.playerSettings == null)
        {
            return;
        }

        GameManager.instance.playerSettings.controllerSensitivity.y = MathF.Round(value, 1);
    }

    public void SetControllerSensitivityY(string value)
    {
        float val;
        try
        {
            val = float.Parse(value);
        }
        catch
        {
            return;
        }

        SetControllerSensitivityY(val);
    }

}
