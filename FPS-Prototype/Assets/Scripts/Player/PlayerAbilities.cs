using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    [Header("Pistol Upgrades")]
    [SerializeField][Tooltip("If the ricochet upgrade is unlocked")]
    public bool ricochet;

    //pistol
    public int w1DmgMod;
    public int w1AmmoMag;
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
    public float moveSlideSpeed;
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

    #region Save and Load
    public void Save(ref AbilitiesSaveData data)
    {
        data.w1DmgMod = w1DmgMod;
        data.w1AmmoMag = w1AmmoMag;
        data.w1RateMod = w1RateMod;
        data.w1Major = w1Major;

        data.w2DmgMod = w2DmgMod;
        data.w2SpeedMod = w2SpeedMod;
        data.w2RateMod = w2RateMod;
        data.w2Major = w2Major;

        data.w3DmgMod = w3DmgMod;
        data.w3SpeedMod = w3SpeedMod;
        data.w3RateMod = w3RateMod;
        data.w3Major = w3Major;

        data.o1Dur = o1Dur;
        data.o1Major = o1Major;
        data.o1Srt = o1Srt;

        data.o2Dur = o2Dur;
        data.o2Major = o2Major;
        data.o2Srt = o2Srt;

        data.o3Dur = o3Dur;
        data.o3Major = o3Major;
        data.o3Srt = o3Srt;

        data.o4Dur = o4Dur;
        data.o4Major = o4Major;
        data.o4Srt = o4Srt;

        data.moveSlideSpeed = moveSlideSpeed;
        data.slideMajor = slideMajor;

        data.moveWallRunSpeed = moveWallRunSpeed;
        data.moveWallRunJump = moveWallRunJump;
        data.wallRunMajor = wallRunMajor;
    }

    public void Load(AbilitiesSaveData data)
    {
        w1DmgMod = data.w1DmgMod;
        w1AmmoMag = data.w1AmmoMag;
        w1RateMod = data.w1RateMod;
        w1Major = data.w1Major;

        w2DmgMod = data.w2DmgMod;
        w2SpeedMod = data.w2SpeedMod;
        w2RateMod = data.w2RateMod;
        w2Major = data.w2Major;

        w3DmgMod = data.w3DmgMod;
        w3SpeedMod = data.w3SpeedMod;
        w3RateMod = data.w3RateMod;
        w3Major = data.w3Major;

        o1Dur = data.o1Dur;
        o1Major = data.o1Major;
        o1Srt = data.o1Srt;

        o2Dur = data.o2Dur;
        o2Major = data.o2Major;
        o2Srt = data.o2Srt;

        o3Dur = data.o3Dur;
        o3Major = data.o3Major;
        o3Srt = data.o3Srt;

        o4Dur = data.o4Dur;
        o4Major = data.o4Major;
        o4Srt = data.o4Srt;

        moveSlideSpeed = data.moveSlideSpeed;
        slideMajor = data.slideMajor;

        moveWallRunSpeed = data.moveWallRunSpeed;
        moveWallRunJump = data.moveWallRunJump;
        wallRunMajor = data.wallRunMajor;
    }

    #endregion
}

[System.Serializable]

public struct AbilitiesSaveData
{
    //pistol
    public int w1DmgMod;
    public int w1AmmoMag;
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
    public float moveSlideSpeed;
    public bool slideMajor;
    //wall run 
    public int moveWallRunSpeed;
    public int moveWallRunJump;
    public bool wallRunMajor;
}
