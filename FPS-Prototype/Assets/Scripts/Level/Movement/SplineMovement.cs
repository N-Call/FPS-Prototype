using UnityEngine;
using UnityEngine.Splines;

public class SplineMovement : MonoBehaviour
{
    public SplineContainer splineContainer;
    public int splinIndex;
    [SerializeField] private bool takeStartPos;
    [SerializeField] private bool isRotating;
    [SerializeField] private bool isWrapable;
    [SerializeField] private bool isCustome;
    [SerializeField] private bool isPingPonging;
    [SerializeField] private bool isAscending;
    [SerializeField] [Range(0f,1f)] private float path = 0;
    [SerializeField] private float speed = 0.2f;


    private void OnValidate()
    {
        if (splineContainer != null)
        {
            splinIndex = (splineContainer.Splines.Count == 1) ? 0 : splinIndex;
        }
        if (!isCustome)
        {
            path = (isAscending)? 0 : 1;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (takeStartPos)
        {
            int index = (isAscending) ? 0 : splineContainer[splinIndex].Count - 1;
            BezierKnot knot = splineContainer[splinIndex][index];
            knot.Position = transform.position - splineContainer.transform.position;

            knot.TangentIn = Vector3.zero;
            knot.TangentOut = Vector3.zero;

            splineContainer[splinIndex][index] = knot;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(splineContainer == null) { return; }

        if (isAscending)
        {
            path += speed * Time.deltaTime;
            path = (isWrapable && path > 1f) ? 0 : path;
            if(path >= 1 && isPingPonging)
            {
                isAscending = false;
            }
        }
        else
        {
            path -= speed * Time.deltaTime;
            path = (isWrapable && path < 0f) ? 1 : path;

            if (path <= 0 && isPingPonging)
            {
                isAscending = true;
            }
        }

        var curv = splineContainer[splinIndex];
        Vector3 point = curv.EvaluatePosition(path);


        transform.position = point + splineContainer.transform.position;
    }
}
