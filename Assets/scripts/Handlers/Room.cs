using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    private Door door;
    // Start is called before the first frame update
    void Start()
    {
        door = transform.GetComponentInChildren<Door>();
    }

    public Door GetDoor()
    {
        return door;
    }
}
