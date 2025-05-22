using UnityEngine;

public class RepeatMovement : ObjectMovement
{

    [Header("Repeat Time Delay Settings")]
    [SerializeField] float initialStartDelay;

    [SerializeField] bool resetPlayerWithPlatform;

    Vector3 parentStartPosition;
    Vector3 startPosition;

    float initialStartTimer;

    protected override void OnStart()
    {
        if (emptyParent != null)
        {
            parentStartPosition = emptyParent.transform.position;
        }

        startPosition = transform.position;
    }

    override protected void Move()
    {
        if (initialStartDelay > 0.0f && initialStartTimer < initialStartDelay)
        {
            initialStartTimer += Time.deltaTime;
            return;
        }

        if (Move(transform.position, currentDestination))
        {
            if (resetPlayerWithPlatform && emptyParent != null)
            {
                emptyParent.transform.position = parentStartPosition;
            }

            transform.position = startPosition;
        }
    }

}
