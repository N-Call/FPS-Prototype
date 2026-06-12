using UnityEngine;

public interface IEActivator
{
    public void ActivateBuffAbility(EAbility ability, float duration, float modifier);
    public void ActivateDebuffAbility(EAbility ability, float duration, float modifier);
}
