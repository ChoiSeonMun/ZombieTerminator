using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SRank : MonoBehaviour
{
    private Button bBack = null;

    private void Start()
    {
        GameObject obj = GameObject.Find("Canvas/Back");
        this.bBack = obj.GetComponent<Button>();
        this.bBack.onClick.AddListener(this.OnClickBack);
    }

    private void OnClickBack()
    {
        SceneManager.LoadScene("Main");
    }
}
