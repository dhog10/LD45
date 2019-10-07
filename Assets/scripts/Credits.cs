using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    public Image logo;
    public AudioSource music;

    private float timeExisted;

    private void Update()
    {
        if (music.isPlaying)
        {
            timeExisted = Mathf.Min(1f, timeExisted + Time.deltaTime * 0.1f);
        }
        else
        {
            timeExisted = Mathf.Max(0f, timeExisted - Time.deltaTime * 0.3f);
            if(timeExisted <= 0f)
            {
                Application.Quit();
            }
        }
        

        var color = new Color(1f, 1f, 1f, timeExisted);

        logo.color = color;


    }
}
