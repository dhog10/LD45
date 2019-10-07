using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeModelWhenClose : MonoBehaviour
{
    public float distance = 1f;
    public GameObject triggerObject;
    public GameObject changeObject;
    public GameObject hideObject;

    void Update()
    {
        if(triggerObject == null)
        {
            return;
        }

        var dist = Vector3.Distance(hideObject.transform.position, triggerObject.transform.position);

        if(triggerObject.activeSelf && dist <= distance)
        {
            if (hideObject.activeSelf)
            {
                hideObject.SetActive(false);
            }

            if (!changeObject.activeSelf)
            {
                changeObject.SetActive(true);
            }
        }
        else
        {
            if (!hideObject.activeSelf)
            {
                hideObject.SetActive(true);
            }

            if (changeObject.activeSelf)
            {
                changeObject.SetActive(false);
            }
        }
    }
}
