using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomHandler : MonoBehaviour
{
    public GameObject hubRoom;
    public GameObject[] rooms;

    private GameObject currentRoom;
    private Room hubComponent;

    // Start is called before the first frame update
    void Start()
    {
        hubComponent = hubRoom.GetComponent<Room>();

        this.SpawnRoom(rooms[0]);
    }

    public void SpawnRoom(GameObject room)
    {
        currentRoom = room;

        var roomComponent = room.GetComponent<Room>();
        roomComponent.GetPortal().destination = hubComponent.GetPortal();
        hubComponent.GetPortal().destination = roomComponent.GetPortal();

        this.PrepareRooms();
    }

    private void PrepareRooms()
    {
        foreach(var room in rooms)
        {
            if(room == currentRoom)
            {
                if (!room.activeSelf)
                {
                    room.SetActive(true);
                }
            }
            else
            {
                if (room.activeSelf)
                {
                    room.SetActive(false);
                }
            }
        }
    }
}
