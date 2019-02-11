using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    public int Level
    {
        get;
        private set;
    }
    public UnityEvent LevelUpEvent = null;
    // 레벨 업을 위한 시간 경과를 저장하는 변수
    private float mLevelTimer = float.NaN;
    // 다음 레벨 업까지의 시간 간격을 저장하는 변수
    private float mLevelInterval = float.NaN;

    void Awake()
    {
        Level = 0;
        mLevelTimer = 0.0f;
        mLevelInterval = 10.0f;
    }

    void Update()
    {
        checkLevelUp();
    }

    private void checkLevelUp()
    {
        mLevelTimer += Time.deltaTime;
        if (mLevelTimer > mLevelInterval)
        {
            levelUp();
            mLevelTimer = 0.0f;
        }
    }

    private void levelUp()
    {
        Level++;
        LevelUpEvent.Invoke();
        mLevelInterval *= 0.95f;
    }
}
