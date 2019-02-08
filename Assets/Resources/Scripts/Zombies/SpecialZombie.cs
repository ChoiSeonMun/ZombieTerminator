using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialZombie : Zombie
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

        ZombieAnimator.Play("SpecialZombieAnimation", 0, (float)(mRuntime) / (float)(mLifetime));
    }

    void OnDestroy()
    {
        base.die();

        // 플레이어의 공격으로 죽었을시, 특수좀비이므로 폭탄 아이템을 증가시킨다
        if (mLife <= 0.0f)
        {
            Player.GetComponent<Bomb>().AddBomb();
        }
    }
}
