using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SplineSpeedController : MonoBehaviour
{
    [Header("Speed Settings")]
    public float maxSpeed = 40f; 
    
    [Tooltip("The X axis is track progress (0 to 1). The Y axis is speed multiplier (0 to 1).")]
    public AnimationCurve speedCurve = AnimationCurve.Linear(0, 1, 1, 1);

    private SplineAnimate splineAnimate;
    private float splineLength;

    // Start is called before the first frame update
    void Start()
    {
        splineAnimate = GetComponent<SplineAnimate>();

        // Ensure the component is set to Speed mode
        splineAnimate.AnimationMethod = SplineAnimate.Method.Speed;

        // Get how long this specific spline track is in meters
        splineLength = splineAnimate.Container.CalculateLength();
        
        // Stop SplineAnimate so we can take over
        splineAnimate.Pause();
    }

    // Update is called once per frame
    void Update()
    {
        // Make sure we have a track
        if(splineAnimate == null || splineLength <= 0) return;

        // Get where we are on the spline
        float currentProgress = splineAnimate.NormalizedTime;

        // If our progress is at or over 1, we're done
        if(currentProgress >= 1f) return;

        // Read the curve to find our speed
        float currentSpeed = maxSpeed * speedCurve.Evaluate(currentProgress);

        // Convert the speed into a percentage of the track
        float percentageSpeed = currentSpeed / splineLength;

        // Push the animation forward
        splineAnimate.NormalizedTime += percentageSpeed * Time.deltaTime;
    }
}
