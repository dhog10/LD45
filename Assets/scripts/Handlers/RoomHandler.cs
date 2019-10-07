using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoomHandler : MonoBehaviour
{
    public static RoomHandler Instance;

    public GameObject hubRoom;
    public GameObject propsObject;
    public Painting painting;
    public int startingRoom = 0;
    public GameObject[] rooms;
    public UnityEvent onRoomCompleted;

    [HideInInspector]
    public PlayerController player;

    private GameObject currentRoom;
    private Room hubComponent;
    private int roomIndex = 0;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        roomIndex = Mathf.Min(rooms.Length - 1, startingRoom);

        hubComponent = hubRoom.GetComponent<Room>();
        hubComponent.EnterRoom();

        this.SpawnRoom(rooms[roomIndex]);

        if(propsObject != null)
        {
            for(var i = 0; i < propsObject.transform.childCount; i++)
            {
                var child = propsObject.transform.GetChild(i);
                child.gameObject.SetActive(false);
            }
        }

        player = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        if(player == null)
        {
            player = FindObjectOfType<PlayerController>();
        }

        if(currentRoom != null)
        {
            var room = currentRoom.GetComponent<Room>();

            var overrideRoom = room.nextRoomOverride;
            if(overrideRoom != null)
            {
                if (room.nextRoomEnabled)
                {
                    if (!overrideRoom.gameObject.activeSelf)
                    {
                        overrideRoom.gameObject.SetActive(true);

                        var overrideDoor = overrideRoom.GetDoor();
                        if(overrideDoor != null)
                        {
                            room.GetPortal().destination = overrideRoom.GetPortal();
                            overrideRoom.GetPortal().destination = room.GetPortal();
                            overrideRoom.GetDoor().Open(false);
                        }
                    }
                    else
                    {
                        var overrideDoor = overrideRoom.GetDoor();
                        if (overrideDoor != null)
                        {
                            room.GetPortal().destination = overrideRoom.GetPortal();
                            overrideRoom.GetPortal().destination = room.GetPortal();
                            overrideRoom.GetDoor().Open(false);
                        }
                    }
                }
                else
                {
                    if (overrideRoom.gameObject.activeSelf)
                    {
                        overrideRoom.gameObject.SetActive(false);
                    }
                }
            }
        }
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

        if (!roomComponent.nextRoomEnabled)
        {
            roomComponent.GetPortal().destination = hubComponent.GetPortal();
            hubComponent.GetPortal().destination = roomComponent.GetPortal();
        }

        this.PrepareRooms();
    }

    public void SpawnCurrentRoom()
    {
        if(rooms[roomIndex] != currentRoom)
        {
            this.SpawnRoom(rooms[roomIndex]);
            hubComponent.GetDoor().Close();
            painting.ResetPainting();
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
