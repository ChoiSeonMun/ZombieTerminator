using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonExtension : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // 버튼이 눌렸는지 아닌지를 확인하기 위해 외부에서 접근할 수 있는 property
    public bool IsPressed
    {
        get
        {
            bool retVal = this.mbPressed;
            // 한 번 확인하고 나면 false 로 변경하여, 오작동을 방지한다
            this.mbPressed = false;
            return retVal;
        }
    }

    // 버튼이 눌렸는지 아닌지를 내부적으로 저장하는 변수
    private bool mbPressed = false;

    // 버튼이 눌리는 이벤트가 발생하면 true 저장
    public void OnPointerDown(PointerEventData ped)
    {
        this.mbPressed = true;
    }

    // 버튼이 떼어지는 이벤트가 발생하면 false 저장
    public void OnPointerUp(PointerEventData ped)
    {
        this.mbPressed = false;
    }
}
