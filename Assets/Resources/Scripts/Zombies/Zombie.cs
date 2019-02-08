using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    public Player Player = null;
    public Fever Fever = null;

    public Object ClawAnimationObject = null;
    public Object CrackAnimationObject = null;

    public Animator CrackAnimator = null;
    public Animator ZombieAnimator = null;

    public int mLife = 0;
    public int mLifeMax = 0;
    // 좀비가 얼마나 생존해있는지 저장하는 변수
    public float mRuntime = float.NaN;
    // 좀비가 최대로 생존해있을 수 있는 시간을 저장하는 변수
    public float mLifetime = float.NaN;

    public void Hit(int damage)
    {
        // 좀비의 생명력을 대미지만큼 감소시킨다
        mLife = mLife - damage;

        if (CrackAnimator == null)
        {
            // 좀비가 공격받는 애니메이션을 생성한다
            GameObject obj = Instantiate(CrackAnimationObject, transform) as GameObject;
            obj.transform.SetParent(transform.parent);
            // 애니메이션을 시작한다
            CrackAnimator = obj.GetComponent<Animator>();
            // 크랙 애니메이션이 mLife에 의해서만 조절될 수 있도록 하기 위함
            CrackAnimator.speed = 0.0f;
            CrackAnimator.Play("AttackAnimation", 0, (float)(mLifeMax - mLife) / (float)mLifeMax);
        }
        else
        {
            // 애니메이션을 다음 프레임으로 넘긴다
            CrackAnimator.Play("AttackAnimation", 0, (float)(mLifeMax - mLife) / (float)mLifeMax);
        }
    }

    protected void initialize()
    {
        GameObject playerGO = GameObject.Find("Player");
        Player = playerGO.GetComponent<Player>();
        Fever = playerGO.GetComponent<Fever>();

        ClawAnimationObject = Resources.Load("Prefabs/HitAnimation");
        CrackAnimationObject = Resources.Load("Prefabs/AttackAnimation");

        ZombieAnimator = this.GetComponent<Animator>();
        // 좀비 애니메이션이 mRuntime에 의해서만 조절될 수 있도록 하기 위함
        ZombieAnimator.speed = 0.0f;
    }

    protected void checkAlive()
    {
        // 좀비의 생존시간을 증가시킨다
        mRuntime += Time.deltaTime;
        // 수명이 다했거나 생명력이 0 이하로 떨어진 경우, 좀비가 죽는다
        if ((mRuntime >= mLifetime) || (mLife <= 0.0f))
        {
            Destroy(gameObject);
        }
    }

    protected void die()
    {
        // 좀비가 공격받는 애니메이션을 제거한다
        if (CrackAnimator != null)
        {
            Destroy(CrackAnimator.gameObject, 0.3f);
        }

        // 좀비가 공격을 당해 죽었을 경우 점수를 얻는다
        if (mRuntime < mLifetime)
        {
            Fever.GainFeverCount();
            Player.GainScore(10);
        }
        // 수명이 다할 때까지 살아남았다면
        else
        {
            attack();
        }
    }

    private void attack()
    {
        // 좀비가 공격하는 애니메이션을 생성하고, 이 애니메이션을 0.25 초 뒤에 삭제한다
        GameObject obj = Instantiate(ClawAnimationObject, transform) as GameObject;
        obj.transform.SetParent(transform.parent);
        Destroy(obj, 0.25f);

        // 플레이어를 공격한다
        Player.Hit();
    }
}
