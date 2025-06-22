using UnityEngine;

public class ChildCallParent : MonoBehaviour
{
    [SerializeField] IParent parent;
    [SerializeField] GameObject target;

    public bool isActive;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(parent == null)
        {
            parent = GetComponentInParent<IParent>();
            if(parent == null)
            {
                
                Destroy(this);
            }
        }

        if (target == null)
        {
            target = GameManager.instance.player;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == target)
        {
            isActive = true;
            parent.CheckChild();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isActive = false;
    }
}
