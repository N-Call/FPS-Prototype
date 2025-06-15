using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
    [SerializeField] GameObject target;

    public UnityEvent onActivation;

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
            onActivation?.Invoke();
        }
    }
}
