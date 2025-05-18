using UnityEngine;

public class PingPongMovement : ObjectMovement
{

    [Header("PingPong Descruction Settings")]
    [SerializeField][Tooltip("Destroy at a specific cycle")]
    int destroyAfterCycle;

    Vector3 startPosition;

    int cycles;

    protected override void OnStart()
    {
        startPosition = transform.position;
    }

    protected override void Move()
    {
        if (Move(transform.position, currentDestination))
        {
            SwapVectors(ref startPosition, ref currentDestination);
            cycles++;
        }
    }

    protected override void Destruction()
    {
        base.Destruction();
        if (destroyAfterCycle <= 0)
        {
            return;
        }

        if (destroyAfterCycle <= cycles)
        {
            DestroySelf();
        }
    }

}
