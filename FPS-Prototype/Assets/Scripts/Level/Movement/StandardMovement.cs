using UnityEngine;
using UnityEngine.Events;

public class StandardMovement : ObjectMovement
{

    [Header("Standard Descruction Settings")]
    [SerializeField][Tooltip("Destroy at the destination after a certain amount of time")]
    float destroyAtDestinationDelay;

    float destinationDestroyTimer;
    [SerializeField] bool isEventTrigerrable;
    [SerializeField] float stopEventDelay;
    [SerializeField] bool keepObject;


    bool finishedMoving;

    public UnityEvent onActivateEvent;

    private bool isInvoked;

    override protected void Move()
    {
        if (finishedMoving)
        {
            if (isEventTrigerrable && !isInvoked)
            {
                onActivateEvent?.Invoke();
                isInvoked = true;
            }
            return;
        }

        if (Move(transform.position, currentDestination))
        {
            finishedMoving = true;
        }
    }

    public void SetNextYDestination(float yDest)
    {
        Debug.Log("is working");
        onActivateEvent?.RemoveAllListeners();
        currentDestination.y += yDest;
        startTimer = 0;
        finishedMoving = false;
        waitedForPlayer = false;
        hasPlayer = false;
        isInvoked = false;
    }

    protected override void Destruction()
    {
        base.Destruction();
        if (!finishedMoving || destroyAtDestinationDelay <= 0.0f)
        {
            return;
        }

        destinationDestroyTimer += Time.deltaTime;
        if (destinationDestroyTimer >= destroyAtDestinationDelay)
        {
            onActivateEvent?.RemoveAllListeners();
            if (keepObject)
            {
                Destroy(this);
            }
            else
            {
                DestroySelf();
            }
        }
    }

}
