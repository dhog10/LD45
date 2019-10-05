using UnityEngine;

public class CullLayers : MonoBehaviour
{
    [SerializeField, Tooltip("Each layer's culling distance. A default value of zero will use the normal far clip plane.")] private float[] distances = new float[32];

    private void Start()
    {
        Camera camera = gameObject.GetComponent<Camera>();
        if (camera)
        {
            camera.layerCullDistances = distances;
        }
    }
}