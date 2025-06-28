using UnityEngine;

public class AmmoCratePickup : MonoBehaviour
{
    [Header("Ammo Settings")]
    [SerializeField] int ammoAmount = 30;
    [SerializeField] string sfxName = "AmmoPickup";

    private void OnValidate()
    {
        ammoAmount = Mathf.Max(0, ammoAmount);
    }

    //    private void OnTriggerEnter(Collider other)
    //    {
    //        if (other.CompareTag("Player"))
    //        {

    //            // For now, I havew it set to the current weapon that you're using, the moment you pick up the ammo crate.
    //            // If you wish to adjust this so it applies to both the bow and the gun, then that's perfectly fine.
    //            //IReloadable weapon = GameManager.instance.playerScript.weaponList[0].GetComponent<IReloadable>();

    //            IReloadable weapon = GameManager.instance.playerScript.weaponList[GameManager.instance.playerScript.currentWeapon].GetComponent<IReloadable>();
    //            if (weapon != null)
    //            {
    //                weapon.AddAmmoToReserve(ammoAmount);

    //                SoundManager.instance.PlaySFX(sfxName);

    //                Destroy(gameObject);
    //            }

    //        }
    //    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Loop through all weapons in the player's list
            foreach (GameObject weaponObj in GameManager.instance.playerScript.weaponList)
            {
                IReloadable reloadable = weaponObj.GetComponent<IReloadable>();
                if (reloadable != null)
                {
                    reloadable.AddAmmoToReserve(ammoAmount);
                }
            }

            SoundManager.instance.PlaySFX(sfxName);
            Destroy(gameObject);
        }
    }
}
