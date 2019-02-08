using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalZombie : Zombie
{
    void Awake()
    {
        base.initialize();

        mLifeMax = 100;
        mLife = mLifeMax;
        mLifetime = 3.0f;
        mRuntime = 0.0f;
    }

    void Update()
    {
        base.checkAlive();

        ZombieAnimator.Play("NormalZombieAnimation", 0, (float)(mRuntime) / (float)(mLifetime));
    }

    void OnDestroy()
    {
        base.die();
    }
}
