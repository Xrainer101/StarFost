using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public Slider boostMeter;
    public bool actionCooling;
    public bool hitZero;

    [SerializeField] Image backgroundImage;
    Color meterBackgroundColor;
    public Color overheatColor = Color.red;

    [Header("Settings")]
    public float overheatPenaltyTime = 2.5f;
    public float drainSpeed = 2f;

    void Awake()
    {
        actionCooling = true;
        hitZero = false;
        boostMeter.value = 1;

        if(backgroundImage != null)
        {
            meterBackgroundColor = backgroundImage.color;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ActionCooldown();
    }

    public void ActionCooldown()
    {
        if(boostMeter.value <= 0f && !hitZero)
        {
            StartCoroutine(OverheatDelay());
        }

        if (actionCooling)
        {
            if(!hitZero)
            {
                boostMeter.value = Mathf.Clamp01(boostMeter.value + Time.deltaTime / drainSpeed);
            }
        }
        else
        {
            boostMeter.value = Mathf.Clamp01(boostMeter.value - Time.deltaTime / drainSpeed);
        }
    }

    IEnumerator OverheatDelay()
    {
        hitZero = true;

        if(backgroundImage != null)
        {
            backgroundImage.color = overheatColor;
        }

        yield return new WaitForSeconds(overheatPenaltyTime);

        if(backgroundImage != null)
        {
            backgroundImage.color = meterBackgroundColor;
        }

        boostMeter.value = 0.01f; // Make sure we aren't stuck at 0
        hitZero = false;
    }
}
