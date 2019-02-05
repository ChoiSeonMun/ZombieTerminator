using System;
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
    public TargetManager TargetManager = null;
    public Player Player = null;
    public Fever Fever = null;
    public Button MenuButton = null;
    public GameObject MenuPanel = null;
    public GameObject StartPanel = null;
    public GameObject EndPanel = null;

    // 게임 상태에 따른 업데이트 함수를 저장하는 map
    private Dictionary<EState, Action> mUpdates = null;
    // 새로운 게임 상태로 전환될 때 실행되는 함수를 저장하는 map
    private Dictionary<EState, Action> mStarts = null;
    // 기존 게임 상태에서 벗어날 때 실행되는 함수를 저장하는 map
    private Dictionary<EState, Action> mEnds = null;
    // 내부적으로 EState 를 저장하는 변수
    private EState mStateInternal = EState.UNDEFINED;
    // EState 대입만으로 함수 호출을 자동화하기 위한 property
    private EState mState
    {
        get
        {
            return mStateInternal;
        }
        set
        {
            if (value != mStateInternal)
            {
                mEnds[mStateInternal]();
                mStateInternal = value;
                mStarts[mStateInternal]();
            }
        }
    }
    // 게임이 몇 번 종료되었는지 저장하는 변수
    // Game scene 이 여러 번 시작되고 종료되어도 그 값을 유지하기 위해 static
    private static int mGameTrial = 0;

    #region OnClick Functions

    public void OnClickMenu()
    {
        if (mState == EState.PLAYING)
        {
            mState = EState.PAUSED;
        }
    }

    public void OnClickYes()
    {
        if (mState == EState.PAUSED)
        {
            SceneManager.LoadScene("Main");
        }
    }

    public void OnClickNo()
    {
        if (mState == EState.PAUSED)
        {
            mState = EState.PLAYING;
        }
    }

    public void OnClickOk()
    {
        if(mState == EState.GAMEOVER)
        {
            SceneManager.LoadScene("Main");
        }
    }

    #endregion

    #region Public Functions

    public void EndGame()
    {
        mState = EState.GAMEOVER;
    }

    #endregion

    #region Unity Functions

    void Awake()
    {
        mUpdates = new Dictionary<EState, Action>();
        mUpdates.Add(EState.READY, updateReady);
        mUpdates.Add(EState.PLAYING, updatePlaying);
        mUpdates.Add(EState.PAUSED, updatePaused);
        mUpdates.Add(EState.GAMEOVER, updateGameover);
        mStarts = new Dictionary<EState, Action>();
        mStarts.Add(EState.READY, onStartReady);
        mStarts.Add(EState.PLAYING, onStartPlaying);
        mStarts.Add(EState.PAUSED, onStartPaused);
        mStarts.Add(EState.GAMEOVER, onStartGameover);
        mEnds = new Dictionary<EState, Action>();
        mEnds.Add(EState.UNDEFINED, onEndUndefined);
        mEnds.Add(EState.READY, onEndReady);
        mEnds.Add(EState.PLAYING, onEndPlaying);
        mEnds.Add(EState.PAUSED, onEndPaused);
        mEnds.Add(EState.GAMEOVER, onEndGameover);
        mState = EState.READY;
    }

    void Update()
    {
        mUpdates[mState]();
    }

    #endregion

    #region Scene Update

    private void updateReady()
    {
        Image img = StartPanel.GetComponent<Image>();

        // PanelStart 의 Image 를 업데이트마다 투명하게 만든다
        // 완전히 투명해졌을 경우 플레이를 시작한다
        Color newColor = img.color;
        newColor.a -= 0.01f;
        img.color = newColor;
        if (img.color.a <= 0.0f)
        {
            mState = EState.PLAYING;
        }
    }

    private void updatePlaying()
    {
        ;
    }

    private void updatePaused()
    {
        ;
    }

    private void updateGameover()
    {
        ;
    }

    #endregion

    #region Scene Change

    // NullException 을 방지하기 위해 형식상 만들어둔 함수
    private void onEndUndefined()
    {
        ;
    }

    // EState.READY 에 진입할 때 실행되는 함수
    private void onStartReady()
    {
        StartPanel.SetActive(true);
    }

    // EState.READY 에서 빠져나갈 때 실행되는 함수
    private void onEndReady()
    {
        StartPanel.SetActive(false);
    }

    // EState.PLAYING 에 진입할 때 실행되는 함수
    private void onStartPlaying()
    {
        Player.SetActive(true);
    }

    // EState.PLAYING 에서 빠져나갈 때 실행되는 함수
    private void onEndPlaying()
    {
        Player.SetActive(false);
    }

    // EState.PAUSED 에 진입할 때 실행되는 함수
    private void onStartPaused()
    {
        MenuPanel.SetActive(true);
        TargetManager.StopTarget();
    }

    private void onEndPaused()
    {
        TargetManager.ResumeTarget();
        MenuPanel.SetActive(false);
    }

    // EState.GAMEOVER 에 진입할 때 실행되는 함수
    private void onStartGameover()
    {
        // 게임이 끝나면, 게임 시도 횟수를 증가한다.
        ++mGameTrial;
        // 게임을 5번 플레이 했다면, 광고를 시청하게 한다
        if (mGameTrial == 5)
        {
            Advertisement.Show();
            mGameTrial = 0;
        }

        EndPanel.SetActive(true);
    }

    // EState.GAMEOVER 에서 빠져나갈 때 실행되는 함수
    private void onEndGameover()
    {
        EndPanel.SetActive(false);
    }

    #endregion
};