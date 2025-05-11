using UnityEngine;

public class Bow : Range
{
    [SerializeField] float shootRate;
    [Header("References")]
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject projectil;

    float shootTimer;

    // Update is called once per frame
    void Update()
    {
        shootTimer += Time.deltaTime;
    }

    public override void Attack(LayerMask playerMask)
    {
        //See if they have bullets
        if (ammoCount > 0 && shootTimer >= shootRate)
        {
            SoundManager.instance.PlaySFX("bowRelease");
            Shoot();
            ammoCount--;
            GameManager.instance.GlobalAmmoCount(ammoCount, ammoCap);
        }
    }

    void Shoot()
    {
        shootTimer = 0;
        Instantiate(projectil, shootPos.position, transform.rotation);

    }
}
