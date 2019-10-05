using UnityEngine;

public class PickUpPointCenter : MonoBehaviour
{
    [SerializeField] GameObject pointObject;

    void Start()
    {

        pointObject.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.height / 2, Screen.width / 2, Camera.main.nearClipPlane + 1));

    }

    void Update()
    {
        pointObject.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.height / 2, Screen.width / 2, Camera.main.nearClipPlane + 1));
    }
}
