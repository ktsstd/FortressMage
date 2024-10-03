using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpirit : MonsterAI
{    
    public override void Start()
    {
        base.Start();
        attackRange = 2.0f; 
        attackCooldown = 1.5f;
        MonsterDmg = 20;
        MaxHp = 60f;
        CurHp = MaxHp;
    }

    public override void Update()
    {
        base.Update();
    }
}
