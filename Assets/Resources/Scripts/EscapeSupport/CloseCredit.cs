using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseCredit : MonoBehaviour
{
    public MainManager mainManager = null;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            mainManager.CloseCredit();
        }
    }
}
