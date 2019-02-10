using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public LevelManager LevelManager = null;
    public Fever Fever = null;
    public ButtonExtension[] TargetButtons = null;
    public Player Player = null;
    public UnityEngine.Object NormalZombieObject = null;
    public UnityEngine.Object SpecialZombieObject = null;

    private System.Random mRand = null;
    // Spawn Cooldown 시간값을 저장하는 변수
    private float mSpawnCooldown = float.NaN;
    // Fever 현재 쿨다운을 임시적으로 기록할 변수
    private float mSpawnCooldownTemp = float.NaN;
    // 이전 Spawn 으로부터 얼마나 시간이 지났는지를 저장하는 변수
    private float mSpawnTimer = float.NaN;
    // 특수 좀비를 Spawn할 확률을 조정하기 위해 일반 좀비와 특수 좀비의 스폰 수를 기록하는 변수들
    private int mNormalSpawnCount = -1;
    private int mSpecialSpawnCount = -1;
    // 특수 좀비를 Spawn할 확률
    private float mSpecialSpawnRate = float.NaN;
    // 특수 좀비의 Spawn 확률을 보정하는 변수
    private float mRateSpecialSpawnAmend = float.NaN;
    // 게임이 중단되는 것을 저장하기 위한 변수
    private bool mIsStopped = false;

    public void onLevelUp()
    {
        mSpawnCooldown *= 0.9f;
    }

    public void onFeverOn()
    {
        mSpawnCooldownTemp = mSpawnCooldown;
        mSpawnCooldown = 0.1f;
    }

    public void onFeverOff()
    {
        mSpawnCooldown = mSpawnCooldownTemp;
        // 좀비를 가지고 있는 target 을 순회하면서, 각 좀비를 죽인다
        foreach (ButtonExtension target in TargetButtons)
        {
            if (target.transform.childCount > 0)
            {
                Zombie zombie = target.transform.GetChild(0).GetComponent<Zombie>();

                if (zombie != null)
                {
                    zombie.Hit(zombie.LifeMax);
                }
            }
        }
    }

    public void StopTarget()
    {
        mIsStopped = true;

        foreach (ButtonExtension target in TargetButtons)
        {
            if (target.transform.childCount > 0)
            {
                GameObject zombie = target.transform.GetChild(0).gameObject;
                zombie.SetActive(false);
            }
        }
    }

    public void ResumeTarget()
    {
        mIsStopped = false;

        foreach (ButtonExtension target in TargetButtons)
        {
            if (target.transform.childCount > 0)
            {
                GameObject zombie = target.transform.GetChild(0).gameObject;
                zombie.SetActive(true);
            }
        }
    }

    void Awake()
    {
        mRand = new System.Random();
        mSpawnCooldown = 2.0f;
        mSpawnCooldownTemp = 0.0f;
        mSpawnTimer = 0.0f;
        mNormalSpawnCount = 0;
        mSpecialSpawnCount = 0;
        mSpecialSpawnRate = 0.15f;
        mRateSpecialSpawnAmend = 0.0f;
    }

    void Update()
    {
        if (mIsStopped == false)
        {
            inputTarget();
            checkSpawn();
        }
    }

    private void inputTarget()
    {
        foreach(ButtonExtension targetButton in TargetButtons)
        {
            // target 이 눌렸다면, 해당 target 에 대해 사격한다
            if (targetButton.IsReleased)
            {
                Player.Shoot(targetButton.gameObject);
            }
        }
    }

    private void checkSpawn()
    {
        mSpawnTimer += Time.deltaTime;
        if (mSpawnTimer >= mSpawnCooldown)
        {
            spawnZombie();
            mSpawnTimer = 0.0f;
        }
    }

    private void spawnZombie()
    {
        // Fisher-Yates 셔플 알고리듬
        int[] order = new int[9];
        for (int i = 0; i < 9; ++i)
        {
            order[i] = i;
        }
        for (int i = 8; i >= 0; --i)
        {
            int num = mRand.Next(i + 1);
            int temp = order[num];
            order[num] = order[i];
            order[i] = temp;
        }

        // 무작위의 순서대로 타겟들을 순회
        for (int i = 0; i < 9; ++i)
        {
            GameObject target = TargetButtons[order[i]].gameObject;

            // 타겟에 자식이 없다면 ( 좀비가 없다면 )
            if (target.transform.childCount == 0)
            {
                // 특수좀비 스폰카운터에 도달했을 경우
                if (canSpawnSpecialZombie())
                {
                    ++mSpecialSpawnCount;

                    // 특수좀비 객체를 생성하고 타겟의 자식으로 설정
                    GameObject zombie = Instantiate(SpecialZombieObject, target.transform) as GameObject;
                    zombie.transform.SetParent(target.transform);
                }
                else
                {
                    ++mNormalSpawnCount;

                    // 일반좀비 객체를 생성하고 타겟의 자식으로 설정
                    GameObject zombie = Instantiate(NormalZombieObject, target.transform) as GameObject;
                    zombie.transform.SetParent(target.transform);
                }

                // 좀비를 스폰했으므로 함수 종료
                break;
            }
        }
    }

    private bool canSpawnSpecialZombie()
    {
        float rateSpawnNow = (float)mSpecialSpawnCount / (float)(mSpecialSpawnCount + mNormalSpawnCount);

        // 실제 확률이 기대 확률보다 크면 스폰 확률을 줄이기 위해 보정값을 줄인다
        if (rateSpawnNow > mSpecialSpawnRate)
        {
            mRateSpecialSpawnAmend -= 0.1f;
        }
        // 반대로 실제 확률이 기대 확률보다 작으면 스폰 확률을 늘이기 위해 보정값을 늘인다
        else if (rateSpawnNow < mSpecialSpawnRate)
        {
            mRateSpecialSpawnAmend += 0.1f;
        }

        // 난수를 생성하여 스폰할 것인지 정한다
        if (mRand.NextDouble() < (mSpecialSpawnRate + mRateSpecialSpawnAmend))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
