using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Sign : MonoBehaviour
{
    public UnityEvent onFall;

    private HiddenAppear hidden;
    private Rigidbody rb;
    private bool fallen;

    private void Start()
    {
        hidden = GetComponent<HiddenAppear>();
        rb = GetComponent<Rigidbody>();
    }

    public void Fall()
    {
        if (fallen)
        {
            return;
        }

        fallen = true;

        hidden.Disable();
        rb.isKinematic = false;
        rb.useGravity = true;

        onFall?.Invoke();
    }

    public bool IsFallen()
    {
        return fallen;
    }
}
