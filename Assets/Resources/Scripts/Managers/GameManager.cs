using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

public class GameManager : MonoBehaviour
{
    // Game scene 이 가질 수 있는 상태값들
    public enum EState
    {
        UNDEFINED,
        READY,
        PLAYING,
        PAUSED,
        GAMEOVER
    };
    public EventManager eventManager = null;

    public Button menuButton = null;
    public GameObject menuPanel = null;
    public GameObject startPanel = null;
    public GameObject endPanel = null;

    private Player mPlayer = null;
    private SoundManager mSoundManager = null;

    private EState mState = EState.UNDEFINED;
    private event EventHandler<StateArgs> mUpdates = null;
    private class StateArgs : EventArgs
    {
        public StateArgs(EState state)
        {
            State = state;
        }

        public EState State = EState.UNDEFINED;
    }

    // 게임이 몇 번 종료되었는지 저장하는 변수
    // Game scene 이 여러 번 시작되고 종료되어도 그 값을 유지하기 위해 static
    private static int mGameTrial = 0;

    #region Public Functions

    public void ChangeToPaused()
    {
        if (mState == EState.PLAYING)
        {
            PauseGame();
        }
    }

    public void LoadMain()
    {
        if ((mState == EState.PAUSED) || (mState == EState.GAMEOVER))
        {
            // 게임이 끝나면, 게임 시도 횟수를 증가한다.
            ++mGameTrial;
            // 게임을 5번 플레이 했다면, 광고를 시청하게 한다
            if (mGameTrial == 5)
            {
                Advertisement.Show();
                mGameTrial = 0;
            }

            SceneManager.LoadScene("Main");
        }
    }

    public void ChangeToPlaying()
    {
        if (mState == EState.PAUSED)
        {
            ResumeGame();
        }
    }

    public void ReadyGame()
    {
        mState = EState.READY;

        startPanel.SetActive(true);
    }

    public void StartGame()
    {
        mState = EState.PLAYING;

        startPanel.SetActive(false);
        mSoundManager.PlayOneShot("game-start");
        mPlayer.SetActive(true);
    }

    public void PauseGame()
    {
        mState = EState.PAUSED;
        eventManager.pauseEvent.Invoke();

        menuPanel.SetActive(true);
    }

    public void ResumeGame()
    {
        mState = EState.PLAYING;
        eventManager.resumeEvent.Invoke();

        menuPanel.SetActive(false);
    }

    public void EndGame()
    {
        mState = EState.GAMEOVER;

        mSoundManager.PlayOneShot("game-over");
        endPanel.SetActive(true);
    }

    #endregion

    #region Unity Functions

    protected void Awake()
    {
        mPlayer = eventManager.player;
        mUpdates = onUpdates;

        ReadyGame();
    }

    protected void Start()
    {
        mSoundManager = SoundManager.Instance;
        mSoundManager.PlayLoop("in-game-normal");
    }

    protected void Update()
    {
        mUpdates(this, new StateArgs(mState));
    }

    protected void OnDestroy()
    {
        // 다른 scene으로 전환할 때 BGM을 교체하기 위함
        if (mSoundManager != null)
        {
            mSoundManager.PlayLoop("out-game-normal");
        }
    }

    #endregion

    #region Scene Change

    private void onUpdates(object sender, StateArgs args)
    {
        switch (args.State)
        {
            case EState.UNDEFINED:
                break;

            case EState.READY:
                Image img = startPanel.GetComponent<Image>();
                // PanelStart 의 Image 를 업데이트마다 투명하게 만든다
                // 완전히 투명해졌을 경우 플레이를 시작한다
                Color newColor = img.color;
                newColor.a -= (Time.deltaTime / 2.0f);
                img.color = newColor;
                if (img.color.a <= 0.0f)
                {
                    StartGame();
                }
                break;

            case EState.PLAYING:
                break;

            case EState.PAUSED:
                break;

            case EState.GAMEOVER:
                break;
        }
    }

    #endregion
};