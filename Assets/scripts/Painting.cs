using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Painting : MonoBehaviour
{
    public UnityEvent onDropped;

    private Rigidbody rb;

    private void Start()
    {
        rb = transform.GetComponent<Rigidbody>();
    }

    public void Fall()
    {
        rb.AddForce(new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)) * 10f);

        rb.isKinematic = false;
        rb.useGravity = true;

        onDropped?.Invoke();
    }
}
