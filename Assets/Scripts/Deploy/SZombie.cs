using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SZombie : MonoBehaviour
{
    private SGame manager = null;

    private float lifetime = float.NaN;
    private float runtime = float.NaN;

    private UnityEvent hit = null;

    private void Start()
    {
        var obj = GameObject.Find("Manager");
        this.manager = obj.GetComponent<SGame>();

        this.lifetime = 3.0f;
        this.runtime = 0.0f;

        this.hit = new UnityEvent();
        this.hit.AddListener(this.manager.OnHit);
    }

    private void Update()
    {
        this.runtime += Time.deltaTime;

        if(this.runtime >= this.lifetime)
        {
            this.hit.Invoke();
            this.gameObject.SetActive(false);
        }
    }
}
