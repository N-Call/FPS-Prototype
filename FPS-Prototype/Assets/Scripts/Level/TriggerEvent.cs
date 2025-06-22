using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
    [SerializeField] GameObject target;

    public UnityEvent onActivation;
    public bool activateOnce;

    private bool hasActivated;
    private void Start()
    {
        if(target == null)
        {
            target = GameManager.instance.player;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == target)
        {
            if(hasActivated && activateOnce) { return; }
            onActivation?.Invoke();
            hasActivated = true;
        }
    }
}
