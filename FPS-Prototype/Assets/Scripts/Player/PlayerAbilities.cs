using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{

    [Header("Pistol Upgrades")]
    [SerializeField][Tooltip("If the ricochet upgrade is unlocked")]
    bool ricochet;

    public bool RicochetUnlocked()
    {
        return ricochet;
    }

    public void UnlockRicochet(bool unlocked)
    {
        ricochet = unlocked;
    }

}
