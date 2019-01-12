using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    // 잔여 탄수의 getter property
    public int BulletCur
    {
        get
        {
            return this.mBulletCur;
        }
    }
    // 최대 탄수의 getter property
    public int BulletMax
    {
        get
        {
            return this.mBulletMax;
        }
    }

    // 현재 잔여 탄수를 저장하는 변수
    private int mBulletCur = -1;
    // 최대 잔여 탄수를 저장하는 변수
    private int mBulletMax = -1;
    // 총의 대미지를 저장하는 변수
    private int mDamage = -1;
    // 현재 재장전 중인지를 저장하는 변수
    public bool mbIsReloading { get; private set; }
    // 재장전 딜레이에 도달했는지를 저장하는 타이머 변수
    private float mReloadTime = float.NaN;
    // 재장전 딜레이를 저장하는 변수
    private float mReloadDelay = float.NaN;
    // 총의 발사 딜레이를 저장하는 변수
    private float mFireDelay = float.NaN;
    // 딜레이를 검사하기 위한 변수들
    private float mShootTimeLast = float.NaN;
    private float mShootTimeCurr = float.NaN;
    private float mSceneTimer = float.NaN;

    public void Fire(GameObject obj)
    {
        this.mShootTimeCurr = this.mSceneTimer;
        // 발사 딜레이에 도달하지 않았다면 함수를 종료
        if ((this.mShootTimeCurr - this.mShootTimeLast) < this.mFireDelay)
        {
            return;
        }
        // 재장전 중일 경우 함수를 종료
        else if(mbIsReloading)
        {
            return;
        }
        // 잔여 탄수가 있을 경우에만 발사
        else if (this.mBulletCur > 0)
        {
            // Color.red 로 설정하는 것은 빨간 색만 통과시키는 마스킹을 하는 것과 같다
            Image image = obj.GetComponent<Image>();
            image.color = Color.red;

            // target 이 좀비를 가지고 있다면 대미지를 입힌다
            if (obj.transform.childCount > 0)
            {
                GameObject zombie = obj.transform.GetChild(0).gameObject;
                zombie.GetComponent<Zombie>().Hit(this.mDamage);
            }

            --this.mBulletCur;
            this.mShootTimeLast = this.mSceneTimer;
        }
    }

    public float GetDelayTimeByReload()
    {
        return (mReloadDelay - (mSceneTimer - mReloadTime));
    }

    public void Reload()
    {
        // 재장전 도중에는 재장전을 할 수 없다
        if (this.mbIsReloading == false)
        {
            this.mReloadTime = this.mSceneTimer;
            // 잔여 탄수를 최대로 채운다
            this.mBulletCur = this.mBulletMax;
            this.mbIsReloading = true;
        }
    }

    internal void Awake()
    {
        this.mBulletCur = 30;
        this.mBulletMax = 30;
        this.mDamage = 35;
        this.mbIsReloading = false;
        this.mReloadTime = 0.0f;
        this.mReloadDelay = 1.0f;
        this.mFireDelay = 0.2f;
        this.mShootTimeLast = 0.0f;
        this.mShootTimeCurr = 0.0f;
        this.mSceneTimer = 1.0f;
    }

    internal void Update()
    {
        this.UpdateSceneTimer();
        this.UpdateReloadingStatus();
    }

    private void UpdateSceneTimer()
    {
        this.mSceneTimer += Time.deltaTime;
    }

    private void UpdateReloadingStatus()
    {
        if (GetDelayTimeByReload() > 0)
        {
            mbIsReloading = true;
        }
        else
        {
            mbIsReloading = false;
        }
    }

    public void SetFever(bool isFeverOn)
    {
        // 대미지가 증가하고 재장전 및 발사 딜레이를 무시한다
        if (isFeverOn)
        {
            this.mDamage = 50;
            this.mReloadDelay = 0.0f;
            this.mFireDelay = 0.0f;
        }
        // 원래 상태로 복귀한다
        else
        {
            this.mDamage = 35;
            this.mReloadDelay = 1.0f;
            this.mFireDelay = 0.2f;
        }
    }
}
