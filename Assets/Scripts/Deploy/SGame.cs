using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SGame : MonoBehaviour
{
    private GameObject[] targets = null;

    private void Awake()
    {
        this.targets = GameObject.FindGameObjectsWithTag("Target");
        Array.Sort(this.targets, delegate(GameObject _1, GameObject _2)
        {
            return _1.name.CompareTo(_2.name);
        });
    }

    private void Update()
    {
        this.ProcessInput();
    }

    private void ProcessInput()
    {
        bool isTargetPressed = false;
        GameObject targetObject = null;

        if(Input.GetMouseButtonDown(0))
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
                        targetObject = go.gameObject;
                        break;
                    }
                }
            }
        }

        this.UpdatePanel(isTargetPressed, targetObject);
    }

    private void UpdatePanel(bool _isTarget, GameObject _obj)
    {
        if(_isTarget)
        {
            Image image = _obj.GetComponent<Image>();
            image.color = Color.red;
        }
        else
        {
            foreach(GameObject go in this.targets)
            {
                Image image = go.GetComponent<Image>();
                image.color = Color.white;
            }
        }
    }
}