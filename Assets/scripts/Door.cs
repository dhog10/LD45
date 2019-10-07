using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public float openSpeed = 2f;
    public float openAngleAmount = 90f;
    public GameObject door;
    public AudioSource openSound;
    public AudioSource closeSound;
    public bool neverClose = false;

    private float openAngle = 0f;
    private bool open = false;
    private Portal portal;
    
    private void Start()
    {
        portal = GetComponentInChildren<Portal>();

        if (portal)
        {
            portal.door = this;
        }

        if (!open)
        {
            this.Close();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (open)
        {
            if (openAngle < openAngleAmount)
            {
                openAngle = Mathf.Min(openAngle + (openAngleAmount - openAngle) * Time.deltaTime * openSpeed, openAngleAmount);
            }
        }
        else
        {
            if (openAngle > 0f)
            {
                openAngle = Mathf.Max(openAngle - openAngle * Time.deltaTime * openSpeed, 0f);
            }
        }

        var quat = Quaternion.AngleAxis(-openAngle, transform.up);
        door.transform.rotation = transform.rotation * quat;

        if (portal != null && portal.IsPortalEnabled() && !this.IsOpen())
        {
            portal.DisablePortal();
        }
    }

    public void Open(bool playSound = true)
    {
        if (playSound && !open && openSound)
        {
            openSound.Stop();
            openSound.Play();
        }

        open = true;

        if (portal)
        {
            portal.EnablePortal();

            if(portal.destination.door != null && !portal.destination.door.IsOpen())
            {
                portal.destination.door.Open();
            }
        }
    }

    public void Close(bool playSound = true)
    {
        if (neverClose)
        {
            return;
        }

        if (playSound && open && closeSound)
        {
            closeSound.Stop();
            closeSound.Play();
        }

        open = false;
    }

    public bool IsOpen()
    {
        return open;
    }

    public bool IsFullyClosed()
    {
        return openAngle < 1f;
    }
}
