using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class IntroManager : MonoBehaviour
{
    public Text NoticeText;
    public float BlinkSpeed = 2.0f;

    void Start()
    {
        // Google Play Games 활성화
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        NoticeText.text = "Touch to Start";
    }

    void Update()
    {
        // Main 씬 로드
        if (Input.GetMouseButtonUp(0))
        {
            SceneManager.LoadScene("Main");
        }

        // Notice Text를 깜빡인다.
        NoticeText.color = Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time * BlinkSpeed, 1));
    }
}
