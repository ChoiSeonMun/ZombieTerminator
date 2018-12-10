using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class STouchPanel : MonoBehaviour
{
    private Image image = null;

    private void Awake()
    {
        var panel = GameObject.Find("Canvas/Panel");
        this.image = panel.GetComponent<Image>();
    }

    private void Update()
    {
        this.ProcessInput();
    }

    private void ProcessInput()
    {
        bool isTargetPressed = false;

        if (Input.GetMouseButton(0))
        {
            PointerEventData pointer = new PointerEventData(EventSystem.current);
            pointer.position = Input.mousePosition;

            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, raycastResults);

            if (raycastResults.Count > 0)
            {
                foreach (var go in raycastResults)
                {
                    if (go.gameObject.tag == "Target")
                    {
                        isTargetPressed = true;
                    }
                }
            }
        }

        this.UpdatePanel(isTargetPressed);
    }

    private void UpdatePanel(bool _isTarget)
    {
        if (_isTarget)
        {
            this.image.color = Color.red;
        }
        else
        {
            this.image.color = Color.gray;
        }
    }
}
