using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using TMPro;
public class NetworkPlayer : MonoBehaviour {

    public static NetworkPlayer localPlayer;

    [Header("Input")]
    public NetworkInputType[] networkInputs;
    public GameObject hitEffectPrefab;

    public GameObject playerPrefab;
    public GameObject playerObject { get; private set; }
    public PlayerController playerController;

    [HideInInspector]
    public string nickname;

    private Dictionary<string, NetworkInputState> inputStates;
    private bool clientInitialized = false;

    private void Awake()
    {
        nickname = "N/A";
    }

    private void Start()
    {
        if (networkInputs.Length > 0)
        {
            inputStates = new Dictionary<string, NetworkInputState>();

            foreach (var type in networkInputs)
            {
                var state = new NetworkInputState();
                state.value = 0f;
                state.key = type.key;
                state.axis = type.axis;

                inputStates.Add(type.key, state);
            }
        }

        localPlayer = this;
    }

    private void Update()
    {
        foreach (var type in networkInputs)
        {
            if (!inputStates.ContainsKey(type.key)) { continue; }
            var state = inputStates[type.key];

            float axis;

            if (type.axis)
            {
                axis = Input.GetAxis(type.key);
            }
            else
            {
                axis = Input.GetButton(type.key) ? 1 : 0;
            }

            if (axis != state.value)
            {
                state.value = axis;
            }
        }
    }

    public float GetInputValue(string key)
    {
        return inputStates[key].value;
    }

    private float lastInteract;

    public void CreatePlayerObject(Vector3 position)
    {
        if (playerObject)
        {
            Destroy(playerObject);
        }

        playerObject = Instantiate(playerPrefab);
        playerController = playerObject.GetComponent<PlayerController>();

        playerController.networkPlayer = this;
    }
}

[System.Serializable]
public class NetworkInputType
{
    public string key;
    public bool axis;
}

public class NetworkInputState
{
    public string key;
    public bool axis;
    public float value;
}