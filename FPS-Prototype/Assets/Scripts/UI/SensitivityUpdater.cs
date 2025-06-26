using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SensitivityUpdater : MonoBehaviour
{

    [Header("Mouse Settings")]
    [SerializeField] Slider mouseSliderX;
    [SerializeField] Slider mouseSliderY;
    [SerializeField] TMP_InputField mouseInputX, mouseInputY;

    [Header("Controller Settings")]
    [SerializeField] Slider controllerSliderX;
    [SerializeField] Slider controllerSliderY;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GameManager.instance.playerSettings == null)
        {
            return;
        }

        if (mouseSliderX != null)
        {
            mouseSliderX.SetValueWithoutNotify(GameManager.instance.playerSettings.mouseSensitivity.x);
        }

        if (mouseInputX != null)
        {
            mouseInputX.SetTextWithoutNotify(MathF.Round(GameManager.instance.playerSettings.mouseSensitivity.x, 1).ToString("#.#"));
        }

        if (mouseSliderY != null)
        {
            mouseSliderY.SetValueWithoutNotify(GameManager.instance.playerSettings.mouseSensitivity.y);
        }

        if (mouseInputY != null)
        {
            mouseInputY.SetTextWithoutNotify(MathF.Round(GameManager.instance.playerSettings.mouseSensitivity.y, 1).ToString("#.#"));
        }

        if (controllerSliderX != null)
        {
            controllerSliderX.SetValueWithoutNotify(GameManager.instance.playerSettings.controllerSensitivity.x);
        }

        if (controllerSliderY != null)
        {
            controllerSliderY.SetValueWithoutNotify(GameManager.instance.playerSettings.controllerSensitivity.y);
        }
    }

}
