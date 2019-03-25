using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class MainManager : MonoBehaviour
{
    public Button tutorialButton;
    public Button startButton;
    public Button rankButton;
    public Button quitButton;
    public Button creditButton;
    public GameObject quitPanel;
    public GameObject creditPanel;
    public GameObject title;

    public void LoadHowToPlay()
    {
        SceneManager.LoadScene("HowToPlay");
    }

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

    public void CloseCredit()
    {
        setActiveAllComponents();
        creditPanel.SetActive(false);
    }

    public void ShowCredit()
    {
        setInactiveAllComponents();
        creditPanel.SetActive(true);
    }

    private void Start()
    {
        addOnclicks();
    }

    private void Update()
    {
        if (Application.isEditor == false)
        {
            // 구글플레이가 로그아웃되어있는 경우, Intro Scene으로 돌려보낸다
            if (Social.localUser.authenticated == false)
            {
                SceneManager.LoadScene("Intro");
            }
        }
    }

    private void addOnclicks()
    {
        tutorialButton.onClick.AddListener(LoadHowToPlay);
        startButton.onClick.AddListener(LoadGame);
        rankButton.onClick.AddListener(ShowLeaderboard);
        quitButton.onClick.AddListener(AskQuit);
        creditButton.onClick.AddListener(ShowCredit);
    }

    private void removeOnclicks()
    {
        tutorialButton.onClick.RemoveAllListeners();
        startButton.onClick.RemoveAllListeners();
        rankButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();
        creditButton.onClick.RemoveAllListeners();
    }

    private void setInactiveAllComponents()
    {
        removeOnclicks();

        tutorialButton.gameObject.SetActive(false);
        startButton.gameObject.SetActive(false);
        rankButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
        creditButton.gameObject.SetActive(false);
        title.SetActive(false);
    }

    private void setActiveAllComponents()
    {
        tutorialButton.gameObject.SetActive(true);
        startButton.gameObject.SetActive(true);
        rankButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
        creditButton.gameObject.SetActive(true);
        title.SetActive(true);

        addOnclicks();
    }
}
