using System.Collections;
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

    private UnityEngine.Object mHitAnimation = null;
    private UnityEngine.Object mAttackAnimation = null;
    private Animator mAttackAnimator = null;
    private float mAttackNormalizedTime = float.NaN;

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
        mType = type;
    }

    public void Hit(int damage)
    {
        if(mAttackAnimator == null)
        {
            // 좀비가 공격받는 애니메이션을 생성하고, 부모의 자식으로 삼는다 (자신과 동일한 계층에 둔다)
            GameObject obj = Instantiate(mAttackAnimation, transform) as GameObject;
            obj.transform.SetParent(transform.parent);

            // 애니메이션을 첫번째 프레임으로 고정한다
            mAttackAnimator = obj.GetComponent<Animator>();
            mAttackAnimator.speed = 0.0f;
            mAttackNormalizedTime = 0.0f;
            mAttackAnimator.Play("AttackAnimation", 0, mAttackNormalizedTime);
        }
        else
        {
            // 애니메이션을 다음 프레임으로 넘긴다
            mAttackNormalizedTime += 0.33f;
            mAttackAnimator.Play("AttackAnimation", 0, mAttackNormalizedTime);
        }

        // 좀비의 생명력을 대미지만큼 감소시키고, 생명력이 0 에 도달했을 경우 죽는다
        mLife = mLife - damage;
    }

    internal void Awake()
    {
        mHitAnimation = Resources.Load("Prefabs/HitAnimation");
        mAttackAnimation = Resources.Load("Prefabs/AttackAnimation");

        mPlayer = GameObject.Find("Player").GetComponent<Player>();
        mFever = GameObject.Find("Player").GetComponent<Fever>();

        mType = EType.NORMAL;
        mLife = 100;
        mLifetime = 3.0f;
        mRuntime = 0.0f;
    }

    internal void Update()
    {
        // 좀비의 생존시간을 증가시킨다
        mRuntime += Time.deltaTime;
        // 수명이 다했거나 생명력이 0 이하로 떨어진 경우, 좀비가 죽는다
        if ((mRuntime >= mLifetime) || (mLife <= 0.0f))
        {
            Destroy(gameObject);
        }
    }

    internal void OnDestroy()
    {
        // 좀비가 공격받는 애니메이션을 일정시간 후에 제거한다
        if (mAttackAnimator != null)
        {
            Destroy(mAttackAnimator.gameObject, 0.3f);
        }

        // 좀비가 공격을 당해 죽었을 경우 점수를 얻는다
        if (mRuntime < mLifetime)
        {
            mFever.GainFeverCount();
            mPlayer.GainScore(10);

            // 이 좀비가 특수좀비일 경우 폭탄 아이템을 증가시킨다
            if (mType == EType.SPECIAL)
            {
                mPlayer.GainBomb();
            }
        }
        // 수명이 다할 때까지 살아남았다면
        else
        {
            Attack();
        }
    }

    private void Attack()
    {
        // 좀비가 공격하는 애니메이션을 생성하고, 이 애니메이션을 0.25 초 뒤에 삭제한다
        GameObject obj = Instantiate(mHitAnimation, transform) as GameObject;
        obj.transform.SetParent(transform.parent);
        Destroy(obj, 0.25f);

        // 플레이어를 공격한다
        mPlayer.Hit();
    }
}
