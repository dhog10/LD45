using System.Collections;
using UnityEngine;

public class Clock : MonoBehaviour
{
    [SerializeField] private Transform secondHand;
    [SerializeField] private Transform minuteHand;
    [SerializeField] private Transform hourHand;
    [Space(5)]
    [SerializeField] private float secondHandTickDuration = 1.0f;
    [SerializeField] private float minuteHandTickDuration = 60.0f;
    [SerializeField] private float hourHandTickDuration = 3600.0f;
    [Space(5)]
    [SerializeField] private AnimationCurve secondHandTickCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
    [SerializeField] private AnimationCurve minuteHandTickCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
    [SerializeField] private AnimationCurve hourHandTickCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
    private IEnumerator secondHandMover;
    private IEnumerator minuteHandMover;
    private IEnumerator hourHandMover;
    private enum HandType { Second, Minute, Hour };

    private void OnEnable()
    {
        this.StartSecondHandMoving();
        this.StartMinuteHandMoving();
        this.StartHourHandMoving();
    }

    public void StartSecondHandMoving()
        => this.StartHandMoving(secondHand, secondHandTickCurve, secondHandMover, HandType.Second);

    public void StartMinuteHandMoving()
        => this.StartHandMoving(minuteHand, minuteHandTickCurve, minuteHandMover, HandType.Minute);

    public void StartHourHandMoving()
        => this.StartHandMoving(hourHand, hourHandTickCurve, hourHandMover, HandType.Hour);

    public void StopSecondHandMoving()
        => this.StopHandMoving(secondHandMover);

    public void StopMinuteHandMoving()
        => this.StopHandMoving(minuteHandMover);

    public void StopHourHandMoving()
        => this.StopHandMoving(hourHandMover);

    private void StartHandMoving(Transform hand, AnimationCurve tickCurve, IEnumerator handMover, HandType handType)
    {
        if (handMover != null)
        {
            StopCoroutine(handMover);
        }
        handMover = this.HandMoverCoroutine(hand, tickCurve, handType);
        StartCoroutine(handMover);
    }

    private void StopHandMoving(IEnumerator handMover)
    {
        if (handMover != null)
        {
            StopCoroutine(handMover);
        }
    }

    private IEnumerator HandMoverCoroutine(Transform hand, AnimationCurve tickCurve, HandType handType)
    {
        while (true)
        {
            var originalHandRotationEuler = hand.localRotation.eulerAngles;
            var newHandRotationEuler = originalHandRotationEuler;
            newHandRotationEuler.z += 6f;
            if (newHandRotationEuler.z > 360.0f)
            {
                newHandRotationEuler.z -= 360.0f;
            }
            var t = 0.0f;
            while (t <= 1.0f)
            {
                hand.localRotation = Quaternion.Euler(new Vector3(originalHandRotationEuler.x,
                                                                  originalHandRotationEuler.y,
                                                                  Mathf.Lerp(originalHandRotationEuler.z, newHandRotationEuler.z, tickCurve.Evaluate(t))));

                if (handType == HandType.Second)
                {
                    t += Time.deltaTime / secondHandTickDuration;
                }
                else if (handType == HandType.Minute)
                {
                    t += Time.deltaTime / minuteHandTickDuration;
                }
                else if (handType == HandType.Hour)
                {
                    t += Time.deltaTime / hourHandTickDuration;
                }
                else
                {
                    t += Time.deltaTime / 1.0f;
                }

                yield return new WaitForEndOfFrame();
            }
            hand.localRotation = Quaternion.Euler(newHandRotationEuler);
        }
    }
}
