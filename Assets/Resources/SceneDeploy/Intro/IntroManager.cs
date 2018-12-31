using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{
    private GameObject panel = null;
    private Text notice = null;

    private Color blink_color = Color.white;
    private float blink_count = 1.0f;

    private void Awake()
    {
        this.panel = GameObject.Find("Canvas/PNotice");
        var children_text = this.panel.GetComponentsInChildren<Text>();
        foreach (var child in children_text)
        {
            if (child.name == "Text")
            {
                this.notice = child;
            }
        }
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
        if(this.blink_count >= 1.0f)
        {
            this.blink_count = 0.0f;
        }
        else
        {
            this.blink_count += 0.025f;
        }

        this.blink_color.r = this.blink_color.g = this.blink_color.b = this.blink_count;
        this.notice.color = this.blink_color;
    }
}
