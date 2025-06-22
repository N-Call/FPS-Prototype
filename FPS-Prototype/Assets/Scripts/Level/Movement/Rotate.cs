using UnityEngine;

public class Rotate : MonoBehaviour
{

    [SerializeField][Range(-1, 1)] int x;
    [SerializeField][Range(-1, 1)] int y;
    [SerializeField][Range(-1, 1)] int z;
    [SerializeField] float speed;

    // Update is called once per frame
    void Update()
    {
        float multiplier = speed * Time.deltaTime;
        transform.Rotate(x * multiplier, y * multiplier, z * multiplier);
    }

}
