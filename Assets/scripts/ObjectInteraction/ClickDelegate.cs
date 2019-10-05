using UnityEngine;

/// <summary>
/// ClickDelegate allows us to interact with different objects where we need and how we need it to as this is generic.
/// </summary>
public class ClickDelegate : MonoBehaviour
{
    [SerializeField] private int pickUpRange;

    public delegate void Clicker(GameObject obj);
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

    void Update()
    {

        var raycast = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;

        if (Physics.Raycast(raycast, out rayHit, pickUpRange))
        {

            if (Input.GetButtonDown("Fire1"))
            {

                OnClick(rayHit.transform.gameObject);

            }

        }
    }
}
