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
        if(Time.time - lastCheck < 0.1f)
        {
            return;
        }

        lastCheck = Time.time;

        hitDictionary.Clear();

        var camera = Camera.main;
        if(camera == null)
        {
            return;
        }

        for(var x = 0; x < Screen.width; x += 80)
        {
            for(var y = 0; y < Screen.height; y += 80)
            {
                RaycastHit hit;
                var ray = camera.ScreenPointToRay(new Vector3(x, y, 0));

                if (Physics.Raycast(ray, out hit))
                {
                    if (!hit.collider.isTrigger)
                    {
                        continue;
                    }

                    var objectHit = hit.transform;

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
