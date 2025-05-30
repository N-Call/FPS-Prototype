using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider SFXSlider;
    [SerializeField] private Slider masterSlider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetSliders();
    }
    public void SetMasterVolume()
    {
        GameManager.instance.volumeSystemData.masterVolume = masterSlider.value;
        myMixer.SetFloat("Master", Mathf.Log10(GameManager.instance.volumeSystemData.masterVolume) * 20);
    }
    public void SetMusicVolume()
    {
        GameManager.instance.volumeSystemData.musicVolume = musicSlider.value;
        myMixer.SetFloat("Music", Mathf.Log10(GameManager.instance.volumeSystemData.musicVolume) * 20);
    }

    public void SetSFXVolume()
    {
        GameManager.instance.volumeSystemData.sfxVolume = SFXSlider.value;
        myMixer.SetFloat("SFX", Mathf.Log10(GameManager.instance.volumeSystemData.sfxVolume) * 20);
    }

    private void SetSliders()
    {
        masterSlider.value = GameManager.instance.volumeSystemData.masterVolume;
        musicSlider.value = GameManager.instance.volumeSystemData.musicVolume;
        SFXSlider.value = GameManager.instance.volumeSystemData.sfxVolume;
    }

}
