using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ZombieType
{
    UNDEFINED,
    NORMAL,
    SPECIAL
};

public class Zombie : MonoBehaviour
{
    private GameManager manager = null;

    private float lifetime = float.NaN;
    private float runtime = float.NaN;
    private int life = 0;
    private ZombieType zombieType = ZombieType.UNDEFINED;
    
    private void Awake()
    {
        var obj = GameObject.Find("Manager");
        this.manager = obj.GetComponent<GameManager>();

        this.zombieType = ZombieType.NORMAL;
        this.life = 100;
        this.lifetime = 3.0f;
        this.runtime = 0.0f;
    }

    private void Update()
    {
        if(manager.getState() == GameState.PLAYING)
        { 
            this.runtime += Time.deltaTime;

            if(this.runtime >= this.lifetime)
            {
                this.manager.OnHit();

                this.Die();
            }
        }
    }

    private void OnDestroy()
    {
        if(this.runtime < this.lifetime)
        {
            this.manager.IncreaseScore(10);
        }
    }

    public void getDamaged(int damage)
    {
        this.life = this.life - damage;

        if(this.life <= 0.0f)
        {
            this.Die();
        }
    }

    private void Die()
    {
        // 이 좀비가 특수좀비일 경우 폭탄 아이템을 증가시킨다
        if (this.zombieType == ZombieType.SPECIAL)
        {
            manager.GetBomb(1);
        }

        Destroy(this.gameObject);
    }

    public void SetZombieType(ZombieType zombieType)
    {
        this.zombieType = zombieType;
    }

    public ZombieType GetZombieType()
    {
        return this.zombieType;
    }

    public int getLife()
    {
        return this.life;
    }
}
