using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SMain : MonoBehaviour
{
    public void OnClickStart()
    {
        SceneManager.LoadScene("Game");
    }

    public void OnClickRank()
    {
        SceneManager.LoadScene("Rank");
    }
}
