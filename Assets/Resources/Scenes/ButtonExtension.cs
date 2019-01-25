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
            return mIsPressed;
        }
    }
    // 버튼이 눌렸는지 아닌지를 내부적으로 저장하는 변수
    private bool mIsPressed = false;

    // 버튼이 눌렸었는지 아닌지를 확인하기 위해 외부에서 접근할 수 있는 propertyy
    public bool WasPressed
    {
        get
        {
            return mWasPressed;
        }
    }
    // 버튼이 눌렸었는지 아닌지를 내부적으로 저장하는 변수
    private bool mWasPressed = false;

    // 버튼이 떼어졌는지 아닌지를 확인하기 위해 외부에서 접근할 수 있는 property
    public bool IsReleased
    {
        get
        {
            return (mIsPressed == false) && (mWasPressed == true);
        }
    }

    // 버튼이 눌리는 이벤트가 발생하면 true 저장
    public void OnPointerDown(PointerEventData ped)
    {
        this.mIsPressed = true;
    }

    // 버튼이 떼어지는 이벤트가 발생하면 false 저장
    public void OnPointerUp(PointerEventData ped)
    {
        this.mIsPressed = false;
    }

    private void LateUpdate()
    {
        mWasPressed = mIsPressed;
    }
}
