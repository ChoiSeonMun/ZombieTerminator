using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public EventManager eventManager = null;

    public ButtonExtension[] targetButtons = null;
    public UnityEngine.Object normalZombieObject = null;
    public UnityEngine.Object specialZombieObject = null;

    // Spawn Cooldown 시간값을 저장하는 변수
    public float spawnCooldown = float.NaN;
    // 특수 좀비를 Spawn할 확률
    public float specialSpawnRate = float.NaN;
    // 특수 좀비를 Spawn할 확률을 조정하기 위해 일반 좀비와 특수 좀비의 스폰 수를 기록하는 변수들
    public int mNormalSpawnCount = -1;
    public int mSpecialSpawnCount = -1;
    // 특수 좀비의 Spawn 확률을 보정하는 변수
    public float mRateSpecialSpawnAmend = float.NaN;

    private Fever mFever = null;
    private Player mPlayer = null;

    private System.Random mRand = null;
    // Fever 현재 쿨다운을 임시적으로 기록할 변수
    private float mSpawnCooldownTemp = float.NaN;
    // 이전 Spawn 으로부터 얼마나 시간이 지났는지를 저장하는 변수
    private float mSpawnTimer = float.NaN;
    // 게임이 중단되는 것을 저장하기 위한 변수
    private bool mIsStopped = false;

    public void DecreaseSpawnCooldown()
    {
        spawnCooldown = (spawnCooldown * 0.5f) + 0.35f;
    }

    public void SpawnSpeedUp()
    {
        mSpawnCooldownTemp = spawnCooldown;
        spawnCooldown = 0.05f;
        specialSpawnRate = 0.05f;
    }

    public void SpawnSpeedDown()
    {
        spawnCooldown = mSpawnCooldownTemp;
        specialSpawnRate = 0.10f;
    }

    public void DestroyZombies()
    {
        // 좀비를 가지고 있는 target 을 순회하면서, 각 좀비를 죽인다
        foreach (ButtonExtension target in targetButtons)
        {
            if (target.transform.childCount > 0)
            {
                Zombie zombie = target.transform.GetChild(0).GetComponent<Zombie>();

                if (zombie != null)
                {
                    Destroy(zombie.gameObject);
                }
            }
        }
    }

    public void StopTargets()
    {
        mIsStopped = true;

        foreach (ButtonExtension target in targetButtons)
        {
            if (target.transform.childCount > 0)
            {
                GameObject zombie = target.transform.GetChild(0).gameObject;
                zombie.SetActive(false);
            }
        }
    }

    public void ResumeTargets()
    {
        mIsStopped = false;

        foreach (ButtonExtension target in targetButtons)
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
        spawnCooldown = 2.0f;
        specialSpawnRate = 0.10f;

        mPlayer = eventManager.player;
        mFever = eventManager.fever;

        mRand = new System.Random();
        mSpawnCooldownTemp = 0.0f;
        mSpawnTimer = 0.0f;
        mNormalSpawnCount = 0;
        mSpecialSpawnCount = 0;
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
        foreach(ButtonExtension targetButton in targetButtons)
        {
            // target 이 눌렸다면, 해당 target 에 대해 사격한다
            if (targetButton.IsReleased)
            {
                mPlayer.Shoot(targetButton.gameObject);
            }
        }
    }

    private void checkSpawn()
    {
        mSpawnTimer += Time.deltaTime;
        if (mSpawnTimer >= spawnCooldown)
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
            GameObject target = targetButtons[order[i]].gameObject;

            // 타겟에 자식이 없다면 ( 좀비가 없다면 )
            if (target.transform.childCount == 0)
            {
                if (canSpawnSpecialZombie())
                {
                    if (mFever.IsFeverOn == false)
                    {
                        ++mSpecialSpawnCount;
                    }
                    // 특수좀비 객체를 생성하고 타겟의 자식으로 설정
                    GameObject zombie = Instantiate(specialZombieObject, target.transform) as GameObject;
                    zombie.transform.SetParent(target.transform);
                }
                else
                {
                    if (mFever.IsFeverOn == false)
                    {
                        ++mNormalSpawnCount;
                    }
                    // 일반좀비 객체를 생성하고 타겟의 자식으로 설정
                    GameObject zombie = Instantiate(normalZombieObject, target.transform) as GameObject;
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

        // 피버가 아닐 때에만 특수좀비 확률계산을 수행한다
        if (mFever.IsFeverOn == false)
        {
            // 실제 확률이 기대 확률보다 크면 스폰 확률을 줄이기 위해 보정값을 줄인다
            if (rateSpawnNow > specialSpawnRate)
            {
                mRateSpecialSpawnAmend = -0.05f;
            }
            // 반대로 실제 확률이 기대 확률보다 작으면 스폰 확률을 늘이기 위해 보정값을 늘인다
            else if (rateSpawnNow < specialSpawnRate)
            {
                mRateSpecialSpawnAmend = +0.05f;
            }
            // 난수를 생성하여 비교한다
            if (mRand.NextDouble() < (specialSpawnRate + mRateSpecialSpawnAmend))
            {
                return true;
            }
        }

        return false;
    }
}
