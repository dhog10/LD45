using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenAppear : MonoBehaviour
{
    [Tooltip("The visibility collider used when hidden.")]
    public GameObject hideTriggerObject;
    [Tooltip("The visibility collider used when visible.")]
    public GameObject showTriggerObject;
    public GameObject disableObject;
    public GameObject enableObject;

    public bool startHidden = true;
    [Tooltip("Minimum time it will stay hidden for.")]
    public float hideCooldown = 5f;
    [Tooltip("Random minimum time it will stay hidden for.")]
    public float hideCooldownRandom = 10f;
    [Tooltip("Minimum time it will stay visible for.")]
    public float showCooldown = 5f;
    [Tooltip("Random minimum time it will stay visible for.")]
    public float showCooldownRandom = 10f;

    private bool hidden = false;
    private float lastSeen = 0f;
    private float lastHidden = 0f;
    private bool wasSeen = false;
    private bool hiderEnabled = true;

    private float hideTime = 0f;
    private float showTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        wasSeen = !startHidden;
        this.GenerateTimes();

        if (startHidden)
        {
            this.Hide();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!hiderEnabled)
        {
            return;
        }

        if (this.IsSeen())
        {
            if (!hidden)
            {
                lastSeen = Time.time;
            }

            wasSeen = true;
        }
        else
        {
            if (hidden)
            {
                lastHidden = Time.time;

                if (wasSeen)
                {
                    if (Time.time - lastSeen >= hideTime)
                    {
                        this.Appear();
                    }
                }
            }
            else
            {
                if (wasSeen)
                {
                    if (Time.time - lastHidden >= showTime)
                    {
                        this.Hide();
                    }
                }
            }
        }
    }

    public void Appear()
    {
        disableObject.SetActive(true);

        if(showTriggerObject != null)
        {
            hideTriggerObject.SetActive(false);
        }

        hidden = false;
        this.GenerateTimes();
    }

    public void Hide()
    {
        disableObject.SetActive(false);

        if (showTriggerObject != null)
        {
            hideTriggerObject.SetActive(true);
        }

        hidden = true;
        this.GenerateTimes();
    }

    public bool IsSeen()
    {
        if(VisibilityHandler.Instance == null)
        {
            return false;
        }

        var camera = Camera.main;

        if(camera == null)
        {
            return false;
        }

        if (hideTriggerObject != null && VisibilityHandler.Instance.IsSeen(hideTriggerObject))
        {
            return true;
        }

        if (showTriggerObject != null && VisibilityHandler.Instance.IsSeen(showTriggerObject))
        {
            return true;
        }

        return false;
    }

    public void Enable()
    {
        hiderEnabled = true;
    }

    public void Disable()
    {
        hiderEnabled = false;
    }

    private void GenerateTimes()
    {
        hideTime = hideCooldown + Random.Range(0f, hideCooldownRandom);
        showTime = showCooldown + Random.Range(0f, showCooldownRandom);
    }
}
