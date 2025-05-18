using UnityEngine;

public class PingPongMovement : ObjectMovement
{

    [Header("Ping Pong Time Delay Settings")]
    [SerializeField] float initialStartDelay;
    [SerializeField] float destinationDelay;

    [Header("PingPong Descruction Settings")]
    [SerializeField][Tooltip("Destroy at a specific cycle")]
    int destroyAfterCycle;

    Vector3 startPosition;

    float initialStartTimer;

    int cycles;

    protected override void OnStart()
    {
        startPosition = transform.position;
    }

    protected override void Move()
    {
        if (initialStartDelay > 0.0f && initialStartTimer < initialStartDelay)
        {
            initialStartTimer += Time.deltaTime;
            return;
        }

        if (Move(transform.position, currentDestination))
        {
            // At start
            if (cycles % 2 != 0)
            {
                startTimer = 0.0f;
            }
            // At destination
            else
            {
                if (destinationDelay > 0.0f && destinationTimer < destinationDelay)
                {
                    destinationTimer += Time.deltaTime;
                    return;
                }

                destinationTimer = 0.0f;
            }

            cycles++;
            SwapVectors(ref startPosition, ref currentDestination);
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
