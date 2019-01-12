using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

// 화면에 출력해야할 정보들을 전달하기 위해 사용할 구조체
public struct GameInfo
{
    public int life;
    public int bomb;
    public int score;
    public int bulletCur;
    public int bulletMax;
    public float delayTimeByReload;
    public bool bReloading;
    public int feverGague;
    public bool bFeverIsOn;
}

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

    // 플레이어 객체와 플레이어 스크립트
    private GameObject mPlayer = null;
    private Player mSPlayer = null;
    // Instantiate() 에서 사용하기 위한 좀비 Prefab
    private UnityEngine.Object mOZombie = null;
    // target 객체들과 각 target 의 ButtonExtension
    private GameObject[] mTargets = null;
    private ButtonExtension[] mBTargets = null;
    // PanelStart 객체 및 Image 컴포넌트
    private GameObject mPStart = null;
    private Image mStartImage = null;
    // EState.PLAYING 에서 클릭할 수 있는 메뉴 버튼
    private ButtonExtension mBMenu = null;
    // PanelDialog 객체 및 그 자식들의 ButtonExtension
    private GameObject mPDialog = null;
    private ButtonExtension mBYes = null;
    private ButtonExtension mBNo = null;
    // 플레이어 체력을 출력하기 위한 Text
    private Text mTLife = null;
    // 잔탄수를 출력하기 위한 Text
    private Text mTBullet = null;
    // PanelEnd 객체 및 그 자식의 ButtonExtension
    private GameObject mPEnd = null;
    private ButtonExtension mBOk = null;
    // EState.PLAYING 에서 클릭할 수 있는 장전 버튼
    private ButtonExtension mBReload = null;
    // EState.PLAYING 에서 클릭할 수 있는 폭탄 버튼
    private ButtonExtension mBBomb = null;
    // 잔여 폭탄 개수를 출력하기 위한 Text
    private Text mTBomb = null;
    // 현재 점수를 출력하기 위한 Text
    private Text mTScore = null;
    // 장전으로 인한 총기 사용 불가 딜레이를 표시하는 Text
    private Text mTDelayTimeByReload = null;
    // Fever게이지를 표시할 Image
    private Image mImageFeverGauge = null;
    // Fever 활성여부를 표시할 Text
    private Text mTFever = null;

    // 게임 상태에 따른 업데이트 함수를 저장하는 map
    private Dictionary<EState, Action> mUpdates = null;
    // 새로운 게임 상태로 전환될 때 실행되는 함수를 저장하는 map
    private Dictionary<EState, Action> mStarts = null;
    // 기존 게임 상태에서 벗어날 때 실행되는 함수를 저장하는 map
    private Dictionary<EState, Action> mEnds = null;
    // 내부적으로 EState 를 저장하는 변수
    private EState _state = EState.UNDEFINED;
    // EState 대입만으로 함수 호출을 자동화하기 위한 property
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

    // Spawn Cooldown 시간값을 저장하는 변수
    private float mCooldownSpawn = float.NaN;
    // 이전 Spawn 으로부터 얼마나 시간이 지났는지를 저장하는 변수
    private float mTimerSpawn = float.NaN;
    // 특수 좀비를 Spawn 하기 위해, 일반 좀비가 몇 번이나 Spawn 되었는지 저장하는 변수
    private int mCountSpawn = -1;
    // 난수 생성을 위한 Random 객체
    private System.Random mRand = null;
    // 게임이 몇 번 종료되었는지 저장하는 변수
    // Game scene 이 여러 번 시작되고 종료되어도 그 값을 유지하기 위해 static
    private static int mGameTrial = 0;

    #region Public Functions

    // 게임 정보를 구조체로 받아와 갱신
    public void RefreshUI(GameInfo gi)
    {
        this.mTLife.text = "X " + gi.life.ToString();
        this.mTBomb.text = "X " + gi.bomb.ToString();
        this.mTBullet.text = gi.bulletCur.ToString() + " / " + gi.bulletMax.ToString();
        this.mTScore.text = gi.score.ToString();

        if (gi.bReloading)
        {
            String mSDelayTimeByReload;
            mSDelayTimeByReload = gi.delayTimeByReload.ToString();
            mTDelayTimeByReload.text = mSDelayTimeByReload.Substring(0, mSDelayTimeByReload.IndexOf('.') + 2) + " Sec";
        }
        else
        {
            mTDelayTimeByReload.text = " ";
        }

        if (gi.feverGague == 0)
        {
            mImageFeverGauge.rectTransform.sizeDelta = new Vector2(0, 0);
        }
        else
        {
            mImageFeverGauge.rectTransform.sizeDelta = new Vector2(((float)(gi.feverGague) / (float)(Fever.maxFeverCount)) * 920, 20);
        }

        mTFever.gameObject.SetActive(gi.bFeverIsOn);
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
        // T0 부터 T8 까지 순서대로 정렬하기 위한 Sort
        Array.Sort(this.mTargets, delegate (GameObject _1, GameObject _2)
        {
            return _1.name.CompareTo(_2.name);
        });
        this.mBTargets = new ButtonExtension[this.mTargets.Length];
        // 각 target 마다의 ButtonExtension 을 대응시켜준다
        for(int i = 0; i < this.mBTargets.Length; ++i)
        {
            this.mBTargets[i] = this.mTargets[i].GetComponent<ButtonExtension>();
        }
        this.mPStart = GameObject.Find("Canvas/PanelStart");
        this.mStartImage = this.mPStart.GetComponent<Image>();
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
        this.mTDelayTimeByReload = GameObject.Find("Canvas/PanelSub/Reload/DelayByReload/Text").GetComponent<Text>();
        this.mImageFeverGauge = GameObject.Find("Canvas/PanelSub/Fever/Image").GetComponent<Image>();
        this.mTFever = GameObject.Find("Canvas/PanelSub/Fever/Text").GetComponent<Text>();

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
        // PanelStart 의 Image 를 업데이트마다 투명하게 만든다
        // 완전히 투명해졌을 경우 플레이를 시작한다
        Color newColor = this.mStartImage.color;
        newColor.a -= 0.01f;
        mStartImage.color = newColor;
        if (mStartImage.color.a <= 0.0f)
        {
            mState = EState.PLAYING;
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
        // 폭탄 버튼이 눌렸고, 폭탄을 사용할 수 있다면 폭탄 기능을 수행한다
        if (this.mBBomb.IsPressed)
        {
            if(this.mSPlayer.CanBomb())
            {
                this.UseBomb();
            }
        }
        // 각 target 에 대해 입력이 있는지 검사한다
        this.InputTarget();
        // Spawn 을 해야하는지 검사한다
        this.CheckSpawn();
    }

    private void InputTarget()
    {
        for (int i = 0; i < this.mBTargets.Length; ++i)
        {
            // target 이 눌렸다면, 해당 target 에 대해 사격한다
            if (this.mBTargets[i].IsPressed)
            {
                this.mSPlayer.Shoot(this.mTargets[i]);
            }
            // 그렇지않다면, 해당 target 의 색을 원래대로 돌린다
            // Color.white 로 설정하는 것은 모든 색을 통과시키는 마스킹을 하는 것과 같다
            else
            {
                Image image = this.mTargets[i].GetComponent<Image>();
                image.color = Color.white;
            }
        }
    }

    private void CheckSpawn()
    {
        // 타이머를 증가시키고, 지정했던 cooldown 에 도달했을 때 좀비를 Spawn 한다
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
        // 좀비를 가지고 있는 target 을 순회하면서, 각 좀비를 죽인다
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

    // NullException 을 방지하기 위해 형식상 만들어둔 함수
    private void OnEndUndefined()
    {
        ;
    }

    // EState.READY 에 진입할 때 실행되는 함수
    private void OnStartReady()
    {
        this.mPStart.SetActive(true);
    }

    // EState.READY 에서 빠져나갈 때 실행되는 함수
    private void OnEndReady()
    {
        this.mPStart.SetActive(false);
    }

    // EState.PLAYING 에 진입할 때 실행되는 함수
    private void OnStartPlaying()
    {
        this.mPlayer.SetActive(true);
    }

    // EState.PLAYING 에서 빠져나갈 때 실행되는 함수
    private void OnEndPlaying()
    {
        this.mPlayer.SetActive(false);
    }

    // EState.PAUSED 에 진입할 때 실행되는 함수
    private void OnStartPaused()
    {
        this.mPDialog.SetActive(true);

        this.StopTarget();
    }

    // 좀비들이 업데이트되는 것을 막기 위해 모두 비활성화
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

    // EState.PAUSED 에서 빠져나갈 때 실행되는 함수
    private void OnEndPaused()
    {
        this.ResumeTarget();

        this.mPDialog.SetActive(false);
    }

    // 좀비들이 다시 업데이트되도록 모두 활성화
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

    // EState.GAMEOVER 에 진입할 때 실행되는 함수
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

    // EState.GAMEOVER 에서 빠져나갈 때 실행되는 함수
    private void OnEndGameover()
    {
        this.mPEnd.SetActive(false);
    }

    #endregion
}