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
    [SerializeField] bool startDelayInitial;
    [SerializeField] float destinationDelay;
    [SerializeField] bool pingPong;
    [SerializeField] bool backToStart;

    [Header("Destruction")]
    [SerializeField] float destroyAfterTime;
    [SerializeField] int destroyAfterCycles;
    [SerializeField] float destroyAtDestinationDelay;

    Vector3 startPosition;
    Vector3 dest;

    float lifeTime;
    float waitTime;
    float cycles;
    float destroyAtDestinationTime;

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
        // Count up elapsed life time
        lifeTime += Time.deltaTime;

        if (destroyAfterTime > 0 && lifeTime >= destroyAfterTime)
        {
            HandleDestruction();
            return;
        }

        // Handle count up to destruction if reached destination
        if (destroyAtDestinationDelay > 0.0f)
        {
            if (cycles >= 0.5f)
            {
                destroyAtDestinationTime += Time.deltaTime;
            }

            if (destroyAtDestinationTime >= destroyAtDestinationDelay)
            {
                HandleDestruction();
                return;
            }
        }

        // If the object has finished its movement
        if (finished)
        {
            return;
        }

        // Count up wait time
        waitTime += Time.deltaTime;

        // Check if they need to wait at start or destination
        if (!waited && !Waited())
        {
            return;
        }

        // Handle movement
        if (Move(transform.position, dest))
        {
            // 0.5 cycles = start to destination
            // 1 cycle = start to start
            cycles += 0.5f;

            if (destroyAfterCycles > 0 && cycles >= destroyAfterCycles)
            {
                HandleDestruction();
                return;
            }

            // If this object does not ping pong
            // (does not move back and forth between start and destination),
            // then the object has finished moving, and no longer needs to do anything
            if (!pingPong && !backToStart)
            {
                finished = true;
                return;
            }

            // Reset waiting and elapsed time
            waited = false;
            waitTime = 0.0f;

            if (backToStart)
            {
                transform.position = startPosition;
            }
            else
            {
                // Swap start & destination to move back and forth
                Swap(ref startPosition, ref dest);
                toStart = !toStart;
            }
        }
    }

    void HandleDestruction()
    {
        Destroy(gameObject);
    }

    bool Waited()
    {
        if (!toStart && waitTime < startDelay)
        {
            return false;
        }

        if (startDelay > 0.0f && startDelayInitial)
        {
            startDelay = 0.0f;
        }

        if ((toStart || backToStart && cycles > 0.0f) && waitTime < destinationDelay)
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

    void Swap(ref Vector3 one, ref Vector3 two)
    {
        Vector3 temp = one;
        one = two;
        two = temp;
    }

}
