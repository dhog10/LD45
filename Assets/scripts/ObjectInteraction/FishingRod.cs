using System.Collections;
using UnityEngine;

public class FishingRod : MonoBehaviour
{
    [SerializeField] private string waterTag = "Water";
    [SerializeField] private GameObject rodTip;
    [SerializeField] private LineRenderer line;
    [SerializeField] private GameObject bobberPrefab;
    [SerializeField] private GameObject splashPrefab;
    [SerializeField] private GameObject fishPrefab;
    [SerializeField] private GameObject targetPrefab;
    [Space(5)]
    [SerializeField] private AnimationCurve bobberCurveXZ = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
    [SerializeField] private AnimationCurve bobberCurveY = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
    [SerializeField] private float minCastDuration = 1.0f;
    [SerializeField] private float maxCastDuration = 2.0f;
    [SerializeField] private float minCastDistance = 3.0f;
    [SerializeField] private float maxCastDistance = 15.0f;
    [SerializeField] private float initialBobDuration = 0.5f;
    [SerializeField] private float initialBobDistance = 0.15f;
    [SerializeField, Tooltip("The cooldown between line casts (in seconds)")] private float cooldown = 1.0f;
    [SerializeField] private float targetOffsetY = 0.1f;
    private GameObject bobberInstance;
    private GameObject fishInstance;
    private GameObject targetInstance;
    private string bobberName = "Bobber";
    private string fishName = "Fish";
    private bool canUseRod = true;
    private IEnumerator castLine;

    private void OnValidate()
    {
        if (!rodTip.GetComponent<Rigidbody>())
        {
            rodTip = null;
            Debug.LogWarning("Rod tip must contain a rigidbody component");
        }

        if (line.useWorldSpace)
        {
            line.useWorldSpace = false;
            Debug.LogWarning("LineRenderer prefab must contain a particle system component");
        }

        if (!bobberPrefab.GetComponent<BobberScript>())
        {
            bobberPrefab = null;
            Debug.LogWarning("Bobber prefab must contain a bobber script component");
        }

        if (!splashPrefab.GetComponent<ParticleSystem>())
        {
            splashPrefab = null;
            Debug.LogWarning("Splash prefab must contain a particle system component");
        }

        if (!fishPrefab.GetComponent<SpringJoint>())
        {
            fishPrefab = null;
            Debug.LogWarning("Fish prefab must contain a spring joint component");
        }

        if (!targetPrefab.GetComponent<Renderer>())
        {
            targetPrefab = null;
            Debug.LogWarning("Target prefab must contain a renderer component");
        }
    }

    private void OnEnable()
    {
        line.enabled = false;

        castLine = this.CastLine(Vector3.zero, Vector3.zero);

        targetInstance = Instantiate(targetPrefab);
        targetInstance.hideFlags = HideFlags.HideInHierarchy;
        targetInstance.GetComponent<Renderer>().enabled = false;
    }

    private void Update()
    {
        if(rodTip == null || !rodTip.activeInHierarchy)
        {
            return;
        }

        // Checking if player can cast rod & generating hit point
        var canCast = false;
        var hit = new RaycastHit();
        if (Camera.main &&
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000.0f) &&
            string.Equals(hit.transform.gameObject.tag, waterTag))
        {
            var castDistance = Vector3.Distance(rodTip.transform.position, hit.point);
            if (castDistance >= minCastDistance &&
                castDistance <= maxCastDistance)
            {
                canCast = true;
            }
        }

        // Showing/hiding the target
        if (canCast && !bobberInstance && (!fishInstance || !fishInstance.GetComponent<SpringJoint>()))
        {
            this.SetTarget(hit.point);
        }
        else
        {
            targetInstance.GetComponent<Renderer>().enabled = false;
        }

        // Casting/pulling the fishing line
        if (canUseRod &&
            (!fishInstance ||
             !fishInstance.GetComponent<SpringJoint>()))
        {
            if (Input.GetMouseButtonDown(0) && !bobberInstance)
            {
                Destroy(bobberInstance);

                if (canCast)
                {
                    StopCoroutine(castLine);
                    castLine = this.CastLine(rodTip.transform.position, hit.point);
                    StartCoroutine(castLine);
                }
                else
                {
                    line.enabled = false;
                }
            }
            else if (Input.GetMouseButtonDown(0) && bobberInstance)
            {
                this.PullLine();
            }
        }
        else if (Input.GetMouseButtonDown(0) &&
                 fishInstance &&
                 fishInstance.GetComponent<SpringJoint>())
        {
            this.DropFish();
        }

        // Updating fishing line
        if (line.enabled)
        {
            this.UpdateLinePositions();
        }
    }

    private void SetTarget(Vector3 targetPosition)
    {
        targetInstance.transform.position = new Vector3(targetPosition.x,
                                                        targetPosition.y + targetOffsetY,
                                                        targetPosition.z);
        targetInstance.GetComponent<Renderer>().enabled = true;
    }

    private void UpdateLinePositions()
    {
        line.SetPosition(0, line.transform.InverseTransformPoint(rodTip.transform.position));

        var target = Vector3.zero;
        if (bobberInstance)
        {
            target = bobberInstance.transform.position;
        }
        else if (fishInstance)
        {
            target = fishInstance.transform.position;
        }

        line.SetPosition(1, line.transform.InverseTransformPoint(target));
    }

    private IEnumerator CastLine(Vector3 start, Vector3 end)
    {
        // Starting the casting cooldown
        StartCoroutine(this.Cooldown());

        // Setting up the new bobber
        bobberInstance = Instantiate(bobberPrefab, start, Quaternion.identity);
        bobberInstance.name = bobberName;
        var bobberTransform = bobberInstance.transform;

        if (bobberTransform == null)
        {
            yield return null;
        }

        var bobberScript = bobberInstance.GetComponent<BobberScript>();
        bobberScript.enabled = false;

        // Re-enabling the fishing line
        this.UpdateLinePositions();
        line.enabled = true;

        // Moving the bobber through the air using the curves
        var t = 0.0f;
        var yDistance = end.y - start.y;
        var castDuration = minCastDuration + ((Vector3.Distance(start, end) - minCastDistance) / (maxCastDistance - minCastDistance) * (maxCastDuration - minCastDuration));
        while (t <= 1.0f)
        {
            bobberTransform.position = new Vector3(Mathf.Lerp(start.x, end.x, bobberCurveXZ.Evaluate(t)),
                                                   start.y + (bobberCurveY.Evaluate(t) * yDistance),
                                                   Mathf.Lerp(start.z, end.z, bobberCurveXZ.Evaluate(t)));

            t += Time.deltaTime / castDuration;

            yield return new WaitForEndOfFrame();
        }

        if (bobberTransform == null)
        {
            yield return null;
        }

        bobberTransform.position = end;

        // Creating a splash
        StartCoroutine(this.Splash(end));

        // Performing the initial 'bob' as the bobber hits the water
        t = 0.0f;
        var initialBobY = end.y - initialBobDistance;
        while (t <= 1.0f)
        {
            float y;
            if (t <= 0.5f)
            {
                y = Mathf.Lerp(end.y, initialBobY, t * 2.0f);
            }
            else
            {
                y = Mathf.Lerp(end.y, initialBobY, 0.5f - (t - 0.5f));
            }

            if(bobberTransform == null)
            {
                yield return null;
            }

            bobberTransform.position = new Vector3(bobberTransform.position.x,
                                                   y,
                                                   bobberTransform.position.z);

            t += Time.deltaTime / initialBobDuration;

            yield return new WaitForEndOfFrame();
        }

        // Enabling the bobbing
        bobberScript.enabled = true;
    }

    private void PullLine()
    {
        if (Random.value > 0.5f)
        {
            this.CatchFish();
        }
        else
        {
            line.enabled = false;

            if (bobberInstance)
            {
                Destroy(bobberInstance);
            }
        }
    }

    private void CatchFish()
    {
        fishInstance = Instantiate(fishPrefab, bobberInstance.transform.position, Quaternion.identity);
        fishInstance.name = fishName;
        fishInstance.GetComponent<SpringJoint>().connectedBody = rodTip.GetComponent<Rigidbody>();

        if (bobberInstance)
        {
            Destroy(bobberInstance);
        }
    }

    private void DropFish()
    {
        Destroy(fishInstance.GetComponent<SpringJoint>());
        line.enabled = false;
        canUseRod = true;
    }

    private IEnumerator Splash(Vector3 splashPosition)
    {
        var splash = Instantiate(splashPrefab, splashPosition, Quaternion.identity);
        splash.hideFlags = HideFlags.HideInHierarchy;
        var splashParticles = splash.GetComponent<ParticleSystem>();

        yield return new WaitUntil(() => !splashParticles.IsAlive());

        Destroy(splash);
    }

    private IEnumerator Cooldown()
    {
        canUseRod = false;

        yield return new WaitForSeconds(cooldown);

        canUseRod = true;
    }
}
