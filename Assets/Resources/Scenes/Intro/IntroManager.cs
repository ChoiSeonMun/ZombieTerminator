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
    
    // touch to start 문구를 깜빡이게 하기 위한 변수들
    private float mBlinkValue = 1.0f;
    private Color mBlinkColor = Color.white;

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
        // 다음 씬으로 넘어가기
        if (Input.GetMouseButtonUp(0))
        {
            SceneManager.LoadScene("Main");
        }

        // 문구 깜빡이기
        NoticeText.color = Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time * BlinkSpeed, 1));
    }
}
