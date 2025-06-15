using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    [Header("Pistol Upgrades")]
    [SerializeField][Tooltip("If the ricochet upgrade is unlocked")]
    bool ricochet;

    //pistol
    public int w1DmgMod;
    public int w1SpeedMod;
    public float w1RateMod;
    public bool w1Major;
    //bow
    public int w2DmgMod;
    public int w2SpeedMod;
    public int w2RateMod;
    public bool w2Major;
    //sword
    public int w3DmgMod;
    public int w3SpeedMod;
    public int w3RateMod;
    public bool w3Major;
    //orbs speed
    public float o1Srt;
    public float o1Dur;
    public bool o1Major;
    //orb jump
    public float o2Srt;
    public float o2Dur;
    public bool o2Major;
    //orb shield
    public float o3Srt;
    public float o3Dur;
    public bool o3Major;
    //orb time
    public float o4Srt;
    public float o4Dur;
    public bool o4Major;
    //slide
    public int moveSlideSpeed;
    public bool slideMajor;
    //wall run 
    public int moveWallRunSpeed;
    public int moveWallRunJump;
    public bool wallRunMajor;

    private void OnLevelWasLoaded(int level)
    {
        GameManager.instance.playerAbilities = this;
    }

    public bool RicochetUnlocked()
    {
        return ricochet;
    }

    public void UnlockRicochet(bool unlocked)
    {
        ricochet = unlocked;
    }

}
