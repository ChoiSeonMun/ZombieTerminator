using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialZombie : Zombie
{
    void Awake()
    {
        base.initialize();

        lifeMax = 100;
        life = lifeMax;
        lifetime = 3.0f;
        runtime = 0.0f;
    }

    void Start()
    {
        soundManager = SoundManager.Instance;
        soundManager.PlayOneShot("special-zombie-spawn");
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
        if (life <= 0.0f)
        {
            soundManager.PlayOneShot("special-zombie-die");
            player.GetComponent<Bomb>().AddBomb();
        }
    }
}
