using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fever : MonoBehaviour
{

    // fever의 최대치를 기록하는 상수
    public const int MAX_FEVER_COUNT = 10;
    // fever 상태의 On/Off를 기록하는 변수
    public bool IsFeverOn { get; private set; }
    // 처치한 좀비 수를 기록하는 변수
    public int FeverCount { get; private set; }
    // fever가 시작된 시간을 기록하는 변수
    private float feverTimeCurr = float.NaN;
    // fever 객체의 시간을 기록하는 변수
    private float feverTimer = float.NaN;
    // fever 상태의 지속시간을 기록하는 변수
    private float feverBuffTime = float.NaN;

    // Gun 시스템에 접근하기 위한 변수
    private Gun gunSys = null;

    #region Public Functions

    public void GainFeverCount()
    {
        // 피버 카운트를 증가시킨다. 좀비를 죽였을 때에 발동한다
        if (!this.IsFeverOn)
        {
            this.FeverCount++;
        }
    }

    public void ResetFeverCount()
    {
        // 피버 카운트를 초기화한다. 좀비에게 데미지를 입었을 때에 발동한다
        this.FeverCount = 0;
    }

    public void SetFeverOn()
    {
        // fever를 활성화 시키고, fever 시작 시간을 기록한다
        this.IsFeverOn = true;
        this.feverTimeCurr = this.feverTimer;
        gunSys.SetFever(IsFeverOn);
    }

    public void SetFeverOff()
    {
        // fever를 비활성화 시킨다
        this.IsFeverOn = false;
        gunSys.SetFever(IsFeverOn);
    }

    #endregion

    #region Unity Function

    internal void Awake()
    {
        this.gunSys = GameObject.Find("Player").GetComponent<Gun>();
        this.FeverCount = 0;
        this.feverTimeCurr = 0.0f;
        this.feverTimer = 0.0f;
        this.feverBuffTime = 20.0f;
    }

    #endregion

    #region Update Functions

    internal void Update()
    {
        this.UpdateFeverTimer();

        // 피버 카운트가 10 이상이면 fever를 킨다
        if (this.FeverCount == MAX_FEVER_COUNT)
        {
            this.FeverCount = 0;
            this.SetFeverOn();
        }

        // (현재 시간-피버 시작 시간)이 피버 지속 시간보다 커지면 fever를 끈다
        if (this.IsFeverOn)
        {
            if ((this.feverTimer - this.feverTimeCurr) > feverBuffTime)
            {
                this.SetFeverOff();
            }
        }
    }

    private void UpdateFeverTimer()
    {
        // fever객체의 타이머의 시간을 증가시킨다
        this.feverTimer = this.feverTimer + Time.deltaTime;
    }

    #endregion

}