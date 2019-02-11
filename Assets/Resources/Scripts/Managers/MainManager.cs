using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public Button StartButton = null;
    public Button RankButton = null;

    public void LoadGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void LoadRank()
    {
        SceneManager.LoadScene("Rank");
    }
}
