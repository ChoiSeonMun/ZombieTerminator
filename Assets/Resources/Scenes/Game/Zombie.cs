﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    // 좀비가 가질 수 있는 종류값들
    public enum EType
    {
        UNDEFINED,
        NORMAL,
        SPECIAL
    };

    private UnityEngine.Object mAnimHit = null;

    // 플레이어 스크립트
    private Player mPlayer = null;
    // 피버 스크립트
    private Fever mFever = null;

    // 좀비가 최대로 생존해있을 수 있는 시간을 저장하는 변수
    private float mLifetime = float.NaN;
    // 좀비가 얼마나 생존해있는지 저장하는 변수
    private float mRuntime = float.NaN;
    // 좀비의 생명력을 저장하는 변수
    private int mLife = 0;
    // 좀비의 종류를 저장하는 변수
    private EType mType = EType.UNDEFINED;

    public void SetType(EType type)
    {
        this.mType = type;
    }

    public void Hit(int damage)
    {
        // 좀비의 생명력을 대미지만큼 감소시키고, 생명력이 0 에 도달했을 경우 죽는다
        this.mLife = this.mLife - damage;
        if (this.mLife <= 0.0f)
        {
            this.Die();
        }
    }

    internal void Awake()
    {
        mAnimHit = Resources.Load("Prefabs/AnimHit");

        this.mPlayer = GameObject.Find("Player").GetComponent<Player>();
        this.mFever = GameObject.Find("Player").GetComponent<Fever>();

        this.mType = EType.NORMAL;
        this.mLife = 100;
        this.mLifetime = 3.0f;
        this.mRuntime = 0.0f;
    }

    internal void Update()
    {
        // 좀비의 생존시간을 증가시키고, 수명이 다했을 경우 플레이어를 공격한 뒤 죽는다
        this.mRuntime += Time.deltaTime;
        if (this.mRuntime >= this.mLifetime)
        {
            this.Attack();

            this.Die();
        }
    }

    internal void OnDestroy()
    {
        // 좀비가 공격을 당해 죽었을 경우 점수를 얻는다
        if (this.mRuntime < this.mLifetime)
        {
            this.mFever.GainFeverCount();
            this.mPlayer.GainScore(10);

            // 이 좀비가 특수좀비일 경우 폭탄 아이템을 증가시킨다
            if (this.mType == EType.SPECIAL)
            {
                this.mPlayer.GainBomb();
            }
        }
    }

    private void Attack()
    {
        // 좀비가 공격하는 애니메이션을 생성하고, 이 애니메이션을 0.5 초 뒤에 삭제한다
        GameObject obj = Instantiate(mAnimHit, this.transform) as GameObject;
        obj.transform.SetParent(this.transform.parent);
        Destroy(obj, 0.5f);

        // 플레이어를 공격한다
        mPlayer.Hit();
    }

    private void Die()
    {
        Destroy(this.gameObject);
    }
}
