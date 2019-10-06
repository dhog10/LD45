using System.Collections;
using UnityEngine;

public class Piano : MonoBehaviour
{
    [SerializeField] private float keyDistance = 0.05f;
    [SerializeField] private float keyDuration = 1.0f;
    [SerializeField] private AnimationCurve keyCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
    [SerializeField] private string keyTag = "Key";
    [SerializeField] private float maxPressDistance = 5.0f;
    [SerializeField] private Material hoverMaterial;
    [Space(5)]
    [SerializeField] private GameObject c1;
    [SerializeField] private GameObject d1;
    [SerializeField] private GameObject e1;
    [SerializeField] private GameObject f1;
    [SerializeField] private GameObject g1;
    [SerializeField] private GameObject a1;
    [SerializeField] private GameObject b1;
    [SerializeField] private GameObject c2;
    [SerializeField] private GameObject d2;
    [SerializeField] private GameObject e2;
    [SerializeField] private GameObject f2;
    [SerializeField] private GameObject g2;
    [SerializeField] private GameObject a2;
    [SerializeField] private GameObject b2;
    private GameObject[] keys;
    private float originalKeyYPosition;
    private Material originalMaterial;

    private void OnValidate()
    {
        var keys = new GameObject[] { c1, d1, e1, f1, g1, a1, b1, c2, d2, e2, f2, g2, a2, b2 };
        for (var i = 0; i < keys.Length; i++)
        {
            if (!keys[i].GetComponent<AudioSource>())
            {
                keys[i] = null;
                Debug.LogWarning($"{keys[i].name} must contain an audio source component");
            }
            else if (!keys[i].GetComponent<Renderer>())
            {
                keys[i] = null;
                Debug.LogWarning($"{keys[i].name} must contain a renderer component");
            }
        }
    }

    private void Start()
    {
        keys = new GameObject[] { c1, d1, e1, f1, g1, a1, b1, c2, d2, e2, f2, g2, a2, b2 };
        originalKeyYPosition = c1.transform.position.y;
        originalMaterial = c1.GetComponent<Renderer>().material;
    }

    private void Update()
    {
        foreach (var key in keys)
        {
            var keyRenderer = key.GetComponent<Renderer>();
            if (keyRenderer.sharedMaterial == hoverMaterial)
            {
                keyRenderer.material = originalMaterial;
            }
        }

        if (Camera.main &&
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, maxPressDistance) &&
            string.Equals(hit.transform.gameObject.tag, keyTag) &&
            hit.transform.position.y == originalKeyYPosition)
        {
            var hitKey = hit.transform.gameObject;

            hitKey.GetComponent<Renderer>().material = hoverMaterial;

            if (Input.GetMouseButtonDown(0))
            {
                hitKey.GetComponent<AudioSource>().Play();
                StartCoroutine(this.KeyPress(hitKey.transform));
            }
        }
    }

    private IEnumerator KeyPress(Transform key)
    {
        var originalPosition = key.position;
        var pressedPosition = new Vector3(originalPosition.x,
                                          originalPosition.y - keyDistance,
                                          originalPosition.z);

        var t = 0.0f;
        while (t <= 1.0f)
        {
            key.position = Vector3.Lerp(originalPosition, pressedPosition, keyCurve.Evaluate(t));

            t += Time.deltaTime / keyDuration;

            yield return new WaitForEndOfFrame();
        }
        key.position = originalPosition;
    }
}
