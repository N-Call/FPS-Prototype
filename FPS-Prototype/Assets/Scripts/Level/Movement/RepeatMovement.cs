using UnityEngine;

public class RepeatMovement : ObjectMovement
{

    [Header("Repeat Time Delay Settings")]
    [SerializeField] float initialStartDelay;

    Vector3 startPosition;

    float initialStartTimer;

    protected override void OnStart()
    {
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
            transform.position = startPosition;
        }
    }

}
