using UnityEngine;

public class ShockWave : MonoBehaviour, IParent
{
    [SerializeField] ChildCallParent[] children;
    [SerializeField] int damage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(children == null)
        {
            children = GetComponentsInChildren<ChildCallParent>(true);

            if(children == null)
            {
                Debug.LogError("Was unable to find children of gameobject");
                Destroy(this);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CheckChild()
    {
        bool isActive = false;
        foreach (var child in children)
        {
            isActive = child.isActive;
            if (!isActive)
            {
                return;
            }
        }

        AttackTarget();
        
    }

    public void SetActiveFalse()
    {
        gameObject.SetActive(false);
    }

    private void AttackTarget()
    {
        GameManager.instance.playerScript.TakeDamage(damage);
    }
}
