using UnityEngine;

/// <summary>
/// Basic object that can be picked up by the player (must have rigid body). 
/// Attach a PickUpPoint Transform to the First Person Controller where it will be held.
/// </summary>
public class PickedObject : MonoBehaviour
{

    [SerializeField] private Transform pickUpPoint;
    [SerializeField] private GameObject pickUpObject;
    [SerializeField] private int force;

    private bool pickedUp = false;
    private Transform originalParent;

    void Awake()
    {

        ClickDelegate.Instance.OnClick += OnClick;

    }

    void Start()
    {
        originalParent = transform.parent;
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(1))
        {
            DropClick();
        }

    }

    void OnClick(GameObject obj)
    {
        if (obj == gameObject)
        {
            this.PickUp();
        }
    }
    
    void EnableColliders(bool enabled)
    {
        foreach(var collider in this.GetComponentsInChildren<Collider>())
        {
            collider.enabled = enabled;
        }
    }

    void PickUp()
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
                this.EnableColliders(false);
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
                var forceVector = Camera.main.transform.forward;

                objRigidbody.useGravity = true;
                objRigidbody.isKinematic = false;
                objRigidbody.AddForce(forceVector * force);
            }

            this.transform.parent = originalParent;
            pickedUp = false;
            this.EnableColliders(true);
        }
    }
}

