using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Portal : MonoBehaviour
{
    public float distance = 50f;
    public Portal destination;
    public GameObject player;
    public Camera portalCamera;
    public Material renderTarget;

    private GameObject playerObject;
    private MeshRenderer mr;
    [HideInInspector]
    public bool playerInside = false;

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

        mr = this.GetComponent<MeshRenderer>();

        if(player == null)
        {
            var playerController = FindObjectOfType<FirstPersonController>();
            if(playerController != null)
            {
                playerObject = playerController.gameObject;

                var camera = playerController.gameObject.GetComponentInChildren<Camera>();
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
        if (playerInside || !this.IsInRoom())
        {
            return;
        }


        var hitObject = other.gameObject;
        Debug.Log("hit " + hitObject);
        
        if(hitObject.GetComponent<CharacterController>() != null)
        {
            playerInside = true;
            destination.playerInside = true;

            Debug.Log("teleport");
            playerObject.transform.position = destination.transform.position + (playerObject.transform.position - transform.position);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var hitObject = other.gameObject;

        if (hitObject.GetComponent<CharacterController>() != null)
        {
            playerInside = false;
        }
    }

    public void UpdatePortal()
    {
        if (destination == null)
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

    private void PositionCamera()
    {
        portalCamera.transform.position = this.GetCameraPosition();
        portalCamera.transform.rotation = this.GetCameraAngle();

        Debug.DrawLine(portalCamera.transform.position, portalCamera.transform.position + portalCamera.transform.forward);
    }

    private Quaternion GetCameraAngle()
    {
        var angDiff = Quaternion.Angle(transform.rotation, destination.transform.rotation);
        var rotDiff = Quaternion.AngleAxis(angDiff, Vector3.up);
        var direction = rotDiff * player.transform.forward;
        return Quaternion.LookRotation(direction, Vector3.up);
    }
    private Vector3 GetCameraPosition()
    {
        var relativePosition = destination.transform.InverseTransformPoint(player.transform.position);

        return this.transform.TransformPoint(relativePosition);
    }
}
