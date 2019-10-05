using UnityEngine;

/// <summary>
/// Basic object that can be picked up by the player (must have rigid body). 
/// Attach a PickUpPoint Transform to the First Person Controller where it will be held.
/// </summary>
public class PickedObject : MonoBehaviour
{

    [SerializeField] private Transform pickUpPoint;
    [SerializeField] private GameObject pickUpObject;

    private bool pickedUp = false;

    void Awake()
    {

        ClickDelegate.Instance.OnClick += OnClick;

    }

    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            DropClick();
        }

    }

    void OnClick(GameObject obj)
    {
        if (obj == gameObject)
        {
            if (pickedUp == false)
            {

                var objRigidbody = GetComponent<Rigidbody>();

                if (objRigidbody.isKinematic == false)
                {

                    objRigidbody.useGravity = false;
                    objRigidbody.isKinematic = true;

                    this.transform.position = pickUpPoint.position;
                    this.transform.parent = pickUpObject.transform;
                    pickedUp = true;

                }
            }
        }
    }

    void DropClick()
    {   
        if (pickedUp)
        {

            var objRigidbody = GetComponent<Rigidbody>();

            if (objRigidbody.isKinematic == true)
            {

                objRigidbody.useGravity = true;
                objRigidbody.isKinematic = false;

            }

            this.transform.parent = null;
            pickedUp = false;

        }
    }
}

