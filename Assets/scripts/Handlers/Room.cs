﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.HDPipeline;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class Room : MonoBehaviour
{
    public float roomSize = 40f;

    private Door door;
    private Portal portal;
    private PlayerController player;
    private Volume sceneSettings;
    private bool inRoom = false;
    private float fogEnd = 0f;
    private float fogStart = 0f;

    // Start is called before the first frame update
    void Start()
    {
        door = transform.GetComponentInChildren<Door>(true);
        portal = transform.GetComponentInChildren<Portal>(true);
        sceneSettings = FindObjectOfType<Volume>();

        for (var i = 0; i < sceneSettings.profile.components.Count; i++)
        {
            if (sceneSettings.profile.components[i].name == "LinearFog(Clone)")
            {
                var fog = (LinearFog)sceneSettings.profile.components[i];
                fogEnd = fog.fogEnd.value;
                fogStart = fog.fogStart.value;
            }
        }

        portal.onTeleportTo.AddListener(() =>
        {
            this.EnterRoom();
        });
    }

    private void Update()
    {
        Debug.DrawLine(transform.position, transform.position + transform.forward * roomSize);

        if (!Application.isPlaying)
        {
            return;
        }

        if(player == null)
        {
            player = FindObjectOfType<PlayerController>();
        }

        if(player == null)
        {
            return;
        }

        if (inRoom)
        {
            var distance = Vector3.Distance(transform.position, player.transform.position);

            if (distance > roomSize)
            {
                var relativePosition = player.transform.position - transform.position;
                relativePosition = -relativePosition * 0.95f;
                relativePosition.y = -relativePosition.y;

                player.transform.position = transform.position + relativePosition;
            }

            var p = 1 - (distance / roomSize);
            RenderSettings.fogEndDistance = p * fogEnd;
            
            for (var i = 0; i < sceneSettings.profile.components.Count; i++)
            {
                if (sceneSettings.profile.components[i].name == "LinearFog(Clone)")
                {
                    var fog = (LinearFog)sceneSettings.profile.components[i];
                    fog.fogEnd.value = p * fogEnd;
                    fog.fogStart.value = p * fogStart;
                }
            }
        }
    }

    public Door GetDoor()
    {
        return door ?? transform.GetComponentInChildren<Door>(true);
    }

    public Portal GetPortal()
    {
        return portal ?? transform.GetComponentInChildren<Portal>(true);
    }

    public void EnterRoom()
    {
        foreach(var room in FindObjectsOfType<Room>())
        {
            room.ExitRoom();
        }

        inRoom = true;
    }

    public void ExitRoom()
    {
        inRoom = false;
    }
}
