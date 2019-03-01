using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuthenticationPanel : MonoBehaviour
{
    public IntroManager introManager = null;

	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            introManager.OnClickCancel();
        }
    }
}
