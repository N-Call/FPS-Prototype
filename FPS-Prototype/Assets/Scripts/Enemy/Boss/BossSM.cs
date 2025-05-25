using UnityEngine;
using UnityEngine.AI;

public class BossSM : StateMachine, IDamage
{
    [HideInInspector] public IdleDecide idle;
    [HideInInspector] public JumpAttack jump;
    [HideInInspector] public RollAttack roll;
    [HideInInspector] public RunAttack run;
    [HideInInspector] public ShootAttack shoot;

    [Header ("Refereances")]
    public Rigidbody rigidBody;
    public Animator animator;
    public NavMeshAgent agent;
    public Transform lShoulder;
    public Transform rShoulder;
    public Damage Bullet;
    public GameObject lShootPos;
    public GameObject rShootPos;

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
    public float jumpForce;

    [Header("Roll Attack Settings")]
    public float rollForce;
    public float rollDecideDis;



    public void Awake()
    {
        this.idle = new IdleDecide(stm:this);
        this.jump = new JumpAttack(stm:this);
        this.roll = new RollAttack(stm:this);
        this.run = new RunAttack(stm:this);
        this.shoot = new ShootAttack(stm:this);

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

}
