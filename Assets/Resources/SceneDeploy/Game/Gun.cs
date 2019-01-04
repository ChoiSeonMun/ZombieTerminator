using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    public int BulletCur
    {
        get
        {
            return this.mBulletCur;
        }
    }
    public int BulletMax
    {
        get
        {
            return this.mBulletMax;
        }
    }

    // gun system 을 위한 변수들
    private int mBulletCur = -1;
    private int mBulletMax = -1;
    private int mDamage = -1;
    private bool mbReloading = false;
    private float mReloadTime = float.NaN;
    private float mReloadDelay = float.NaN;
    private float mFireDelay = float.NaN;
    private float mShootTimeLast = float.NaN;
    private float mShootTimeCurr = float.NaN;
    private float mSceneTimer = float.NaN;

    public void Fire(GameObject obj)
    {
        this.mShootTimeCurr = this.mSceneTimer;

        if ((this.mShootTimeCurr - this.mShootTimeLast) < this.mFireDelay)
        {
            return;
        }
        else if ((this.mSceneTimer - this.mReloadTime) < this.mReloadDelay)
        {
            return;
        }
        else if (this.mBulletCur > 0)
        {
            Image image = obj.GetComponent<Image>();
            image.color = Color.red;

            if (obj.transform.childCount > 0)
            {
                GameObject zombie = obj.transform.GetChild(0).gameObject;
                zombie.GetComponent<Zombie>().getDamaged(this.mDamage);
            }

            --this.mBulletCur;

            this.mShootTimeLast = this.mSceneTimer;
        }
    }

    public void Reload()
    {
        // 타임 딜레이 기능 추가 할 것.
        // 리로드 중에는 리로드가 되어서는 안됨.
        if (this.mbReloading == false)
        {
            this.mReloadTime = this.mSceneTimer;
            this.mBulletCur = this.mBulletMax;
        }
    }

    internal void Awake()
    {
        this.mBulletCur = 30;
        this.mBulletMax = 30;
        this.mDamage = 35;
        this.mbReloading = false;
        this.mReloadTime = 1.5f;
        this.mReloadDelay = 1.0f;
        this.mFireDelay = 0.1f;
        this.mShootTimeLast = 0.0f;
        this.mShootTimeCurr = 0.0f;
        this.mSceneTimer = 0.0f;
    }

    internal void Update()
    {
        this.UpdateSceneTimer();
    }

    private void UpdateSceneTimer()
    {
        this.mSceneTimer += Time.deltaTime;
    }
}
