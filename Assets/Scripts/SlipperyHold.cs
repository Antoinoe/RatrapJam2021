using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlipperyHold : Hold
{
    public float slipCooldown;
    float slipTimer;
    Climber climber;

    private void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        climber = FindObjectOfType<Climber>();
    }

    public override void Holding()
    {
        base.Holding();
        StartCoroutine(SlipCooldown());
    }

    IEnumerator SlipCooldown()
    {
        slipTimer = 0;

        while (slipTimer < slipCooldown)
        {
            slipTimer += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        if(inUse) climber.Release();
    }
}
