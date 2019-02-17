﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

// Game scene 을 총괄하는 클래스
// 담당 업무는 UI 관리, 아이템 기능 그리고 Spawn 기능
// 그 외 업무는 다른 클래스에게 맡긴다
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
    public Player player = null;
    public Fever fever = null;
    public Button menuButton = null;
    public GameObject menuPanel = null;
    public GameObject startPanel = null;
    public GameObject endPanel = null;

    private EState mState = EState.UNDEFINED;
    private event EventHandler<StateArgs> mStarts = null;
    private event EventHandler<StateArgs> mUpdates = null;
    private event EventHandler<StateArgs> mEnds = null;
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

    private SoundManager mSoundManager = null;

    #region Public Functions

    public void ChangeToPaused()
    {
        if (mState == EState.PLAYING)
        {
            changeState(EState.PAUSED);
        }
    }

    public void LoadMain()
    {
        if ((mState == EState.PAUSED) || (mState == EState.GAMEOVER))
        {
            SceneManager.LoadScene("Main");
        }
    }

    public void ChangeToPlaying()
    {
        if (mState == EState.PAUSED)
        {
            changeState(EState.PLAYING);
        }
    }

    public void EndGame()
    {
        changeState(EState.GAMEOVER);
    }

    #endregion

    #region Unity Functions

    void Awake()
    {
        mStarts = onStarts;
        mUpdates = onUpdates;
        mEnds = onEnds;
        changeState(EState.READY);

        mSoundManager = SoundManager.Instance;
        mSoundManager.PlayLoop("in-game-normal");
    }

    void Update()
    {
        mUpdates(this, new StateArgs(mState));
    }
    void OnDestroy()
    {
        // 다른 scene으로 전환할 때 BGM을 교체하기 위함
        if (mSoundManager != null)
        {
            mSoundManager.PlayLoop("out-game-normal");
        }
    }

    #endregion

    #region Scene Change

    private void changeState(EState state)
    {
        mEnds(this, new StateArgs(mState));
        mState = state;
        mStarts(this, new StateArgs(mState));
    }

    private void onStarts(object sender, StateArgs args)
    {
        switch(args.State)
        {
            case EState.UNDEFINED:
                break;

            case EState.READY:
                startPanel.SetActive(true);
                break;

            case EState.PLAYING:
                mSoundManager.PlayOneShot("game-start");
                player.SetActive(true);
                break;

            case EState.PAUSED:
                menuPanel.SetActive(true);
                eventManager.pauseEvent.Invoke();
                break;

            case EState.GAMEOVER:
                mSoundManager.PlayOneShot("game-over");
                // 게임이 끝나면, 게임 시도 횟수를 증가한다.
                ++mGameTrial;
                // 게임을 5번 플레이 했다면, 광고를 시청하게 한다
                if (mGameTrial == 5)
                {
                    Advertisement.Show();
                    mGameTrial = 0;
                }
                endPanel.SetActive(true);
                break;
        }
    }

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
                newColor.a -= (Time.deltaTime / 4.0f);
                img.color = newColor;
                if (img.color.a <= 0.0f)
                {
                    changeState(EState.PLAYING);
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

    private void onEnds(object sender, StateArgs args)
    {
        switch (args.State)
        {
            case EState.UNDEFINED:
                break;

            case EState.READY:
                startPanel.SetActive(false);
                break;

            case EState.PLAYING:
                player.SetActive(false);
                break;

            case EState.PAUSED:
                eventManager.resumeEvent.Invoke();
                menuPanel.SetActive(false);
                break;

            case EState.GAMEOVER:
                endPanel.SetActive(false);
                break;
        }
    }

    #endregion
};