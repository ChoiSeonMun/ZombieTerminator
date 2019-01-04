using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RankManager : MonoBehaviour
{
    private Button mBBack = null;

    internal void Start()
    {
        this.mBBack = GameObject.Find("Canvas/Panel/Back").GetComponent<Button>();
        this.mBBack.onClick.AddListener(this.OnClickBack);
    }

    private void OnClickBack()
    {
        SceneManager.LoadScene("Main");
    }
}
