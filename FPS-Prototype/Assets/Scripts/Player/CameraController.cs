using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float mouseSensitivityX, mouseSensitivityY;
    [SerializeField] float controllerSensitivityX, controllerSensitivityY;
    [SerializeField] int lockVertMin, lockVertMax;
    [SerializeField] bool invertY;

    [Header("Wall Running")]
    [SerializeField] float wallRunTiltAngle;
    [SerializeField] float wallRunTiltSpeed;
    float currTiltZ = 0f;
    float targetTiltZ = 0f;

    float rotX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //get input
        float sensitivityX = InputActionManager.instance.isUsingGamepad ? controllerSensitivityX : mouseSensitivityX;
        float sensitivityY = InputActionManager.instance.isUsingGamepad ? controllerSensitivityY : mouseSensitivityY;

        float x = InputActionManager.instance.playerLook.x * sensitivityX * Time.deltaTime;
        float y = InputActionManager.instance.playerLook.y * sensitivityY * Time.deltaTime;

        //give option to invert mouse look up and down
        if (invertY)
            rotX += y;
        else
            rotX -= y;

        //clamp the camera on the x-axis
        rotX = Mathf.Clamp(rotX, lockVertMin, lockVertMax);

        //rotate the camera on the x-axis to look up and down
        //transform.localRotation = Quaternion.Euler(rotX, 0, 0);
        currTiltZ = Mathf.Lerp(currTiltZ, targetTiltZ, wallRunTiltSpeed * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(rotX, 0, currTiltZ);

        // rotate the player on the y-axis to look left and right
        transform.parent.Rotate(Vector3.up * x);
    }

    public void SetWallRunTilt(float tilt)
    {
        targetTiltZ = tilt;
    }

    public void SetMouseSensitivityX(float value)
    {
        mouseSensitivityX = MathF.Round(value, 1);
    }

    public void SetMouseSensitivityX(string value)
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

        SetMouseSensitivityX(val);
    }

    public void SetMouseSensitivityY(float value)
    {
        mouseSensitivityY = MathF.Round(value, 1);
    }

    public void SetMouseSensitivityY(string value)
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

        SetMouseSensitivityY(val);
    }

    public void SetControllerSensitivityX(float value)
    {
        controllerSensitivityX = MathF.Round(value, 1);
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
        controllerSensitivityY = MathF.Round(value, 1);
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

    public float GetMouseSensitivityX()
    {
        return mouseSensitivityX;
    }

    public float GetMouseSensitivityY()
    {
        return mouseSensitivityY;
    }

    public float GetControllerSensitivityX()
    {
        return controllerSensitivityX;
    }

    public float GetControllerSensitivityY()
    {
        return controllerSensitivityY;
    }

}
