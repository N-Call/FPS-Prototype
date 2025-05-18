using UnityEngine;

public class ObjectMovement : MonoBehaviour
{

    // Prevents the player's parent being reset when moving onto a second moving platform
    static bool shouldResetPlayerParent;

    [Header("Destination Settings")]
    [SerializeField] Vector3 destination;
    [SerializeField] bool relative = true;

    [Header("Movement Settings")]
    [SerializeField] float moveSpeed;
    [SerializeField] bool lerp;

    [Header("Passenger Settings")]
    [SerializeField] bool carryPassengers;
    [SerializeField] bool moveWithoutPlayer = true;
    [SerializeField] bool waitForPlayer;

    [Header("Time Delay Settings")]
    [SerializeField] float startDelay;

    [Header("Destruction Settings")]
    [SerializeField] float destroyAfterTime;

    BoxCollider passengerCollider;

    protected Vector3 currentDestination;

    protected float startTimer;
    protected float destinationTimer;
    float destroyTimer;

    bool waitedForPlayer;
    bool hasPlayer;

    void Start()
    {
        currentDestination = relative ? transform.position + destination : destination;
        if (carryPassengers)
        {
            passengerCollider = gameObject.AddComponent<BoxCollider>();
            passengerCollider.enabled = true;
            passengerCollider.isTrigger = true;
            passengerCollider.size = new Vector3(1.0f, 2.0f, 1.0f);
            passengerCollider.center = new Vector3(0.0f, 1.4f, 0.0f);
        }

        OnStart();
    }

    void FixedUpdate()
    {
        if (carryPassengers)
        {
            if (!moveWithoutPlayer && !hasPlayer)
            {
                return;
            }

            if (waitForPlayer && !waitedForPlayer && !hasPlayer)
            {
                return;
            }

            if (hasPlayer)
            {
                waitedForPlayer = true;
            }
        }

        if (startDelay > 0.0f && startTimer < startDelay)
        {
            startTimer += Time.deltaTime;
            return;
        }

        Move();
        Destruction();
    }

    virtual protected void OnStart()
    {

    }

    virtual protected void Move()
    {

    }

    virtual protected void Destruction()
    {
        if (destroyAfterTime <= 0.0f)
        {
            return;
        }

        destroyTimer += Time.deltaTime;
        if (destroyTimer >= destroyAfterTime)
        {
            DestroySelf();
        }
    }

    protected bool Move(Vector3 from, Vector3 to)
    {
        if (Vector3.Distance(from, to) <= 0.1f)
        {
            return true;
        }

        if (lerp)
        {
            transform.position = Vector3.Lerp(from, to, moveSpeed * Time.deltaTime);
        }
        else
        {
            transform.position += (to - from).normalized * moveSpeed * Time.deltaTime;
        }

        return false;
    }

    protected void SwapVectors(ref Vector3 one, ref Vector3 two)
    {
        Vector3 temp = one;
        one = two;
        two = temp;
    }

    protected void DestroySelf()
    {
        if (gameObject.transform.childCount == 0)
        {
            Destroy(gameObject);
            return;
        }

        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            Transform child = gameObject.transform.GetChild(i);
            if (child.tag == "Player")
            {
                child.parent = null;
                break;
            }
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!carryPassengers || other.tag != "Player")
        {
            return;
        }

        if (other.transform.parent != null)
        {
            shouldResetPlayerParent = false;
        }

        hasPlayer = true;
        other.transform.parent = passengerCollider.transform;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!carryPassengers || other.tag != "Player" || !shouldResetPlayerParent)
        {
            shouldResetPlayerParent = true;
            return;
        }

        hasPlayer = false;
        other.transform.parent = null;
    }

}
