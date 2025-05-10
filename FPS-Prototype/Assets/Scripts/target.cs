using System.Collections;
using System.Collections.Generic;
using System.Timers;
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

    [SerializeField] private GameObject artToDisable = null;
    private Collider targCollider;

    [Header("1: Speed. 2: Jump. 3: Time")]
    [SerializeField][Range(1, 3)] int element;
    bool affected;

    [Header("Speed Element")]
    [SerializeField][Range(0.01f, 999999)] int speedMod;
    [SerializeField] float speedModTime;

    [Header("Jump Element")]
    [SerializeField][Range(0.01f, 999999)] int jumpMod;
    [SerializeField] float jumpModTime;

    [Header("Time Element")]
    [SerializeField][Range(0.01f, 999999)] float timeMod;
    [SerializeField] float timeModTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //colorOrig = model.material.color;
        startPos = transform.position;
        targCollider = GetComponent<Collider>();

    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    void Movement()
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

    public void TakeDamage(int amount)
    {
        SoundManager.instance.PlaySFX("targetHit");

        HP -= amount;

        if(HP <= 0)
        {
            targCollider.enabled = false;
            artToDisable.SetActive(false);
        }   
    }

    public void ActivateElem(int modifier)
    {
        Debug.Log("Activating Element");
        int result;
        if (element >= modifier)
        {
            result = element - modifier;
        }
        else { result = modifier - element; }

        if (result > 0)
        {
            Debuff();
        }
        else
        {
            Buff();
        }
    }

    void Buff()
    {
        switch (element)
        {
            case 1:
                StartCoroutine(SpeedBuff());
                break;
            case 2:
                StartCoroutine(JumpBuff());
                break;
            case 3:
                StartCoroutine(TimeBuff());
                break;
        }
    }

    void Debuff()
    {
        switch (element)
        {
            case 1:
                StartCoroutine(SpeedDebuff());
                break;
            case 2:
                StartCoroutine(JumpDebuff());
                break;
            case 3:
                StartCoroutine(TimeDebuff());
                break;
        }
    }

    public IEnumerator SpeedBuff()
    {
        Debug.Log("Giving Speed");
        GameManager.instance.playerScript.baseSpeed *= speedMod;

        yield return new WaitForSeconds(speedModTime);

        Debug.Log("Taking Away Speed");
        GameManager.instance.playerScript.baseSpeed /= speedMod;

        Destroy(gameObject);
    }

    public IEnumerator SpeedDebuff()
    {
        Debug.Log("Taking Speed");
        GameManager.instance.playerScript.baseSpeed /= speedMod;

        yield return new WaitForSeconds(speedModTime);

        Debug.Log("Giving Speed");
        GameManager.instance.playerScript.baseSpeed *= speedMod;

        Destroy(gameObject);
    }

    public IEnumerator JumpBuff()
    {
        Debug.Log("Giving Jump");
        GameManager.instance.playerScript.jumpForce *= jumpMod;

        yield return new WaitForSeconds(jumpModTime);

        Debug.Log("Taking Away Jump");
        GameManager.instance.playerScript.jumpForce /= jumpMod;

        Destroy(gameObject);
    }

    public IEnumerator JumpDebuff()
    {
        Debug.Log("Taking Jump");
        GameManager.instance.playerScript.jumpForce /= jumpMod;

        yield return new WaitForSeconds(jumpModTime);

        Debug.Log("Giving Jump");
        GameManager.instance.playerScript.jumpForce *= jumpMod;

        Destroy(gameObject);
    }

    public IEnumerator TimeBuff() 
    {
        Debug.Log("Giving Time");
        Time.timeScale = timeMod;

        yield return new WaitForSeconds(timeModTime);

        Time.timeScale = GameManager.instance.timeScaleOrig;
    }

    public IEnumerator TimeDebuff()
    {
        Debug.Log("Taking Time");
        Time.timeScale = timeMod;

        yield return new WaitForSeconds(timeModTime);

        Time.timeScale = GameManager.instance.timeScaleOrig;
    }
}
