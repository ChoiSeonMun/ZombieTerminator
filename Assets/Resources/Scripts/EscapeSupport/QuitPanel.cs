using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitPanel : MonoBehaviour
{
    public MainManager mainManager = null;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            mainManager.BackToGame();
        }
    }
}
