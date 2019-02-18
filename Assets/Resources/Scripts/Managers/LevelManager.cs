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
    public EventManager eventManager = null;
    // 다음 레벨 업까지의 시간 간격을 저장하는 변수
    public float levelInterval = float.NaN;
    // 레벨 업을 위한 시간 경과를 저장하는 변수
    private float mLevelTimer = float.NaN;
    // Update 를 중단할지 결정하는 변수
    private bool mShouldUpdate = false;

    public void BlockUpdate()
    {
        mShouldUpdate = false;
    }

    public void GrantUpdate()
    {
        mShouldUpdate = true;
    }

    void Awake()
    {
        Level = 0;
        levelInterval = 10.0f;
        mLevelTimer = 0.0f;
        mShouldUpdate = true;
    }

    void Update()
    {
        if (mShouldUpdate)
        {
            checkLevelUp();
        }
    }

    private void checkLevelUp()
    {
        mLevelTimer += Time.deltaTime;
        if (mLevelTimer > levelInterval)
        {
            levelUp();
        }
    }

    private void levelUp()
    {
        Level++;
        levelInterval = levelInterval + 5.0f;
        mLevelTimer = 0.0f;
        eventManager.levelUpEvent.Invoke();
    }
}
