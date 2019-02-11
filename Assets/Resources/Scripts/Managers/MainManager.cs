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

    public void LoadGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void LoadRank()
    {
        SceneManager.LoadScene("Rank");
    }
}
