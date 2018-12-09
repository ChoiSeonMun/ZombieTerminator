using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SGame : MonoBehaviour
{
    private GameObject[] targets = null;
    private GameObject indicator = null;

    private void Awake()
    {
        this.targets = GameObject.FindGameObjectsWithTag("Target");
        Array.Sort(this.targets, delegate(GameObject _1, GameObject _2)
        {
            return _1.name.CompareTo(_2.name);
        });
        this.indicator = GameObject.Find("Canvas/PTarget/Indicator");
    }

    private void Update()
    {
        ;
    }
}