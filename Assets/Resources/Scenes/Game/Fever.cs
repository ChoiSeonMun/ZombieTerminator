using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fever : MonoBehaviour
{
    // fever의 최대치를 기록하는 상수
    public const int MAX_FEVER_COUNT = 10;
    public bool IsFeverOn { get; private set; }
    public int FeverGauge { get; private set; }
    // fever 경과시간을 기록하는 변수
    private float mFeverTimer = float.NaN;
    // fever 상태의 지속시간을 저장하는 변수
    private float mFeverDuration = float.NaN;

    // Gun 시스템에 접근하기 위한 변수
    private Gun mGun = null;

    // 배경에 대한 panel 의 GameObject
    private GameObject mBackgroundPanel = null;
    // 피버 리소스에 대한 panel 의 GameObject
    private GameObject mFeverPanel = null;
    // 피버 애니메이션에 대한 Prefab
    private UnityEngine.Object mFeverAnimationObject = null;
    private GameObject mFeverAnimation = null;

    #region Public Functions

    public void GainFeverCount()
    {
        // 피버 카운트를 증가시킨다. 좀비를 죽였을 때에 발동한다
        if (IsFeverOn == false)
        {
            FeverGauge++;
        }
    }

    public void ResetFeverCount()
    {
        // 피버 카운트를 초기화한다. 좀비에게 데미지를 입었을 때에 발동한다
        FeverGauge = 0;
    }

    public void SetFeverOn()
    {
        // 피버 리소스 변경
        mBackgroundPanel.transform.GetChild(1).gameObject.SetActive(false);
        mFeverPanel.transform.GetChild(1).gameObject.SetActive(false);
        mFeverPanel.transform.GetChild(2).gameObject.SetActive(true);
        mFeverPanel.transform.GetChild(3).gameObject.SetActive(true);

        // fever를 활성화 시키고, fever 시작 시간을 기록한다
        IsFeverOn = true;
        mFeverTimer = 0.0f;
        mGun.SetFever(IsFeverOn);
    }

    public void SetFeverOff()
    {
        // 피버 리소스 복구
        mBackgroundPanel.transform.GetChild(1).gameObject.SetActive(true);
        mFeverPanel.transform.GetChild(1).gameObject.SetActive(true);
        mFeverPanel.transform.GetChild(2).gameObject.SetActive(false);
        mFeverPanel.transform.GetChild(3).gameObject.SetActive(false);

        // fever를 비활성화 시킨다
        IsFeverOn = false;
        mGun.SetFever(IsFeverOn);
    }

    #endregion

    #region Unity Function

    void Awake()
    {
        mGun = GameObject.Find("Player").GetComponent<Gun>();
        FeverGauge = 0;
        mFeverTimer = 0.0f;
        mFeverDuration = 20.0f;

        mBackgroundPanel = GameObject.Find("Canvas/Panel/Background");
        mFeverPanel = GameObject.Find("Canvas/Panel/Fever");
        mFeverPanel.transform.GetChild(2).gameObject.SetActive(false);
        mFeverPanel.transform.GetChild(3).gameObject.SetActive(false);
    }

    void Update()
    {
        // 피버 조건을 달성하면 피버 활성화
        if (FeverGauge == MAX_FEVER_COUNT)
        {
            FeverGauge = 0;
            SetFeverOn();
        }

        // 피버 지속시간이 지나면 피버 비활성화
        if (IsFeverOn)
        {
            mFeverTimer += Time.deltaTime;

            if (mFeverTimer >= mFeverDuration)
            {
                SetFeverOff();
            }

            // 경과시간에 따라 게이지를 줄인다
            RectTransform rect = mFeverPanel.transform.GetChild(2).GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2((1.0f - mFeverTimer / mFeverDuration) * 950, 39);
        }
        else
        {
            // 피버카운트에 따라 게이지를 늘린다
            RectTransform rect = mFeverPanel.transform.GetChild(1).GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(((float)(FeverGauge) / (float)(Fever.MAX_FEVER_COUNT)) * 950, 39);
        }
    }

    #endregion
}