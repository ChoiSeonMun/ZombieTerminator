using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private GameManager mManager = null;
    private Gun mGun = null;
    private Fever mFever = null;

    private int mLife = -1;
    private int mBomb = -1;
    private int mScore = -1;

    public void Shoot(GameObject obj)
    {
        this.mGun.Fire(obj);
    }

    public void Reload()
    {
        this.mGun.Reload();
    }

    public bool CanBomb()
    {
        bool retVal = this.mBomb > 0;
        this.mBomb -= 1;
        return retVal;
    }

    public void GainBomb()
    {
        this.mBomb += 1;
    }

    public void GainScore(int score)
    {
        this.mScore += score;
    }

    public void Hit()
    {
        this.mLife -= 1;


        if(this.mLife <= 0)
        {
            this.mManager.EndGame();
        }
    }

    internal void Awake()
    {
        this.mManager = GameObject.Find("Manager").GetComponent<GameManager>();
        this.mGun = this.gameObject.GetComponent<Gun>();
        this.mFever = GameObject.Find("Player").GetComponent<Fever>();

        this.mLife = 3;
        this.mBomb = 3;
        this.mScore = 0;
    }

    internal void Update()
    {
        GameInfo gi;
        gi.life = this.mLife;
        gi.bomb = this.mBomb;
        gi.score = this.mScore;
        gi.bulletCur = this.mGun.BulletCur;
        gi.bulletMax = this.mGun.BulletMax;

        this.mManager.RefreshUI(gi);
    }
}
