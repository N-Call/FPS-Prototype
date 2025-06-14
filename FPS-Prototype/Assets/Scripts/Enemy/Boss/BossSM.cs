using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class BossSM : StateMachine, IDamage
{
    [HideInInspector] public IdleDecide idle;
    [HideInInspector] public JumpAttack jump;
    [HideInInspector] public RollAttack roll;
    [HideInInspector] public RunAttack run;
    [HideInInspector] public ShootAttack shoot;
    [HideInInspector] public SpeedAttack speed;
    [HideInInspector] public BeamAttack beam;
    [HideInInspector] public DeadState dead;

    [Header ("Refereances")]
    public Rigidbody rigidBody;
    public Animator animator;
    public NavMeshAgent agent;
    public Transform[] bodyParts;
    public BossOrb[] orbs;
    public Transform lShoulder;
    public Transform rShoulder;
    public Transform targetPoint;
    public Damage homingBullet;
    public Damage regularBullet;
    public Damage currentbullet;
    public GameObject lShootPos;
    public GameObject rShootPos;
    public LayerMask ignorelayer;
    public GameObject orbLocation;

    [Header("Boss Settings")]
    public int health;
    public int currentHealth;
    public float currentDecideDis;
    public bool isAnimDone;
    public int currentDamage;
    public float deathTimer;
    public float orbSpawnTimer;

    [Header("Idle Settings")]
    public int decideDis;
    public float decideTime;

    [Header("Jump Attack Settings")]
    public float jumpHeight;
    public float gravity;

    [Header("Speed Attack Settings")]
    public float speedAttackRate;
    public float speedAttackRange;
    public int speedAmount;
    public int speedDuration;

    [Header("Roll Attack Settings")]
    public float rollForce;
    public float rollDecideDis;
    public float rollSpeed;

    public EAbility currentAbility;

    public float orbSpawnCounter;
    private bool isInvensible;

    public UnityEvent onSpawnOrb;

    public void Awake()
    {
        BossOrb.OnDeath += ActivateAbility;
        this.idle = new IdleDecide(stm:this);
        this.jump = new JumpAttack(stm:this);
        this.roll = new RollAttack(stm:this);
        this.run = new RunAttack(stm:this);
        this.shoot = new ShootAttack(stm:this);
        this.speed = new SpeedAttack(stm:this);
        this.beam = new BeamAttack(stm:this);
        this.dead = new DeadState(stm:this);

        targetPoint = new GameObject("Jump Pos").transform;
        targetPoint.transform.position = transform.position + Vector3.up * jumpHeight;
        currentHealth = 99;
        orbSpawnCounter = orbSpawnTimer;
    }

    protected override BaseState GetFirstState()
    {
        return this.idle;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public void SetDecideAnim()
    {
        animator.SetFloat("DecideDis", currentDecideDis);
    }

    public void TakeDamage(int amount)
    {
        if(isInvensible) { return; }

        currentHealth -= amount;
        if (currentHealth > 0)
        {
            if(currentHealth <= health / 2 && orbSpawnCounter >= orbSpawnTimer && currentAbility == 0) 
            { 
                onSpawnOrb?.Invoke();
                orbSpawnCounter = 0;
            }

            //GameManager.instance.bossHPbar.fillAmount = (float)currentHealth / (float)health;
            StartCoroutine(FlashRed());
        }
        else
        {
            Dead();
        }
    }

    IEnumerator FlashRed()
    {
        List<Color> colors = new List<Color>();

        // Set children's colors to red
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            colors.Add(renderer.material.color);
            renderer.material.color = Color.red;
        }

        yield return new WaitForSeconds(0.01f);

        int index = 0;
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            if (index < colors.Count)
            {
                renderer.material.color = colors[index];
            }
            index++;
        }
    }

    private void OnDestroy()
    {
        BossOrb.OnDeath -= ActivateAbility;
    }

    public void ShootOrbPos()
    {
        idle.shouldShoot = true;
    }

    public void SpawnLeftProjectile()
    {
        Instantiate(currentbullet, lShootPos.transform.position, lShootPos.transform.rotation).target = GameManager.instance.player.transform;
    }

    public void SpawnRightProjectile()
    {
        Instantiate(currentbullet, rShootPos.transform.position, rShootPos.transform.rotation).target = GameManager.instance.player.transform;
    }

    public void ActivateAbility(EAbility ability)
    {
        currentAbility = ability;
    }

    private void Dead()
    {
        //Death animation
        ChangeState(dead);
    }

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<IDamage>()?.TakeDamage(currentDamage);
    }

    public void ActivateJumpAnim()
    {
        jump.JumpToTarget();
    }

    public bool GetInvincible()
    {
        return isInvensible;
    }

    public void SetInvincible(bool answer)
    {
        isInvensible = answer;
    }

    public void ResetRigid()
    {
        if(currentState == jump)
            jump.ResetRigid();
        if (currentState == speed)
            speed.ChangeToIdle();
        if (currentState == shoot)
            shoot.StopShooting();
    }

    public void LookAtPlayer(int partIndex)
    {
        bodyParts[partIndex].transform.LookAt(new Vector3(GameManager.instance.player.transform.position.x, bodyParts[partIndex].transform.position.y, GameManager.instance.player.transform.position.z));
    }
}
