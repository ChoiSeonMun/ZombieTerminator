using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    // START 버튼
    private Button mStartButton = null;
    private ButtonExtension mStartButtonExtension = null;
    // RANK 버튼
    private Button mRankButton = null;
    private ButtonExtension mRankButtonExtension = null;

    void Awake()
    {
        mStartButton = GameObject.Find("Canvas/Panel/Start").GetComponent<Button>();
        // START 버튼이 클릭되면 OnClickStart 함수를 실행하도록 설정
        mStartButton.onClick.AddListener(OnClickStart);
        mStartButtonExtension = mStartButton.gameObject.GetComponent<ButtonExtension>();

        mRankButton = GameObject.Find("Canvas/Panel/Rank").GetComponent<Button>();
        // RANK 버튼이 클릭되면 OnClickRank 함수를 실행하도록 설정
        mRankButton.onClick.AddListener(OnClickRank);
        mRankButtonExtension = mRankButton.gameObject.GetComponent<ButtonExtension>();
    }

    void Update()
    {
        if(mStartButtonExtension.IsPressed)
        {
            mStartButton.transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            mStartButton.transform.GetChild(1).gameObject.SetActive(true);
        }

        if(mRankButtonExtension.IsPressed)
        {
            mRankButton.transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            mRankButton.transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    private void OnClickStart()
    {
        SceneManager.LoadScene("Game");
    }

    private void OnClickRank()
    {
        SceneManager.LoadScene("Rank");
    }
}
