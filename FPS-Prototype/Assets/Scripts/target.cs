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

    bool isSpeedBuffed;
    bool isJumpBuffed;
    bool isSpeedDebuffed;
    bool isJumpDebuffed;

    enum ElementType { speed = 1, jump = 2, ammo = 3 }

    private Collider targCollider;
    [SerializeField] private GameObject artToDisable = null;

    [Header("Element Type")]
    [SerializeField] ElementType elem;
    bool affected;

    [Header("Speed Element")]
    [SerializeField][Range(0.01f, 999999)] float speedMod;
    [SerializeField] float speedModTime;

    [Header("Jump Element")]
    [SerializeField][Range(0.01f, 999999)] float jumpMod;
    [SerializeField] float jumpModTime;

    [Header("Reload Element")]
    [SerializeField][Range(1, 100)] float reloadPercentBuff;
    [SerializeField][Range(1, 100)] float reloadPercentDebuff;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
        dest = relative ? startPosition + destination : destination;
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
        int element = (int) elem;
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
        
        int element = (int)elem;
        switch (element)
        {
            case 1:
                if (isSpeedBuffed == true)
                {
                    break;
                }
                else
                {
                    isSpeedBuffed = true;
                    StartCoroutine(SpeedBuff());
                    
                }
                break;
            case 2:
                if (isJumpBuffed == true)
                {
                    break;
                }
                else
                {
                    isJumpBuffed = true;
                    StartCoroutine(JumpBuff());
                    
                }
                break;
            case 3:
                AmmoBuff();
                
                break;
        }
    }

    void Debuff()
    {
        
        int element = (int)elem;

        switch (element)
        {
            case 1:
                if (isSpeedDebuffed == true)
                {
                    break;
                }
                else
                {
                    
                    isSpeedDebuffed = true;
                    StartCoroutine(SpeedDebuff());
                    
                }
                break;
            case 2:
                if (isJumpDebuffed == true)
                {
                    break;
                }
                else
                {
                    isJumpDebuffed = true;
                    StartCoroutine(JumpDebuff());
                    
                }
                break;
            case 3:
                AmmoDebuff();
                break;
        }
    }

    public IEnumerator SpeedBuff()
    {
        SoundManager.instance.PlaySFX("powerUp");
        GameManager.instance.playerScript.baseSpeed *= speedMod;
       

        yield return new WaitForSeconds(speedModTime);

        
        isSpeedBuffed = false;
        GameManager.instance.playerScript.baseSpeed /= speedMod;
        

        Destroy(gameObject);
    }

    public IEnumerator SpeedDebuff()
    {
        SoundManager.instance.PlaySFX("debuff");
        GameManager.instance.playerScript.baseSpeed /= speedMod;
        
        yield return new WaitForSeconds(speedModTime);

       
        isSpeedDebuffed = false;
        GameManager.instance.playerScript.baseSpeed *= speedMod;

        Destroy(gameObject);
    }

    public IEnumerator JumpBuff()
    {
        SoundManager.instance.PlaySFX("powerUp");
        GameManager.instance.playerScript.jumpForce *= jumpMod;

        yield return new WaitForSeconds(jumpModTime);

       
        isJumpBuffed = false;
        GameManager.instance.playerScript.jumpForce /= jumpMod;

        Destroy(gameObject);
    }

    public IEnumerator JumpDebuff()
    {
        SoundManager.instance.PlaySFX("debuff");
        GameManager.instance.playerScript.jumpForce /= jumpMod;

        yield return new WaitForSeconds(jumpModTime);

        
        isJumpDebuffed = false;
        GameManager.instance.playerScript.jumpForce *= jumpMod;

        Destroy(gameObject);
    }

    private void AmmoBuff() 
    {
        SoundManager.instance.PlaySFX("powerUp");
        for (int i = 0; i < GameManager.instance.playerScript.weaponList.Count; i++)
        {
            IReloadable rld = GameManager.instance.playerScript.weaponList[i].GetComponent<IReloadable>();
            rld?.SetAmmo(reloadPercentBuff);
        }

    }

    private void AmmoDebuff()
    {
        SoundManager.instance.PlaySFX("debuff");
        for (int i = 0; i < GameManager.instance.playerScript.weaponList.Count; i++)
        {
            IReloadable rld = GameManager.instance.playerScript.weaponList[i].GetComponent<IReloadable>();
            rld?.SetAmmo(reloadPercentDebuff);
        }
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
