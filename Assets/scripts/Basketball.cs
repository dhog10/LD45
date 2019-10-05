using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basketball : MonoBehaviour
{

    [SerializeField] Transform playerPosition;
    [SerializeField] float distanceMax;
    private float distance;

    public AudioSource tickSource;

    // Start is called before the first frame update
    void Start()
    {
        tickSource = GetComponent<AudioSource>();
    }

    void Update()
    {

        distance = Vector3.Distance(gameObject.transform.position, playerPosition.position);
        if (distance > distanceMax)
        {
            gameObject.transform.position = new Vector3((playerPosition.position.x + 1),1,(playerPosition.position.z + 1));
        }

    }
    void OnCollisionEnter (Collision collision)
    {

//        var raycast = playerPosition
        tickSource.Play();
    }

}
