﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPanel : MonoBehaviour
{
    public GameManager gameManager = null;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameManager.LoadMain();
        }
    }
}
