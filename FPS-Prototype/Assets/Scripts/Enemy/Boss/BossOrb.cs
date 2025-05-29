using UnityEngine;

public class BossOrb : MonoBehaviour
{
    public enum Ability
    {
        None,
        speedBoost = 1,
        jumpBoost = 2,
        invensBoost = 3,
    }

    [SerializeField] Ability ability;
    public BossSM boss;
    [SerializeField] float activationTime;

    private float timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = activationTime;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if(timer <= 0)
        {
            boss.currentAbility = (BossSM.Ability)ability;
            Destroy(gameObject);
        }
    }
}
