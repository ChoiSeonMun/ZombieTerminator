using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SMain : MonoBehaviour
{
    Button bStart = null;
    Button bRank = null;

    private void Start()
    {
        var obj = GameObject.Find("Canvas/Game");
        this.bStart = obj.GetComponent<Button>();
        this.bStart.onClick.AddListener(this.OnClickStart);

        obj = GameObject.Find("Canvas/Rank");
        this.bRank = obj.GetComponent<Button>();
        this.bRank.onClick.AddListener(this.OnClickRank);
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
