using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperRoom : Room
{
    [Header("Super Room")]
    public int doorJebaitAmount = 4;
    public float doorJebaitDistance = 8f;
    public GameObject signPositions;
    public GameObject[] signs;

    private int doorJebaits = 0;
    private bool hasHooked = false;
    private bool firstDoorJebait = true;
    private int signIndex = -1;
    private GameObject currentSign;

    public override void Start()
    {
        base.Start();

        foreach(var obj in signs)
        {
            var appear = obj.GetComponent<HiddenAppear>();
            appear.Hide();
            appear.Disable();

            Debug.Log("hide " + appear.gameObject.name);
            var signSpawns = new GameObject[signPositions.transform.childCount];
            for(var i = 0; i < signPositions.transform.childCount; i++)
            {
                signSpawns[i] = signPositions.transform.GetChild(i).gameObject;
            }

            appear.SetPositions(signSpawns, false, true);
        }

        this.NextSign();
    }

    public override void Update()
    {
        base.Update();

        if(RoomHandler.Instance == null)
        {
            return;
        }

        var player = RoomHandler.Instance.player;
        var door = this.GetDoor();

        if (player != null && door != null)
        {
            if (!hasHooked)
            {
                var hider = door.transform.parent.GetComponentInChildren<HiddenAppear>(true);

                if(hider != null)
                {
                    hasHooked = true;

                    hider.onHide.AddListener(() =>
                    {
                        door.Open(false);
                    });
                }
            }
            var distance = Vector3.Distance(player.transform.position, door.transform.position);

            if (firstDoorJebait)
            {
                if(distance <= 2f)
                {
                    distance = 100f;
                }
            }

            if(doorJebaits < doorJebaitAmount && door.IsOpen() && distance <= doorJebaitDistance)
            {
                if(signIndex >= signs.Length)
                {
                    doorJebaits++;
                }

                door.Close(false);
                firstDoorJebait = false;
            }
        }
    }

    public void NextSign()
    {
        signIndex++;

        if(signIndex >= signs.Length)
        {
            return;
        }

        var sign = signs[signIndex];
        var appear = sign.GetComponent<HiddenAppear>();
        var signComponent = sign.GetComponent<Sign>();

        appear.Enable();
        signComponent.onFall.AddListener(() =>
        {
            appear.Disable();
            this.NextSign();
        });
    }
}
