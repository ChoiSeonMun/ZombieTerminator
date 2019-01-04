using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

public struct GameInfo
{
    public int life;
    public int bomb;
    public int score;
    public int bulletCur;
    public int bulletMax;
}

public class GameManager : MonoBehaviour
{
    public enum EState
    {
        UNDEFINED,
        READY,
        PLAYING,
        PAUSED,
        GAMEOVER
    };

    private GameObject mPlayer = null;
    private Player mSPlayer = null;

    private UnityEngine.Object mOZombie = null;

    private GameObject[] mTargets = null;
    private ButtonExtension[] mBTargets = null;
    private GameObject mPStart = null;
    private ButtonExtension mBStart = null;
    private ButtonExtension mBMenu = null;
    private GameObject mPDialog = null;
    private ButtonExtension mBYes = null;
    private ButtonExtension mBNo = null;
    private Text mTLife = null;
    private Text mTBullet = null;
    private GameObject mPEnd = null;
    private ButtonExtension mBOk = null;
    private ButtonExtension mBReload = null;
    private ButtonExtension mBBomb = null;
    private Text mTBomb = null;
    private Text mTScore = null;

    private Dictionary<EState, Action> mUpdates = null;
    private Dictionary<EState, Action> mStarts = null;
    private Dictionary<EState, Action> mEnds = null;
    private EState _state = EState.UNDEFINED;
    private EState mState
    {
        get
        {
            return this._state;
        }
        set
        {
            if (value != this._state)
            {
                this.mEnds[this._state]();
                this._state = value;
                this.mStarts[this._state]();
            }
        }
    }

    private float mCooldownSpawn = float.NaN;
    private float mTimerSpawn = float.NaN;
    private int mCountSpawn = -1;
    private System.Random mRand = null;

    private static int mGameTrial = 0;

    #region Public Functions

    public void RefreshUI(GameInfo gi)
    {
        this.mTLife.text = "X " + gi.life.ToString();
        this.mTBomb.text = "X " + gi.bomb.ToString();
        this.mTBullet.text = gi.bulletCur.ToString() + " / " + gi.bulletMax.ToString();
        this.mTScore.text = gi.score.ToString();
    }

    public void EndGame()
    {
        this.mState = EState.GAMEOVER;
    }

    #endregion

    #region Unity Functions

    internal void Awake()
    {
        this.mPlayer = GameObject.Find("Player");
        this.mSPlayer = this.mPlayer.GetComponent<Player>();
        this.mPlayer.SetActive(false);

        this.mOZombie = Resources.Load("Prefabs/Zombie");

        this.mTargets = GameObject.FindGameObjectsWithTag("Target");
        Array.Sort(this.mTargets, delegate (GameObject _1, GameObject _2)
        {
            return _1.name.CompareTo(_2.name);
        });
        this.mBTargets = new ButtonExtension[this.mTargets.Length];
        for(int i = 0; i < this.mBTargets.Length; ++i)
        {
            this.mBTargets[i] = this.mTargets[i].GetComponent<ButtonExtension>();
        }
        this.mPStart = GameObject.Find("Canvas/PanelStart");
        this.mBStart = GameObject.Find("Canvas/PanelStart/OK").GetComponent<ButtonExtension>();
        this.mPStart.SetActive(false);
        this.mBMenu = GameObject.Find("Canvas/PanelSub/Menu").GetComponent<ButtonExtension>();
        this.mPDialog = GameObject.Find("Canvas/PanelDialog");
        this.mBYes = GameObject.Find("Canvas/PanelDialog/YES").GetComponent<ButtonExtension>();
        this.mBNo = GameObject.Find("Canvas/PanelDialog/NO").GetComponent<ButtonExtension>();
        this.mPDialog.SetActive(false);
        this.mTLife = GameObject.Find("Canvas/PanelSub/Life/Text").GetComponent<Text>();
        this.mTBullet = GameObject.Find("Canvas/PanelSub/Bullet").GetComponent<Text>();
        this.mPEnd = GameObject.Find("Canvas/PanelEnd");
        this.mBOk = GameObject.Find("Canvas/PanelEnd/OK").GetComponent<ButtonExtension>();
        this.mPEnd.SetActive(false);
        this.mBReload = GameObject.Find("Canvas/PanelSub/Reload").GetComponent<ButtonExtension>();
        this.mBBomb = GameObject.Find("Canvas/PanelSub/Bomb").GetComponent<ButtonExtension>();
        this.mTBomb = GameObject.Find("Canvas/PanelSub/Bomb/Text").GetComponent<Text>();
        this.mTScore = GameObject.Find("Canvas/PanelMain/Score").GetComponent<Text>();

        this.mUpdates = new Dictionary<EState, Action>();
        this.mUpdates.Add(EState.READY, this.UpdateReady);
        this.mUpdates.Add(EState.PLAYING, this.UpdatePlaying);
        this.mUpdates.Add(EState.PAUSED, this.UpdatePaused);
        this.mUpdates.Add(EState.GAMEOVER, this.UpdateGameover);
        this.mStarts = new Dictionary<EState, Action>();
        this.mStarts.Add(EState.READY, this.OnStartReady);
        this.mStarts.Add(EState.PLAYING, this.OnStartPlaying);
        this.mStarts.Add(EState.PAUSED, this.OnStartPaused);
        this.mStarts.Add(EState.GAMEOVER, this.OnStartGameover);
        this.mEnds = new Dictionary<EState, Action>();
        this.mEnds.Add(EState.UNDEFINED, this.OnEndUndefined);
        this.mEnds.Add(EState.READY, this.OnEndReady);
        this.mEnds.Add(EState.PLAYING, this.OnEndPlaying);
        this.mEnds.Add(EState.PAUSED, this.OnEndPaused);
        this.mEnds.Add(EState.GAMEOVER, this.OnEndGameover);
        this.mState = EState.READY;

        this.mCooldownSpawn = 2.0f;
        this.mTimerSpawn = 0.0f;
        this.mCountSpawn = 0;
        this.mRand = new System.Random();
    }

    internal void Update()
    {
        this.mUpdates[this.mState]();
    }

    #endregion

    #region Scene Update

    private void UpdateReady()
    {
        if (this.mBStart.IsPressed)
        {
            this.mState = EState.PLAYING;
        }
    }

    private void UpdatePlaying()
    {
        if (this.mBMenu.IsPressed)
        {
            this.mState = EState.PAUSED;
        }

        if (this.mBReload.IsPressed)
        {
            this.mSPlayer.Reload();
        }
        if (this.mBBomb.IsPressed)
        {
            if(this.mSPlayer.CanBomb())
            {
                this.UseBomb();
            }
        }

        this.InputTarget();
        this.CheckSpawn();
    }

    private void InputTarget()
    {
        for (int i = 0; i < this.mBTargets.Length; ++i)
        {
            if (this.mBTargets[i].IsPressed)
            {
                this.mSPlayer.Shoot(this.mTargets[i]);
            }
            else
            {
                Image image = this.mTargets[i].GetComponent<Image>();
                image.color = Color.white;
            }
        }
    }

    private void CheckSpawn()
    {
        this.mTimerSpawn += Time.deltaTime;

        if (this.mTimerSpawn >= this.mCooldownSpawn)
        {
            this.SpawnZombie();
            this.mTimerSpawn = 0.0f;
        }
    }

    private void SpawnZombie()
    {
        // Fisher-Yates 셔플 알고리듬
        int[] order = new int[9];
        for (int i = 0; i < 9; ++i)
        {
            order[i] = i;
        }
        for (int i = 8; i >= 0; --i)
        {
            int num = this.mRand.Next(i + 1);
            int temp = order[num];
            order[num] = order[i];
            order[i] = temp;
        }

        // 무작위의 순서대로 타겟들을 순회
        for (int i = 0; i < 9; ++i)
        {
            GameObject target = this.mTargets[order[i]];

            // 타겟에 자식이 없다면 ( 좀비가 없다면 )
            if (target.transform.childCount == 0)
            {
                // 좀비 객체를 생성하고 타겟의 자식으로 설정
                GameObject zombie = Instantiate(this.mOZombie, target.transform) as GameObject;
                zombie.transform.SetParent(target.transform);

                // 특수좀비 스폰카운터에 도달했을 경우
                if (this.mCountSpawn == 3)
                {
                    this.mCountSpawn = 0;

                    // 일반좀비와의 구분을 위해 이미지 변경 및 특수타입 설정
                    Image image = zombie.transform.GetComponent<Image>();
                    image.color = Color.blue;
                    zombie.GetComponent<Zombie>().SetType(Zombie.EType.SPECIAL);
                }

                ++this.mCountSpawn;

                // 좀비를 스폰했으므로 함수 종료
                break;
            }
        }
    }

    private void UseBomb()
    {
        foreach (GameObject target in this.mTargets)
        {
            if (target.transform.childCount > 0)
            {
                Destroy(target.gameObject.transform.GetChild(0).gameObject);
            }
        }
    }

    private void UpdatePaused()
    {
        if (this.mBYes.IsPressed)
        {
            SceneManager.LoadScene("Main");
        }
        if (this.mBNo.IsPressed)
        {
            this.mState = EState.PLAYING;
        }
    }

    private void UpdateGameover()
    {
        if (this.mBOk.IsPressed)
        {
            SceneManager.LoadScene("Main");
        }
    }

    #endregion

    #region Scene Change

    private void OnEndUndefined()
    {
        ;
    }

    private void OnStartReady()
    {
        this.mPStart.SetActive(true);
    }

    private void OnEndReady()
    {
        this.mPStart.SetActive(false);
    }

    private void OnStartPlaying()
    {
        this.mPlayer.SetActive(true);
    }

    private void OnEndPlaying()
    {
        this.mPlayer.SetActive(false);
    }

    private void OnStartPaused()
    {
        this.mPDialog.SetActive(true);

        this.StopTarget();
    }

    private void StopTarget()
    {
        foreach (GameObject target in this.mTargets)
        {
            if (target.transform.childCount > 0)
            {
                GameObject zombie = target.transform.GetChild(0).gameObject;
                zombie.SetActive(false);
            }
        }
    }

    private void OnEndPaused()
    {
        this.ResumeTarget();

        this.mPDialog.SetActive(false);
    }

    private void ResumeTarget()
    {
        foreach (GameObject target in this.mTargets)
        {
            if (target.transform.childCount > 0)
            {
                GameObject zombie = target.transform.GetChild(0).gameObject;
                zombie.SetActive(true);
            }
        }
    }

    private void OnStartGameover()
    {
        // 게임이 끝나면, 게임 시도 횟수를 증가한다.
        ++mGameTrial;
        // 게임을 5번 플레이 했다면, 광고를 시청하게 한다
        if (mGameTrial == 5)
        {
            Advertisement.Show();
            mGameTrial = 0;
        }

        this.mPEnd.SetActive(true);
    }

    private void OnEndGameover()
    {
        this.mPEnd.SetActive(false);
    }

    #endregion
}