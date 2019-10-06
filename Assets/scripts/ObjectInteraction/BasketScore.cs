using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BasketScore : MonoBehaviour
{
    public int scoreNeeded = 5;
    public UnityEvent onScoreReached;

    private int score = 0;
    private float lastScore = 0f;

    void OnTriggerEnter(Collider collider)
    {
        if(Time.time - lastScore < 1f)
        {
            return;
        }

        lastScore = Time.time;

        var rb = collider.gameObject.GetComponent<Rigidbody>();
        if(rb == null)
        {
            return;
        }

        Debug.Log("rb.velocity.y " + rb.velocity.y);

        if(rb.velocity.y > 0f)
        {
            return;
        }

        score++;
        Debug.Log("score " + score);

        if(score >= scoreNeeded)
        {
            Debug.Log("reached score");
            onScoreReached.Invoke();
        }
    }
}
