using UnityEngine;
using UnityEngine.Audio;

public class VolumeSystemData : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;

    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;

    private void Start()
    {
        SetVolumes();
    }
    public void SetVolumes()
    {
        myMixer.SetFloat("Master", Mathf.Log10(masterVolume) * 20);
        myMixer.SetFloat("Music", Mathf.Log10(musicVolume) * 20);
        myMixer.SetFloat("SFX", Mathf.Log10(sfxVolume) * 20);
    }

    #region Save and Load

    public void Save(ref VolumeSaveData data)
    {
        data.masterVol = masterVolume;
        data.musicVol = musicVolume;
        data.sfxVol = sfxVolume;
    }

    public void Load(VolumeSaveData data)
    {
        masterVolume = data.masterVol;
        musicVolume = data.musicVol;
        sfxVolume = data.sfxVol;
        SetVolumes();
    }

    #endregion
}

[System.Serializable]

public struct VolumeSaveData
{
    public float masterVol;
    public float musicVol;
    public float sfxVol;
}