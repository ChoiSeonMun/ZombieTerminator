using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{
    private Text mNotice = null;

    private Color mBlinkColor = Color.white;
    private float mBlinkCount = 1.0f;

    private void Awake()
    {
        this.mNotice = GameObject.Find("Canvas/Panel/Notice").GetComponent<Text>();
    }

    private void Update()
    {
        this.ProcessInput();

        this.BlinkNotice();
    }

    private void ProcessInput()
    {
        if (Input.GetMouseButtonUp(0))
        {
            this.LoadMain();
        }
    }

    private void LoadMain()
    {
        SceneManager.LoadScene("Main");
    }

    private void BlinkNotice()
    {
        if(this.mBlinkCount >= 1.0f)
        {
            this.mBlinkCount = 0.0f;
        }
        else
        {
            this.mBlinkCount += 0.025f;
        }

        this.mBlinkColor.r = this.mBlinkColor.g = this.mBlinkColor.b = this.mBlinkCount;
        this.mNotice.color = this.mBlinkColor;
    }
}
