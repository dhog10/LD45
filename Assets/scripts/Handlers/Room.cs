using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Experimental.Rendering.HDPipeline;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class Room : MonoBehaviour
{
    public float roomSize = 40f;
    public float musicVolume = 1f;
    public float musicFadeMultiplier = 1f;
    public float ambientVolume = 1f;
    public bool doorStartOpen = false;
    public string completeText = "";
    public float completeFontSize = 1.8f;
    public AudioSource ambientSound;
    public bool musicSoundOn = true;
    public bool ambientSoundOn = true;
    public Room nextRoomOverride;
    public bool nextRoomEnabled;
    public GameObject[] completionObjects;    

    private Door door;
    private Portal portal;
    private PlayerController player;
    private Volume sceneSettings;
    private bool inRoom = false;
    private float fogEnd = 0f;
    private float fogStart = 0f;
    private bool complete = false;
    private float currentMusicVolume = 0f;
    private float currentAmbientVolume = 0f;

    private AudioSource musicAudio;
    
    public AudioSource MusicAudio => musicAudio;

    // Start is called before the first frame update
    public virtual void Start()
    {
        musicAudio = GetComponent<AudioSource>();
        if (musicAudio != null)
        {
            musicAudio.volume = currentMusicVolume;
        }

        if(ambientSound != null)
        {
            ambientSound.volume = currentAmbientVolume;
        }

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

        if (doorStartOpen)
        {
            door.Open(false);
        }

        var hiddenAppear = GetComponentInChildren<HiddenAppear>();
        if (hiddenAppear)
        {
            hiddenAppear.room = this;
        }
    }

    public virtual void Update()
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

            if (musicSoundOn)
            {
                currentMusicVolume = Mathf.Min(1f, currentMusicVolume + musicFadeMultiplier * Time.deltaTime);
            }
            else
            {
                currentMusicVolume = Mathf.Max(0f, currentMusicVolume - musicFadeMultiplier * Time.deltaTime);
            }

            if (ambientSoundOn)
            {
                currentAmbientVolume = Mathf.Min(1f, currentAmbientVolume + musicFadeMultiplier * Time.deltaTime);
            }
            else
            {
                currentAmbientVolume = Mathf.Max(0f, currentAmbientVolume - musicFadeMultiplier * Time.deltaTime);
            }

        }
        else
        {
            currentMusicVolume = Mathf.Max(0f, currentMusicVolume - musicFadeMultiplier * Time.deltaTime);
            currentAmbientVolume = Mathf.Max(0f, currentAmbientVolume - musicFadeMultiplier * Time.deltaTime);
        }

        if(ExitScreen.Instance != null && ExitScreen.Instance.HasExited())
        {
            currentMusicVolume = 0f;
            currentAmbientVolume = 0f;
        }

        if(musicAudio != null)
        {
            musicAudio.volume = currentMusicVolume * musicVolume;

            if(ambientSound != null)
            {
                ambientSound.volume = currentAmbientVolume * ambientVolume;
            }

            if(currentMusicVolume > 0f)
            {
                if (!musicAudio.isPlaying)
                {
                    musicAudio.Play();
                }
            }
            else
            {
                if (musicAudio.isPlaying)
                {
                    musicAudio.Stop();
                }
            }

            if(currentAmbientVolume > 0f)
            {
                if (ambientSound != null && !ambientSound.isPlaying)
                {
                    ambientSound.Play();
                }
            }
            else
            {
                if (ambientSound != null && ambientSound.isPlaying)
                {
                    ambientSound.Stop();
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

    public void Complete()
    {
        if (complete)
        {
            return;
        }

        complete = true;
        RoomHandler.Instance.IncrementRoom();
        RoomHandler.Instance.onRoomCompleted?.Invoke();

        if(completeText.Length > 0)
        {
            RoomHandler.Instance.painting.SetText(completeText);
            RoomHandler.Instance.painting.SetFontSize(completeFontSize);
        }

        if (completionObjects != null)
        {
            foreach(var obj in completionObjects)
            {
                obj.SetActive(true);
            }
        }
    }

    public bool IsComplete()
    {
        return complete;
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

    public void EnableAmbientSound()
    {
        ambientSoundOn = true;
    }

    public void DisableAmbientSound()
    {
        ambientSoundOn = false;
    }

    public void EnableMusic()
    {
        musicSoundOn = true;
    }

    public void DisableMusic()
    {
        musicSoundOn = false;
    }

    public void EnableNextRoom()
    {
        nextRoomEnabled = true;
    }
}
