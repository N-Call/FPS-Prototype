using UnityEngine;

public class PathMovement : MonoBehaviour
{

    [SerializeField] Transform[] positions;
    [SerializeField] float[] speeds;
    [SerializeField] float stoppingDistance;
    [SerializeField] bool reset;
    [SerializeField] bool destroyFinished;
    [SerializeField] float startDelay;

    Vector3 startPosition;

    int index;
    
    float distanceLeft;
    float startDelayTimer;

    bool finished;

    private void OnValidate()
    {
        stoppingDistance = Mathf.Clamp(stoppingDistance, 0.0f, float.MaxValue);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (positions.Length < 1 || speeds.Length < 1 || speeds.Length != positions.Length)
        {
            Destroy(gameObject);
            return;
        }

        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (startDelay > 0.0f && startDelayTimer < startDelay)
        {
            startDelayTimer += Time.deltaTime;
            return;
        }

        if (finished)
        {
            if (destroyFinished)
            {
                Destroy(gameObject);
            }

            return;
        }

        if (positions.Length <= index)
        {
            if (reset)
            {
                transform.position = startPosition;
                index = 0;
            }
            else
            {
                finished = true;
            }

            return;
        }

        if (Move(transform.position, positions[index].position))
        {
            index++;
        }
    }

    bool Move(Vector3 from, Vector3 to)
    {
        distanceLeft = Vector3.Distance(from, to);
        if (distanceLeft <= stoppingDistance)
        {
            return true;
        }

        transform.position += (to - from).normalized * speeds[index] * Time.deltaTime;
        return false;
    }

}
