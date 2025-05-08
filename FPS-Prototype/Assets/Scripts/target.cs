using System.Collections;
using UnityEngine;

public class Target : MonoBehaviour, IDamage
{

    //[SerializeField] Renderer model;

    [SerializeField] int HP;


    //Ping Pong variables
    Vector3 startPos;
    public bool movingTarget;
    public bool horizontal;
    public bool vertical;
    [SerializeField] int speed;
    [SerializeField] float travelDistance;


    [SerializeField][Range(1, 3)] int modifier;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //colorOrig = model.material.color;
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (movingTarget)
        {
            if (horizontal)
            {
                transform.position = startPos + transform.right * Mathf.PingPong(Time.time * speed, travelDistance);
            }
            if (vertical)
            {
                transform.position = startPos + transform.forward * Mathf.PingPong(Time.time * speed, travelDistance);   
            }
        }
    }

    public void takeDamage(int amount)
    {
        HP -= amount;

        if(HP <= 0)
        {
            Destroy(gameObject);
        }
        
    }

}
