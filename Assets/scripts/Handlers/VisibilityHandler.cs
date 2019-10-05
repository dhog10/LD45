using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityHandler : MonoBehaviour
{
    public static VisibilityHandler Instance;

    private Dictionary<GameObject, bool> hitDictionary = new Dictionary<GameObject, bool>();

    private float lastCheck = 0f;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Time.time - lastCheck < 0.2f)
        {
            return;
        }

        this.UpdateVisibility();
    }

    public void UpdateVisibility()
    {
        lastCheck = Time.time;

        hitDictionary.Clear();

        var camera = Camera.main;
        if (camera == null)
        {
            return;
        }

        for (var x = -100; x < Screen.width + 100; x += 50)
        {
            for (var y = -100; y < Screen.height + 100; y += 50)
            {
                RaycastHit hit;
                var ray = camera.ScreenPointToRay(new Vector3(x, y, 0));

                if (Physics.Raycast(ray, out hit))
                {
                    var objectHit = hit.collider;

                    if (!hitDictionary.ContainsKey(objectHit.gameObject))
                    {
                        hitDictionary.Add(objectHit.gameObject, true);
                    }
                }
            }
        }
    }

    public bool IsSeen(GameObject obj)
    {
        return hitDictionary.ContainsKey(obj);
    }
}
