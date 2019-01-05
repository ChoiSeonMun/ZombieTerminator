using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fever : MonoBehaviour
{
    public int feverCount = 0;
    public float feverTimeCurr = float.NaN;
    public float feverTimer = float.NaN;
    public float feverBuffTime = float.NaN;

    private bool isFeverOn = false;
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

        if (this.feverCount == 10)
        {
            this.feverCount = 0;
            this.SetFeverOn();
        }

        if (this.isFeverOn)
        {
            if ((this.feverTimer - this.feverTimeCurr) < feverBuffTime)
            {
                this.SetFeverOff();
            }
        }
    }

    public void GainFeverCount()
    {
        if (!this.isFeverOn)
        {
            this.feverCount++;
        }
    }

    public void ResetFeverCount()
    {
        this.feverCount = 0;
    }

    public void SetFeverOn()
    {
        this.isFeverOn = true;
        this.feverTimeCurr = this.feverTimer;
        gunSys.SetFever(isFeverOn);
    }

    public void SetFeverOff()
    {
        this.isFeverOn = false;

        gunSys.SetFever(isFeverOn);
    }

    private void UpdateFeverTimer()
    {
        this.feverTimer = this.feverTimer + Time.deltaTime;
    }
}