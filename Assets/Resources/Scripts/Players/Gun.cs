using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    public EventManager eventManager = null;

    public Text bulletText = null;
    public Text reloadDelayText = null;
    public Button reloadButton = null;
    // 잔여 탄수의 getter property
    public int BulletCur
    {
        get
        {
            return mBulletCur;
        }
    }
    // 최대 탄수의 getter property
    public int BulletMax
    {
        get
        {
            return mBulletMax;
        }
    }
    // 현재 재장전 중인지를 저장하는 변수
    public bool IsReloading { get; private set; }

    private Fever mFever = null;
    private SoundManager mSoundManager = null;

    // 현재 잔여 탄수를 저장하는 변수
    private int mBulletCur = -1;
    // 최대 잔여 탄수를 저장하는 변수
    private int mBulletMax = -1;
    // 총의 대미지를 저장하는 변수
    private int mDamage = -1;
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

    public void BlockReload()
    {
        reloadButton.onClick.RemoveAllListeners();
    }

    public void GrantReload()
    {
        reloadButton.onClick.AddListener(OnClickReload);
    }

    public void PowerUp()
    {
        mDamage = mDamage * 3;
        mReloadDelay = 0.0f;
        mFireDelay = 0.0f;
    }

    public void PowerDown()
    {
        mDamage = 35;
        mReloadDelay = 1.0f;
        mFireDelay = 0.0f;
    }

    public void OnClickReload()
    {
        if (IsReloading == false)
        {
            if (mFever.IsFeverOn == false)
            {
                mSoundManager.PlayOneShot("reload");
            }

            mReloadTime = mSceneTimer;
            // 잔여 탄수를 최대로 채운다
            mBulletCur = mBulletMax;
            IsReloading = true;
        }
    }

    public void Fire(GameObject obj)
    {
        mShootTimeCurr = mSceneTimer;
        // 발사 딜레이에 도달하지 않았다면 함수를 종료
        if ((mShootTimeCurr - mShootTimeLast) < mFireDelay)
        {
            return;
        }
        // 재장전 중일 경우 함수를 종료
        else if(IsReloading)
        {
            return;
        }
        // 잔여 탄수가 있을 경우에만 발사
        else if (mBulletCur > 0)
        {
            mSoundManager.PlayOneShot("gun-fire");

            if (obj.transform.childCount > 0)
            {
                Zombie zombie = obj.transform.GetChild(0).GetComponent<Zombie>();

                if (zombie != null)
                {
                    zombie.TakeDamage(mDamage);
                }
            }

            mBulletCur -= 1;
            mShootTimeLast = mSceneTimer;
        }
        else
        {
            mSoundManager.PlayOneShot("fire-fail");
        }
    }

    public float GetDelayTimeByReload()
    {
        return (mReloadDelay - (mSceneTimer - mReloadTime));
    }

    void Awake()
    {
        IsReloading = false;

        mFever = eventManager.fever;

        mBulletCur = 30;
        mBulletMax = 30;
        mDamage = 35;
        mReloadTime = 0.0f;
        mReloadDelay = 1.0f;
        mFireDelay = 0.15f;
        mShootTimeLast = 0.0f;
        mShootTimeCurr = 0.0f;
        mSceneTimer = 1.0f;
        // 게임이 pause 되었을 때 removeListener 가 정상적으로 작동할 수 있도록 하기 위해
        // 여기에서 OnClick 을 할당
        reloadButton.onClick.AddListener(OnClickReload);
    }

    void Start()
    {
        mSoundManager = SoundManager.Instance;
    }

    void Update()
    {
        bulletText.text = BulletCur.ToString() + "/" + BulletMax.ToString();
        if (IsReloading)
        {
            string delayTime = (mReloadDelay - (mSceneTimer - mReloadTime)).ToString();
            reloadDelayText.text = delayTime.Substring(0, delayTime.IndexOf('.') + 2) + " Sec";
        }
        else
        {
            reloadDelayText.text = "";
        }

        updateSceneTimer();
        updateReloadingStatus();
    }

    private void updateSceneTimer()
    {
        mSceneTimer += Time.deltaTime;
    }

    private void updateReloadingStatus()
    {
        if (GetDelayTimeByReload() > 0)
        {
            IsReloading = true;
        }
        else
        {
            IsReloading = false;
        }
    }
}
