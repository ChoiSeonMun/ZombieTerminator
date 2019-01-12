using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fever : MonoBehaviour
{
    // 처치한 좀비 수를 기록하는 변수
    private int feverCount = 0;
    // fever가 시작된 시간을 기록하는 변수
    private float feverTimeCurr = float.NaN;
    // fever 객체의 시간을 기록하는 변수
    private float feverTimer = float.NaN;
    // fever 상태의 지속시간을 기록하는 변수
    private float feverBuffTime = float.NaN;

    // fever의 최대치를 기록하는 상수
    public const int maxFeverCount = 10;
    // fever 상태의 On/Off를 기록하는 변수
    private bool isFeverOn = false;
    // Gun 시스템에 접근하기 위한 변수
    private Gun gunSys = null;

    internal void Awake()
    {
        this.gunSys = GameObject.Find("Player").GetComponent<Gun>();
        this.feverCount = 0;
        this.feverTimeCurr = 0.0f;
        this.feverTimer = 0.0f;
        this.feverBuffTime = 20.0f;
    }

    internal void Update()
    {
        this.UpdateFeverTimer();

        // 피버 카운트가 10 이상이면 fever를 킨다
        if (this.feverCount == maxFeverCount)
        {
            this.feverCount = 0;
            this.SetFeverOn();
        }

        // (현재 시간-피버 시작 시간)이 피버 지속 시간보다 커지면 fever를 끈다
        if (this.isFeverOn)
        {
            if ((this.feverTimer - this.feverTimeCurr) > feverBuffTime)
            {
                this.SetFeverOff();
            }
        }
    }

    public void GainFeverCount()
    {
        // 피버 카운트를 증가시킨다. 좀비를 죽였을 때에 발동한다
        if (!this.isFeverOn)
        {
            this.feverCount++;
        }
    }

    public int GetFeverCount()
    {
        return feverCount;
    }

    public void ResetFeverCount()
    {
        // 피버 카운트를 초기화한다. 좀비에게 데미지를 입었을 때에 발동한다
        this.feverCount = 0;
    }

    public void SetFeverOn()
    {
        // fever를 활성화 시키고, fever 시작 시간을 기록한다
        this.isFeverOn = true;
        this.feverTimeCurr = this.feverTimer;
        gunSys.SetFever(isFeverOn);
    }

    public void SetFeverOff()
    {
        // fever를 비활성화 시킨다
        this.isFeverOn = false;
        gunSys.SetFever(isFeverOn);
    }

    private void UpdateFeverTimer()
    {
        // fever객체의 타이머의 시간을 증가시킨다
        this.feverTimer = this.feverTimer + Time.deltaTime;
    }

    public bool GetIsFeverOn()
    {
        return isFeverOn;
    }
}