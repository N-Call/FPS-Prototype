using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class Target : MonoBehaviour, IDamage, ITarget
{
    [SerializeField] int HP;

    [Header("Direction")]
    [SerializeField] Vector3 destination;
    [SerializeField] bool relative;

    [Header("Movement")]
    [SerializeField] float speed = 1.0f;
    [SerializeField] bool lerp;

    [Header("Behavior")]
    [SerializeField] float startDelay;
    [SerializeField] float destinationDelay;
    [SerializeField] bool pingPong;

    Vector3 startPosition;
    Vector3 dest;

    float elapsedTime;

    bool toStart;
    bool waited;
    bool finished;

    private Collider targCollider;
    [SerializeField] private GameObject artToDisable = null;

    [Header("1: Speed. 2: Jump. 3: Time")]
    [SerializeField][Range(1, 3)] int element;
    bool affected;

    [Header("Speed Element")]
    [SerializeField][Range(0.01f, 999999)] float speedMod;
    [SerializeField] float speedModTime;

    [Header("Jump Element")]
    [SerializeField][Range(0.01f, 999999)] float jumpMod;
    [SerializeField] float jumpModTime;

    [Header("Time Element")]
    [SerializeField][Range(0.01f, 999999)] float timeMod;
    [SerializeField] float timeModTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //colorOrig = model.material.color;
        startPosition = transform.position;
        dest = relative ? startPosition + destination : destination;
        //startPos = transform.position;
        targCollider = GetComponent<Collider>();

    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    void Movement()
    {
        // If the object has finished its movement
        if (finished)
        {
            return;
        }

        // Count up elapsed time
        elapsedTime += Time.deltaTime;

        // Check if they need to wait at start or destination
        if (!waited && !Waited())
        {
            return;
        }

        // Handle movement
        if (Move(transform.position, dest))
        {
            // If this object does not ping pong
            // (does not move back and forth between start and destination),
            // then the object has finished moving, and no longer needs to do anything
            if (!pingPong)
            {
                finished = true;
                return;
            }

            // Swap start & destination to move back and forth
            Swap(ref startPosition, ref dest);

            // Reset waiting and elapsed time
            toStart = !toStart;
            waited = false;
            elapsedTime = 0.0f;
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
                SoundManager.instance.PlaySFX("powerUp");
                break;
            case 2:
                StartCoroutine(JumpBuff());
                SoundManager.instance.PlaySFX("powerUp");
                break;
            case 3:
                StartCoroutine(TimeBuff());
                SoundManager.instance.PlaySFX("powerUp");
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

    bool Waited()
    {
        if (!toStart && elapsedTime < startDelay)
        {
            return false;
        }

        if (toStart && elapsedTime < destinationDelay)
        {
            return false;
        }

        waited = true;
        return true;
    }

    bool Move(Vector3 from, Vector3 to)
    {
        if (Vector3.Distance(from, to) <= 0.1f)
        {
            return true;
        }

        if (lerp)
        {
            transform.position = Vector3.Lerp(from, to, speed * Time.deltaTime);
        }
        else
        {
            transform.position += (to - from).normalized * speed * Time.deltaTime;
        }

        return false;
    }

    void Swap(ref Vector3 one, ref Vector3 two)
    {
        Vector3 temp = one;
        one = two;
        two = temp;
    }
}
