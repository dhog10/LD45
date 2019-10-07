using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class Painting : MonoBehaviour
{
    public HiddenAppear hiddenAppear;
    public TextMeshPro textMesh;
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

    public void ResetPainting()
    {
        hiddenAppear.Enable();
        hiddenAppear.Hide();

        rb.isKinematic = true;
        rb.useGravity = false;

        transform.position = transform.parent.transform.position;
        transform.rotation = transform.parent.transform.rotation;
    }

    public void SetText(string message)
    {
        textMesh.text = message;
    }

    public void SetFontSize(float size)
    {
        textMesh.fontSize = size;
    }
}
