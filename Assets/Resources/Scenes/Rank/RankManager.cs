using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RankManager : MonoBehaviour
{
    // BACK 버튼
    private Button mBackButton = null;
    private ButtonExtension mBackButtonExtension = null;

    void Start()
    {
        mBackButton = GameObject.Find("Canvas/Panel/Back").GetComponent<Button>();
        // BACK 버튼이 클릭되면 OnClickBack 함수를 실행하도록 설정
        mBackButton.onClick.AddListener(OnClickBack);
        mBackButtonExtension = mBackButton.gameObject.GetComponent<ButtonExtension>();
    }

    void Update()
    {
        if(mBackButtonExtension.IsPressed)
        {
            mBackButton.transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            mBackButton.transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    private void OnClickBack()
    {
        SceneManager.LoadScene("Main");
    }
}
