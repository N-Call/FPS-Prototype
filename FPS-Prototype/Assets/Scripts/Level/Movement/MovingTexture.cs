using UnityEngine;

[RequireComponent (typeof(MeshRenderer))]
public class MovingTexture : MonoBehaviour
{

    [SerializeField] Vector2 speed;

    Material material;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        material = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        material.mainTextureOffset += speed * Time.deltaTime;
    }

}
