using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SliderInputHandler : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] TMP_InputField inputField;

    void UpdateSliderAlreadyRounded(float value)
    {
        if (slider != null)
        {
            slider.SetValueWithoutNotify(value);
        }
    }

    public void UpdateSlider(float value)
    {
        UpdateSliderAlreadyRounded(MathF.Round(value, 1));
    }

    public void UpdateSlider(string value)
    {
        if (slider == null)
        {
            return;
        }

        float val;
        try
        {
            val = float.Parse(value);
        }
        catch
        {
            return;
        }

        UpdateSlider(val);
    }

    public void UpdateInputField(float value)
    {
        if (inputField != null)
        {
            inputField.SetTextWithoutNotify(MathF.Round(value, 1).ToString("#.#"));
        }
    }

    public void RoundInputField()
    {
        if (inputField == null)
        {
            return;
        }

        string text = inputField.text;
        float val;
        try
        {
            val = float.Parse(text);
        }
        catch
        {
            return;
        }

        inputField.text = MathF.Round(val, 1).ToString("#.#");
        UpdateSliderAlreadyRounded(val);
    }

}
