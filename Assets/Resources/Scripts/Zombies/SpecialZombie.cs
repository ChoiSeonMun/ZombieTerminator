﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialZombie : Zombie
{
    void Awake()
    {
        base.initialize();

        LifeMax = 100;
        Life = LifeMax;
        Lifetime = 3.0f;
        Runtime = 0.0f;

        SoundManager.PlayOneShot("special-zombie-spawn");
    }

    void Update()
    {
        base.checkAlive();

        base.updateAnimation();
    }

    void OnDestroy()
    {
        base.die();

        // 플레이어의 공격으로 죽었을시
        if (Life <= 0.0f)
        {
            SoundManager.PlayOneShot("special-zombie-die");
            Player.GetComponent<Bomb>().AddBomb();
        }
    }
}
