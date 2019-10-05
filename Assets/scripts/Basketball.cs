using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basketball : MonoBehaviour
{

    public AudioSource tickSource;

    // Start is called before the first frame update
    void Start()
    {

        tickSource = GetComponent<AudioSource>();

    }

    void OnCollisionEnter (Collision collision)
    {
        tickSource.Play();
    }

}
