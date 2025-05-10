using UnityEngine;

public class PlatformMovement : MonoBehaviour
{

    [Header("Direction")]
    [SerializeField] Vector3 destination;
    [SerializeField] bool relative;

    [Header("Movement")]
    [SerializeField] float speed = 1.0f;
    [SerializeField] bool lerp;

    [Header("Behavior")]
    [SerializeField] float startDelay;
    [SerializeField] float destinationDelay;
    [SerializeField] bool pingPong;

    Vector3 startPosition;
    Vector3 dest;

    float elapsedTime;

    bool toStart;
    bool waited;
    bool finished;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
        dest = relative ? startPosition + destination : destination;
    }

    // Update is called once per frame
    void Update()
    {
        // If the object has finished its movement
        if (finished)
        {
            return;
        }

        // Count up elapsed time
        elapsedTime += Time.deltaTime;

        // Check if they need to wait at start or destination
        if (!waited && !Waited())
        {
            return;
        }

        // Handle movement
        if (Move(transform.position, dest))
        {
            // If this object does not ping pong
            // (does not move back and forth between start and destination),
            // then the object has finished moving, and no longer needs to do anything
            if (!pingPong)
            {
                finished = true;
                return;
            }

            // Swap start & destination to move back and forth
            Swap(ref startPosition, ref dest);

            // Reset waiting and elapsed time
            toStart = !toStart;
            waited = false;
            elapsedTime = 0.0f;
        }
    }

    bool Waited()
    {
        if (!toStart && elapsedTime < startDelay)
        {
            return false;
        }

        if (toStart && elapsedTime < destinationDelay)
        {
            return false;
        }

        waited = true;
        return true;
    }

    bool Move(Vector3 from, Vector3 to)
    {
        if (Vector3.Distance(from, to) <= 0.1f)
        {
            return true;
        }

        if (lerp)
        {
            transform.position = Vector3.Lerp(from, to, speed * Time.deltaTime);
        }
        else
        {
            transform.position += (to - from).normalized * speed * Time.deltaTime;
        }

        return false;
    }

    void Destroy()
    {
        Destroy(gameObject);
    }

    void Swap(ref Vector3 one, ref Vector3 two)
    {
        Vector3 temp = one;
        one = two;
        two = temp;
    }

}
