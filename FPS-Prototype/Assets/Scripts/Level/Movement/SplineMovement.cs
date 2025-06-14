using UnityEngine;
using UnityEngine.Splines;

public class SplineMovement : MonoBehaviour
{
    public SplineContainer spline;
    [SerializeField] private bool takeStartPos;
    [SerializeField] private bool isWrapable;
    [SerializeField] private bool isCustome;
    [SerializeField] private bool isAscending;
    [SerializeField] [Range(0f,1f)] private float path = 0;
    [SerializeField] private float speed = 0.2f;


    private void OnValidate()
    {
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
            int index = (isAscending) ? 0 : spline.Spline.Count - 1;
            BezierKnot knot = spline.Spline[index];
            knot.Position = transform.position - spline.transform.position;

            knot.TangentIn = Vector3.zero;
            knot.TangentOut = Vector3.zero;

            spline.Spline[index] = knot;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(spline == null) { return; }

        if (isAscending)
        {
            path += speed * Time.deltaTime;
            path = (isWrapable && path > 1f) ? 0 : path;
        }
        else
        {
            path -= speed * Time.deltaTime;
            path = (isWrapable && path < 0f) ? 1 : path;
        }

        var curv = spline.Spline;
        Vector3 point = curv.EvaluatePosition(path);


        transform.position = point + spline.transform.position;
    }
}
