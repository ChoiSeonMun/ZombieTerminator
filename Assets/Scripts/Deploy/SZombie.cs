using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SZombie : MonoBehaviour
{
    private SGame manager = null;

    private float lifetime = float.NaN;
    private float runtime = float.NaN;
    private int life = 0;

    private void Start()
    {
        var obj = GameObject.Find("Manager");
        this.manager = obj.GetComponent<SGame>();

        this.lifetime = 3.0f;
        this.runtime = 0.0f;
        this.life = 100;
    }

    private void Update()
    {
        this.runtime += Time.deltaTime;

        if(this.runtime >= this.lifetime)
        {
            this.manager.OnHit();

            this.Die();
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
        Destroy(this.gameObject);
    }
}
