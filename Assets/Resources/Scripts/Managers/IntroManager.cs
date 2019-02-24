using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class IntroManager : MonoBehaviour
{
    public Text noticeText;
    public float blinkSpeed = 2.0f;
    public GameObject authenticationPanel;

    private bool mIsCanInput = false;

    public void OnClickLogin()
    {
        authenticate();
        loadMain();
    }

    public void OnClickCancel()
    {
        authenticationPanel.SetActive(false);
        mIsCanInput = true;
    }

    void Start()
    {
        // Google Play Games 활성화
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();

        authenticate();
        mIsCanInput = true;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && mIsCanInput == true)
        {
            if (Social.localUser.authenticated)
            {
                // Main 씬 로드
                loadMain();
            }
            else
            {
                Debug.Log("dialog panel activated");

                // 대화상자를 띄운다
                authenticationPanel.SetActive(true);
                mIsCanInput = false;
            }
        }

        // Notice Text를 깜빡인다.
        noticeText.color = Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time * blinkSpeed, 1));
    }

    private void authenticate()
    {
        Social.localUser.Authenticate(bSuccess =>
        {
            Debug.Log("Authentication: " + bSuccess);
        });
    }

    private void loadMain()
    {
        if (Social.localUser.authenticated)
        {
            SceneManager.LoadScene("Main");
        }
    }
}
