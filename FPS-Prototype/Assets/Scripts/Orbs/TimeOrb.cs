using UnityEngine;

public class TimeOrb : BaseOrb
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        curAbility = EAbility.timeBoost;
        if (GameManager.instance.playerAbilities != null)
        {
            duration += GameManager.instance.playerAbilities.o4Dur;
            modifier += GameManager.instance.playerAbilities.o4Srt;
            major = GameManager.instance.playerAbilities.o4Major;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
