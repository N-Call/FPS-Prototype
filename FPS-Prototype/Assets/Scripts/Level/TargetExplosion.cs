using UnityEngine;

public class TargetExplosion : MonoBehaviour
{
    [SerializeField] Target parentTarg;

    private void OnTriggerEnter(Collider other)
    {
        IElemental affected = other.GetComponent<IElemental>();
        if (parentTarg.buff)
        {
            affected?.ApplyElement((int)parentTarg.elem, parentTarg.buff, parentTarg.speedElemMod, parentTarg.jumpElemMod);
        }
        else
        {
            affected?.ApplyElement((int)parentTarg.elem, parentTarg.buff, parentTarg.speedElemMod, parentTarg.jumpElemMod);
        }


        IDamage dmgTarg = other.GetComponent<IDamage>();

        if (dmgTarg != null && other.CompareTag("Enemy") && (int)parentTarg.elem == 1 && GameManager.instance.playerAbilities.o1Major)
        {
            dmgTarg.TakeDamage(1);
        }
        if (dmgTarg != null && other.CompareTag("Enemy") && (int)parentTarg.elem == 2 && GameManager.instance.playerAbilities.o2Major)
        {
            dmgTarg.TakeDamage(1);
        }
        if (dmgTarg != null && other.CompareTag("Enemy") && (int)parentTarg.elem == 3 && GameManager.instance.playerAbilities.o3Major)
        {
            dmgTarg.TakeDamage(1);
        }
    }
}
