using System.Collections;
using UnityEngine;

public class Target : MonoBehaviour, IDamage, ITarget
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

    [SerializeField][Range(1, 3)] int element;

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

    public void activateElem(int modifier, Transform player)
    {
        int result;
        if (element >= modifier)
        {
            result = element = modifier;
        }
        else { result = modifier - element; }

        switch (result)
        {
            case 0:
                Buff(player);
                break;
            case 1:
                Neutral(player);
                break;
            case 2:
                Debuff(player);
                break;
        }
    }

    void Buff(Transform player)
    {
        switch (element)
        {
            case 1:
                
                break;
            case 2:
                //Cry
                break;
            case 3:
                
                break;
        }
    }

    void Neutral(Transform player)
    {

    }

    void Debuff(Transform player)
    {

    }
}
