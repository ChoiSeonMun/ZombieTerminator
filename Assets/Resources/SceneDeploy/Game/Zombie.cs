using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    public enum EType
    {
        UNDEFINED,
        NORMAL,
        SPECIAL
    };

    private Player mPlayer = null;

    private float mLifetime = float.NaN;
    private float mRuntime = float.NaN;
    private int mLife = 0;
    private EType mType = EType.UNDEFINED;

    public void SetType(EType type)
    {
        this.mType = type;
    }

    internal void Awake()
    {
        this.mPlayer = GameObject.Find("Player").GetComponent<Player>();

        this.mType = EType.NORMAL;
        this.mLife = 100;
        this.mLifetime = 3.0f;
        this.mRuntime = 0.0f;
    }

    internal void Update()
    {
        this.mRuntime += Time.deltaTime;

        if (this.mRuntime >= this.mLifetime)
        {
            this.mPlayer.Hit();
            this.Die();
        }
    }

    internal void OnDestroy()
    {
        // 이 좀비가 특수좀비일 경우 폭탄 아이템을 증가시킨다
        if (this.mType == EType.SPECIAL)
        {
            this.mPlayer.GainBomb();
        }
        // 좀비가 공격을 당해 죽었을 경우 점수를 얻는다
        if (this.mRuntime < this.mLifetime)
        {
            this.mPlayer.GainScore(10);
        }
    }

    public void getDamaged(int damage)
    {
        this.mLife = this.mLife - damage;

        if(this.mLife <= 0.0f)
        {
            this.Die();
        }
    }

    private void Die()
    {
        Destroy(this.gameObject);
    }
}
