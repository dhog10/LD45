using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenAppear : MonoBehaviour
{
    public GameObject hideTriggerObject;
    public GameObject disableObject;

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
        if (this.IsSeen())
        {
            Debug.Log("seen");

            if (!hidden)
            {
                lastSeen = Time.time;
            }

            wasSeen = true;
        }
        else
        {
            Debug.Log("hidden");

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

        hidden = false;
        this.GenerateTimes();
    }

    public void Hide()
    {
        disableObject.SetActive(false);

        hidden = true;
        this.GenerateTimes();
    }

    public bool IsSeen()
    {
        var camera = Camera.main;

        if(camera == null)
        {
            return false;
        }

        return VisibilityHandler.Instance.IsSeen(hideTriggerObject);
    }

    private void GenerateTimes()
    {
        hideTime = hideCooldown + Random.Range(0f, hideCooldownRandom);
        showTime = showCooldown + Random.Range(0f, showCooldownRandom);
    }
}
