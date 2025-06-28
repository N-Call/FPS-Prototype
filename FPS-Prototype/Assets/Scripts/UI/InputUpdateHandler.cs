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

        GameManager.instance.playerSettings.mouseSensitivity.x = Mathf.Clamp(MathF.Round(sensitivity, 1), 0.1f, 20.0f);
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

        GameManager.instance.playerSettings.mouseSensitivity.y = Mathf.Clamp(MathF.Round(sensitivity, 1), 0.1f, 20.0f);
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

    public void SetControllerSensitivityX(float sensitivity)
    {
        if (GameManager.instance.playerSettings == null)
        {
            return;
        }

        GameManager.instance.playerSettings.controllerSensitivity.x = Mathf.Clamp(MathF.Round(sensitivity, 1), 25.0f, 475.0f);
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

    public void SetControllerSensitivityY(float sensitivity)
    {
        if (GameManager.instance.playerSettings == null)
        {
            return;
        }

        GameManager.instance.playerSettings.controllerSensitivity.y = Mathf.Clamp(MathF.Round(sensitivity, 1), 25.0f, 475.0f);
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
