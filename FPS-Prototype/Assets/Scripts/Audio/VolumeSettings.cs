using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider SFXSlider;
    [SerializeField] private Slider masterSlider;
    public AudioSource preview;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetSliders();
    }

    public void SetMasterVolume()
    {
        GameManager.instance.volumeSystemData.masterVolume = masterSlider.value;
        float volume = Mathf.Clamp(masterSlider.value, 0.0001f, 1f);
        GameManager.instance.volumeSystemData.masterVolume = volume;
        myMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
    }
    public void SetMusicVolume()
    {
        GameManager.instance.volumeSystemData.musicVolume = musicSlider.value;
        float volume = Mathf.Clamp(musicSlider.value, 0.0001f, 1f);
        GameManager.instance.volumeSystemData.musicVolume = volume;
        myMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
        
    }

    public void SetSFXVolume()
    {
        GameManager.instance.volumeSystemData.sfxVolume = SFXSlider.value;
        float volume = Mathf.Clamp(SFXSlider.value, 0.0001f, 1f);
        GameManager.instance.volumeSystemData.sfxVolume = volume;
        myMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        SoundManager.instance.PlaySFX("hoverClip");
    }

    private void SetSliders()
    {
        masterSlider.value = GameManager.instance.volumeSystemData.masterVolume;
        musicSlider.value = GameManager.instance.volumeSystemData.musicVolume;
        SFXSlider.value = GameManager.instance.volumeSystemData.sfxVolume;
    }

}
