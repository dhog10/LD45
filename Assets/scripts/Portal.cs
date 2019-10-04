using UnityEngine;

public class Portal : MonoBehaviour
{
    public float distance = 50f;
    public Portal destination;
    public GameObject player;
    public Camera portalCamera;

    void Start()
    {
        
    }

    void Update()
    {
        if (this.IsInRoom())
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
    }

    public bool IsInRoom()
    {
        if(player == null)
        {
            return false;
        }

        return Vector3.Distance(player.gameObject.transform.position, this.transform.position) <= distance;
    }

    private void PositionCamera()
    {
        portalCamera.gameObject.transform.position = this.GetCameraPosition();
        portalCamera.gameObject.transform.rotation = this.GetCameraAngle();
    }

    private Quaternion GetCameraAngle()
    {
        var relativeRotation = Quaternion.Inverse(destination.transform.rotation) * player.gameObject.transform.rotation;

        return this.transform.rotation * relativeRotation;
    }
    private Vector3 GetCameraPosition()
    {
        var relativePosition = destination.transform.InverseTransformPoint(player.gameObject.transform.position);

        return this.transform.position + relativePosition;
    }
}
