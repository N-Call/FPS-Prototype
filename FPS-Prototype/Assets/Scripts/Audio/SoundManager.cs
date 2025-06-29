using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource, wallRunSource;

    private void Awake()
    {
        
        if (instance == null)
        {
            instance = this;    
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        sfxSource.playOnAwake = false;
    }

    private void Start()
    {
        PlayMusic("Theme");
    }


    public void PlayMusic(string name)
    {
        foreach (Sound s in musicSounds)
        {
            if (s.name == name && s.isMusic)
            {
                if (musicSource.clip != null && musicSource.clip.name == s.GetClip().name)
                {
                    return;
                }
                AudioClip clip = s.GetClip();
                if (clip != null)
                {
                    musicSource.clip = clip;
                    musicSource.volume = s.volume;
                    musicSource.loop = true;
                    musicSource.Play();
                }
                return;
            }
        }
    }
    public void PlaySFX(string name)
    {
        foreach(Sound s in sfxSounds)
        {
            if (s.name == name)
            {
                AudioClip fxClip = s.GetClip();
                if (fxClip != null)
                {
                    sfxSource.PlayOneShot(fxClip, s.volume);
                }
                return;
            }

        }

    }
    
}
