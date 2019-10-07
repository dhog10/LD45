using UnityEngine;
using UnityEngine.Events;
using UnityStandardAssets.Characters.FirstPerson;

public class Portal : MonoBehaviour
{
    public float distance = 50f;
    public bool inverted = false;
    public Portal destination;
    public GameObject player;
    public Camera portalCamera;
    public Material renderTarget;

    public UnityEvent onTeleportFrom;
    public UnityEvent onTeleportTo;

    private PlayerController playerController;
    private GameObject playerObject;
    private MeshRenderer mr;
    [HideInInspector]
    public bool playerInside = false;
    [HideInInspector]
    public Door door;

    private bool teleporting = false;
    private float lastTeleport = 0f;
    private bool portalEnabled = true;

    public Transform PortalTransform => mr.transform;

    void Start()
    {
        if(portalCamera.targetTexture != null)
        {
            portalCamera.targetTexture.Release();
        }

        if(portalCamera != null)
        {
            portalCamera.targetTexture = (RenderTexture)renderTarget.mainTexture ?? new RenderTexture(Screen.width, Screen.height, 24);
            renderTarget.mainTexture = portalCamera.targetTexture;
        }

        mr = this.GetComponentInChildren<MeshRenderer>(true);

        if(player == null)
        {
            playerController = FindObjectOfType<PlayerController>();
            
            if(playerController != null)
            {
                playerObject = playerController.gameObject;

                var camera = playerController.gameObject.GetComponentInChildren<Camera>(true);
                if(camera != null)
                {
                    player = camera.gameObject;
                }
                
            }
        }
    }

    private void Update()
    {
        this.UpdatePortal();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!portalEnabled || teleporting || destination == null || !destination.isActiveAndEnabled || playerInside || !this.IsInRoom())
        {
            return;
        }

        var hitObject = other.gameObject;

        var controller = hitObject.GetComponent<PlayerController>();

        if (controller != null)
        {
            playerInside = true;

            var dot = this.GetPlayerDot();

            if (Time.time - lastTeleport > 0.3f && dot > 0f)
            {
                lastTeleport = Time.time;

                destination.NotifyTeleporting();

                playerObject.transform.rotation = destination.portalCamera.transform.rotation;
                var euler = destination.portalCamera.transform.rotation.eulerAngles;

                controller.cameraYaw = euler.y;
                controller.NotifyTeleported();

                var localPosition = transform.InverseTransformPoint(playerObject.transform.position);

                localPosition.x = -localPosition.x;
                localPosition.y = -localPosition.y;

                playerObject.transform.position = destination.transform.TransformPoint(localPosition);

                onTeleportFrom?.Invoke();
                destination.onTeleportTo?.Invoke();
            }            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var hitObject = other.gameObject;

        if (hitObject.GetComponent<PlayerController>() != null)
        {
            playerInside = false;
            teleporting = false;
        }
    }

    public void NotifyTeleporting()
    {
        teleporting = true;
    }

    public void UpdatePortal()
    {
        if (!portalEnabled || destination == null || !destination.isActiveAndEnabled)
        {
            if (portalCamera.gameObject.activeSelf)
            {
                portalCamera.gameObject.SetActive(false);
            }

            if (mr.enabled)
            {
                mr.enabled = false;
            }

            return;
        }

        if (destination.IsInRoom())
        {
            if (!portalCamera.gameObject.activeSelf)
            {
                portalCamera.gameObject.SetActive(true);
            }

            this.PositionCamera();
        }
        else
        {
            if (portalCamera.gameObject.activeSelf)
            {
                portalCamera.gameObject.SetActive(false);
            }
        }

        if (this.IsInRoom())
        {
            if (!mr.enabled)
            {
                mr.enabled = true;
            }
        }
        else
        {
            if (mr.enabled)
            {
                mr.enabled = false;
            }
        }
    }

    public bool IsInRoom()
    {
        if(player == null)
        {
            return false;
        }

        var distanceFromPlayer = Vector3.Distance(player.transform.position, this.transform.position);

        return distanceFromPlayer <= distance;
    }

    public void EnablePortal()
    {
        portalEnabled = true;
    }

    public void DisablePortal()
    {
        portalEnabled = false;
    }

    private void PositionCamera()
    {
        portalCamera.transform.position = this.GetCameraPosition();
        portalCamera.transform.rotation = this.GetCameraAngle();

        Debug.DrawLine(portalCamera.transform.position, portalCamera.transform.position + portalCamera.transform.forward, Color.green);
    }

    private Quaternion GetCameraAngle()
    {
        var camera = Camera.main;
        var worldRotation = camera.transform.rotation;
        var door1 = transform;
        var door2 = destination.transform;

        var C = door1.transform.rotation * Quaternion.Inverse(door2.transform.rotation);

        var rot = (C * worldRotation).eulerAngles;
        rot = new Vector3(rot.x, rot.y + 180, rot.z);
        return Quaternion.Euler(rot);
    }

    private Vector3 GetCameraPosition()
    {
        var relativePosition = destination.transform.InverseTransformPoint(player.transform.position);
        relativePosition.x = -relativePosition.x;
        relativePosition.y = -relativePosition.y;

        return this.transform.TransformPoint(relativePosition);
    }

    private float GetPlayerDot()
    {
        var origin = this.PortalTransform.position;

        var toPlayer = Vector3.Normalize(playerObject.transform.position - origin);
        return Vector3.Dot(this.PortalTransform.up, toPlayer);
    }
}
