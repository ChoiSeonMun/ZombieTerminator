using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    public Player player = null;
    public Fever fever = null;
    public SoundManager soundManager = null;

    public Object clawAnimationObject = null;
    public Object crackAnimationObject = null;

    public Animator crackAnimator = null;
    public Animator zombieAnimator = null;

    public int life = 0;
    public int lifeMax = 0;
    // 좀비가 얼마나 생존해있는지 저장하는 변수
    public float runtime = float.NaN;
    // 좀비가 최대로 생존해있을 수 있는 시간을 저장하는 변수
    public float lifetime = float.NaN;


    public void TakeDamage(int damage)
    {
        // 좀비의 생명력을 대미지만큼 감소시킨다
        life = life - damage;

        if (crackAnimator == null)
        {
            // 좀비가 공격받는 애니메이션을 생성한다
            GameObject obj = Instantiate(crackAnimationObject, transform) as GameObject;
            obj.transform.SetParent(transform.parent);
            // 애니메이션을 시작한다
            crackAnimator = obj.GetComponent<Animator>();
            // 크랙 애니메이션이 life에 의해서만 조절될 수 있도록 하기 위함
            crackAnimator.speed = 0.0f;
            crackAnimator.Play("Target", 0, (float)(lifeMax - life) / (float)lifeMax);
        }
        else
        {
            // 애니메이션을 다음 프레임으로 넘긴다
            crackAnimator.Play("Target", 0, (float)(lifeMax - life) / (float)lifeMax);
        }
    }

    protected void initialize()
    {
        GameObject playerGO = GameObject.Find("Player");
        player = playerGO.GetComponent<Player>();
        fever = playerGO.GetComponent<Fever>();

        clawAnimationObject = Resources.Load("Prefabs/ClawAnimation");
        crackAnimationObject = Resources.Load("Prefabs/CrackAnimation");

        zombieAnimator = this.GetComponent<Animator>();
        // 좀비 애니메이션이 runtime에 의해서만 조절될 수 있도록 하기 위함
        zombieAnimator.speed = 0.0f;
    }

    protected void checkAlive()
    {
        // 좀비의 생존시간을 증가시킨다
        runtime += Time.deltaTime;
        // 수명이 다했거나 생명력이 0 이하로 떨어진 경우, 좀비가 죽는다
        if ((runtime >= lifetime) || (life <= 0.0f))
        {
            Destroy(gameObject);
        }
    }

    protected void updateAnimation()
    {
        zombieAnimator.Play("Target", 0, (float)(runtime) / (float)(lifetime));
    }

    protected void die()
    {
        // 좀비가 공격받는 애니메이션을 제거한다
        if (crackAnimator != null)
        {
            Destroy(crackAnimator.gameObject, 0.3f);
        }

        // 좀비가 공격을 당해 죽었을 경우 점수를 얻는다
        if (life <= 0.0f)
        {
            soundManager.PlayOneShot("glass-crack");

            if (fever.IsFeverOn == false)
            {
                fever.GainFeverCount();
            }
            player.GainScore(10);
        }
        else if (fever.IsFeverOn == false)
        {
            attack();
        }
    }

    private void attack()
    {
        // 좀비가 공격하는 애니메이션을 생성하고, 이 애니메이션을 일정시간 뒤에 삭제한다
        GameObject obj = Instantiate(clawAnimationObject, transform) as GameObject;
        obj.transform.SetParent(transform.parent);
        Destroy(obj, 0.25f);

        // 플레이어를 공격한다
        player.Hit();
    }
}
