using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public float openSpeed = 2f;
    public float openAngleAmount = 90f;
    public GameObject door;

    private float openAngle = 0f;
    private bool open = true;

    // Update is called once per frame
    void Update()
    {
        if (open)
        {
            if (openAngle < openAngleAmount)
            {
                openAngle = Mathf.Min(openAngle + (openAngleAmount - openAngle) * Time.deltaTime * openSpeed, openAngleAmount);
            }
        }
        else
        {
            if (openAngle > 0f)
            {
                openAngle = Mathf.Max(openAngle - openAngle * Time.deltaTime * openSpeed, 0f);
            }
        }

        var quat = Quaternion.AngleAxis(-openAngle, transform.up);
        door.transform.rotation = transform.rotation * quat;
    }

    public void Open()
    {
        open = true;
    }

    public void Close()
    {
        open = false;
    }
}
