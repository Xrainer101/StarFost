using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public Slider actionCDSlider;
    public bool actionCooling;

    void Awake()
    {
        actionCooling = true;
        actionCDSlider.value = 1;
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
        if (actionCooling)
            actionCDSlider.value = Mathf.Clamp01(actionCDSlider.value + Time.deltaTime / 2f);
        else
            actionCDSlider.value = Mathf.Clamp01(actionCDSlider.value - Time.deltaTime / 2f);
    }
}
