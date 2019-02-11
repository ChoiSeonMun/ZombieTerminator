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
        // Main 씬 로드
        if (Input.GetMouseButtonUp(0))
        {
            if (Social.localUser.authenticated)
            {
                SceneManager.LoadScene("Main");
            }
            else
            {
                // 대화상자를 띄운다.
                Debug.Log("네트워크 환경을 확인해주세요.");
                Debug.Log("[구글 플레이 게임에 로그인하기 버튼]");
                Debug.Log("[취소버튼]");

                // if (loginButton.isClicked)
                //  authenticate();
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
