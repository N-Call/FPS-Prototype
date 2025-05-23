using UnityEngine;

public class BossSM : StateMachine
{
    [HideInInspector] public IdleDecide idle;
    [HideInInspector] public JumpAttack jump;
    [HideInInspector] public RollAttack roll;
    [HideInInspector] public RunAttack run;
    [HideInInspector] public ShootAttack shoot;

    [Header ("Refereances")]
    public Rigidbody rigidBody;
    public Animator animator;

    [Header("Boss Settings")]
    public int health;
    public int currentHealth;
    public int currentDecideDis;
    public bool isAnimDone;

    [Header("Idle Settings")]
    public int decideDis;
    public int decideTime;

    [Header("Jump Attack Settings")]
    public float jumpForce;

    [Header("Roll Attack Settings")]
    public float rollForce;
    public int rollDecideDis;



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
        animator.SetInteger("DecideDis", currentDecideDis);
    }
}
