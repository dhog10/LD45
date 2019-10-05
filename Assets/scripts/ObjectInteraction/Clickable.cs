using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Clickable : MonoBehaviour
{
    public float range = 5f;

    public UnityEvent onClick;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var camera = Camera.main;

        if(camera == null)
        {
            return;
        }

        var clicked = Input.GetMouseButtonDown(0);

        if (!clicked)
        {
            return;
        }

        var raycast = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;

        if (Physics.Raycast(raycast, out rayHit, range))
        {            
            if(rayHit.rigidbody.gameObject == gameObject)
            {
                onClick.Invoke();
            }
        }
    }
}
