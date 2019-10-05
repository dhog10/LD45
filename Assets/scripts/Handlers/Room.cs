using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    private Door door;
    private Portal portal;

    // Start is called before the first frame update
    void Start()
    {
        door = transform.GetComponentInChildren<Door>(true);
        portal = transform.GetComponentInChildren<Portal>(true);
    }

    public Door GetDoor()
    {
        return door ?? transform.GetComponentInChildren<Door>(true);
    }

    public Portal GetPortal()
    {
        return portal ?? transform.GetComponentInChildren<Portal>(true);
    }
}
