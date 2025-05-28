using UnityEngine;

public interface IElemental
{
    void ApplyElement(int elem, bool buffStatus, float speedMod, float jumpMod);
    void ElementInverse();
}
