using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class MainManager : MonoBehaviour
{    
    IEnumerator Start()
    {
        // 사용자 인증을 요청한다.
        yield return StartCoroutine(authentication());
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene("Game");
    }

    public void LoadRankScene()
    {
        SceneManager.LoadScene("Rank");
    }

    private IEnumerator authentication()
    {
        Authenticate("Very first authentication");
        
        yield return new WaitForSeconds(1.0f);
    }

    public void Authenticate(string debugMessage)
    {
        PlayGamesPlatform.Instance.Authenticate((bool bSuccess) =>
        {
            Debug.Log($"{debugMessage}({bSuccess})");
        }, false);
    }
}
