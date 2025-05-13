using UnityEngine;
using UnityEngine.UI;

public interface IWeapon
{
    void AttackBegin(LayerMask playerMask);
    void AttackEnd(LayerMask playerMask);
}
