using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalZombie : Zombie
{
    void Awake()
    {
        base.initialize();

        LifeMax = 100;
        Life = LifeMax;
        Lifetime = 3.0f;
        Runtime = 0.0f;
    }

    void Update()
    {
        base.checkAlive();

        base.updateAnimation();
    }

    void OnDestroy()
    {
        base.die();
    }
}
