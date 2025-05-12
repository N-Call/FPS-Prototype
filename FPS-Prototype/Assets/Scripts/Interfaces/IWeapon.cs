using UnityEngine;

public interface IWeapon
{
    void AttackBegin(LayerMask playerMask);
    void AttackEnd(LayerMask playerMask);
}
