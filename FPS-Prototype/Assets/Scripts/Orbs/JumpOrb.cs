using UnityEngine;

public class JumpOrb : BaseOrb
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        curAbility = EAbility.jumpBoost;
        if (GameManager.instance.playerAbilities != null)
        {
            duration += GameManager.instance.playerAbilities.o2Dur;
            modifier += GameManager.instance.playerAbilities.o2Srt;
            major = GameManager.instance.playerAbilities.o2Major;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
