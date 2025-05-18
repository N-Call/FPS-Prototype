using UnityEngine;

public class StandardMovement : ObjectMovement
{

    [Header("Standard Descruction Settings")]
    [SerializeField][Tooltip("Destroy at the destination after a certain amount of time")]
    float destroyAtDestinationDelay;

    float destinationDestroyTimer;

    bool finishedMoving;

    override protected void Move()
    {
        if (finishedMoving)
        {
            return;
        }

        if (Move(transform.position, currentDestination))
        {
            finishedMoving = true;
        }
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
            DestroySelf();
        }
    }

}
