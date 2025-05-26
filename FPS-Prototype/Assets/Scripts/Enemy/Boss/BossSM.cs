using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BossSM : StateMachine, IDamage
{
    [HideInInspector] public IdleDecide idle;
    [HideInInspector] public JumpAttack jump;
    [HideInInspector] public RollAttack roll;
    [HideInInspector] public RunAttack run;
    [HideInInspector] public ShootAttack shoot;
    [HideInInspector] public SpeedAttack speed;

    public enum Ability
    {
        None,
        speedBoost = 1,
        jumpBoost = 2,
        invensBoost = 3,
    }

    [Header ("Refereances")]
    public Rigidbody rigidBody;
    public Animator animator;
    public NavMeshAgent agent;
    public Transform[] bodyParts;
    public Transform lShoulder;
    public Transform rShoulder;
    public Transform targetPoint;
    public Damage Bullet;
    public GameObject lShootPos;
    public GameObject rShootPos;
    public LayerMask ignorelayer;

    [Header("Boss Settings")]
    public int health;
    public int currentHealth;
    public float currentDecideDis;
    public bool isAnimDone;
    public int currentDamage;

    [Header("Idle Settings")]
    public int decideDis;
    public int decideTime;

    [Header("Jump Attack Settings")]
    public float jumpHeight;
    public float gravity;

    [Header("Roll Attack Settings")]
    public float rollForce;
    public float rollDecideDis;

    public Ability currentAbility;
    private bool isInvensible;

    public void Awake()
    {
        this.idle = new IdleDecide(stm:this);
        this.jump = new JumpAttack(stm:this);
        this.roll = new RollAttack(stm:this);
        this.run = new RunAttack(stm:this);
        this.shoot = new ShootAttack(stm:this);
        this.speed = new SpeedAttack(stm:this);

        currentHealth = health;
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
        Dead();
    }

    public void SpawnLeftProjectile()
    {
        Instantiate(Bullet, lShootPos.transform.position, transform.rotation);
    }

    public void SpawnRightProjectile()
    {
        Instantiate(Bullet, rShootPos.transform.position, transform.rotation);
    }

    private void Dead()
    {
        //Death animation
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
        jump.ResetRigid();
    }

    public void LookAtPlayer(int partIndex)
    {
        bodyParts[partIndex].transform.LookAt(new Vector3(GameManager.instance.player.transform.position.x, bodyParts[partIndex].transform.position.y, GameManager.instance.player.transform.position.z));
    }
}
