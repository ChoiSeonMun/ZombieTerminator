using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // GameManager 스크립트에 접근하기 위한 변수
    private GameManager mManager = null;
    // Player 객체에 접근하기 위한 변수
    private GameObject mPlayer = null;
    // Gun 스크립트에 접근하기 위한 변수
    private Gun mGun = null;
    // Fever 스크립트에 접근하기 위한 변수
    private Fever mFever = null;
    // 플레이어가 가지는 생명력, 폭탄 개수 그리고 점수를 저장하는 변수들
    private int mLife = -1;
    private int mBomb = -1;
    private int mScore = -1;

    #region Public Functions

    // Gun 으로 하여금 target 을 쏘도록 명령한다
    public void Shoot(GameObject obj)
    {
        mGun.Fire(obj);
    }

    // Gun 으로 하여금 재장전을 하도록 명령한다
    public void Reload()
    {
        mGun.Reload();
    }

    // 자신이 폭탄을 사용할 수 있다면, 폭탄 개수를 감소시킨 뒤 true 반환
    public bool CanBomb()
    {
        return mBomb > 0;
    }

    public void GainBomb()
    {
        mBomb += 1;
    }

    public void LoseBomb()
    {
        mBomb -= 1;
    }

    public void GainScore(int score)
    {
        mScore += score;
    }

    public void SetActive(bool isActive)
    {
        if(isActive)
        {
            mPlayer.SetActive(true);
        }
        else
        {
            mPlayer.SetActive(false);
        }
    }

    // 자신의 생명력을 감소시키고, 만약 이것이 0 에 도달했다면 게임을 종료시킨다
    // 데미지를 입는다면 fever count를 0로 만든다
    public void Hit()
    {
        mLife -= 1;
        mFever.ResetFeverCount();
    }

    #endregion

    internal void Awake()
    {
        mManager = GameObject.Find("Manager").GetComponent<GameManager>();
        mPlayer = GameObject.Find("Player");
        mGun = this.gameObject.GetComponent<Gun>();
        mFever = GameObject.Find("Player").GetComponent<Fever>();

        mLife = 3;
        mBomb = 3;
        mScore = 0;
    }

    // 매 업데이트마다 UI 에 필요한 정보를 전달한다
    internal void Update()
    {
        GameInfo gi;
        gi.Life = mLife;
        gi.Bomb = mBomb;
        gi.Score = mScore;
        gi.BulletCur = mGun.BulletCur;
        gi.BulletMax = mGun.BulletMax;
        gi.IsReloading = mGun.IsReloading;
        gi.DelayTimeByReload = mGun.GetDelayTimeByReload();
        gi.FeverGague = mFever.FeverCount;
        gi.IsFeverOn = mFever.IsFeverOn;
        mManager.RefreshUI(gi);

        if (mLife <= 0)
        {
            mManager.EndGame();
        }
    }
}
