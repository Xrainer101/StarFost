using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipEmissions : MonoBehaviour
{
    public Transform emitter;
    public TrailRenderer[] wingTrails;
    [SerializeField] bool emitTrails;

    [SerializeField] float scaleMultiplier = 1.5f;
    [SerializeField] float scaleSpeed = 5f;

    Vector3 defaultScale, targetScale;

    // Start is called before the first frame update
    void Start()
    {
        defaultScale = emitter.localScale;
        targetScale = defaultScale;
    }

    // Update is called once per frame
    void Update()
    {
        emitter.localScale = Vector3.Lerp(emitter.localScale, targetScale, Time.deltaTime * scaleSpeed);

        EmitTrail();
    }

    public void EmitNorm()
    {
        targetScale = defaultScale;
        emitTrails = false;
    }

    public void EmitBoost()
    {
        targetScale = defaultScale * scaleMultiplier;
        emitTrails = true;
    }

    public void EmitBrake()
    {
        targetScale = defaultScale * 0.5f;
    }

    public void EmitTrail() // Use this to toggle emitter on death
    {
        if (emitTrails)
        {
            foreach(TrailRenderer trail in wingTrails)
                trail.emitting = true;
        } 
        else
            foreach (TrailRenderer trail in wingTrails)
                trail.emitting = false;
    }
}
