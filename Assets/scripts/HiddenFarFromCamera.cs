using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenFarFromCamera : MonoBehaviour
{
    public GameObject hideObject;
    public float distance = 20f;

    void FixedUpdate()
    {
        var camera = Camera.main;

        if(camera == null)
        {
            return;
        }

        var dist = Vector3.Distance(transform.position, camera.transform.position);

        if (dist >= distance)
        {
            if (hideObject.activeSelf)
            {
                hideObject.SetActive(false);
            }
        }
        else
        {
            if (!hideObject.activeSelf)
            {
                hideObject.SetActive(true);
            }
        }
    }
}
