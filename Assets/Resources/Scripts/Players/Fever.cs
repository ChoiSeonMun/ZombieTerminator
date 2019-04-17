using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Threading.Tasks;

public class Fever : MonoBehaviour
{
    public EventManager eventManager = null;
    // 배경 이미지와 스프라이트 리소스
    public Image backgroundPanel = null;
    public Sprite normalBackground = null;
    public Sprite feverBackground = null;
    // 피버 리소스들
    public GameObject feverPanel = null;
    // fever의 최대치를 기록하는 상수
    public const int MAX_FEVER_COUNT = 20;
    // fever 상태의 On/Off를 기록하는 변수
    public bool IsFeverOn { get; private set; }
    // 처치한 좀비 수를 기록하는 변수
    public int FeverCount { get; private set; }

    private SoundManager mSoundManager = null;

    // fever가 시작된 시간을 기록하는 변수
    private float mFeverTimeCurr = float.NaN;
    // fever 객체의 시간을 기록하는 변수
    private float mFeverTimer = float.NaN;
    // fever 상태의 지속시간을 기록하는 변수
    private float mFeverBuffTime = float.NaN;
    private bool mbFeverUpdate = false;
    private bool mbFeverPause = false;

    public void GainFeverCount()
    {
        // 피버 카운트를 증가시킨다. 좀비를 죽였을 때에 발동한다
        if (IsFeverOn == false)
        {
            FeverCount++;
        }
    }

    public void ResetFeverCount()
    {
        // 피버 카운트를 초기화한다. 좀비에게 데미지를 입었을 때에 발동한다
        FeverCount = 0;
    }

    public void SetFeverOn()
    {
        // 피버 리소스 변경
        backgroundPanel.sprite = feverBackground;
        feverPanel.transform.GetChild(1).gameObject.SetActive(false);
        feverPanel.transform.GetChild(2).gameObject.SetActive(true);
        feverPanel.transform.GetChild(3).gameObject.SetActive(true);

        // fever를 활성화 시키고, fever 시작 시간을 기록한다
        mSoundManager.PlayLoop("in-game-fever");
        mbFeverUpdate = true;
        mFeverTimeCurr = mFeverTimer;
        eventManager.feverOnEvent.Invoke();
    }

    public void SetFeverOff()
    {
        // 피버 리소스 복구
        backgroundPanel.sprite = normalBackground;
        feverPanel.transform.GetChild(1).gameObject.SetActive(true);
        feverPanel.transform.GetChild(2).gameObject.SetActive(false);
        feverPanel.transform.GetChild(3).gameObject.SetActive(false);

        // fever를 비활성화 시킨다
        eventManager.feverOffEvent.Invoke();
        mbFeverUpdate = true;
        mSoundManager.PlayLoop("in-game-normal");
    }

    public void PauseFever() {
        mbFeverPause = true;
    }

    public void ResumeFever() {
        mbFeverPause = false;
    }

    void Awake()
    {
        FeverCount = 0;

        mFeverTimeCurr = 0.0f;
        mFeverTimer = 0.0f;
        mFeverBuffTime = 15.0f;
    }

    void Start()
    {
        mSoundManager = SoundManager.Instance;
    }

    void Update()
    {
        // fever객체의 타이머의 시간을 증가시킨다
        if (!mbFeverPause)
        {
            mFeverTimer = mFeverTimer + Time.deltaTime;
        }
        // 피버 카운트가 일정 수치 이상이면 fever를 킨다
        if (FeverCount >= MAX_FEVER_COUNT)
        {
            FeverCount = 0;
            SetFeverOn();
        }

        updateFeverGuage();
    }

    private void LateUpdate()
    {
        if(mbFeverUpdate)
        {
            mbFeverUpdate = false;
            IsFeverOn = !IsFeverOn;
        }
    }

    private void updateFeverGuage()
    {
        // (현재 시간-피버 시작 시간)이 피버 지속 시간보다 커지면 fever를 끈다
        if (IsFeverOn)
        {
            float timeLapsed = mFeverTimer - mFeverTimeCurr;

            if (timeLapsed > mFeverBuffTime)
            {
                SetFeverOff();
            }

            // 게이지 바 업데이트
            RectTransform rect = feverPanel.transform.GetChild(2).GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2((1.0f - timeLapsed / mFeverBuffTime) * 950, 39);
        }
        else
        {
            // 게이지 바 업데이트
            RectTransform rect = feverPanel.transform.GetChild(1).GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(((float)(FeverCount) / (float)(Fever.MAX_FEVER_COUNT)) * 950, 39);
        }
    }
}