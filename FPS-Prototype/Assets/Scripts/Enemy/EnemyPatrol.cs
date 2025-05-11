using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrolling")]
    [SerializeField] int targetPoint = 0;
    [SerializeField] public Transform[] wayPoints;
    [SerializeField] float speed;

    Vector3 targetDir;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        targetDir = wayPoints[targetPoint].position - transform.position;

        //faceTarget();

        transform.position = Vector3.MoveTowards(transform.position, wayPoints[targetPoint].position, speed * Time.deltaTime);
        Debug.Log("Found waypoint moving to next");

        if (Vector3.Distance(transform.position, wayPoints[targetPoint].position) <= 0.02f)
        {
            targetPoint++;
        }
        if(targetPoint == wayPoints.Length)
        {
            targetPoint = 0;
        }        
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(targetDir);

        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * speed);
    }
}
