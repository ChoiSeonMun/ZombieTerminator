using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    private Button mBStart = null;
    private Button mBRank = null;

    private void Start()
    {
        this.mBStart = GameObject.Find("Canvas/Panel/Start").GetComponent<Button>();
        this.mBStart.onClick.AddListener(this.OnClickStart);

        this.mBRank = GameObject.Find("Canvas/Panel/Rank").GetComponent<Button>();
        this.mBRank.onClick.AddListener(this.OnClickRank);
    }

    private void OnClickStart()
    {
        SceneManager.LoadScene("Game");
    }

    private void OnClickRank()
    {
        SceneManager.LoadScene("Rank");
    }
}
