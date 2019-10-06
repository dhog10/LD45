using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PianoGame : MonoBehaviour
{
    public UnityEvent onComplete;

    [SerializeField] private Transform player;
    [SerializeField] private Piano piano;
    [SerializeField] private AudioSource failAudio;
    [SerializeField] private AudioSource successAudio;
    [Space(5)]
    [SerializeField] private float activationDistance = 5.0f;
    [SerializeField] private float introDelay = 0.25f;
    [SerializeField] private float sequencesDelay = 1.0f;
    [SerializeField] private float resultAudioDelay = 1.0f;
    [SerializeField] private float perKeyResponseTime = 2.0f;
    [Space(5)]
    [SerializeField] private PianoSequence[] pianoSequences;
    private bool activated = false;
    private bool introFinished = false;
    private int currentPianoSequence = 0;
    private bool sequencePlaying = false;
    private bool sequenceInputting = false;
    private bool outroTriggered = false;
    private GameObject pressedKey;

    private void Update()
    {
        if (!activated &&
            Camera.main &&
            Vector3.Distance(Camera.main.transform.position, transform.position) <= activationDistance)
        {
            StartCoroutine(this.Intro());

            activated = true;
        }

        if (!activated || !introFinished || outroTriggered)
        {
            return;
        }

        if (currentPianoSequence >= pianoSequences.Length)
        {
            StartCoroutine(this.Outro());
            onComplete?.Invoke();
            return;
        }

        if (Input.GetMouseButtonDown(0) &&
            Camera.main &&
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, piano.maxPressDistance) &&
            string.Equals(hit.transform.gameObject.tag, piano.keyTag) &&
            hit.transform.position.y == piano.originalKeyYPosition)
        {
            pressedKey = hit.transform.gameObject;
        }
        else
        {
            pressedKey = null;
        }

        if (!sequencePlaying && !sequenceInputting)
        {
            StartCoroutine(this.PlaySequence(pianoSequences[currentPianoSequence]));
        }
    }

    private IEnumerator Intro()
    {
        piano.playable = false;

        foreach (var key in piano.keys.Reverse())
        {
            StartCoroutine(piano.KeyPress(key));

            yield return new WaitForSeconds(introDelay);
        }

        piano.playable = true;
        introFinished = true;
    }

    private IEnumerator Outro()
    {
        outroTriggered = true;
        piano.playable = false;

        yield return new WaitForSeconds(sequencesDelay);

        foreach (var key in piano.keys)
        {
            StartCoroutine(piano.KeyPress(key));

            yield return new WaitForSeconds(introDelay);
        }
    }

    private IEnumerator PlaySequence(PianoSequence pianoSequence)
    {
        sequencePlaying = true;
        piano.playable = false;

        yield return new WaitForSeconds(sequencesDelay);

        foreach (var pianoKeyPress in pianoSequence.keyPresses)
        {
            StartCoroutine(piano.KeyPress(pianoKeyPress.key));

            yield return new WaitForSeconds(pianoKeyPress.delay);
        }

        piano.playable = true;
        sequencePlaying = false;

        StartCoroutine(this.InputSequence(pianoSequence));
    }

    private IEnumerator InputSequence(PianoSequence pianoSequence)
    {
        sequenceInputting = true;
        var currentKeyPress = 0;

        var timeRemaining = pianoSequence.keyPresses.Length * perKeyResponseTime;
        while (sequenceInputting)
        {
            if (pressedKey)
            {
                if (pressedKey.GetInstanceID() == pianoSequence.keyPresses[currentKeyPress].key.GetInstanceID())
                {

                    currentKeyPress++;
                    if (currentKeyPress >= pianoSequence.keyPresses.Length)
                    {
                        yield return new WaitForSeconds(resultAudioDelay);
                        successAudio.Play();
                        currentPianoSequence++;
                        sequenceInputting = false;
                    }
                }
                else
                {
                    yield return new WaitForSeconds(resultAudioDelay);
                    failAudio.Play();
                    sequenceInputting = false;
                }
            }

            timeRemaining -= Time.deltaTime;
            
            yield return new WaitForEndOfFrame();

            if (timeRemaining <= 0.0f)
            {
                failAudio.Play();
                sequenceInputting = false;
            }
        }
    }
}

[System.Serializable]
public class PianoSequence
{
    public PianoKeyPress[] keyPresses;

    public PianoSequence(PianoKeyPress[] keyPresses)
        => this.keyPresses = keyPresses;
}

[System.Serializable]
public class PianoKeyPress
{
    public GameObject key;
    public float delay;

    public PianoKeyPress(GameObject key, float delay)
    {
        this.key = key;
        this.delay = delay;
    }
}
