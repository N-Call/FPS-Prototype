using UnityEngine;

public class RepeatMovement : ObjectMovement
{

    Vector3 startPosition;

    protected override void OnStart()
    {
        startPosition = transform.position;
    }

    override protected void Move()
    {
        if (Move(transform.position, currentDestination))
        {
            transform.position = startPosition;
        }
    }

}
