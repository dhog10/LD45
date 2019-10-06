using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomHandler : MonoBehaviour
{
    public static RoomHandler Instance;

    public GameObject hubRoom;
    public GameObject[] rooms;

    private GameObject currentRoom;
    private Room hubComponent;
    private int roomIndex = 0;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        hubComponent = hubRoom.GetComponent<Room>();
        hubComponent.EnterRoom();

        this.SpawnRoom(rooms[roomIndex]);
    }

    public void IncrementRoom()
    {
        roomIndex++;
    }

    public void SpawnRoom(GameObject room)
    {
        if(hubComponent == null)
        {
            return;
        }

        currentRoom = room;

        var roomComponent = room.GetComponent<Room>();

        roomComponent.GetPortal().destination = hubComponent.GetPortal();
        hubComponent.GetPortal().destination = roomComponent.GetPortal();

        this.PrepareRooms();
    }

    public void SpawnCurrentRoom()
    {
        if(rooms[roomIndex] != currentRoom)
        {
            this.SpawnRoom(rooms[roomIndex]);
        }
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
