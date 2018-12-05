using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SGame : MonoBehaviour
{
    private GameObject[] targets = null;
    private GameObject indicator = null;
    private int indicator_pos = 4;

    private Button[] buttons = new Button[4];

    private void Awake()
    {
        this.targets = GameObject.FindGameObjectsWithTag("Target");
        Array.Sort(this.targets, delegate(GameObject _1, GameObject _2)
        {
            return _1.name.CompareTo(_2.name);
        });
        this.indicator = GameObject.Find("Canvas/PTarget/Indicator");

        this.buttons[0] = GameObject.Find("Canvas/PControl/Left").GetComponent<Button>();
        this.buttons[0].onClick.AddListener(this.OnClickLeft);
        this.buttons[1] = GameObject.Find("Canvas/PControl/Up").GetComponent<Button>();
        this.buttons[1].onClick.AddListener(this.OnClickUp);
        this.buttons[2] = GameObject.Find("Canvas/PControl/Right").GetComponent<Button>();
        this.buttons[2].onClick.AddListener(this.OnClickRight);
        this.buttons[3] = GameObject.Find("Canvas/PControl/Down").GetComponent<Button>();
        this.buttons[3].onClick.AddListener(this.OnClickDown);
    }

    private void Update()
    {
        this.UpdateIndicator();
    }

    private void UpdateIndicator()
    {
        this.indicator.transform.position = this.targets[this.indicator_pos].transform.position;
    }

    public void OnClickLeft()
    {
        switch(this.indicator_pos)
        {
            case 0:
            case 3:
            case 6:
                break;

            default:
                this.indicator_pos = this.indicator_pos - 1;
                break;
        }
    }

    public void OnClickUp()
    {
        switch (this.indicator_pos)
        {
            case 0:
            case 1:
            case 2:
                break;

            default:
                this.indicator_pos = this.indicator_pos - 3;
                break;
        }
    }

    public void OnClickRight()
    {
        switch (this.indicator_pos)
        {
            case 2:
            case 5:
            case 8:
                break;

            default:
                this.indicator_pos = this.indicator_pos + 1;
                break;
        }
    }

    public void OnClickDown()
    {
        switch (this.indicator_pos)
        {
            case 6:
            case 7:
            case 8:
                break;

            default:
                this.indicator_pos = this.indicator_pos + 3;
                break;
        }
    }
}