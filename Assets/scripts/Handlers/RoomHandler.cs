using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomHandler : MonoBehaviour
{
    public GameObject hubRoom;
    public GameObject[] rooms;

    private GameObject currentRoom;

    // Start is called before the first frame update
    void Start()
    {
        // this.SpawnRoom(rooms[0]);
    }

    public void SpawnRoom(GameObject room)
    {
        currentRoom = room;
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
