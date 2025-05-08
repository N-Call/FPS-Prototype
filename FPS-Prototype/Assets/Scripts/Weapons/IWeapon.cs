using UnityEngine;

public interface IWeapon
{
    void Attack(LayerMask playerMask, Camera camera);
}
