using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitScreen : MonoBehaviour
{
    public static ExitScreen Instance;

    public AudioSource cinematicHit;
    public AudioSource creditMusic;
    public GameObject canvasObject;
    public GameObject creditsObject;

    private bool creditsActive;
    private float creditsStarted = 0f;
    private bool creditMusicPlayed;

    // Start is called before the first frame update
    void Start()
    {
        canvasObject.gameObject.SetActive(false);
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(creditsActive)
        {
            if(Time.time - creditsStarted > 0.2f && !canvasObject.gameObject.activeSelf)
            {
                canvasObject.gameObject.SetActive(true);
            }

            if(Time.time - creditsStarted > 5f)
            {
                if (!creditMusicPlayed)
                {
                    creditMusicPlayed = true;
                    creditMusic.Play();
                }
            }

            if(Time.time - creditsStarted > 7f)
            {
                if (!creditsObject.activeSelf)
                {
                    creditsObject.SetActive(true);
                }
            }
            
        }
    }

    public void RollCredits()
    {
        cinematicHit.Play();

        creditsActive = true;
        creditsStarted = Time.time;
    }

    public bool HasExited()
    {
        return creditsActive;
    }
}
