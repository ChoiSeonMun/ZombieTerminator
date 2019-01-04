using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonExtension : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool IsPressed
    {
        get
        {
            bool retVal = this.mbPressed;
            this.mbPressed = false;
            return retVal;
        }
    }

    private bool mbPressed = false;

    public void OnPointerDown(PointerEventData ped)
    {
        this.mbPressed = true;
    }

    public void OnPointerUp(PointerEventData ped)
    {
        this.mbPressed = false;
    }
}
