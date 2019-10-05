using UnityEngine;

public class ClickDelegate : MonoBehaviour
{
    [SerializeField] private int pickUpRange;

    public delegate void Clicker(GameObject obj, RaycastHit rayHit);
    public event Clicker OnClick;

    private static ClickDelegate instance;
    
    private ClickDelegate() { }

    public static ClickDelegate Instance
    {
        get
        {
            if (instance == null)
            {

                instance = GameObject.FindObjectOfType(typeof(ClickDelegate)) as ClickDelegate;

            }

            return instance;
        }
    }

    // Update is called once per frame
    void Update()
    {
        var raycast = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;

        if (Physics.Raycast(raycast, out rayHit, pickUpRange))
        {
            // If we click it
            if (Input.GetButtonDown("Fire1"))
            {
                // Notify of the event!
                OnClick(rayHit.transform.gameObject, rayHit);

            }
        }
    }
}
