using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class MainManager : MonoBehaviour
{
    public Button startButton;
    public Button rankButton;
    public Button quitButton;
    public GameObject quitPanel;

    public void LoadGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void ShowLeaderboard()
    {
        PlayGamesPlatform.Instance.ShowLeaderboardUI("CgkItcrr5bIWEAIQBw");
    }

    public void AskQuit()
    {
        removeOnclicks();
        quitPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void BackToGame()
    {
        quitPanel.SetActive(false);
        addOnclicks();
    }

    private void Start()
    {
        addOnclicks();
    }

    private void Update()
    {
        // 구글플레이가 로그아웃되어있는 경우, Intro Scene으로 돌려보낸다
        if (Social.localUser.authenticated == false)
        {
            SceneManager.LoadScene("Intro");
        }
    }

    private void addOnclicks()
    {
        startButton.onClick.AddListener(LoadGame);
        rankButton.onClick.AddListener(ShowLeaderboard);
        quitButton.onClick.AddListener(AskQuit);
    }

    private void removeOnclicks()
    {
        startButton.onClick.RemoveAllListeners();
        rankButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();
    }
}
