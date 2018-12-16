using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SGame : MonoBehaviour
{
    private enum GameState
    {
        READY,
        PLAYING,
        PAUSED
    };

    private GameObject[] targets = null;
    private GameObject pStart = null;
    private Button bStart = null;
    private Button bMenu = null;
    private GameObject pDialog = null;
    private Button bYes = null;
    private Button bNo = null;

    private GameState state;
    private Dictionary<GameState, Action> inputs = null;

    private void Awake()
    {
        this.targets = GameObject.FindGameObjectsWithTag("Target");
        Array.Sort(this.targets, delegate (GameObject _1, GameObject _2)
        {
            return _1.name.CompareTo(_2.name);
        });

        this.pStart = GameObject.Find("Canvas/PStart");
        var button = GameObject.Find("Canvas/PStart/Button");
        this.bStart = button.GetComponent<Button>();
        this.bStart.onClick.AddListener(this.OnClickStart);

        button = GameObject.Find("Canvas/PMenu/Button");
        this.bMenu = button.GetComponent<Button>();
        this.bMenu.onClick.AddListener(this.OnClickMenu);

        this.pDialog = GameObject.Find("Canvas/PDialog");
        button = GameObject.Find("Canvas/PDialog/Yes");
        this.bYes = button.GetComponent<Button>();
        this.bYes.onClick.AddListener(this.OnClickYes);
        button = GameObject.Find("Canvas/PDialog/No");
        this.bNo = button.GetComponent<Button>();
        this.bNo.onClick.AddListener(this.OnClickNo);
        this.pDialog.SetActive(false);

        this.state = GameState.READY;

        this.inputs = new Dictionary<GameState, Action>();
        this.inputs.Add(GameState.READY, this.InputReady);
        this.inputs.Add(GameState.PLAYING, this.InputPlaying);
        this.inputs.Add(GameState.PAUSED, this.InputPaused);
    }

    private void Update()
    {
        this.inputs[this.state]();
    }

    private void InputReady()
    {
        ;
    }

    private void OnClickStart()
    {
        this.pStart.SetActive(false);
        this.state = GameState.PLAYING;
    }

    private void InputPlaying()
    {
        bool isTargetPressed = false;
        GameObject targetObject = null;

        if (Input.GetMouseButtonDown(0))
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
        if (_isTarget)
        {
            Image image = _obj.GetComponent<Image>();
            image.color = Color.red;
        }
        else
        {
            foreach (GameObject go in this.targets)
            {
                Image image = go.GetComponent<Image>();
                image.color = Color.white;
            }
        }
    }

    private void OnClickMenu()
    {
        this.state = GameState.PAUSED;
        this.pDialog.SetActive(true);
    }

    private void InputPaused()
    {
        ;
    }

    private void OnClickYes()
    {
        SceneManager.LoadScene("Main");
    }

    private void OnClickNo()
    {
        this.state = GameState.PLAYING;
        this.pDialog.SetActive(false);
    }
}