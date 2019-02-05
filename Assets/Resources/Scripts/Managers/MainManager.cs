using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    // START 버튼
    public Button StartButton = null;
    // RANK 버튼
    public Button RankButton = null;

    public void OnClickStart()
    {
        SceneManager.LoadScene("Game");
    }

    public void OnClickRank()
    {
        SceneManager.LoadScene("Rank");
    }
}
