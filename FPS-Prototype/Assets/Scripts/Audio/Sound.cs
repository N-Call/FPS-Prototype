using UnityEngine;

[System.Serializable]
public class Sound 
{
    public string name;
    public AudioClip[] clips;
    [Range(0, 1)] public float volume;
    public bool isMusic = false;

    public AudioClip GetClip()
    {
        if (clips == null || clips.Length == 0)
        {
            return null;
        }
        if (isMusic)
        {
            return clips[0];
        }
        else
        {
            return clips[Random.Range(0, clips.Length)];
        }
    }
}


