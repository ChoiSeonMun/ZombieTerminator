using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public GameManager GameManager = null;
    public Gun Gun = null;
    public Fever Fever = null;

    public Text ScoreText = null;
    public Text LifeText = null;
    private int mScore = -1;
    private int mLife = -1;

    private SoundManager mSoundManager = null;

    #region Public Functions

    // Gun 으로 하여금 target 을 쏘도록 명령한다
    public void Shoot(GameObject obj)
    {
        Gun.Fire(obj);
    }

    public void GainScore(int score)
    {
        mScore += score;
    }

    public void SetActive(bool isActive)
    {
        if(isActive)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    // 자신의 생명력을 감소시키고, 만약 이것이 0 에 도달했다면 게임을 종료시킨다
    // 데미지를 입는다면 fever count를 0로 만든다
    // 피버 상태에서는 데미지를 입지 않는다
    public void Hit()
    {
        if (Fever.IsFeverOn == false)
        {
            mSoundManager.PlayOneShot("hit");

            mLife -= 1;
        }

        Fever.ResetFeverCount();
    }

    #endregion

    void Awake()
    {
        mLife = 3;
        mScore = 0;

        mSoundManager = SoundManager.Instance;
    }

    void Update()
    {
        ScoreText.text = mScore.ToString();
        LifeText.text = "X " + mLife.ToString();
        if(mLife <= 0)
        {
            GameManager.EndGame();
        }
    }
}
