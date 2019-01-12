using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // GameManager 에 접근하기 위한 변수
    private GameManager mManager = null;
    // Gun 에 접근하기 위한 변수
    private Gun mGun = null;
    // Fever 에 접근하기 위한 변수
    private Fever mFever = null;
    // 플레이어가 가지는 생명력, 폭탄 개수 그리고 점수를 저장하는 변수들
    private int mLife = -1;
    private int mBomb = -1;
    private int mScore = -1;

    #region Public Functions

    // Gun 으로 하여금 target 을 쏘도록 명령한다
    public void Shoot(GameObject obj)
    {
        this.mGun.Fire(obj);
    }

    // Gun 으로 하여금 재장전을 하도록 명령한다
    public void Reload()
    {
        this.mGun.Reload();
    }

    // 자신이 폭탄을 사용할 수 있다면, 폭탄 개수를 감소시킨 뒤 true 반환
    public bool CanBomb()
    {
        bool retVal = this.mBomb > 0;
        this.mBomb -= 1;
        return retVal;
    }

    public void GainBomb()
    {
        this.mBomb += 1;
    }

    public void GainScore(int score)
    {
        this.mScore += score;
    }

    // 자신의 생명력을 감소시키고, 만약 이것이 0 에 도달했다면 게임을 종료시킨다
    // 데미지를 입는다면 fever count를 0로 만든다
    public void Hit()
    {
        this.mLife -= 1;
        this.mFever.ResetFeverCount();
    }

    #endregion

    internal void Awake()
    {
        this.mManager = GameObject.Find("Manager").GetComponent<GameManager>();
        this.mGun = this.gameObject.GetComponent<Gun>();
        this.mFever = GameObject.Find("Player").GetComponent<Fever>();

        this.mLife = 3;
        this.mBomb = 3;
        this.mScore = 0;
    }

    // 매 업데이트마다 UI 에 필요한 정보를 전달한다
    internal void Update()
    {
        GameInfo gi;
        gi.life = this.mLife;
        gi.bomb = this.mBomb;
        gi.score = this.mScore;
        gi.bulletCur = this.mGun.BulletCur;
        gi.bulletMax = this.mGun.BulletMax;
        gi.bReloading = mGun.mbIsReloading;
        gi.delayTimeByReload = mGun.GetDelayTimeByReload();
        gi.feverGague = mFever.FeverCount;
        gi.bFeverIsOn = mFever.IsFeverOn;
        this.mManager.RefreshUI(gi);

        if (mLife <= 0)
        {
            mManager.EndGame();
        }
    }
}
