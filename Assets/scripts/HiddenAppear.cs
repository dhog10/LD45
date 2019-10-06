using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HiddenAppear : MonoBehaviour
{
    [Tooltip("The visibility collider used when hidden.")]
    public GameObject hideTriggerObject;
    [Tooltip("The visibility collider used when visible.")]
    public GameObject showTriggerObject;
    public GameObject disableObject;
    public GameObject enableObject;

    public float distanceThreshold = 0f;
    public bool startHidden = true;
    [Tooltip("Minimum time it will stay hidden for.")]
    public float hideCooldown = 5f;
    [Tooltip("Random minimum time it will stay hidden for.")]
    public float hideCooldownRandom = 10f;
    [Tooltip("Minimum time it will stay visible for.")]
    public float showCooldown = 5f;
    [Tooltip("Random minimum time it will stay visible for.")]
    public float showCooldownRandom = 10f;

    [Tooltip("Other hiddens which must be visible for this to appear.")]
    public HiddenAppear[] requiredAppears;
    public GameObject[] spawnPositions;

    public UnityEvent onHide;
    public UnityEvent onAppear;
    public UnityEvent onHideDoorClosed;
    public UnityEvent onHideRoomIncomplete;

    [HideInInspector]
    public Room room;

    private List<SpawnPosition> spawns;

    private bool hidden = false;
    public bool startEnabled = true;
    private float lastSeen = 0f;
    private float lastHidden = 0f;
    private bool wasSeen = false;
    private bool hiderEnabled = true;
    private bool seenSinceHidden = true;
    private bool alwaysHidden = false;

    private float hideTime = 0f;
    private float showTime = 0f;

    public bool Hidden => hidden;

    // Start is called before the first frame update
    void Start()
    {
        if (!startEnabled)
        {
            this.Disable();
        }

        spawns = new List<SpawnPosition>();

        spawns.Add(new SpawnPosition()
        {
            pos = transform.position,
            ang = transform.rotation,
        });

        foreach(var spawn in spawnPositions)
        {
            spawns.Add(new SpawnPosition()
            {
                pos = spawn.transform.position,
                ang = spawn.transform.rotation,
            });
        }

        wasSeen = !startHidden;
        this.GenerateTimes();

        if (startEnabled && startHidden)
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
                seenSinceHidden = true;
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
                if (alwaysHidden)
                {
                    this.Hide();
                    return;
                }

                foreach (var hidden in requiredAppears)
                {
                    if (hidden.Hidden)
                    {
                        this.Hide();
                    }
                }

                if (seenSinceHidden && wasSeen)
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
        if (alwaysHidden)
        {
            return;
        }

        if (this.IsDefinitelyVisible())
        {
            return;
        }

        var camera = Camera.main;
        if (distanceThreshold > 0f && camera != null)
        {
            var distance = Vector3.Distance(camera.transform.position, transform.position);

            if (distance <= distanceThreshold)
            {
                return;
            }
        }

        foreach (var hidden in requiredAppears)
        {
            if (hidden.Hidden)
            {
                lastSeen = Time.time;
                return;
            }
        }

        VisibilityHandler.Instance.UpdateVisibility();
        if (this.IsSeen())
        {
            return;
        }

        disableObject.SetActive(true);

        if(showTriggerObject != null)
        {
            hideTriggerObject.SetActive(false);
        }

        hidden = false;
        this.GenerateTimes();
        seenSinceHidden = false;
        this.onAppear?.Invoke();
    }

    public void Hide(bool force = false)
    {
        if (!force && this.IsDefinitelyVisible())
        {
            return;
        }

        var camera = Camera.main;
        if(!force && distanceThreshold > 0f && camera != null)
        {
            var distance = Vector3.Distance(camera.transform.position, transform.position);
            if (distance <= distanceThreshold)
            {
                return;
            }
        }

        if(disableObject != null)
        {
            disableObject.SetActive(false);
        }

        if (showTriggerObject != null)
        {
            hideTriggerObject.SetActive(true);
        }

        hidden = true;
        this.GenerateTimes();
        this.PickPosition();
        this.onHide?.Invoke();

        var door = this.FindDoorInParent(gameObject);
        if(door && !door.IsOpen())
        {
            onHideDoorClosed?.Invoke();
        }

        if(room && !room.IsComplete())
        {
            onHideRoomIncomplete?.Invoke();
        }
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

    public void MakeAlwaysHidden(bool alwaysHidden)
    {
        this.alwaysHidden = alwaysHidden;
    }

    private void GenerateTimes()
    {
        hideTime = hideCooldown + Random.Range(0f, hideCooldownRandom);
        showTime = showCooldown + Random.Range(0f, showCooldownRandom);
    }

    private void PickPosition()
    {
        var index = Random.Range(0, spawns.Count - 1);

        var spawn = spawns[index];

        transform.position = spawn.pos;
        transform.rotation = spawn.ang;
    }

    private bool IsDefinitelyVisible()
    {
        var camera = Camera.main;
        if(camera == null)
        {
            return false;
        }

        RaycastHit hit;
        var fromPosition = camera.transform.position;
        var toPosition = transform.position;
        var direction = toPosition - fromPosition;

        var dot = Vector3.Dot(Vector3.Normalize(direction), camera.transform.forward);
        if(dot < 0.4f)
        {
            return false;
        }

        if (Physics.Raycast(fromPosition, direction * 2f, out hit))
        {
            return true;
        }

        return false;
    }

    private Door FindDoorInParent(GameObject obj, int tries = 0)
    {
        if(tries > 20)
        {
            return null;
        }

        var door = obj.GetComponent<Door>();

        if (door)
        {
            return door;
        }

        if (transform.parent && transform.parent != transform)
        {
            return this.FindDoorInParent(transform.parent.gameObject, tries + 1);
        }

        return null;
    }
}

public class SpawnPosition
{
    public Vector3 pos;
    public Quaternion ang;
}