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

        if(Application.isEditor)
        {
            SceneManager.LoadScene("Main");
        }
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

        mIsCanInput = true;
    }

    void Update()
    {
        // 로그인이 완료되면 Main Scene으로 이동한다
        if (Social.localUser.authenticated)
        {
            SceneManager.LoadScene("Main");
        }

        if (Input.GetMouseButtonDown(0) && mIsCanInput == true)
        {
            // 대화상자를 띄운다
            authenticationPanel.SetActive(true);
            mIsCanInput = false;
        }

        // Notice Text를 깜빡인다
        noticeText.color = Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time * blinkSpeed, 1));
    }

    private void authenticate()
    {
        Social.localUser.Authenticate(bSuccess =>
        {
            Debug.Log("Authentication: " + bSuccess);
        });
    }
}
