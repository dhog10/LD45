using UnityEngine;

public class BobberScript : MonoBehaviour
{
    [SerializeField] [Tooltip("The speed of the bobbing")] private float bobbingSpeed = 1.0f;
    [SerializeField] [Tooltip("The Y increase and decrease of the bobbing")] private float bobbingStrength = 1.0f;
    private float originalY;
    private float lastY;

    private void OnValidate()
    {
        if (bobbingSpeed < 0.0f)
        {
            bobbingSpeed = 0.0f;
        }
        if (bobbingStrength < 0.0f)
        {
            bobbingStrength = 0.0f;
        }
    }

    private void OnEnable()
    {
        originalY = transform.position.y;
        lastY = originalY;
    }

    private void FixedUpdate()
    {
        var diffY = transform.position.y - lastY;

        if (diffY != 0.0f)
        {
            originalY += diffY;
        }

        transform.position = new Vector3(transform.position.x, originalY + (Mathf.Sin(Time.fixedTime * bobbingSpeed) * bobbingStrength), transform.position.z);

        lastY = transform.position.y;
    }
}
