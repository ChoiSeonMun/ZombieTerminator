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
    public Text authInfo;

    void Start()
    {
        // Google Play Games 활성화
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        Social.localUser.Authenticate(bSuccess =>
        {
            Debug.Log("Authentication: " + bSuccess);
            if (bSuccess)
            {
                authInfo.text = "Welcome " + Social.localUser.userName;
            }
            else
            {
                authInfo.text = "Failed";
            }
        });

        noticeText.text = "Touch to Start";
    }

    void Update()
    {
        // Main 씬 로드
        if (Input.GetMouseButtonUp(0))
        {
            if (Social.localUser.authenticated)
            {
                SceneManager.LoadScene("Main");
            }
            else
            {
                // 알람메시지 띄움
            }
        }

        // Notice Text를 깜빡인다.
        noticeText.color = Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time * blinkSpeed, 1));
    }
}
