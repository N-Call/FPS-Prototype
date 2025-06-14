// The reason for the separate file is to be able to reuse enums anywhere in the project
public enum UpgradeType
{
    Speed, Damage, Rate,
    SlideSpeed, WallRunJump, WallRunSpeed,
    OrbDuration, OrbStrength,
    Major
}

public enum UpgradeCategory
{
    Weapon1, Weapon2, Weapon3,
    Slide, WallRun,
    OrbSpeed, OrbJump, OrbShield, OrbTime
}