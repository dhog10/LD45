using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basketball : MonoBehaviour
{

    [SerializeField] Transform playerPosition;
    [SerializeField] float distanceMax;
    private float distance;

    public AudioSource tickSource;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        tickSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if(playerPosition == null)
        {
            return;
        }

        distance = Vector3.Distance(gameObject.transform.position, playerPosition.position);

        var p = distance / distanceMax;
        p = Mathf.Clamp((p - 0.5f) / 0.5f, 0f, 1f);

        rb.AddForce((playerPosition.position - transform.position) * p * 10f * Time.deltaTime);
    }
    void OnCollisionEnter (Collision collision)
    {
        tickSource.Play();
    }

}
