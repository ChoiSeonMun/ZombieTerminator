using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum ZombieType { NORMAL, SPECIAL };

public class SZombie : MonoBehaviour
{
    private SGame manager = null;

    private float lifetime = float.NaN;
    private float runtime = float.NaN;
    private int life = 0;
    private ZombieType zombieType;
    
    private void Start()
    {
        var obj = GameObject.Find("Manager");
        this.manager = obj.GetComponent<SGame>();

        this.setZombieType(ZombieType.NORMAL);

    }

    private void Update()
    {
        if(manager.getState() == SGame.GameState.PLAYING) { 
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
            if (this.zombieType == ZombieType.SPECIAL)
            {
                manager.getBombItem(1);
            }
            this.Die();
        }
    }

    private void Die()
    {
        // Debug.Log(this.getZombieType());
        Destroy(this.gameObject);
    }

    public void setZombieType(ZombieType zombieType)
    {
        this.zombieType = zombieType;

        if (this.zombieType == ZombieType.NORMAL)
        {
            this.life = 100;
            this.lifetime = 3.0f;
            this.runtime = 0.0f;
        }
        else if(this.zombieType == ZombieType.SPECIAL)
        {
            this.life = 300;
            this.lifetime = 3.0f;
            this.runtime = 0.0f;
        }
    }

    public ZombieType getZombieType()
    {
        return this.zombieType;
    }

    public int getLife()
    {
        return this.life;
    }
}
