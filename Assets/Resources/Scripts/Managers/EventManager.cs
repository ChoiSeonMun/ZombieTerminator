using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class EventManager : MonoBehaviour
{
    public UnityEvent pauseEvent = null;
    public UnityEvent resumeEvent = null;

    public UnityEvent feverOnEvent = null;
    public UnityEvent feverOffEvent = null;

    public UnityEvent levelUpEvent = null;
}
