using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    // START 버튼
    private Button mBStart = null;
    // RANK 버튼
    private Button mBRank = null;

    internal void Start()
    {
        this.mBStart = GameObject.Find("Canvas/Panel/Start").GetComponent<Button>();
        // START 버튼이 클릭되면 OnClickStart 함수를 실행하도록 설정
        this.mBStart.onClick.AddListener(this.OnClickStart);

        this.mBRank = GameObject.Find("Canvas/Panel/Rank").GetComponent<Button>();
        // RANK 버튼이 클릭되면 OnClickRank 함수를 실행하도록 설정
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
