using UnityEngine;

public interface IReloadable
{
    void Reload();

    void SetAmmo(float amount);

    void AddAmmoToReserve(int amount);
}
