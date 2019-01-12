using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class IntroManager : MonoBehaviour
{
    // touch to start 문구를 출력하기 위한 Text
    private Text mNotice = null;
    // touch to start 문구를 깜빡이게 하기 위한 변수들
    private Color mBlinkColor = Color.white;
    private float mBlinkCount = 1.0f;

    void Start()
    {
        // Google Play Games 활성화
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        // Google ID로 유저 인증
        PlayGamesPlatform.Instance.Authenticate((bool bSuccess) =>
        {
            Debug.Log("Authentication: " + bSuccess);
        }, false);
    }

    void Awake()
    {
        this.mNotice = GameObject.Find("Canvas/Panel/Notice").GetComponent<Text>();
    }

    void Update()
    {
        this.ProcessInput();

        this.BlinkNotice();
    }

    private void ProcessInput()
    {
        if (Input.GetMouseButtonUp(0))
        {
            this.LoadMain();
        }
    }

    private void LoadMain()
    {
        SceneManager.LoadScene("Main");
    }

    private void BlinkNotice()
    {
        // 카운터가 1.0 보다 커지면 == 문구가 흰색이 된다면
        // 카운터를 0.0 값으로 한다 == 문구를 검은색으로 한다
        // 그 외의 경우 카운터를 조금씩 증가시킨다 == 문구를 밝게 변화시킨다
        if(this.mBlinkCount >= 1.0f)
        {
            this.mBlinkCount = 0.0f;
        }
        else
        {
            this.mBlinkCount += 0.025f;
        }
        this.mBlinkColor.r = this.mBlinkColor.g = this.mBlinkColor.b = this.mBlinkCount;
        this.mNotice.color = this.mBlinkColor;
    }
}
