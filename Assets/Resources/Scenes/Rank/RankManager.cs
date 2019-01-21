using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RankManager : MonoBehaviour
{
    // BACK 버튼
    private Button mBBack = null;

    internal void Start()
    {
        this.mBBack = GameObject.Find("Canvas/Panel/Back").GetComponent<Button>();
        // BACK 버튼이 클릭되면 OnClickBack 함수를 실행하도록 설정
        this.mBBack.onClick.AddListener(this.OnClickBack);
    }

    private void OnClickBack()
    {
        SceneManager.LoadScene("Main");
    }
}
