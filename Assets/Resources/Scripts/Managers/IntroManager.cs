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

    public void OnClickLogin()
    {
        authenticate();
    }

    public void OnClickCancel()
    {
        authenticationPanel.SetActive(false);
    }

    void Start()
    {
        // Google Play Games 활성화
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();

        authenticate();

        noticeText.text = "Touch to Start";
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Social.localUser.authenticated)
            {
                // Main 씬 로드
                SceneManager.LoadScene("Main");
            }
            else
            {
                // 대화상자를 띄운다
                authenticationPanel.SetActive(true);
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
}
