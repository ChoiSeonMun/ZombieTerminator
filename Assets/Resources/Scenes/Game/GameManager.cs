using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

// 화면에 출력해야할 정보들을 전달하기 위해 사용할 구조체
public struct GameInfo
{
    public int Life;
    public int Bomb;
    public int Score;
    public int BulletCur;
    public int BulletMax;
    public float DelayTimeByReload;
    public bool IsReloading;
    public int FeverGague;
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

    // 플레이어 스크립트
    private Player mPlayerScript = null;
    // Fever Script
    private Fever mFever = null;
    // Panel 의 GameObject
    private GameObject mPanel = null;
    // EState.PLAYING 에서 클릭할 수 있는 메뉴 버튼
    private ButtonExtension mMenuButton = null;
    // 플레이어 체력을 출력하기 위한 Text
    private Text mLifeText = null;
    // 잔탄수를 출력하기 위한 Text
    private Text mBulletText = null;
    // EState.PLAYING 에서 클릭할 수 있는 장전 버튼
    private ButtonExtension mReloadButton = null;
    // EState.PLAYING 에서 클릭할 수 있는 폭탄 버튼
    private ButtonExtension mBombButton = null;
    // 현재 점수를 출력하기 위한 Text
    private Text mScoreText = null;
    // 장전으로 인한 총기 사용 불가 딜레이를 표시하는 Text
    private Text mDelayTimeByReloadText = null;

    // Instantiate() 에서 사용하기 위한 좀비 Prefab
    private UnityEngine.Object mNormalZombieObject = null;
    private UnityEngine.Object mSpecialZombieObject = null;
    private GameObject[] mZombies = null;
    private ButtonExtension[] mZombieButtonExtensions = null;
    // 폭탄 애니메이션 Prefab
    private UnityEngine.Object mBombAnimationObject = null;
    // 시작시 띄우는 panel 의 Prefab
    private UnityEngine.Object mStartPanelObject = null;
    private GameObject mStartPanel = null;
    // 메뉴 버튼을 눌렀을 때 띄우는 panel 의 Prefab
    private UnityEngine.Object mMenuPanelObject = null;
    private GameObject mMenuPanel = null;
    // 게임 종료시 띄우는 panel 의 Prefab
    private UnityEngine.Object mEndPanelObject = null;
    private GameObject mEndPanel = null;

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

    private int mLevel;
    // 레벨 업을 위한 시간 경과를 저장하는 변수
    private float mLevelTimer = float.NaN;
    // 다음 레벨 업까지의 시간 간격을 저장하는 변수
    private float mLevelInterval = float.NaN;

    // Spawn Cooldown 시간값을 저장하는 변수
    private float mCooldownSpawn = float.NaN;
    // Fever 현재 쿨다운을 임시적으로 기록할 변수
    private float mCooldownSpawnTemp = float.NaN;
    // 이전 Spawn 으로부터 얼마나 시간이 지났는지를 저장하는 변수
    private float mTimerSpawn = float.NaN;
    // 특수 좀비를 Spawn할 확률을 조정하기 위해 일반 좀비와 특수 좀비의 스폰 수를 기록하는 변수들
    private int mCountNormalSpawn = -1;
    private int mCountSpecialSpawn = -1;
    // 특수 좀비를 Spawn할 확률
    private float mRateSpecialSpawn = float.NaN;
    // 특수 좀비의 Spawn 확률을 보정하는 변수
    private float mAmendRateSpecialSpawn = float.NaN;
    // 난수 생성을 위한 Random 객체
    private System.Random mRand = null;
    // 게임이 몇 번 종료되었는지 저장하는 변수
    // Game scene 이 여러 번 시작되고 종료되어도 그 값을 유지하기 위해 static
    private static int mGameTrial = 0;

    #region Public Functions

    // 게임 정보를 구조체로 받아와 갱신
    public void RefreshUI(GameInfo gi)
    {
        mLifeText.text = "X " + gi.Life.ToString();

        Text bombText = mBombButton.transform.GetChild(2).GetComponent<Text>();
        bombText.text = "X " + gi.Bomb.ToString();
        // 폭탄의 개수에 따라 폭탄 버튼의 리소스를 결정한다
        mBombButton.transform.GetChild(1).gameObject.SetActive(mPlayerScript.Bomb > 0);

        mBulletText.text = gi.BulletCur.ToString() + " / " + gi.BulletMax.ToString();

        mScoreText.text = gi.Score.ToString();

        // 장전 중일 경우 잔여시간을 갱신한다
        if (gi.IsReloading)
        {
            String delayTimeByReloadString = gi.DelayTimeByReload.ToString();
            // 소수점 아래 두 숫자까지 표시
            mDelayTimeByReloadText.text = delayTimeByReloadString.Substring(0, delayTimeByReloadString.IndexOf('.') + 2) + " Sec";
        }
        else
        {
            // 장전 중이 아니면 표시하지 않는다
            mDelayTimeByReloadText.text = " ";
        }
    }

    public void EndGame()
    {
        mState = EState.GAMEOVER;
    }

    public void SetFever(bool isFeverOn)
    {
        if (isFeverOn)
        {
            mCooldownSpawnTemp = mCooldownSpawn;
            mCooldownSpawn = 0.1f;
        }
        else
        {
            mCooldownSpawn = mCooldownSpawnTemp;
            foreach (GameObject target in mZombies)
            {
                if (target.transform.childCount > 0)
                {
                    Destroy(target.gameObject.transform.GetChild(0).gameObject);
                }
            }
        }
    }

    #endregion

    #region Unity Functions

    void Awake()
    {
        mPlayerScript = GameObject.Find("Player").GetComponent<Player>();
        mFever = GameObject.Find("Player").GetComponent<Fever>();
        mPanel = GameObject.Find("Canvas/Panel");
        mMenuButton = GameObject.Find("Canvas/Panel/Menu").GetComponent<ButtonExtension>();
        mLifeText = GameObject.Find("Canvas/Panel/Life/Text").GetComponent<Text>();
        mBulletText = GameObject.Find("Canvas/Panel/Bullet").GetComponent<Text>();
        mReloadButton = GameObject.Find("Canvas/Panel/Reload").GetComponent<ButtonExtension>();
        mBombButton = GameObject.Find("Canvas/Panel/Bomb").GetComponent<ButtonExtension>();
        mScoreText = GameObject.Find("Canvas/Panel/Score").GetComponent<Text>();
        mDelayTimeByReloadText = GameObject.Find("Canvas/Panel/Reload/DelayByReload/Text").GetComponent<Text>();

        mNormalZombieObject = Resources.Load("Prefabs/NormalZombie");
        mSpecialZombieObject = Resources.Load("Prefabs/SpecialZombie");
        // T0 부터 T8 까지 순서대로 정렬하기 위한 Sort
        mZombies = GameObject.FindGameObjectsWithTag("Target");
        Array.Sort(mZombies, delegate (GameObject _1, GameObject _2)
        {
            return _1.name.CompareTo(_2.name);
        });
        mZombieButtonExtensions = new ButtonExtension[mZombies.Length];
        // 각 target 마다의 ButtonExtension 을 대응시켜준다
        for (int i = 0; i < mZombieButtonExtensions.Length; ++i)
        {
            mZombieButtonExtensions[i] = mZombies[i].GetComponent<ButtonExtension>();
        }
        mBombAnimationObject = Resources.Load("Prefabs/BombAnimation");
        mStartPanelObject = Resources.Load("Prefabs/StartPanel");
        mStartPanel = Instantiate(mStartPanelObject, mPanel.transform) as GameObject;
        mStartPanel.SetActive(false);
        mMenuPanelObject = Resources.Load("Prefabs/MenuPanel");
        mMenuPanel = Instantiate(mMenuPanelObject, mPanel.transform) as GameObject;
        mMenuPanel.SetActive(false);
        mEndPanelObject = Resources.Load("Prefabs/EndPanel");
        mEndPanel = Instantiate(mEndPanelObject, mPanel.transform) as GameObject;
        mEndPanel.SetActive(false);

        mUpdates = new Dictionary<EState, Action>();
        mUpdates.Add(EState.READY, UpdateReady);
        mUpdates.Add(EState.PLAYING, UpdatePlaying);
        mUpdates.Add(EState.PAUSED, UpdatePaused);
        mUpdates.Add(EState.GAMEOVER, UpdateGameover);
        mStarts = new Dictionary<EState, Action>();
        mStarts.Add(EState.READY, OnStartReady);
        mStarts.Add(EState.PLAYING, OnStartPlaying);
        mStarts.Add(EState.PAUSED, OnStartPaused);
        mStarts.Add(EState.GAMEOVER, OnStartGameover);
        mEnds = new Dictionary<EState, Action>();
        mEnds.Add(EState.UNDEFINED, OnEndUndefined);
        mEnds.Add(EState.READY, OnEndReady);
        mEnds.Add(EState.PLAYING, OnEndPlaying);
        mEnds.Add(EState.PAUSED, OnEndPaused);
        mEnds.Add(EState.GAMEOVER, OnEndGameover);
        mState = EState.READY;

        mCooldownSpawn = 2.0f;
        mCooldownSpawnTemp = 0.0f;
        mTimerSpawn = 0.0f;
        mCountNormalSpawn = 0;
        mCountSpecialSpawn = 0;
        mRand = new System.Random();

        mLevel = 0;
        mLevelTimer = 0.0f;
        mLevelInterval = 10.0f;

        mRateSpecialSpawn = 0.15f;
        mAmendRateSpecialSpawn = 0.0f;
    }

    void Update()
    {
        mUpdates[mState]();
    }

    #endregion

    #region Scene Update

    private void UpdateReady()
    {
        Image img = mStartPanel.GetComponent<Image>();

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

    private void UpdatePlaying()
    {
        if (mMenuButton.IsReleased)
        {
            mState = EState.PAUSED;
        }
        if (mReloadButton.IsReleased)
        {
            mPlayerScript.Reload();
        }
        // 폭탄 버튼이 눌렸고, 폭탄을 사용할 수 있다면 폭탄 기능을 수행한다
        if (mBombButton.IsReleased)
        {
            if(mPlayerScript.Bomb > 0)
            {
                UseBomb();
            }
        }
        // 각 target 에 대해 입력이 있는지 검사한다
        InputTarget();
        // Spawn 을 해야하는지 검사한다
        CheckSpawn();
        // Fever 상태가 아닐 때 레벨업을 체크한다
        if(!mFever.IsFeverOn) CheckLevelUp();
    }

    private void InputTarget()
    {
        for (int i = 0; i < mZombieButtonExtensions.Length; ++i)
        {
            // target 이 눌렸다면, 해당 target 에 대해 사격한다
            if (mZombieButtonExtensions[i].IsReleased)
            {
                mPlayerScript.Shoot(mZombies[i]);
            }
            // 그렇지않다면, 해당 target 의 색을 원래대로 돌린다
            // Color.white 로 설정하는 것은 모든 색을 통과시키는 마스킹을 하는 것과 같다
            else
            {
                Image image = mZombies[i].GetComponent<Image>();
                image.color = Color.white;
            }
        }
    }

    private void CheckSpawn()
    {
        // 타이머를 증가시키고, 지정했던 cooldown 에 도달했을 때 좀비를 Spawn 한다
        mTimerSpawn += Time.deltaTime;
        if (mTimerSpawn >= mCooldownSpawn)
        {
            SpawnZombie();
            mTimerSpawn = 0.0f;
        }
    }

    private void CheckLevelUp()
    {
         mLevelTimer += Time.deltaTime;
         if (mLevelTimer > mLevelInterval)
         {
            LevelUp();
            mLevelTimer = 0.0f;
         }
    }

    private void LevelUp()
    {
        mLevel++;
        mCooldownSpawn *= 0.9f;
        mLevelInterval *= 0.95f;
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
            int num = mRand.Next(i + 1);
            int temp = order[num];
            order[num] = order[i];
            order[i] = temp;
        }

        // 무작위의 순서대로 타겟들을 순회
        for (int i = 0; i < 9; ++i)
        {
            GameObject target = mZombies[order[i]];

            // 타겟에 자식이 없다면 ( 좀비가 없다면 )
            if (target.transform.childCount == 0)
            {
                // 특수좀비 스폰카운터에 도달했을 경우
                if (DecideSpawnSpecial())
                {
                    ++mCountSpecialSpawn;

                    // 특수좀비 객체를 생성하고 타겟의 자식으로 설정
                    GameObject zombie = Instantiate(mSpecialZombieObject, target.transform) as GameObject;
                    zombie.transform.SetParent(target.transform);

                    // 일반좀비와의 구분을 위해 특수타입 설정
                    zombie.GetComponent<Zombie>().SetType(Zombie.EType.SPECIAL);
                    // 레벨에 따라 능력치 조정
                    zombie.GetComponent<Zombie>().SetStatus(mLevel);
                }
                else
                {
                    ++mCountNormalSpawn;

                    // 일반좀비 객체를 생성하고 타겟의 자식으로 설정
                    GameObject zombie = Instantiate(mNormalZombieObject, target.transform) as GameObject;
                    zombie.transform.SetParent(target.transform);
                    // 레벨에 따라 능력치 조정
                    zombie.GetComponent<Zombie>().SetStatus(mLevel);
                }

                // 좀비를 스폰했으므로 함수 종료
                break;
            }
        }
    }

    private bool DecideSpawnSpecial()
    {
        float rateSpawnNow = (float)mCountSpecialSpawn / (float)(mCountSpecialSpawn + mCountNormalSpawn);

        // 실제 확률이 기대 확률보다 크면 스폰 확률을 줄이기 위해 보정값을 줄인다
        if (rateSpawnNow > mRateSpecialSpawn)
        {
            mAmendRateSpecialSpawn -= 0.1f;
        }
        // 반대로 실제 확률이 기대 확률보다 작으면 스폰 확률을 늘이기 위해 보정값을 늘인다
        else if (rateSpawnNow < mRateSpecialSpawn)
        {
            mAmendRateSpecialSpawn += 0.1f;
        }

        // 난수를 생성하여 스폰할 것인지 정한다
        if (mRand.NextDouble() < (mRateSpecialSpawn + mAmendRateSpecialSpawn))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void UseBomb()
    {
        // 플레리어의 폭탄 개수를 줄인다
        mPlayerScript.LoseBomb();

        // 폭탄 애니메이션을 생성하고, 이 애니메이션을 0.25 초 뒤에 삭제한다
        GameObject obj = Instantiate(mBombAnimationObject, mPanel.transform) as GameObject;
        obj.transform.SetParent(mPanel.transform);
        Destroy(obj, 0.25f);
        // 좀비를 가지고 있는 target 을 순회하면서, 각 좀비를 죽인다
        foreach (GameObject target in mZombies)
        {
            if (target.transform.childCount > 0)
            {
                Destroy(target.gameObject.transform.GetChild(0).gameObject);
            }
        }
    }

    private void UpdatePaused()
    {
        ButtonExtension yes = mMenuPanel.transform.GetChild(1).gameObject.GetComponent<ButtonExtension>();
        ButtonExtension no = mMenuPanel.transform.GetChild(2).gameObject.GetComponent<ButtonExtension>();

        if (yes.IsReleased)
        {
            SceneManager.LoadScene("Main");
        }
        if (no.IsReleased)
        {
            mState = EState.PLAYING;
        }
    }

    private void UpdateGameover()
    {
        ButtonExtension ok = mEndPanel.transform.GetChild(1).gameObject.GetComponent<ButtonExtension>();

        if (ok.IsReleased)
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
        mStartPanel.SetActive(true);
    }

    // EState.READY 에서 빠져나갈 때 실행되는 함수
    private void OnEndReady()
    {
        mStartPanel.SetActive(false);
    }

    // EState.PLAYING 에 진입할 때 실행되는 함수
    private void OnStartPlaying()
    {
        mPlayerScript.SetActive(true);
    }

    // EState.PLAYING 에서 빠져나갈 때 실행되는 함수
    private void OnEndPlaying()
    {
        mPlayerScript.SetActive(false);
    }

    // EState.PAUSED 에 진입할 때 실행되는 함수
    private void OnStartPaused()
    {
        mMenuButton.transform.GetChild(1).gameObject.SetActive(false);
        mMenuPanel.SetActive(true);

        StopTarget();
    }

    // 좀비들이 업데이트되는 것을 막기 위해 모두 비활성화
    private void StopTarget()
    {
        foreach (GameObject target in mZombies)
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
        ResumeTarget();

        mMenuPanel.SetActive(false);
        mMenuButton.transform.GetChild(1).gameObject.SetActive(true);
    }

    // 좀비들이 다시 업데이트되도록 모두 활성화
    private void ResumeTarget()
    {
        foreach (GameObject target in mZombies)
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

        mEndPanel.SetActive(true);
    }

    // EState.GAMEOVER 에서 빠져나갈 때 실행되는 함수
    private void OnEndGameover()
    {
        mEndPanel.SetActive(false);
    }

    #endregion
}